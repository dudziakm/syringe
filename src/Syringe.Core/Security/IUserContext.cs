namespace Syringe.Core.Security
{
	public interface IUserContext
	{
		string TeamName { get; }
		string Username { get; }
	}
}