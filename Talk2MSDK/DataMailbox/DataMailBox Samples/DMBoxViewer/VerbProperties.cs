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

namespace VerbProperties
{
    public class getewons
    {
    }

    public class getewon
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class getdata
    {
        public string ewonId { get; set; }
        public string tagId { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public bool   fullConfig { get; set; }
        public string limit { get; set; }
    }

    public class syncdata
    {
        public string lastTransactionId { get; set; }
        public string createTransaction { get; set; }
    }

    public class delete
    {
        public bool all { get; set; }
        public string transactionId { get; set; }
        public string ewonId { get; set; }
        public string to { get; set; }
    }

    public class clean
    {
        public bool all { get; set; }
        public string ewonId { get; set; }
    }
}
