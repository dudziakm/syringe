namespace Syringe.Web.Models
{
    public class RunningTestCaseViewModel
    {
        public RunningTestCaseViewModel(int id, string description)
        {
            Id = id;
            Description = description;
        }

        public int Id { get; private set; }
        public string Description { get; private set; }
    }
}