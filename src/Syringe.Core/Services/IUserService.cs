namespace Syringe.Core.Services
{
	public interface IUserService
	{
		bool Authenticate(string email, string password);
	}
}