namespace CC.BabyNameDb.SourceProcessor.Models
{
	public class SourceConfiguration
	{
		/// <summary>
		/// The key used to identify the data source, used for processing
		/// </summary>
		public string Key { get; set; } = string.Empty;

		/// <summary>
		/// A description of the data source
		/// </summary>
		public string DatasetDescription { get; set; } = string.Empty;

		/// <summary>
		/// The title of the data source
		/// </summary>
		public string DatasetName { get; set; } = string.Empty;

		/// <summary>
		/// The name of the file containing the data
		/// </summary>
		public string FileName { get; set; } = string.Empty;

		/// <summary>
		/// The URL of the file containing the data
		/// </summary>
		public string FileUrl { get; set; } = string.Empty;/// <summary>
		
		/// <summary>
		/// The date of the last update, as specified by the data provider
		/// </summary>
		public DateTime? LastUpdatedDate { get; set; }

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
	}
}