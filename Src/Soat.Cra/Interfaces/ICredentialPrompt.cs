using CredentialManagement;
using System.Net;

namespace Soat.Cra.Interfaces
{
    public interface ICredentialPrompt
    {
        DialogResult ShowDialog(CredentialType credentialType, out NetworkCredential networkCredential);
    }
}