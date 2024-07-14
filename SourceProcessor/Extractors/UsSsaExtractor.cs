using System.IO.Compression;
using CC.BabyNameDb.EFCore;
using CC.BabyNameDb.EFCore.Models;
using CC.BabyNameDb.SourceProcessor.Extractors;
using Microsoft.Extensions.Logging;

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

		public List<string> SupportedKeys => new() { "us-ssa-national", "us-ssa-states" };

		public async Task ExtractSource(Source source, string sourceFilePath)
		{
			_logger.LogInformation("Extracting source from {sourcePath}", sourceFilePath);

			// Open the zip file
			using (var archive = ZipFile.OpenRead(sourceFilePath)) {
				foreach (var entry in archive.Entries)
				{
					if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
					{
						using (StreamReader sr = new(entry.Open()))
						{
							var file = await sr.ReadToEndAsync();
						}
					}
				}
			}

			// // Extract the data from the source ZIP file

			// // Normalizes the path
			// var extractPath = Path.GetFullPath(sourcePath);

			// // Ensures that the last character on the extraction path
			// // is the directory separator char.
			// // Without this, a malicious zip file could try to traverse outside of the expected
			// // extraction path
			// if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
			// 	extractPath += Path.DirectorySeparatorChar;

			// using (ZipArchive archive = ZipFile.OpenRead(zipPath))
			// {
			// 	foreach (ZipArchiveEntry entry in archive.Entries)
			// 	{
			// 		if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
			// 		{
			// 			// Gets the full path to ensure that relative segments are removed.
			// 			string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

			// 			// Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
			// 			// are case-insensitive.
			// 			if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
			// 				entry.ExtractToFile(destinationPath);
			// 		}
			// 	}
			// }

			throw new NotImplementedException();
		}
	}
}