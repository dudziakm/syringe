using System.Collections.Generic;

namespace Syringe.Web.Models
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int NoOfResults { get; set; }
        public double PageNumbers { get; set; }

        private readonly List<string> _files;

        public IEnumerable<string> Files { get; set; }

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