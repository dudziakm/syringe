using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.TestCases;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;
using HeaderItem = Syringe.Web.Models.HeaderItem;
using ParseResponseItem = Syringe.Web.Models.ParseResponseItem;

namespace Syringe.Tests.Unit.ModelBuilders
{
	[TestFixture]
	public class TestCaseCoreModelBuilderTests
	{
		[Test]
		public void Build_should_throw_argument_null_exception_if_view_model_is_null()
		{
			var testCaseCoreModelBuilder = new TestCaseCoreModelBuilder();
			Assert.Throws<ArgumentNullException>(() => testCaseCoreModelBuilder.Build(null));
		}

		[Test]
		public void Build_should_set_correct_properties_when_model_is_populated()
		{
			var testCaseCoreModelBuilder = new TestCaseCoreModelBuilder();
			var build = testCaseCoreModelBuilder.Build(testCaseViewModel);

			Assert.AreEqual(testCaseViewModel.ErrorMessage, build.ErrorMessage);
			Assert.AreEqual(testCaseViewModel.Headers.Count, build.Headers.Count);
			Assert.AreEqual(testCaseViewModel.Id, build.Id);
			Assert.AreEqual(testCaseViewModel.LogRequest, build.LogRequest);
			Assert.AreEqual(testCaseViewModel.LogResponse, build.LogResponse);
			Assert.AreEqual(testCaseViewModel.LongDescription, build.LongDescription);
			Assert.AreEqual(testCaseViewModel.ParentFilename, build.ParentFilename);
			Assert.AreEqual(testCaseViewModel.ParseResponses.Count, build.ParseResponses.Count);
			Assert.AreEqual(testCaseViewModel.PostBody, build.PostBody);
			Assert.AreEqual(1, build.VerifyNegatives.Count);
			Assert.AreEqual(1, build.VerifyPositives.Count);
			Assert.AreEqual(testCaseViewModel.ShortDescription, build.ShortDescription);
			Assert.AreEqual(testCaseViewModel.Url, build.Url);
			Assert.AreEqual(testCaseViewModel.Sleep, build.Sleep);
			Assert.AreEqual(testCaseViewModel.PostType.ToString(), build.PostType);
			Assert.AreEqual(testCaseViewModel.VerifyResponseCode, build.VerifyResponseCode);
		}


		private TestCaseViewModel testCaseViewModel
		{
			get
			{
				return new TestCaseViewModel
					   {
						   ErrorMessage = "error",
						   Headers = new List<HeaderItem> { new HeaderItem { Key = "Key", Value = "Value" } },
						   Id = 10,
						   LogRequest = true,
						   LogResponse = true,
						   LongDescription = "long description",
						   ParentFilename = "Test.xml",
						   ParseResponses = new List<ParseResponseItem>() { new ParseResponseItem { Description = "Description", Regex = "Regex" } },
						   PostBody = "Post Body",
						   Verifications = new List<VerificationItemModel>() { new VerificationItemModel { Description = "Description", Regex = "Regex", VerifyType = VerifyType.Negative }, 
							   new VerificationItemModel { Description = "Description", Regex = "Regex", VerifyType = VerifyType.Positive } },
						   ShortDescription = "short d3escription",
						   Url = "url",
						   Sleep = 10,
						   PostType = PostType.POST,
						   VerifyResponseCode = HttpStatusCode.Accepted
					   };
			}
		}
	}
}
