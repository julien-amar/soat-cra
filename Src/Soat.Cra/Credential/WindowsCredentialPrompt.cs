using CredentialManagement;
using Soat.Cra.Interfaces;
using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Soat.Cra.Credential
{
    public class WindowsCredentialPrompt : ICredentialPrompt
    {
        private enum CredUIReturnCodes
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_BAD_ARGUMENTS = 160,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004,
        }

        [DllImport("ole32.dll")]
        private static extern void CoTaskMemFree(IntPtr ptr);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern bool CredUnPackAuthenticationBuffer(int dwFlags,
                                                                   IntPtr pAuthBuffer,
                                                                   uint cbAuthBuffer,
                                                                   StringBuilder pszUserName,
                                                                   ref int pcchMaxUserName,
                                                                   StringBuilder pszDomainName,
                                                                   ref int pcchMaxDomainame,
                                                                   StringBuilder pszPassword,
                                                                   ref int pcchMaxPassword);

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern CredUIReturnCodes CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere,
                                                                     int authError,
                                                                     ref uint authPackage,
                                                                     IntPtr InAuthBuffer,
                                                                     uint InAuthBufferSize,
                                                                     out IntPtr refOutAuthBuffer,
                                                                     out uint refOutAuthBufferSize,
                                                                     ref bool fSave,
                                                                     int flags);



        public DialogResult ShowDialog(CredentialType credentialType, out NetworkCredential networkCredential)
        {
            CREDUI_INFO credui = new CREDUI_INFO();

            credui.pszCaptionText = "Please enter the credentails";
            credui.pszMessageText = "DisplayedMessage";
            credui.cbSize = Marshal.SizeOf(credui);

            uint authPackage = 0;
            IntPtr outCredBuffer = new IntPtr();
            uint outCredSize;
            bool save = false;

            try
            {
                //Show the dialog
                var dialogResult = CredUIPromptForWindowsCredentials(ref credui,
                                                           0,
                                                           ref authPackage,
                                                           IntPtr.Zero,
                                                           0,
                                                           out outCredBuffer,
                                                           out outCredSize,
                                                           ref save,
                                                           (int)credentialType);

                switch (dialogResult)
                {
                    case CredUIReturnCodes.ERROR_CANCELLED:
                        networkCredential = null;
                        return DialogResult.Cancel;

                    case CredUIReturnCodes.ERROR_NO_SUCH_LOGON_SESSION:
                    case CredUIReturnCodes.ERROR_NOT_FOUND:
                    case CredUIReturnCodes.ERROR_INVALID_ACCOUNT_NAME:
                    case CredUIReturnCodes.ERROR_INSUFFICIENT_BUFFER:
                    case CredUIReturnCodes.ERROR_INVALID_PARAMETER:
                    case CredUIReturnCodes.ERROR_INVALID_FLAGS:
                    case CredUIReturnCodes.ERROR_BAD_ARGUMENTS:
                        throw new InvalidOperationException("Invalid properties were specified.", new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new InvalidOperationException("This functionality is not supported by this operating system.", e);
            }

            var usernameBuf = new StringBuilder(1000);
            var passwordBuf = new StringBuilder(1000);
            var domainBuf = new StringBuilder(1000);

            int maxUserName = 1000;
            int maxDomain = 1000;
            int maxPassword = 1000;

            if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxUserName,
                                                domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
            {
                // Clear the memory allocated by CredUIPromptForWindowsCredentials 
                CoTaskMemFree(outCredBuffer);

                networkCredential = new NetworkCredential()
                {
                    UserName = usernameBuf.ToString(),
                    Password = passwordBuf.ToString(),
                    Domain = domainBuf.ToString()
                };

                if (passwordBuf.Length > 0)
                {
                    passwordBuf.Remove(0, passwordBuf.Length);
                }
            }
            else
            {
                networkCredential = null;
            }

            return DialogResult.OK;
        }
    }
}
