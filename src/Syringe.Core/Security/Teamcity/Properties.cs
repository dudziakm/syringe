using System.Collections.Generic;

namespace Syringe.Core.Security.Teamcity
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