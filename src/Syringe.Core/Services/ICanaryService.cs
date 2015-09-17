using Syringe.Core.Canary;

namespace Syringe.Core.Services
{
	public interface ICanaryService
	{
		CanaryResult Check();
	}
}