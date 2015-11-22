namespace Syringe.Core.Security
{
	public interface IUserContext
	{
		string Id { get; set; }
		string TeamName { get; }
		string FullName { get; }
	}
}