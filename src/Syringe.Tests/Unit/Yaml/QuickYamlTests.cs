using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Results;
using YamlDotNet.Serialization;

namespace Syringe.Tests.Unit.Yaml
{
	public class QuickYamlTests
	{
		private string ReadEmbeddedFile(string file)
		{
			string path = string.Format("Syringe.Tests.Unit.Yaml.TestCaseExamples.{0}", file);

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			if (stream == null)
				throw new InvalidOperationException(string.Format("Unable to find '{0}' as an embedded resource", path));

			string result = "";
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}

		private string GetFullExample()
		{
			// Yaml doesn't allow tabs.
			return ReadEmbeddedFile("full.yaml").Replace("\t"," ");
		}

		[Test]
		public void should_do_something()
		{
			// Arrange
			string yaml = GetFullExample();

			var stringReader = new StringReader(yaml);
			var deserializer = new Deserializer();

			// Act
			dynamic container = deserializer.Deserialize(stringReader);

			// Assert - container is one big dictionary full of dictionaries or lists
			Assert.That(container["repeat"], Is.EqualTo("10"));
			Assert.That(container["variables"]["LOGIN1"], Is.EqualTo("bob"));
			Assert.That(container["testcases"][1]["id"], Is.EqualTo("300"));
		}
	}
}
