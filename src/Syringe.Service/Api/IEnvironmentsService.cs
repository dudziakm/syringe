using System.Collections.Generic;
using Syringe.Core.Environment;

namespace Syringe.Service.Api
{
	public interface IEnvironmentsService
	{
		IEnumerable<Environment> List();
	}
}