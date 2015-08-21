using NUnit.Framework;
using RestSharp;
using Syringe.Core.Domain.Entities.Teamcity;
using Syringe.Core.Domain.Providers;
using Syringe.Tests.Unit.StubsMocks;

namespace Syringe.Tests.Unit.Providers
{
    [TestFixture]
    public class TeamcityMembershipProviderTests
    {
        [Test]
        public void should_deserialize_user_from_json()
        {
            // given
            var user = "anyoldvalue";
            var expectedUserEmail = "test.account@totaljobsgroup.com";

            var restclientStub = new RestClientMock {RestResponse = new RestResponse {Content = cannedJson}};

            var provider = new TeamcityMembershipProvider(restclientStub);

            // when
            var result = provider.GetUserForUserName(user);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(expectedUserEmail, result.Email);

        }

        [Test]
        public void should_return_team_for_username()
        {
            // given
            var user = "helloyourmywifenowdave";
            var team = "Loki";

            var restclientStub = new RestClientMock {RestResponse = new RestResponse {Content = cannedJson}};

            var provider = new TeamcityMembershipProvider(restclientStub);

            // when
            var result = provider.GetTeamNameForUser(user);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(team, result);
        }

        private string cannedJson =
            "{\"username\":\"test.account\",\"name\":\"An account for testing the api\",\"id\":99,\"email\":\"test.account@totaljobsgroup.com\",\"href\":\"/guestAuth/app/rest/users/id:99\",\"properties\":{\"property\":[{\"name\":\"plugin:auth:nt-domain:nt-domain-login\",\"value\":\"\"},{\"name\":\"plugin:notificator:jabber:jabber-account\",\"value\":\"\"},{\"name\":\"plugin:notificator:piazza:userImage\",\"value\":\"\"},{\"name\":\"plugin:vcs:anyVcs:anyVcsRoot\",\"value\":\"test.account\"}]},\"roles\":{\"role\":[]},\"groups\":{\"count\":2,\"group\":[{\"key\":\"ALL_USERS_GROUP\",\"name\":\"All Users\",\"href\":\"/guestAuth/app/rest/userGroups/key:ALL_USERS_GROUP\",\"description\":\"Contains all TeamCity users\"},{\"key\":\"TEAMLOKI\",\"name\":\"Loki\",\"href\":\"/guestAuth/app/rest/userGroups/key:TEAMLOKI\",\"description\":\"Team Loki\"}]}}";
    }
}
