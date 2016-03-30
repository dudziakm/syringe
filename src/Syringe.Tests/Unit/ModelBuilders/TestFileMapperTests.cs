using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;
using HeaderItem = Syringe.Web.Models.HeaderItem;

namespace Syringe.Tests.Unit.ModelBuilders
{
	[TestFixture]
	public class TestFileMapperTests
	{
		[Test]
		public void Build_should_set_correct_properties_when_model_is_populated()
		{
			var testCaseMapper = new TestFileMapper();
			var build = testCaseMapper.BuildCoreModel(testViewModel);

			Assert.AreEqual(testViewModel.ErrorMessage, build.ErrorMessage);
			Assert.AreEqual(testViewModel.Headers.Count, build.Headers.Count);
			Assert.AreEqual(testViewModel.Position, build.Position);
			Assert.AreEqual(testViewModel.LongDescription, build.LongDescription);
			Assert.AreEqual(testViewModel.Filename, build.Filename);
			Assert.AreEqual(testViewModel.CapturedVariables.Count, build.CapturedVariables.Count);
			Assert.AreEqual(testViewModel.PostBody, build.PostBody);
			Assert.AreEqual(1, build.VerifyNegatives.Count);
			Assert.AreEqual(1, build.VerifyPositives.Count);
			Assert.AreEqual(testViewModel.ShortDescription, build.ShortDescription);
			Assert.AreEqual(testViewModel.Url, build.Url);
			Assert.AreEqual(testViewModel.PostType.ToString(), build.PostType);
			Assert.AreEqual(testViewModel.VerifyResponseCode, build.VerifyResponseCode);
		}

		[Test]
		public void BuildTestCase_should_throw_argumentnullexception_when_testcase_is_null()
		{
			var testCaseMapper = new TestFileMapper();

			Assert.Throws<ArgumentNullException>(() => testCaseMapper.BuildCoreModel(null));
		}

		[Test]
		public void BuildViewModel_should_throw_argumentnullexception_when_testcase_is_null()
		{
			var testCaseMapper = new TestFileMapper();

			Assert.Throws<ArgumentNullException>(() => testCaseMapper.BuildViewModel(null));
		}

		[Test]
		public void BuildTestCases_should_throw_argumentnullexception_when_testcase_is_null()
		{
			var testCaseMapper = new TestFileMapper();

			Assert.Throws<ArgumentNullException>(() => testCaseMapper.BuildTestCases(null));
		}

		[Test]
		public void BuildTestCases_should_return_correct_model_values_from_casecollection()
		{
			// given
			var testFileMapper = new TestFileMapper();
		    var testFileId1 = 1;
		    var testFileId2 = 2;
			var testFile = new TestFile
			{
				Tests = new List<Test>
				{
					new Test {Position = testFileId1, ShortDescription = "Short Description 1", Url = "http://www.google.com"},
					new Test {Position = testFileId2, ShortDescription = "Short Description 2", Url = "http://www.arsenal.com"},
				}
			};

			// when
			IEnumerable<TestViewModel> viewModels = testFileMapper.BuildTestCases(testFile.Tests);

			// then
			Assert.NotNull(viewModels);
			Assert.AreEqual(2, viewModels.Count());

			var firstCase = viewModels.First();
			Assert.AreEqual(testFileId1, firstCase.Position);
			Assert.AreEqual("Short Description 1", firstCase.ShortDescription);
			Assert.AreEqual("http://www.google.com", firstCase.Url);

			var lastCase = viewModels.Last();
			Assert.AreEqual(testFileId2, lastCase.Position);
			Assert.AreEqual("Short Description 2", lastCase.ShortDescription);
			Assert.AreEqual("http://www.arsenal.com", lastCase.Url);
		}

		[Test]
		public void BuildViewModel_should_return_correct_model_values_from_case()
		{
			// given
			var testCaseMapper = new TestFileMapper();

			var test = new Test
			{
                Position = 1,
				ShortDescription = "Short Description",
				Url = "http://www.google.com",
				ErrorMessage = "Error",
				LongDescription = "Long Description",
				Method = "Method",
				PostBody = "PostBody",
				PostType = PostType.GET.ToString(),
				VerifyResponseCode = HttpStatusCode.Accepted,
				Headers = new List<Core.Tests.HeaderItem> { new Core.Tests.HeaderItem() },
				CapturedVariables = new List<CapturedVariable> { new CapturedVariable() },
				VerifyNegatives = new List<Assertion> { new Assertion() },
				VerifyPositives = new List<Assertion> { new Assertion() },
				Filename = "test.xml",
			};

			// when
			TestViewModel testViewModel = testCaseMapper.BuildViewModel(test);

			// then
			Assert.NotNull(testViewModel);
			Assert.AreEqual(test.Position, testViewModel.Position);
			Assert.AreEqual(test.ShortDescription, testViewModel.ShortDescription);
			Assert.AreEqual(test.Url, testViewModel.Url);
			Assert.AreEqual(test.ErrorMessage, testViewModel.ErrorMessage);
			Assert.AreEqual(test.LongDescription, testViewModel.LongDescription);
			Assert.AreEqual(test.PostBody, testViewModel.PostBody);
			Assert.AreEqual(PostType.GET, testViewModel.PostType);
			Assert.AreEqual(test.VerifyResponseCode, testViewModel.VerifyResponseCode);
			Assert.AreEqual(test.Filename, testViewModel.Filename);

			Assert.AreEqual(1, testViewModel.CapturedVariables.Count);
			Assert.AreEqual(2, testViewModel.Assertions.Count);
			Assert.AreEqual(1, test.Headers.Count);
		}

		private TestViewModel testViewModel
		{
			get
			{
				return new TestViewModel
				{
					ErrorMessage = "error",
					Headers = new List<HeaderItem> { new HeaderItem { Key = "Key", Value = "Value" } },
                    Position = 1,
					LongDescription = "long description",
					Filename = "Test.xml",
					CapturedVariables = new List<CapturedVariableItem>() { new CapturedVariableItem { Description = "Description", Regex = "Regex" } },
					PostBody = "Post Body",
					Assertions = new List<AssertionViewModel>() { new AssertionViewModel { Description = "Description", Regex = "Regex", AssertionType = AssertionType.Negative },
							   new AssertionViewModel { Description = "Description", Regex = "Regex", AssertionType = AssertionType.Positive } },
					ShortDescription = "short d3escription",
					Url = "url",
					PostType = PostType.POST,
					VerifyResponseCode = HttpStatusCode.Accepted,
				};
			}
		}
	}
}
