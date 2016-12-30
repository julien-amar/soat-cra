using Soat.Cra.Credential;

namespace Soat.Cra.Interfaces
{
    public interface ICredentialManager
    {
        UserAccount LoadCredential();

        void SaveCredentials(UserAccount userAccount);

        void DeleteCredentials();
    }
}