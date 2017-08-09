//
// Sample M2Web Library source code. Delivered as M2Web API sample code. Not supported by eWON s.a.
//
// Disclaimer: 
//    Delivered as is. eWON s.a. takes no responsibility for anything bad that can result from the use of this code.
//    Actual mileage may vary. Price does not include tax, title, and license. Some assembly required. Each sold 
//    separately. Batteries not included. Objects in mirror are closer than they appear. If conditions persist, 
//    contact a physician. Keep out of reach of children. Avoid prolonged exposure to direct sunlight. Keep in a cool 
//    dark place.
//    You've been warned!
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace M2WebLibrary
{
    /// <summary>
    /// The M2Web object is the "entry-point" to the M2Web API.
    /// It lets you login in M2Web, retrieve M2Web information and communicate with your eWONs.
    /// It derives from M2WebBase which implements the basic HTTP communication services
    /// </summary>
    public class M2Web : M2WebBase
    {
        /// <summary>
        /// Guid: Mandatory property. Your own Talk2M Developer ID.
        /// </summary>
        public string Talk2MDevId { get; set; }

        /// <summary>
        /// AccountName, Username and Password are your M2Web login. You must set them prior to call M2Web
        /// methods (using the stateless model). Alternately, you can user the Login() method to use the
        /// stateful model.
        /// </summary>
        public string AccountName { get; set; }
        public string Username { get; set; }
        public string Password { protected get; set; }

        /// <summary>
        /// Stateless: Says if you want to use the stateless or stateful session model. By default, the
        /// stateless model is used (don't forget to intialize AccountName, Username and Password).
        /// You don't really need to modify this property: Either you use the default Stateless model
        /// or you call Login(), which set stateless to false.
        /// </summary>
        public bool Stateless { get; set; }

        /// <summary>
        /// SessionId: Set by the Login() function when using the stateful model
        /// </summary>
        public string SessionId { get; protected set; }

        /// <summary>
        /// Account: General information Talk2M about your Talk2M account. Set by LoadAccountInfo()
        /// </summary>
        public Account Account { get; private set; } // set by LoadAccountInfo()

        /// <summary>
        /// Ewons: The list of eWONs in your Talk2M account. Set by LoadEwons()
        /// </summary>
        public IEnumerable<Ewon> Ewons { get; private set; } // set by LoadEwons()

        public M2Web()
        {
            Stateless = true;
        }

        protected string Credentials
        {   // Builds the URL query string that contains the login/session and guid settings
            get
            {
                if (Talk2MDevId == null) throw new InvalidOperationException("Missing Guid");

                return Stateless ? 
                    string.Format("t2maccount={0}&t2musername={1}&t2mpassword={2}&t2mdeveloperid={3}", AccountName, Username, Password, Talk2MDevId)
                    : string.Format("t2msession={0}&t2mdeveloperid={1}", SessionId, Talk2MDevId);
            }
        }

        protected override string MakeUrl(string hostname, string path)
        {   // Builds the URL for a given path
            // eg.: turns "getewons" into https://m2web.talk2m.com/t2mapi/getewons?t2maccount=...&...

            if (Talk2MDevId == null) throw new InvalidOperationException("Missing Guid");

            var url = "https://" + hostname + "/t2mapi/" + path + (path.Contains('?') ? "&" : "?") + Credentials;
            Debug.WriteLine(url);
            return url;
        }

        protected override string MakeUrl(string path)
        {   // Builds the URL for a given path
            // eg.: turns "getewons" into https://m2web.talk2m.com/t2mapi/getewons?t2maccount=...&...
            return MakeUrl("m2web.talk2m.com", path);
        }

        /// <summary>
        /// Login: Use this method to create an M2Web session. Don't forget to call Logout() at the end of your session.
        /// Alternately you can use the stateless model by simply initializing the AccountName, Username and Password
        /// properties
        /// </summary>
        public void Login(string account, string username, string password)
        {
            if (SessionId != null) throw new Exception("Already logged in.");

            try
            {
                AccountName = account;
                Username = username;
                Password = password;
                Stateless = true; // This is to cheat the call to MakeUrl() below into adding the account name parameter et al to the login URL

                var response = GetJSonObject<Responses.Login>(MakeUrl("login"));
                
                Stateless = false; // We're now in a stateful session
                SessionId = response.t2msession;
            }
            catch (M2WebException ex)
            {
                Debug.WriteLine("Login failed: " + ex);
                throw;
            }
        }

        /// <summary>
        /// Logout: Call this one when you're done with a session created using Login().
        /// Don't call it in stateless mode.
        /// </summary>
        public void Logout()
        {
            if (SessionId == null) throw new Exception("Not logged in.");

            try
            {
                var response = GetJSonObject<Responses.SimpleResponse>(MakeUrl("logout"));
                SessionId = null;
                Stateless = true;
            }
            catch { }
        }

        /// <summary>
        /// GetAccountInfo() retrieves the main attributes of your Talk2M account such as its
        /// name and reference, company, list of pools,...
        /// Note: It does not retrieve the list of eWONs. See GetEwons()
        /// </summary>
        /// <returns>An Account object that contains all this information</returns>
        public Account GetAccountInfo()
        {
            var response = GetJSonObject<Responses.GetAccountInfo>(MakeUrl("getaccountinfo"));
            return new Account
            {
                Reference = response.accountReference,
                Name = response.accountName,
                Company = response.company,
                CustomAttributes = response.customAttributes,
                Pools = response.pools.ToDictionary( p => p.name, p => p.id ),
                Type = response.accountType=="Pro" ? AccountType.Pro : AccountType.Free, // #6329
            };
        }

        /// <summary>
        /// Same as GetAccountInfo() but stores the info in the Account property of this M2Web object
        /// </summary>
        public void LoadAccountInfo()
        {
            Account = GetAccountInfo();
        }

        /// <summary>
        /// GetEwons(): Retrieves the list of eWONs in the Talk2M account, as a list of Ewon objects
        /// that can then be used to communicate with the eWON device
        /// </summary>
        public IEnumerable<Ewon> GetEwons()
        {
            var response = GetJSonObject<Responses.GetEwons>(MakeUrl("getewons"));
            foreach (var ewon in response.ewons) ewon.M2web = this;
            return response.ewons;
        }

        /// <summary>
        /// Same as GetEwons() but stores the list in the Ewons property of this M2Web object
        /// </summary>
        public void LoadEwons()
        {
            Ewons = GetEwons();
        }

        /// <summary>
        /// Same as GetEwons() but returns only the eWONs that are part of the given pool.
        /// Note: The list of pools (with their id) can be retreived using GetAccountInfo()
        /// or LoadAccountInfo()
        /// </summary>
        public IEnumerable<Ewon> GetEwons(int poolId)
        {
            var response = GetJSonObject<Responses.GetEwons>(MakeUrl("getewons?pool="+poolId));
            foreach (var ewon in response.ewons) ewon.M2web = this;
            return response.ewons;
        }

        /// <summary>
        /// Same as GetEwons but returns a single Ewon object for the eWON with the said name
        /// (name is the Talk2M name of the eWON)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Ewon GetEwon(string name)
        {
            var response = GetJSonObject<Responses.GetEwon>(MakeUrl("getewon?name="+name));
            response.ewon.M2web = this;
            return response.ewon;
        }

        public Ewon GetEwon(int id)
        {
            var response = GetJSonObject<Responses.GetEwon>(MakeUrl("getewon?id=" + id));
            response.ewon.M2web = this;
            return response.ewon;
        }

        /// <summary>
        /// Sends a wakeup SMS to the eWON with said Talk2M name
        /// </summary>
        /// <param name="name"></param>
        public void Wakeup(string name)
        {
            GetJSonObject<Responses.SimpleResponse>(MakeUrl("wakeup?name=" + name));
        }

        public void Wakeup(int id)
        {
            GetJSonObject<Responses.SimpleResponse>(MakeUrl("wakeup?id=" + id));
        }

        /// <summary>
        /// Sends a Sendoffline command sto the eWON with said Talk2M name
        /// </summary>
        /// <param name="name"></param>
        public void SendOffline(string name)
        {
            GetJSonObject<Responses.SimpleResponse>(MakeUrl("sendoffline?name=" + name));
        }

        public void SendOffline(int id)
        {
            GetJSonObject<Responses.SimpleResponse>(MakeUrl("sendoffline?id=" + id));
        }
    }
}
