﻿using System;

namespace Syringe.Core.Tasks
{
    public class TaskRequest
    {
        public string Filename { get; set; }
        public string Username { get; set; }
        public string BranchName { get; set; }
        public int? Position { get; set; }
    }
}