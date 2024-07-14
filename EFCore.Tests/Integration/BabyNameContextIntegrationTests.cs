using System.Diagnostics;
using System.Collections;
using CC.BabyNameDb.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace CC.BabyNameDb.EFCore.Tests.Integration;

[TestClass]
public class BabyNameContextIntegrationTests
{
	private static readonly string _filename = @"babyname.db";

	[TestMethod]
	public void DatabaseIsCreatedAndTableExists()
	{
		var testDir = new DirectoryInfo(Environment.CurrentDirectory);
		var dataSourcePath = Path.Combine(testDir.FullName, _filename);
		var cs = new SqliteConnectionStringBuilder { DataSource = dataSourcePath }.ConnectionString;
		
		using var connection = new SqliteConnection(cs);
		connection.Open();

		// Arrange
		var options = new DbContextOptionsBuilder<BabyNameContext>()
			.UseSqlite(connection)
			.Options;

		// Act
		List<Source>? sources = null;
		using (var context = new BabyNameContext(options))
		{
			sources = context.Sources.ToList();
		}

		// Assert
		Assert.IsTrue(sources != null);
	}
}