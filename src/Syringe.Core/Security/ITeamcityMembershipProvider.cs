namespace Syringe.Core.Security
{
    public interface ITeamLookupProvider
    {
        string GetTeamNameForUser(string userName);
    }
}