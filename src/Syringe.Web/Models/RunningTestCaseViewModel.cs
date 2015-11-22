using System;
using System.Collections.Generic;
using Syringe.Core.TestCases;

namespace Syringe.Web.Models
{
    public class RunningTestCaseViewModel
    {
        public RunningTestCaseViewModel(Guid id, string description, List<VerificationItem> verificationItems )
        {
            Id = id;
            Description = description;
            VerificationItems = verificationItems;
        }

        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public List<VerificationItem> VerificationItems {get; set; } 
    }
}