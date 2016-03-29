using System;
using System.Collections.Generic;
using Syringe.Core.TestCases;

namespace Syringe.Web.Models
{
    public class RunningTestCaseViewModel
    {
        public RunningTestCaseViewModel(int index, string description, List<VerificationItem> verificationItems)
        {
             Position = index;
            Description = description;
            VerificationItems = verificationItems;
        }

        public int  Position { get; private set; }
        public string Description { get; private set; }
        public List<VerificationItem> VerificationItems {get; set; } 
    }
}