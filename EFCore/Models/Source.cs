namespace CC.BabyNameDb.EFCore.Models
{
	public class Source
	{
		public int Id { get; set; }

		/// <summary>
		/// The key used to identify the data source, used for processing
		/// </summary>
		public string Key { get; set; } = string.Empty;

		/// <summary>
		/// A description of the data source
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// The date and time the data source was last synced
		public DateTime LastSyncedUtc { get; set; }

		/// <summary>
		/// The date of the last update, as specified by the data provider
		public DateTime? LastUpdatedUtc { get; set; }

		/// <summary>
		/// The copyright that applies to the data source
		/// </summary>
		public string LicenseName { get; set; } = string.Empty;

		/// <summary>
		/// The URL for the license that applies to the data source
		/// </summary>
		public string LicenseUrl { get; set; } = string.Empty;

		/// <summary>
		/// The name of the organization that provides the data
		/// </summary>
		public string OrganizationName { get; set; } = string.Empty;

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