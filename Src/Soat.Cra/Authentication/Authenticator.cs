using SimpleBrowser;
using Soat.Cra.Credential;
using Soat.Cra.Interfaces;
using System.Configuration;
using System.Net;

namespace Soat.Cra.Authentication
{
    public class Authenticator : IAuthenticator
	{
        private readonly string _userAgent;
        private readonly string _home;

        public Authenticator()
		{
            _userAgent = ConfigurationManager.AppSettings["Browser.UserAgent"];
            _home = ConfigurationManager.AppSettings["Browser.Home"];
        }

		public bool Authenticate(UserAccount account, out CookieContainer cookieContainer)
        {
            Browser browser = new Browser()
            {
                UserAgent = _userAgent
            };

            browser.Navigate(_home);

            CheckException(browser);

            SendKeys(browser, "Email", account.Username);
            Click(browser, "next");

            SendKeys(browser, "Passwd", account.Password);
            Click(browser, "signIn");

            CheckException(browser);

            cookieContainer =  browser.Cookies;

            return (browser.Url.AbsoluteUri == _home);
        }

        private void SendKeys(Browser browser, string fieldId, string fieldValue)
        {
            browser.Find(fieldId).Value = fieldValue;
        }

        private void Click(Browser browser, string buttonId)
        {
            browser.Find(ElementType.Button, FindBy.Id, buttonId).Click();
        }

        private void CheckException(Browser browser)
		{
			if (browser.LastWebException != null)
			{
				throw browser.LastWebException;
			}
		}
	}
}