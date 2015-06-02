namespace Syringe.Core.Repositories
{
    public interface ICaseRepository
    {
        Case GetTestCase(string filename, int caseId);
    }
}