using System.Diagnostics;
using System.Collections;
using CC.BabyNameDb.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace CC.BabyNameDb.EFCore.Tests.Integration;

[TestClass]
public class BabyNameContextIntegrationTests
{
	[TestMethod]
	public void DatabaseIsCreatedAndTableExists()
	{
		// using var connection = new SqliteConnection("DataSource=:memory:");
		using var connection = new SqliteConnection(@"DataSource=E:\babyname.db");
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