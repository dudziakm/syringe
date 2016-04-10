using System;
using System.IO;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Git
{
	public class GitProvider
	{
		private readonly IConfiguration _configuration;

		public GitProvider(IConfiguration configuration)
		{
			_configuration = configuration;
			EnsureConfiguration(configuration);
		}

		private void EnsureConfiguration(IConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if (configuration.GitConfiguration == null)
				throw new ConfigurationException("The IConfiguration.GitConfiguration is null");

			if (string.IsNullOrEmpty(configuration.TestFilesBaseDirectory))
				throw new ConfigurationException("The TestFilesBaseDirectory is null or empty");

			if (string.IsNullOrEmpty(configuration.GitConfiguration.RepositoryUrl))
				throw new ConfigurationException("The RepositoryUrl is null or empty");

			if (configuration.GitConfiguration.Branches == null || configuration.GitConfiguration.Branches.Count == 0)
				throw new ConfigurationException("There are no Git branches defined (GitConfiguration.Branches is empty/null)");
		}

		public void InitializeAll()
		{
			foreach (string branch in _configuration.GitConfiguration.Branches)
			{
				string repoPath = Path.Combine(_configuration.TestFilesBaseDirectory, branch);
				var cloneOptions = new CloneOptions()
				{
					BranchName = branch,
					CertificateCheck = (certificate, valid, host) => true
				};

				Repository.Clone(_configuration.GitConfiguration.RepositoryUrl, repoPath, cloneOptions);
			}
		}

		public void CommitAndPush(string filename, string name, string email, string branchName)
		{
			string repoPath = _configuration.TestFilesBaseDirectory;
			repoPath = Path.Combine(repoPath, branchName);

			using (var repo = new Repository(repoPath))
			{
				// Stage
				repo.Stage(Path.Combine(repoPath, filename), new StageOptions());

				// Commit
				var signature = new Signature(name, email, new DateTimeOffset(DateTime.Now));
				repo.Commit("Automatic commit by Syringe", signature, signature, new CommitOptions());

				// Push
				PushOptions options = new PushOptions();
				options.CertificateCheck = (certificate, valid, host) => true;

				var credentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) =>
				new UsernamePasswordCredentials()
				{
					Username = _configuration.GitConfiguration.Username,
					Password = _configuration.GitConfiguration.Password
				});
				options.CredentialsProvider = credentialsProvider;

				repo.Network.Push(repo.Branches[branchName], options);
			}
		}

		public void Pull(string branchName)
		{
			string repoPath = _configuration.TestFilesBaseDirectory;
			repoPath = Path.Combine(repoPath, branchName);

			using (var repo = new Repository(repoPath))
			{
				PullOptions options = new PullOptions();
				options.FetchOptions = new FetchOptions()
				{
					CertificateCheck = (certificate, valid, host) => true
				};
				options.MergeOptions = new MergeOptions()
				{
					FileConflictStrategy = CheckoutFileConflictStrategy.Theirs
				};

				var signature = new Signature("nobody", "nobody@nobody.com", new DateTimeOffset(DateTime.Now));
				repo.Network.Pull(signature, options);
			}
		}
	}
}
