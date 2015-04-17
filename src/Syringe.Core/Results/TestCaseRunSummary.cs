using System;
using System.Collections;
using System.Collections.Generic;

namespace Syringe.Core
{
    //<test-summary>
    //    <start-time>Fri Apr 17 10:51:59 2015</start-time>
    //    <start-seconds>39119</start-seconds>
    //    <start-date-time>2015-04-17T10:51:59</start-date-time>
    //    <total-run-time>2.395</total-run-time>
    //    <test-cases-run>1</test-cases-run>
    //    <test-cases-passed>0</test-cases-passed>
    //    <test-cases-failed>1</test-cases-failed>
    //    <verifications-passed>6</verifications-passed>
    //    <verifications-failed>1</verifications-failed>
    //    <average-response-time>2.233</average-response-time>
    //    <max-response-time>2.233</max-response-time>
    //    <min-response-time>2.233</min-response-time>
    //    <sanity-check-passed>true</sanity-check-passed>
    //</test-summary>
    public class TestCaseRunSummary
    {
        public List<TestCaseResult> TestCaseResults { get; set; }

        public DateTime StartTime { get; set; }
        public TimeSpan TotalRunTime { get; set; }
        public int TotalCasesRun { get; set; }
        public int TotalCasesPassed { get; set; }
        public int TotalCasesFailed { get; set; }
        public int TotalVerificationsPassed { get; set; }
        public int TotalVerificationsFailed { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
        public TimeSpan MinResponseTime { get; set; }

        public TestCaseRunSummary()
        {
            TestCaseResults = new List<TestCaseResult>();
        }
    }
}