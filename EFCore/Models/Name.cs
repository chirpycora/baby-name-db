using Microsoft.EntityFrameworkCore;

namespace CC.BabyNameDb.EFCore.Models
{
	[Index(nameof(BabyName), nameof(Sex))]
	public class Name
	{
		public int Id { get; set; }

		/// <summary>
		/// The baby name
		/// </summary>
		public string BabyName { get; set; } = string.Empty;

		/// <summary>
		/// The sex (assigned at birth! 🏳️‍⚧️) - either "M" or "F"
		/// </summary>
		public string Sex { get; set; } = string.Empty;

		/// <summary>
		/// Yearly counts of the name
		/// </summary>
		public List<YearCount> YearCounts { get; set; } = new List<YearCount>();
	}
}