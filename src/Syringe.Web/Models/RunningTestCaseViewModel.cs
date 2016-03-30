using System;
using System.Collections.Generic;
using Syringe.Core.Tests;

namespace Syringe.Web.Models
{
    public class RunningTestViewModel
    {
        public RunningTestViewModel(Guid id, string description, List<Assertion> assertions )
        {
            Id = id;
            Description = description;
            Assertions = assertions;
        }

        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public List<Assertion> Assertions {get; set; } 
    }
}