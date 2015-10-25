using System;

namespace Syringe.Web.Models
{
    public class RunningTestCaseViewModel
    {
        public RunningTestCaseViewModel(Guid id, string description)
        {
            Id = id;
            Description = description;
        }

        public Guid Id { get; private set; }
        public string Description { get; private set; }
    }
}