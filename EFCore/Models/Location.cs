namespace CC.BabyNameDb.EFCore.Models
{
	public class Location
	{
		public int Id { get; set; }
		
		/// <summary>
		/// The code for the country (eg. "US"	for United States)
		public string CountryCode { get; set; } = string.Empty;

		/// <summary>
		/// The name of the country
		/// </summary>
		public string CountryName { get; set; } = string.Empty;

		/// <summary>
		/// The code for the state or province (eg. "CA" for California)
		public string? StateOrProvinceCode { get; set; } = null;

		/// <summary>
		/// The name of the state or province
		/// </summary>
		public string? StateOrProvinceName { get; set; } = null;

		/// <summary>
		/// Yearly counts of the name
		/// </summary>
		public List<YearCount> YearCounts { get; set; } = new List<YearCount>();
	}
}