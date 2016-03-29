using System.ComponentModel.DataAnnotations;

namespace Syringe.Web.Models
{
    public class TestFileVariableModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

	    public string Environment { get; set; }
    }
}