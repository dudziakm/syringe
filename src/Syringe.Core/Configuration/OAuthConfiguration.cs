namespace Syringe.Core.Configuration
{
	public class OAuthConfiguration
	{
		public string GithubAuthClientId { get; set; }
		public string GithubAuthClientSecret { get; set; }

		public string GoogleAuthClientId { get; set; }
		public string GoogleAuthClientSecret { get; set; }

		public string MicrosoftAuthClientId { get; set; }
		public string MicrosoftAuthClientSecret { get; set; }

		public OAuthConfiguration()
		{
			GithubAuthClientId = "";
			GithubAuthClientSecret = "";
			GoogleAuthClientId = "";
			GoogleAuthClientSecret = "";
			MicrosoftAuthClientId = "";
			MicrosoftAuthClientSecret = "";
		}
	}
}