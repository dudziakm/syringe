using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Web.Extensions;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void MinutesAndSecondsFormat_should_return_time_in_seconds_if_less_then_a_minute()
        {
            // given + when
            var minutesAndSecondsFormat = new TimeSpan(0, 0, 0, 58).MinutesAndSecondsFormat();

            // then
            Assert.AreEqual("58 seconds", minutesAndSecondsFormat);
        }

        [Test]
        public void MinutesAndSecondsFormat_should_return_time_in_minutes_and_seconds_if_more_then_a_minute()
        {
            // given + when
            var minutesAndSecondsFormat = new TimeSpan(0, 0, 1, 58).MinutesAndSecondsFormat();

            // then
            Assert.AreEqual("1m 58s", minutesAndSecondsFormat);
        }
    }
}
