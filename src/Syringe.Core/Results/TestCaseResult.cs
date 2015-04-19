using System;
using System.Collections.Generic;

namespace Syringe.Core.Results
{

    //<testcases file=".\testcases\MobileClient.xml">
    //    <testcase id="5">
    //        <description1>Read saved unique job id</description1>
    //        <description2>Read in storedvalues from Setup</description2>
    //        <verifypositive>_S_</verifypositive>
    //        <verifypositive-success>true</verifypositive-success>
    //        <verifypositive97-success>true</verifypositive97-success>
    //        <verifypositive98-success>true</verifypositive98-success>
    //        <verifypositive99-success>true</verifypositive99-success>
    //        <verifyresponsecode-success>true</verifyresponsecode-success>
    //        <verifyresponsecode-message>Passed HTTP Response Code Verification (not in error range)</verifyresponsecode-message>
    //        <success>true</success>
    //        <result-message>TEST CASE PASSED</result-message>
    //        <responsetime>0.016</responsetime>
    //    </testcase>
	public class TestCaseResult
	{
	    public TestCase TestCase { get; set; }
	    public bool Success { get; set; }
	    public string Message { get; set; }
	    public TimeSpan ResponseTime { get; set; }

        public Dictionary<string, bool> VerifyPositiveResults { get; set; }
        public Dictionary<string, bool> VerifyNegativeResults { get; set; }
	}
}
