using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Services
{
	public interface ICanaryService
	{
		CanaryResult Check();
	}
}