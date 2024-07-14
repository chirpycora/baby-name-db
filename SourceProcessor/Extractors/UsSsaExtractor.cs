using System.IO.Compression;
using CC.BabyNameDb.EFCore;
using CC.BabyNameDb.EFCore.Models;
using CC.BabyNameDb.SourceProcessor.Extractors;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using nietras.SeparatedValues;

namespace CC.BabyNameDb.SourceProcessor.Extractors
{
	/// <summary>
	/// Extracts data from the US Social Security Administration
	/// Current as of 2024-07-13
	/// </summary>
	public class UsSsaExtractor(BabyNameContext db, ILogger logger) : ISourceExtractor
	{
		private readonly BabyNameContext _context = db;
		private readonly ILogger _logger = logger;

		private readonly string _countryCode = "US";
		private readonly string _countryName = "United States";

		public List<string> SupportedKeys => new() { "us-ssa-national", "us-ssa-states" };

		public async Task ExtractSource(Source source, string sourceFilePath)
		{
			_logger.LogInformation("Extracting source from {sourcePath}", sourceFilePath);

			var textFileContents = new Dictionary<string, string>();

			// Open the zip file
			using (var archive = ZipFile.OpenRead(sourceFilePath)) {
				foreach (var entry in archive.Entries)
				{
					if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
					{
						using (StreamReader sr = new(entry.Open()))
						{
							var file = await sr.ReadToEndAsync();
							textFileContents.Add(entry.FullName, file);
						}
					}
				}
			}

			// Process the text files
			var records = ProcessTextFile(textFileContents, source);

			// Store in the database
			// Get the names
			var existingNames = _context.Names.ToLookup(r => new { r.BabyName, r.Sex });
			var names = records.ToLookup(r => new { r.Name, r.Sex });
			var nameEntities = names
				.Where(n => !existingNames.Contains(new { BabyName = n.Key.Name, n.Key.Sex }))
				.Select(n => new Name {
					BabyName = n.Key.Name,
					Sex = n.Key.Sex
				}).ToList();
			_context.Names.AddRange(nameEntities);

			// Get the locations
			var existingLocations = _context.Locations
				.Where(l => l.CountryCode == _countryCode)
				.ToLookup(r => r.StateOrProvinceCode);
			var locations = records.ToLookup(r => r.State);
			var locationEntities = locations
				.Where(l => !existingLocations.Contains(l.Key))
				.Select(l => new Location {
					CountryCode = _countryCode,
					CountryName = _countryName,
					StateOrProvinceCode = l.Key,
					// TODO: Fetch the state name from a lookup
				}).ToList();
			_context.Locations.AddRange(locationEntities);

			await _context.SaveChangesAsync();

			// Get the counts
			var yearRecords = records.ToLookup(r => new { r.Name, r.Sex, r.State });
			var existingNameIds = _context.Names.ToDictionary(n => new {n.BabyName, n.Sex}, n => n.Id);
			var existingLocationIds = _context.Locations.ToDictionary(l => l.StateOrProvinceCode!, l => l.Id);
			var sourceId = source.Id;
			var yearCountsToAdd = new List<YearCount>();
			foreach (var set in yearRecords)
			{
				var name = existingNameIds[new { BabyName = set.Key.Name, Sex = set.Key.Sex }];
				var location = existingLocationIds[set.Key.State];
				var yearCounts = set.Select(r => new YearCount {
					Count = r.Count,
					LocationId = location,
					NameId = name,
					Year = r.Year,
					SourceId = sourceId
				}).ToList();
				yearCountsToAdd.AddRange(yearCounts);
			}

			// Delete everything with this source in year counts
			await _context.YearCounts.Where(yc => yc.SourceId == sourceId).ExecuteDeleteAsync();

			// Insert all the year counts
			await _context.BulkInsertAsync(yearCountsToAdd);
		}
		private static List<Record> ProcessTextFile(Dictionary<string, string> textFileContents, Source source)
		{
			var records = new List<Record>();
			foreach (var file in textFileContents)
			{
				using var reader = Sep.Reader().FromText(file.Value);  // Sep is a library for reading separated values
				foreach (var row in reader)
				{
					records.Add(new Record(row));
				}
			}
			return records;
		}
	}

	internal class Record {

		internal Record() { }

		internal Record(SepReader.Row row)
		{
			State = row[0].Parse<string>();
			Sex = row[1].Parse<string>();
			Year = row[2].Parse<int>();
			Name = row[3].Parse<string>().Trim();
			Count = row[4].Parse<int>();
		}

		public string Name { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;
		public int Year { get; set; }
		public int Count { get; set; }
		public string Sex { get; set; } = string.Empty;
	}
}