using Soat.Cra.Credential;
using System.Collections.Generic;

namespace Soat.Cra.Interfaces
{
    public interface IMailer
    {
        void Mail(UserAccount account, IEnumerable<string> attachedFiles, int month, int year);
    }
}