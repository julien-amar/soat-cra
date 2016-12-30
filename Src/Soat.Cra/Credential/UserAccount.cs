namespace Soat.Cra.Credential
{
    public class UserAccount
	{
		public string Password { get; private set; }
		public string Username { get; private set; }

        public UserAccount(string username, string password)
		{
			Username = username;
			Password = password;
		}
	}
}