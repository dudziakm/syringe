namespace Syringe.Core.TestCases
{
    public class AutomaticVariable
    {
        public string Name { get; set; }
        public string Value { get; set; }

		/// <summary>
		/// Either "variable" (a constant), or "Parse Response"
		/// </summary>
		public string Type { get; set; }
    }
}
