using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace CC.BabyNameDb.SourceProcessor.Tests.Integration;

[TestClass]
public class ProcessingServiceTests
{
	private static readonly string _filename = @"babyname.db";

	[TestMethod]
	public async Task ProcessSource_FromStateData_DoesNotReturnErrors()
	{
		// Arrange
		var testDir = new DirectoryInfo(Environment.CurrentDirectory);
		var dataSourcePath = Path.Combine(testDir.FullName, _filename);
		var cs = new SqliteConnectionStringBuilder { DataSource = dataSourcePath }.ConnectionString;
		
		var sourcePath = testDir.EnumerateDirectories().First(d => d.Name == "us-ssa-states");
		var processingService = new ProcessingService(cs, new LoggerFactory().CreateLogger<ProcessingService>());

		// Act
		await processingService.ProcessSource(sourcePath);

		// Assert
	}

	[TestMethod]
	public async Task ProcessSource_FromNationalData_DoesNotReturnErrors()
	{
		// Arrange
		var testDir = new DirectoryInfo(Environment.CurrentDirectory);
		var dataSourcePath = Path.Combine(testDir.FullName, _filename);
		var cs = new SqliteConnectionStringBuilder { DataSource = dataSourcePath }.ConnectionString;
		
		var sourcePath = testDir.EnumerateDirectories().First(d => d.Name == "us-ssa-national");
		var processingService = new ProcessingService(cs, new LoggerFactory().CreateLogger<ProcessingService>());

		// Act
		await processingService.ProcessSource(sourcePath);

		// Assert
	}

}
