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

		var sourceOption = new Option<DirectoryInfo?>
			("--source", "The path to the source data.") { IsRequired = true };

        var outputOption = new Option<FileInfo?>(
            name: "--output",
            description: "The location to save the Sqlite DB to.") { IsRequired = true };

		var rootCommand = new RootCommand("Processes a source of baby name data") { sourceOption, outputOption };

		rootCommand.SetHandler(async (sourceOptionValue, outputOptionValue) =>
		{
			if (sourceOptionValue == null)
			{
				logger.LogError("No source provided.");
				return;
			}
			var outputLocation = outputOptionValue?.FullName;
			if (outputLocation == null)
			{
				var currentDirectory = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
				outputLocation = Path.Combine(currentDirectory!.FullName, "babynames.db");				
			}
			await ProcessSource(logger, sourceOptionValue, outputLocation);
		},
		sourceOption, outputOption);

		return await rootCommand.InvokeAsync(args);
	}

	async static Task ProcessSource(ILogger logger, DirectoryInfo sourcePath, string outputLocation)
	{
		logger.LogInformation("Processing source in {sourcePath} to {outputLocation}", sourcePath.FullName, outputLocation);

		var connectionString = $"Data Source={outputLocation}";
		var processingService = new ProcessingService(connectionString, logger);

		await processingService.ProcessSource(sourcePath);
	}
}