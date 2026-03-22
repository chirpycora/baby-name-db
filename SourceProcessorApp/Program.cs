// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using CC.BabyNameDb.SourceProcessor;
using Microsoft.Extensions.Logging;

internal class Program
{
	private static async Task<int> Main(string[] args)
	{
		using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
		ILogger logger = factory.CreateLogger("Program");

		var sourceOption = new Option<DirectoryInfo?>("--source")
		{
			Description = "The path to the source data."
		};

		var outputOption = new Option<FileInfo?>("--output")
		{
			Description = "The location to save the Sqlite DB to."
		};

		var rootCommand = new RootCommand("Processes a source of baby name data") { sourceOption, outputOption };

		rootCommand.SetAction(async (parseResult, cancellationToken) =>
		{
			var sourceOptionValue = parseResult.GetValue(sourceOption);
			var outputOptionValue = parseResult.GetValue(outputOption);

			var currentDirectory = new FileInfo(AppContext.BaseDirectory);
			
			var outputLocation = outputOptionValue?.FullName;
			if (outputLocation == null)
			{
				outputLocation = Path.Combine(currentDirectory!.FullName, "babynames.db");				
			}
			
			var sourcePath = sourceOptionValue?.FullName;
			if (sourcePath == null)
			{
				// Process all sources in Sources folder
				sourcePath = Path.Combine(currentDirectory!.FullName, "Sources");
				var allSources = new DirectoryInfo(sourcePath).EnumerateDirectories();
				foreach (var source in allSources)
				{
					await ProcessSource(logger, source, outputLocation);
				}
			} else {
				await ProcessSource(logger, sourceOptionValue!, outputLocation);
			}
		});

		return await rootCommand.Parse(args).InvokeAsync();
	}

	async static Task ProcessSource(ILogger logger, DirectoryInfo sourcePath, string outputLocation)
	{
		logger.LogInformation("Processing source in {sourcePath} to {outputLocation}", sourcePath.FullName, outputLocation);

		var connectionString = $"Data Source={outputLocation}";
		var processingService = new ProcessingService(connectionString, logger);

		await processingService.ProcessSource(sourcePath);
	}
}