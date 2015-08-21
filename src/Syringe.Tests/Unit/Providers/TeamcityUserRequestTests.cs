using NUnit.Framework;
using Syringe.Core.Domain.Providers;

namespace Syringe.Tests.Unit.Providers
{
    [TestFixture]
    public class TeamcityUserRequestTests
    {
        [Test]
        public void should_create_request_for_user()
        {
            // given
            var user = "helloyourmywifenowdave";
            var teamcityRequest = new TeamcityUserRequest();

            // when
            var result = teamcityRequest.GetRequest(user);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Resource, "/guestAuth/app/rest/users/" + user);
            Assert.AreEqual(result.Method, RestSharp.Method.GET);
           // Assert.AreEqual("application/json", result.Parameters[0].Value);
        }
    }
}
