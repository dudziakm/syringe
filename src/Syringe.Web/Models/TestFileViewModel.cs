using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syringe.Web.Models
{
    public class TestFileViewModel
    {
        public IEnumerable<TestCaseViewModel> TestCases { get; set; }
        public int PageNumber { get; set; }
        public int NoOfResults { get; set; }
        public string Filename { get; set; }
        public double PageNumbers { get; set; }
        public List<VariableItem> Variables { get; set; } 
    }
}