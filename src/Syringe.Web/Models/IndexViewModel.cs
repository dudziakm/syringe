using System.Collections.Generic;

namespace Syringe.Web.Models
{
	public class IndexViewModel
	{
		private readonly List<string> _files;

		public IEnumerable<string> Files
		{
			get { return _files; }
		}

		public IndexViewModel()
		{
			_files = new List<string>();
		}

		public void AddFiles(IEnumerable<string> files)
		{
			_files.AddRange(files);
		}
	}
}