using System.Collections.Generic;

namespace Syringe.Core.Domain.Entities.Teamcity
{
    public class Properties
    {
        public override string ToString()
        {
            return "properties";
        }
        public List<Property> Property { get; set; }
    }
}