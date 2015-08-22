using NUnit.Framework;
using Syringe.Core.Domain.Providers;

namespace Syringe.Tests.Integration.Providers
{
    [TestFixture]
	[Explicit]
    public class TeamcityMembershipProviderTests
    {
        [Test]
        public void should_return_user_for_username()
        {
            // given
            var user = "test.account";

            var provider = new TeamcityMembershipProvider();

            // when
            var result = provider.GetUserForUserName(user);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(2, result.Groups.Group.Count);
        }
    
        [Test]
        public void should_return_team_for_username()
        {
            // given
            var user = "test.account";
            var team = "Loki";

            var provider = new TeamcityMembershipProvider();

            // when
            var result = provider.GetTeamNameForUser(user);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(team, result);
        }
    }
}
