interface TestCaseResult
{
	TestCase: Case;
	ActualUrl: string;
	Message: string;
	ResponseTime: string;
	VerifyPositiveResults: VerificationItem[];
	VerifyNegativeResults: VerificationItem[];
	ResponseCodeSuccess: boolean;
	HttpResponse: HttpResponse;
	ExceptionMessage: string;
	Success: boolean;
	VerifyPositivesSuccess: boolean;
	VerifyNegativeSuccess: boolean;
}