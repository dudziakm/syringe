/// <reference path="../typings/Hubs.d.ts" />
interface TestCaseResult
{
	TestCase: Case;
	ActualUrl: string;
	Message: string;
	ResponseTime: string;
	VerifyPositiveResults: VerificationItem[];
	VerifyNegativeResults: VerificationItem[];
	ResponseCodeSuccess: boolean;
	ResponseInfo: Syringe.Core.Http.HttpResponseInfo;
	ExceptionMessage: string;
	Success: boolean;
	VerifyPositivesSuccess: boolean;
	VerifyNegativeSuccess: boolean;
}