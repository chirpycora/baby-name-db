using System.IO.Compression;
using CC.BabyNameDb.EFCore;
using CC.BabyNameDb.EFCore.Helpers;
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

			// Get the text files from the zip
			var textFileContents = await GetTextFilesFromZip(sourceFilePath);

			// Process the text files
			var records = ProcessTextFiles(textFileContents);

			// Store in the database
			_logger.LogInformation("Storing names & locations in the database");
			var existingNames = _context.Names.ToLookup(r => new { r.BabyName, r.Sex });
			var names = records.ToLookup(r => new { r.Name, r.Sex });
			var nameEntities = names
				.Where(n => !existingNames.Contains(new { BabyName = n.Key.Name, n.Key.Sex }))
				.Select(n => new Name {
					BabyName = n.Key.Name,
					Sex = n.Key.Sex
				}).ToList();
			_context.Names.AddRange(nameEntities);
			_logger.LogInformation("Adding {nameCount} names to the database", nameEntities.Count);

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
					StateOrProvinceName = l.Key?.ToStateName()
				}).ToList();
			_context.Locations.AddRange(locationEntities);
			_logger.LogInformation("Adding {locationCount} locations to the database", locationEntities.Count);

			await _context.SaveChangesAsync();
			_logger.LogInformation("Names & locations stored in the database");

			// Get the counts

			// Delete everything with this source in year counts
			_logger.LogInformation("Deleting existing yearly counts for this source");
			var sourceId = source.Id;
			await _context.YearCounts.Where(yc => yc.SourceId == sourceId).ExecuteDeleteAsync();

			_logger.LogInformation("Building yearly counts for each name, sex, and location");
			var yearRecords = records.ToLookup(r => new { r.Name, r.Sex, r.State });
			var existingNameIds = _context.Names.ToDictionary(n => new {n.BabyName, n.Sex}, n => n.Id);
			var existingLocationIds = _context.Locations.ToDictionary(l => l.StateOrProvinceCode ?? string.Empty, l => l.Id);
			var yearCountsToAdd = new List<YearCount>();
			foreach (var set in yearRecords)
			{
				var name = existingNameIds[new { BabyName = set.Key.Name, Sex = set.Key.Sex }];
				var location = existingLocationIds[set.Key.State ?? string.Empty];
				var yearCounts = set.Select(r => new YearCount {
					Count = r.Count,
					LocationId = location,
					NameId = name,
					Year = r.Year,
					SourceId = sourceId
				}).ToList();
				yearCountsToAdd.AddRange(yearCounts);
			}

			// Insert all the year counts
			_logger.LogInformation("Inserting {yearCount} yearly counts to the database", yearCountsToAdd.Count);
			await _context.BulkInsertAsync(yearCountsToAdd);

			_logger.LogInformation("Processing complete");
		}

		private async Task<Dictionary<string, string>> GetTextFilesFromZip(string sourceFilePath)
		{
			_logger.LogInformation("Reading zip file from {sourcePath}", sourceFilePath);

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

			_logger.LogInformation("Found {fileCount} text files in the zip file", textFileContents.Count);
			return textFileContents;
		}

		private List<Record> ProcessTextFiles(Dictionary<string, string> textFileContents)
		{
			_logger.LogInformation("Processing text files");

			var records = new List<Record>();
			foreach (var file in textFileContents)
			{
				int? year = null;
				if (file.Key.StartsWith("yob", StringComparison.OrdinalIgnoreCase))
				{
					year = int.Parse(file.Key.Substring(3, 4));
				}

				using var reader = Sep.Reader(o => o with { HasHeader = false })
					.FromText(file.Value);  // Sep is a library for reading separated values
				foreach (var row in reader)
				{
					if (year != null)
						records.Add(new Record(row, year.Value));
					else {
						records.Add(new Record(row));
					}
				}
			}
			return records;
		}
	}

	internal class Record {

		internal Record() { }

		/// <summary>
		/// This constructor is used for the state data
		/// </summary>
		/// <param name="row">The row from SepReader for the record</param>
		internal Record(SepReader.Row row)
		{
			State = row[0].Parse<string>();
			Sex = row[1].Parse<string>();
			Year = row[2].Parse<int>();
			Name = row[3].Parse<string>().Trim();
			Count = row[4].Parse<int>();
		}

		/// <summary>
		/// This constructor is used for the national data
		/// </summary>
		/// <param name="row">The row from SepReader for the record</param>
		/// <param name="year">The year of names</param>
		internal Record(SepReader.Row row, int year)
		{
			Sex = row[1].Parse<string>();
			Year = year;
			Name = row[0].Parse<string>().Trim();
			Count = row[2].Parse<int>();
		}

		public string Name { get; set; } = string.Empty;
		public string? State { get; set; } = null;
		public int Year { get; set; }
		public int Count { get; set; }
		public string Sex { get; set; } = string.Empty;
	}
}