﻿using System.IO;
using System.Text;

namespace Syringe.Core.Xml.Writer
{
	internal class Utf8StringWriter : StringWriter
	{
		public Utf8StringWriter(StringBuilder sb) : base(sb)
		{
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}
	}
}