﻿namespace Syringe.Core.Security
{
	public interface IUserContext
	{
		string Id { get; set; }
		string FullName { get; }
		string DefaultBranchName { get; }
	}
}