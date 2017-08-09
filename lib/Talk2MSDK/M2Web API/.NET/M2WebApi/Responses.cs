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
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace M2WebLibrary.Responses
{
    [DataContract]
    public class M2WebError
    {
        [DataMember] public bool success;
        [DataMember] public int code;
        [DataMember] public string message;
    }

    [DataContract]
    public class GetEwons
    {
        [DataMember] public bool success;
        [DataMember] public Ewon[] ewons;
    }

    [DataContract]
    public class GetEwon
    {
        [DataMember] public bool success;
        [DataMember] public Ewon ewon;
    }

    [DataContract]
    public class Login
    {
        [DataMember] public bool success;
        [DataMember] public string t2msession;
    }

    [DataContract]
    public class SimpleResponse
    {
        [DataMember] public bool success;
    }

    [DataContract]
    public class GetAccountInfo
    {
        [DataMember] public bool success;
        [DataMember] public string accountReference;
        [DataMember] public string accountName;
        [DataMember] public string company;
        [DataMember] public string[] customAttributes;
        [DataMember] public Pool[] pools;
        [DataMember] public string accountType;
    }

    [DataContract]
    public class Pool
    {
        [DataMember] public int id;
        [DataMember] public string name;
    }

    [XmlRoot("server")]
    public class GetTags
    {
        [XmlArray("vars")]
        [XmlArrayItem("var")]
        public VowsTag[] tags;
    }

    public class VowsTag
    {
        [XmlAttribute()]
        public string name;

        [XmlAttribute()]
        public string value;

        [XmlAttribute()]
        public int quality;
    }

}
