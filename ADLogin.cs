using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
public class Startup
    {
        public async Task<object> Invoke(object data)
        {
            var input = (IDictionary<string,object>) data;
            return ADLogin.AuthenticateUser((string)input["username"], (string)input["password"]);
        }
    }

public class ADLogin
{
    [DllImport("ADVAPI32.dll", EntryPoint = "LogonUserW", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

    /// <summary>
    /// Parses the string to pull the domain name out.
    /// </summary>
    /// <param name="usernameDomain">The string to parse that must contain the domain in either the domain\username or UPN format username@domain</param>
    /// <returns>The domain name or "" if not domain is found.</returns>
    public static string GetDomainName(string usernameDomain)
    {
        if (string.IsNullOrEmpty(usernameDomain))
        {
            throw (new ArgumentException("Argument can't be null.", "usernameDomain"));
        }
        if (usernameDomain.Contains("\\"))
        {
            int index = usernameDomain.IndexOf("\\");
            return usernameDomain.Substring(0, index);
        }
        else if (usernameDomain.Contains("@"))
        {
            int index = usernameDomain.IndexOf("@");
            return usernameDomain.Substring(index + 1);
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Parses the string to pull the user name out.
    /// </summary>
    /// <param name="usernameDomain">The string to parse that must contain the username in either the domain\username or UPN format username@domain</param>
    /// <returns>The username or the string if no domain is found.</returns>
    public static string GetUsername(string usernameDomain)
    {
        if (string.IsNullOrEmpty(usernameDomain))
        {
            throw (new ArgumentException("Argument can't be null.", "usernameDomain"));
        }
        if (usernameDomain.Contains("\\"))
        {
            int index = usernameDomain.IndexOf("\\");
            return usernameDomain.Substring(index + 1);
        }
        else if (usernameDomain.Contains("@"))
        {
            int index = usernameDomain.IndexOf("@");
            return usernameDomain.Substring(0, index);
        }
        else
        {
            return usernameDomain;
        }
    }


    public static bool AuthenticateUser(string username, string password, string domain = "facultystaff")
    {
        IntPtr token = IntPtr.Zero;
        //userName, domainName and Password parameters are very obvious.
        //dwLogonType (3rd paramter): I used LOGON32_LOGON_INTERACTIVE, This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching logon information for disconnected operations. For more details about this parameter please see http://msdn.microsoft.com/en-us/library/aa378184(VS.85).aspx
        //dwLogonProvider (4th parameter) : I used LOGON32_PROVIDER_DEFAUL, This provider use the standard logon provider for the system. The default security provider is negotiate, unless you pass NULL for the domain name and the user name is not in UPN format. In this case, the default provider is NTLM. For more details about this parameter please see http://msdn.microsoft.com/en-us/library/aa378184(VS.85).aspx
        //phToken (5th parameter): A pointer to a handle variable that receives a handle to a token that represents the specified user. We can use this handler for impersonation purpose. 
        return LogonUser(username, domain, password, 2, 0, ref token);


    }

}

