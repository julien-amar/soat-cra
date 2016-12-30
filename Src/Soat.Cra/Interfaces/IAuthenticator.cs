using Soat.Cra.Credential;
using System.Net;

namespace Soat.Cra.Interfaces
{
    public interface IAuthenticator
    {
        bool Authenticate(UserAccount account, out CookieContainer cookieContainer);
    }
}