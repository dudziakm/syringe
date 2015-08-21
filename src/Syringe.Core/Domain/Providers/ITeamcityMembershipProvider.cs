namespace Syringe.Core.Domain.Providers
{
    public interface ITeamcityMembershipProvider
    {
        string GetTeamNameForUser(string userName);
    }
}