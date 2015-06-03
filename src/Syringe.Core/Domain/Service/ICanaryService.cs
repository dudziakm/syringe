using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Service
{
	public interface ICanaryService
	{
		CanaryResult Check();
	}
}