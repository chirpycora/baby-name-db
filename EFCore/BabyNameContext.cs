using CC.BabyNameDb.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CC.BabyNameDb.EFCore;

public class BabyNameContext : DbContext
{
	public BabyNameContext(DbContextOptions options) : base(options) { 
		Database.EnsureCreated();
	}

	public DbSet<Location> Locations { get; set; }

	public DbSet<Name> Names { get; set; }

	public DbSet<Source> Sources { get; set; }

	public DbSet<YearCount> YearCounts { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

		modelBuilder.Entity<YearCount>()
			.HasOne(yc => yc.Name)
			.WithMany(n => n.YearCounts)
			.HasForeignKey(yc => yc.NameId);

		modelBuilder.Entity<YearCount>()
			.HasOne(yc => yc.Location)
			.WithMany(l => l.YearCounts)
			.HasForeignKey(yc => yc.LocationId);

		modelBuilder.Entity<YearCount>()
			.HasOne(yc => yc.Source)
			.WithMany(s => s.YearCounts)
			.HasForeignKey(yc => yc.SourceId);
			
	}
}
