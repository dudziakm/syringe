namespace Syringe.Core.Security.Teamcity.Providers
{
    public interface ITeamcityMembershipProvider
    {
        string GetTeamNameForUser(string userName);
    }
}