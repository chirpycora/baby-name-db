using System.Text.Json;
using CC.BabyNameDb.EFCore;
using CC.BabyNameDb.EFCore.Models;
using CC.BabyNameDb.SourceProcessor.Extractors;
using CC.BabyNameDb.SourceProcessor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CC.BabyNameDb.SourceProcessor;

public class ProcessingService
{
	private readonly BabyNameContext _context;
	private readonly ILogger _logger;

	public ProcessingService(string sqliteConnectionString, ILogger logger)
	{
		var options = new DbContextOptionsBuilder<BabyNameContext>()
			.UseSqlite(sqliteConnectionString)
			.Options;
		_context = new BabyNameContext(options);
		_logger = logger;
	}

	public ProcessingService(BabyNameContext context, ILogger logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task ProcessSource(DirectoryInfo sourcePath)
	{
		await ProcessSource(sourcePath.FullName);
	}

	public async Task ProcessSource(string sourcePath)
	{
		_logger.LogInformation("Processing source in {sourcePath}", sourcePath);
		
		// Load the source configuration
		var sourceConfiguration = await GetConfiguration(sourcePath);
		if (sourceConfiguration == null)
		{
			return;
		}

		// Update the existing source
		var source = await UpsertSource(sourceConfiguration);

		// Extract the data from the source
		try {
			var extractors = new List<ISourceExtractor> { 
				new UsSsaExtractor(_context, _logger) 
			};
			var extractor = extractors.FirstOrDefault(e => e.SupportedKeys.Contains(sourceConfiguration.Key));
			if (extractor == null)
			{
				_logger.LogError("No extractor found for source {sourceKey}", sourceConfiguration.Key);
				return;
			}
			await extractor.ExtractSource(source, Path.Combine(sourcePath, sourceConfiguration.FileName));
		} catch (Exception ex) {
			_logger.LogError(ex, "Error extracting source {sourceKey}", sourceConfiguration.Key);
			return;
		}
	}

	internal async Task<SourceConfiguration?> GetConfiguration(string sourcePath) {
		
		// Load the source configuration
		var sourceConfigPath = Path.Combine(sourcePath, "source.json");
		var sourceConfig = await File.ReadAllTextAsync(sourceConfigPath);
		var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		var sourceConfiguration = JsonSerializer.Deserialize<SourceConfiguration>(sourceConfig, opts);

		// Validate source file
		if (sourceConfiguration == null)
		{
			_logger.LogError("Invalid source configuration file {sourceConfigPath}", sourceConfigPath);
			return null;
		}
		if (sourceConfiguration.Key == null)
		{
			_logger.LogError("Invalid source configuration file {sourceConfigPath}: Key is required", sourceConfigPath);
			return null;
		}
		if (sourceConfiguration.FileName == null)
		{
			_logger.LogError("Invalid source configuration file {sourceConfigPath}: FileName is required", sourceConfigPath);
			return null;
		}

		return sourceConfiguration;
	}

	internal async Task<Source> UpsertSource(SourceConfiguration sourceCfg) {
		var source = await _context.Sources.FirstOrDefaultAsync(s => s.Key == sourceCfg.Key);	
		if (source == null)
		{
			_logger.LogInformation("Adding new source {sourceKey}", sourceCfg.Key);
			source = new Source();
			_context.Sources.Add(source);
		}
		else
		{
			_logger.LogInformation("Updating existing source {sourceKey}", sourceCfg.Key);
		}

		source.Key = sourceCfg.Key;
		source.LastSyncedUtc = DateTime.UtcNow;
		source.LastUpdatedUtc = sourceCfg.LastUpdatedDate;
		source.Description = sourceCfg.DatasetDescription;
		source.Title = sourceCfg.DatasetName;
		source.LicenseName = sourceCfg.LicenseName;
		source.LicenseUrl = sourceCfg.LicenseUrl;
		source.OrganizationName = sourceCfg.OrganizationName;
		source.PublicUrl = sourceCfg.PublicUrl;

		// Save changes to sources
		await _context.SaveChangesAsync();
		
		return source;
	}
}
