using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syringe.Core.Configuration;

namespace Syringe.Core.Extensions
{
	public static class VariableExtensions
	{
		public static string ByName(this List<Variable> list, string name)
		{
			Variable item = list.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if (item != null)
			{
				return item.Value;
			}
			else
			{
				return "";
			}
		}
	}
}
