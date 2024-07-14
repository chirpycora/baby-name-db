using CC.BabyNameDb.EFCore;
using CC.BabyNameDb.EFCore.Models;

namespace CC.BabyNameDb.SourceProcessor.Extractors
{
	public interface ISourceExtractor
	{
		public List<string> SupportedKeys { get; }

		Task ExtractSource(string sourcePath);
	}
}