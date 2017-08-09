//
// Sample DataMailbox Viewer application. Delivered as Talk2M DataMailbox SDK sample code. Not supported by eWON s.a.
//
// Disclaimer: 
//    Delivered as is. eWON s.a. takes no responsibility for anything bad that can result from the use of this code.
//    Actual mileage may vary. Price does not include tax, title, and license. Some assembly required. Each sold 
//    separately. Batteries not included. Objects in mirror are closer than they appear. If conditions persist, 
//    contact a physician. Keep out of reach of children. Avoid prolonged exposure to direct sunlight. Keep in a cool 
//    dark place.
//    You've been warned!
//
// Third-party components used in these examples:
//    - JSonViewer : http://jsonviewer.codeplex.com
//    - FlUrl (nuGet package)
//

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EPocalipse.Json.Viewer;
using Flurl;
using System.Collections.Specialized;


namespace DMBoxViewer
{
    public partial class FormMain : Form
    {
        class MyWebClient : WebClient
        {   // Enable compression of server response
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return request;
            }

            public string CallApi(string url)
            {
                var credentials = new NameValueCollection();
                credentials.Add("t2mdevid", Properties.Settings.Default.Talk2MDevId);
                credentials.Add("t2maccount", Properties.Settings.Default.Account);
                credentials.Add("t2musername", Properties.Settings.Default.Username);
                credentials.Add("t2mpassword", Properties.Settings.Default.Password);

                var bytes = UploadValues(url, credentials); // Send credentials in POST body
                return Encoding.GetString(bytes);
            }
        }

        Dictionary<string, object> propertiesObjects = new Dictionary<string, object>
        {
            { "getewons", new VerbProperties.getewons() },
            { "getewon", new VerbProperties.getewon() },
            { "getdata", new VerbProperties.getdata() },
            { "syncdata", new VerbProperties.syncdata() },
            { "delete", new VerbProperties.delete() },
            { "clean", new VerbProperties.clean() },
        };

        public FormMain()
        {
            InitializeComponent();
        }

        private void linkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new FormLogin().ShowDialog();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                CheckLogin();

                // Display URL
                txtUrl.Text = 
                    BuildUrl(true) // This display the credentials in the query string for easier display
                    .Replace("&", "\x200B&"); // insert zero-width space to allow nicer line breaking in display
                lblCredentialsWarning.Visible = true;

                // Call API & display result
                using (var webClient = new MyWebClient())
                {
                    jsonViewer.Json = webClient.CallApi(BuildUrl(false)); // false means credentials are passed as POST params (security good practices)
                    jsonViewer.ShowTab(Tabs.Viewer);
                }
            }
            catch (Exception ex)
            {
                jsonViewer.Json = ex.ToString();
                jsonViewer.ShowTab(Tabs.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private string BuildUrl(bool includeCredentials)
        {
            var verb = cbVerb.Text.Trim();

            var url = "https://data.talk2m.com".AppendPathSegment(verb);
            
            if (includeCredentials)
                url = url.SetQueryParams(new {
                    t2mdevid = Properties.Settings.Default.Talk2MDevId,
                    t2maccount = Properties.Settings.Default.Account,
                    t2musername = Properties.Settings.Default.Username,
                    t2mpassword = "***", // This is for display only hence hide password
                });

            try
            {   // Append contents from the property grid
                // url.SetQueryParams(propertiesObjects[verb]); <- Rats! this is nearly enough to do the job. Unfortunately, it doesn't manage bool properties correctly :-(
                
                var propertiesObject = propertiesObjects[verb]; // throws for unknown verbs
                var properties = propertiesObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in properties)
                {
                    var value = p.GetValue(propertiesObject, null);
                    if (p.PropertyType == typeof(bool))
                        value = (bool)value ? "" : null;

                    if (value!=null && (value.ToString()!="" || p.PropertyType == typeof(bool)))
                        url.SetQueryParam(p.Name, value.ToString());
                }
            }
            catch (KeyNotFoundException) { } // Wrong verb typed by user: We don't have any custom param
            
            return url;
        }

        private void CheckLogin()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Talk2MDevId) ||
                string.IsNullOrEmpty(Properties.Settings.Default.Account) ||
                string.IsNullOrEmpty(Properties.Settings.Default.Username) ||
                string.IsNullOrEmpty(Properties.Settings.Default.Password))
            {
                if (new FormLogin().ShowDialog()!=DialogResult.OK)
                    throw new Exception("Please login first.");
            }
        }

        private void btCopyUrl_Click(object sender, EventArgs e)
        {   // Copy URL to clipboard
            Clipboard.Clear();
            var url = txtUrl.Text.Replace("\x200B",""); // Remove the zero-width spaces that we introduced for nicer display

            if (!string.IsNullOrEmpty(url))
            {
                Clipboard.SetText(url);
            }
            else
            {
                Console.Beep();
            }
        }

        private void cbVerb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {   // Display the list of params for the selected DMWeb verb
                propertyGrid.SelectedObject = propertiesObjects[cbVerb.Text.Trim()];
            }
            catch (KeyNotFoundException)
            {   // unknown DMWeb verb: No custom params.
                propertyGrid.SelectedObject = null;
            }
        }
    }
}
