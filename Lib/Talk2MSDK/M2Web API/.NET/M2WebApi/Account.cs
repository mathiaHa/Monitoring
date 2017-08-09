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
using System.Runtime.Serialization;

namespace M2WebLibrary
{
    public enum AccountType { Free, Pro };

    [DataContract]
    public class Account
    {
        public string Reference;
        public string Name;
        public string Company;
        public string[] CustomAttributes;
        public Dictionary<string, int> Pools = new Dictionary<string, int>();
        public AccountType Type;
    }
}
