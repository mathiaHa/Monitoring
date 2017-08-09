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

namespace M2WebLibrary
{
    public enum AlarmStatus { Normal = 0, PreTrigger, ALM, ACK, RTN, End, Unknown = 100 }; // NB: "End" appears in alarm history only
    public enum AlarmType { Normal = 0, Hi, Lo, Level, HiHi, LoLo, Unknown = 100 };

    /// <summary>
    /// the Alarm class wraps each realtime (live) or historical alarm record retrieved using GetRealtimeAlarms()
    /// and GetAlarmsHistory()
    /// </summary>
    public struct Alarm
    {
        public bool Live; // true = live alarm (aka realtime alarm) , false = history alarm
        
        public int TagId; // For live alarms only. Always 0 for history alarms.
        public string Tagname;

        public DateTime StartTime; // For live alarms only. Not relevant for history alarms.

        public DateTime StatusTime;
        public AlarmStatus Status;

        public AlarmType Type;
        public string UserAck;

        public string Description;

        public override string ToString()
        {
            if (Live)
            {
                return string.Format("{0}: {1} (at {2}) - {3} is {4} {5}", StartTime, Status, StatusTime, Tagname, Type, !string.IsNullOrEmpty(UserAck) ? "(ack'ed by " + UserAck + ")" : "");
            }
            else
            {
                return string.Format("{0}: {1} - {2} is {3} {4}", StatusTime, Status, Tagname, Type, !string.IsNullOrEmpty(UserAck) ? "(ack'ed by " + UserAck + ")" : "");
            }
        }

        public static AlarmStatus ParseStatus(string status)
        {
            return string.IsNullOrEmpty(status) ? AlarmStatus.Normal : (AlarmStatus)Enum.Parse(typeof(AlarmStatus), status, true);
        }

        public static AlarmType ParseType(string type)
        {
            return string.IsNullOrEmpty(type) ? AlarmType.Normal : (AlarmType)Enum.Parse(typeof(AlarmType), type, true);
        }
    }
}
