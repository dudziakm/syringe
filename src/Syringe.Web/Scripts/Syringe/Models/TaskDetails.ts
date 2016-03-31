interface TaskDetails
{
    TaskId: number;
    Filename: string;
	Username: string;
	BranchName: string;

	Status: string;
	CurrentIndex: number;
	TotalTests: number;

	Results: TestResult[];
	Errors: string;
}