using CC.BabyNameDb.EFCore;
using CC.BabyNameDb.EFCore.Models;
using CC.BabyNameDb.SourceProcessor.Extractors;

namespace CC.BabyNameDb.SourceProcessor.Extractors
{
	/// <summary>
	/// Extracts data from the US Social Security Administration
	/// Current as of 2024-07-13
	/// </summary>
	public class UsSsaExtractor(BabyNameContext db) : ISourceExtractor
	{
		private readonly BabyNameContext _context = db;

		public List<string> SupportedKeys => new() { "us-ssa-national", "us-ssa-state" };

		public Task ExtractSource(string sourcePath)
		{
			throw new NotImplementedException();
		}
	}
}