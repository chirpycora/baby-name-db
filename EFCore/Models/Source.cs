namespace CC.BabyNameDb.EFCore.Models
{
	public class Source
	{
		public int Id { get; set; }

		/// <summary>
		/// Whether the data source is being actively synced
		/// </summary>
		public bool Active { get; set; }

		/// <summary>
		/// The copyright that applies to the data source
		/// </summary>
		public string Copyright { get; set; } = string.Empty;

		/// <summary>
		/// A description of the data source
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Set to true if the data source should not be displayed
		/// </summary>
		public bool Hidden { get; set; }

		/// <summary>
		/// The date and time the data source was last synced
		public DateTime LastSyncedUtc { get; set; }

		/// <summary>
		/// The date of the last update, as specified by the data provider
		public DateTime? LastUpdatedUtc { get; set; }

		/// <summary>
		/// The public URL of the data source
		/// </summary>
		public string PublicUrl { get; set; } = string.Empty;

		/// <summary>
		/// The title of the data source
		/// </summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// Yearly counts of the name
		/// </summary>
		public List<YearCount> YearCounts { get; set; } = new List<YearCount>();
	}
}