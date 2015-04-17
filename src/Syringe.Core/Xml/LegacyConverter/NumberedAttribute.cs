namespace Syringe.Core.Xml.LegacyConverter
{
    /// <summary>
    /// A class to represent Webinject's numbered attributes, e.g. verifiypositive1, verifypositive2.
    /// (the author didn't want to use <verifypositives></verifypositives>)
    /// TODO: future versions of Syringe could use this class in some kind of LegacyXmlConverter.
    /// </summary>
    public class NumberedAttribute
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public NumberedAttribute(int index, string name, string value)
        {
            Index = index;
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}{1} - {2}", Name, Index, Value);
        }
    }
}