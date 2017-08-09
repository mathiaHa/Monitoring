//
// Sample M2Web Library consumer program.
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
using M2WebLibrary;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var m2web = new M2Web
                {
                    Talk2MDevId = "TODO",
                    AccountName = "patrice_airicom",
                    Username = "alpiq",
                    Password = "alpiq92521",
                };

                if (m2web.Talk2MDevId.StartsWith("TODO"))
                {
                    throw new Exception("Please set your Talk2M Developer ID and account credentials in the source code: Look for the TODO strings near the top of the program.");
            }

                Console.WriteLine("Retrieving list of eWONs...");
                m2web.LoadEwons();

                Console.WriteLine("Online eWONs:");
                var onlineEwons = m2web.Ewons.Where(ewon => ewon.Status == EwonStatus.Online);
                foreach (var ewon in onlineEwons) 
                    Console.WriteLine(ewon.Name + (!string.IsNullOrEmpty(ewon.Description) ? " ("+ewon.Description+")" : ""));

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                var eWONName = "TODO";
                if (eWONName.StartsWith("TODO"))
                {
                    throw new Exception("Please set your eWON name in the source code: Look for the appropriate TODO string.");
                }

                Console.WriteLine("Retrieving info about eWON '{0}'...", eWONName);
                var myEwon = m2web.Ewons.Where(ewon => ewon.Name == eWONName).First();
                // Alternately, you can retrieve the eWON object from M2Web without getting the whole list of eWONs:
                //var myEwon = m2web.GetEwon(eWONName);

                // Set eWON credentials. We assume adm/adm. But pleaaaase, always modify that default login!
                myEwon.Username = "adm"; // TODO: YOUR eWON LOGIN COMES HERE
                myEwon.Password = "adm";
                if (myEwon.Username == "adm" && myEwon.Password == "adm")
                {
                    Console.WriteLine("WARNING: You may need to set the eWON's login in the source code.");
                }

                Console.WriteLine("Retrieving basic info from eWON...");
                myEwon.LoadInfo();
                Console.WriteLine("S/N: "+myEwon.SerialNumber);
                Console.WriteLine("Firmware: {0} ({1})", myEwon.Firmware, myEwon.FirmwareCodeName);

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                Console.WriteLine("Retrieving tags...");
                myEwon.LoadTags();
                foreach (var tag in myEwon.Tags) Console.WriteLine(tag);

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                Console.WriteLine("Writing 137 into tag A... (NOTE: We assume you have a tag named A in your eWON)");
                myEwon.WriteTag("A", 137);
                Console.WriteLine("Refreshing tags value...");
                myEwon.RefreshTags();
                foreach (var tag in myEwon.Tags) Console.WriteLine(tag);

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                Console.WriteLine("Retrieving history of tag C...");
                var samples = myEwon.GetTagHistory("C");
                foreach (var sample in samples)
                    Console.WriteLine(sample);

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                Console.WriteLine("Retrieving live alarms...");
                var liveAlarms = myEwon.GetRealtimeAlarms();
                foreach (var liveAlarm in liveAlarms)
                    Console.WriteLine(liveAlarm);

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                Console.WriteLine("Retrieving alarms history...");
                var histAlarms = myEwon.GetAlarmsHistory();
                foreach (var histAlarm in histAlarms)
                    Console.WriteLine(histAlarm);

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                Console.WriteLine("Read selected tags:");
                var tags = myEwon.ReadTags(new string[] { "A", "C" });
                foreach (var t in tags) Console.WriteLine(t);

                var A = tags.First();
                Console.WriteLine("Updating {0}: {0}+=10 ...", A.Name);
                myEwon.WriteTag(A.Name, A.FloatValue + 10);

                Console.WriteLine("Strike <Enter> to continue...");
                Console.ReadLine();

                Console.WriteLine("Re-reading tag {0}...", A.Name);
                A = myEwon.ReadTag(A.Name);
                Console.WriteLine(A);
            }
            catch (M2WebException ex)
            {
                Console.WriteLine(string.Format("ERROR {0}: {1}", ex.StatusCode, ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Strike <Enter>...");
                Console.ReadLine();
            }
        }
    }
}
