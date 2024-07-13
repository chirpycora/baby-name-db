using CC.BabyNameDb.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CC.BabyNameDb.EFCore;

public class BabyNameContext : DbContext
{
	public BabyNameContext(DbContextOptions options) : base(options) { 
		Database.EnsureCreated();
	}
	
	public DbSet<Source> Sources { get; set; }
}
