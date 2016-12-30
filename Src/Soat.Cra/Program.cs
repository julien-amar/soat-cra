using Colorful;
using LightInject;
using Soat.Cra.Authentication;
using Soat.Cra.Credential;
using Soat.Cra.Input;
using Soat.Cra.Interfaces;
using Soat.Cra.Mailing;
using Soat.Cra.Signing;
using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using Console = Colorful.Console;

namespace Soat.Cra
{
    class Program
    {
        private static void Main(string[] args)
        {
            var container = new ServiceContainer();

            container.Register<IAuthenticator, Authenticator>(new PerContainerLifetime());
            container.Register<ICredentialPrompt, WindowsCredentialPrompt>(new PerContainerLifetime());
            container.Register<ICredentialManager, CredentialManager>(new PerContainerLifetime());
            container.Register<IInputReader, InputReader>(new PerContainerLifetime());
            container.Register<ITemplater, SimpleTemplater>(new PerContainerLifetime());
            container.Register<IMailer, GmailMailer>(new PerContainerLifetime());
            container.Register<ICraDownloader, CraDownloader>(new PerContainerLifetime());
            container.Register<IPdfWatermarker, PdfWatermarker>(new PerContainerLifetime());

            var reader = container.GetInstance<IInputReader>();
            var credentials = container.GetInstance<ICredentialManager>();
            var authentication = container.GetInstance<IAuthenticator>();
            var downloader = container.GetInstance<ICraDownloader>();
            var mailer = container.GetInstance<IMailer>();

            Run(reader, credentials, authentication, downloader, mailer);
        }

        public static void Run(IInputReader reader, ICredentialManager credentials, IAuthenticator authentication, ICraDownloader downloader, IMailer mailer)
        {
            try
            {
                var input = reader.Read();

                Console.WriteLineFormatted("{0} Authenticating", Color.White, new Formatter(">>", Color.Cyan));

                CookieContainer cookieContainer = null;

                var account = credentials.LoadCredential();

                if (account == null || !authentication.Authenticate(account, out cookieContainer))
                {
                    credentials.DeleteCredentials();
                    return;
                }

                credentials.SaveCredentials(account);

                HttpClient client = new HttpClient(new HttpClientHandler()
                {
                    CookieContainer = cookieContainer
                });

                Console.WriteLineFormatted("{0} Watermarking", Color.White, new Formatter(">>", Color.Cyan));

                var cras = downloader.SignAllMissionsCra(client, input.Year, input.Month).Result;

                Console.WriteLineFormatted("{0} Mailing", Color.White, new Formatter(">>", Color.Cyan));

                mailer.Mail(account, cras, input.Month, input.Year);

                Console.WriteFormatted("====== {0} ====== ", Color.White, new Formatter("SUCCESS", Color.Green));

                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Concat("An error occured: ", exception.Message));
            }
        }
    }
}
