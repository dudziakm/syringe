using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syringe.Web.Models
{
    public class TestFileViewModel
    {
        public IEnumerable<TestViewModel> TestCases { get; set; }
        public string TestCaseXml { get; set; }
        public int PageNumber { get; set; }
        public int NoOfResults { get; set; }
        [Required]
        public string Filename { get; set; }
        public double PageNumbers { get; set; }
        public List<VariableViewModel> Variables { get; set; } 
    }
}