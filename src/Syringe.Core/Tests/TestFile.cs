﻿using System.Collections.Generic;

namespace Syringe.Core.Tests
{
	public class TestFile
	{
		public int Repeat { get; set; }
		public IEnumerable<Test> Tests { get; set; }
		public string Filename { get; set; }
		public List<Variable> Variables { get; set; }

		public TestFile()
		{
			Variables = new List<Variable>();
			Tests = new List<Test>();
		}
	}
}