interface Case
{
	Id: number;
	Method: string;
	Url: string;
	PostBody: string;
	ErrorMessage: string;
	PostType: string;
	VerifyResponseCode: number;
	LogRequest: boolean;
	LogResponse: boolean;
	Headers: HeaderItem[];
	ParentFilename: string;
	Sleep: number;

	ShortDescription: string;
	LongDescription: string;

	ParseResponses: ParseResponseItem[];
	VerifyPositives: VerificationItem[];
	VerifyNegatives: VerificationItem[];
}