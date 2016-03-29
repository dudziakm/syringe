using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syringe.Web.Models
{
    public class TestFileViewModel
    {
        public IEnumerable<TestCaseViewModel> TestCases { get; set; }
        public string TestCaseXml { get; set; }
        public int PageNumber { get; set; }
        public int NoOfResults { get; set; }
        [Required]
        public string Filename { get; set; }
        public double PageNumbers { get; set; }
        public List<TestFileVariableModel> Variables { get; set; } 
    }
}