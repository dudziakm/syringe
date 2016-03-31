interface Test
{
	Id: number;
	Method: string;
	Url: string;
	PostBody: string;
	ErrorMessage: string;
	PostType: string;
	VerifyResponseCode: number;
	Headers: HeaderItem[];
	ParentFilename: string;
	Sleep: number;

	ShortDescription: string;
	LongDescription: string;

	ParseResponses: CapturedVariable[];
	VerifyPositives: Assertion[];
	VerifyNegatives: Assertion[];
}