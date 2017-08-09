//
// Sample My Little Historian application. Delivered as Talk2M DataMailbox SDK sample code. Not supported by eWON s.a.
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
//    - DynamicJson : http://dynamicjson.codeplex.com/
//    - FlUrl (nuGet package)
//

using System;
using System.IO;
using System.Net;
using Codeplex.Data;
using Flurl;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Specialized;

namespace MyLittleHistorian
{
    class Program
    {
        class MyWebClient : WebClient
        {   // Enable compression of server response. Highly recommended to speed up the transfer of data
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

        static string Prompt(string prompt)
        {
            Console.Write(prompt + " ? ");
            var value = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(value)) throw new Exception("Login parameters are mandatory.");
            return value;
        }

        static void CheckLogin()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Talk2MDevId))
            {
                Console.WriteLine("Please enter your Talk2M credentials.");
                Console.WriteLine("Warning: For your convenience, your login will be stored in the applications");
                Console.WriteLine("         settings. Please do NOT use a valuable login.");
                Properties.Settings.Default.Talk2MDevId = Prompt("Talk2MDevId");
                Properties.Settings.Default.Account = Prompt("Account");
                Properties.Settings.Default.Username = Prompt("Username");
                Properties.Settings.Default.Password = Prompt("Password");
                string deleteInputString = Prompt("Delete data after synchronization? (yes/no)");
                Properties.Settings.Default.DeleteData = deleteInputString.ToLower().StartsWith("y");
                Properties.Settings.Default.TransactionId = "";
                Properties.Settings.Default.Save();
            }
        }

        static string BuildUrl(string verb, object parameters)
        {
            return "https://data.talk2m.com" // Uses the Flurl library to easily build up URLs
                .AppendPathSegment(verb)
                .SetQueryParams(parameters);
        }

        static void Main(string[] args)
        {
            try
            {
                CheckLogin();

                using (var webClient = new MyWebClient())
                {
                    Console.WriteLine("Collecting the latest data...");

                    bool moreDataAvailable;
                    int samplesCount = 0;
                    string transactionId = Properties.Settings.Default.TransactionId;

                    do
                    {
                        var url = BuildUrl("syncdata", new { createTransaction = "", lastTransactionId = transactionId });
                        var response = webClient.CallApi(url);
                        var data = DynamicJson.Parse(response);
                        transactionId = data.transactionId;

                        Console.WriteLine("Data received. transactionId = {0}", transactionId);

                        // Browse history of eWONs and tags
                        foreach (var ewon in data.ewons)
                        {
                            Directory.CreateDirectory(ewon.name);
                            foreach (var tag in ewon.tags)
                            {
                                try
                                {
                                    Console.WriteLine(Path.Combine(ewon.name, tag.name + ".txt"));

                                    foreach (var sample in tag.history)
                                    {
                                        File.AppendAllText(Path.Combine(ewon.name, tag.name + ".txt"), string.Format("{0},{1}\n", sample.date, sample.value));
                                        samplesCount++;
                                    }
                                }
                                catch (RuntimeBinderException)
                                {   // Tag has no history. If it's in the transaction, it's most likely because it has alarm history
                                    Console.WriteLine("Tag {0}.{1} has no history.", ewon.name, tag.name);
                                }
                            }
                        }

                        Console.WriteLine("{0} samples written to disk", samplesCount);

                        // Flush data received in this transaction
                        if (Properties.Settings.Default.DeleteData) {
                            //Console.WriteLine("Flushing received data from the DataMailbox...");
                            url = BuildUrl("delete", new { transactionId = transactionId });
                            webClient.CallApi(url);
                            Console.WriteLine("DataMailbox flushed.");
                        }

                        //save the transaction id for next run of this program
                        Properties.Settings.Default.TransactionId = transactionId;
                        Properties.Settings.Default.Save();

                        // Did we receive all data?
                        try
                        {
                            moreDataAvailable = data.moreDataAvailable;
                        }
                        catch (RuntimeBinderException)
                        {	// The moreDataAvailable flag is not specified in the server response
                            moreDataAvailable = false;
                        }
                        if (moreDataAvailable)
                            Console.WriteLine("There's more data available. Let's get the next part...");
                    }
                    while (moreDataAvailable);

                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine("Oops!");
                Console.WriteLine(ex.Message);
                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    var jsonError = DynamicJson.Parse(reader.ReadToEnd());
                    Console.WriteLine("DataMailbox Error: " + jsonError.message);
                }
                Console.WriteLine("--------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine("Oops!");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Details:");
                Console.WriteLine(ex);
                Console.WriteLine("--------------------------------------------------------------------");
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Strike Enter to end");
                Console.ReadLine();
            }
        }
    }
}
