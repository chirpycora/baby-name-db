namespace CC.BabyNameDb.EFCore.Models
{
	public class YearCount
	{
		public int Id { get; set; }

		/// <summary>
		/// The ID of the baby name
		/// </summary>
        public int NameId { get; set; }

		/// <summary>
		/// The ID of the location
		/// </summary>
		public int LocationId { get; set; }

		/// <summary>
		/// The ID of the source
		/// </summary>
		public int SourceId { get; set; }

		/// <summary>
		/// The baby name
		/// </summary>
		public Name Name { get; set; } = new Name();

		/// <summary>
		/// The country or country + state/province where the name was recorded
		/// </summary>
		public Location Location { get; set; } = new Location();

		/// <summary>
		/// The source of the data
		/// </summary>
		public Source Source { get; set; } = new Source();
		
		/// <summary>
		/// The year the name was recorded
		/// </summary>
		public int Year { get; set; }

		/// <summary>
		/// The number of children born with the name in the given year
		/// </summary>
		public int Count { get; set; }
    }
}