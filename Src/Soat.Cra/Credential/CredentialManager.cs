using CredentialManagement;
using Soat.Cra.Interfaces;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Soat.Cra.Credential
{
    public class CredentialManager : ICredentialManager
    {
        private readonly ICredentialPrompt _credentialPrompt;

        private readonly string _credentialKey;

        public CredentialManager(ICredentialPrompt credentialPrompt)
        {
            _credentialPrompt = credentialPrompt;

            _credentialKey = ConfigurationManager.AppSettings["Credential.Store"];
        }

        public void SaveCredentials(UserAccount userAccount)
        {
            var credential = new CredentialManagement.Credential(userAccount.Username, userAccount.Password, _credentialKey, CredentialType.Generic);

            credential.Save();
        }

        public UserAccount LoadCredential()
		{
            CredentialSet set = new CredentialSet(_credentialKey);

            CredentialManagement.Credential credential = null;

            set = set.Load();

            if (set.Count != 0)
			{
				credential = set[0];
			}
			else
			{
                NetworkCredential prompt = null;

               var promptDialogResult = _credentialPrompt.ShowDialog(CredentialType.Generic, out prompt);

                if (promptDialogResult == DialogResult.Cancel)
                {
                    return null;
                }
			}

			return new UserAccount(credential.Username.Split(new char[] { '\\' }).Last(), credential.Password);
		}

        public void DeleteCredentials()
        {
            var credential = new CredentialManagement.Credential()
            {
                Target = _credentialKey,
                Type = CredentialType.Generic
            };

            credential.Delete();
        }
    }
}