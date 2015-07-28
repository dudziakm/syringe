namespace Syringe.Core.Domain.Services
{
	public interface IUserService
	{
		bool Authenticate(string email, string password);
	}
}