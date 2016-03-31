interface TestResult
{
	Test: Test;
	ActualUrl: string;
	Message: string;
	ResponseTime: string;
	PositiveAssertions: Assertion[];
	NegativeAssertions: Assertion[];
	ResponseCodeSuccess: boolean;
	HttpResponse: HttpResponse;
	ExceptionMessage: string;
	Success: boolean;
	PositiveAssertionSuccess: boolean;
	NegativeAssertionSuccess: boolean;
}