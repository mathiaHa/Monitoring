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
using System.Text.RegularExpressions;
using System.Globalization;

namespace M2WebLibrary
{
    public enum EwonStatus
    {
        Offline,
        Online,
    }

    /// <summary>
    /// The Ewon object wraps the properties of the eWON in its Talk2M account (such as Name, Id, Status,...)
    /// It also helps communicating with the eWON device, such as to read its tags, load a page,...
    /// Those objects are usually instantiated using M2Web.GetEwons(), M2Web.LoadEwons() and M2Web.GetEwon(name)
    /// </summary>
    [DataContract]
    public class Ewon
    {
        /// <summary>
        /// Set by the M2Web method that creates this object.
        /// This property is used internally to communicate with the eWON device.
        /// </summary>
        internal M2Web M2web;

        /// <summary>
        /// Id is the Id of the eWON in the Talk2M account. Not widely used.
        /// </summary>
        [DataMember(Name="id")]
        public int Id { get; set; }

        /// <summary>
        /// Name of the eWON in the Talk2M account.
        /// </summary>
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="status")]
        public string stringStatus
        {   // Member used for serialization: strings "online" and "offline" are converted to/from their matching EwonStatus enum value
            get { return Status.ToString().ToLowerInvariant(); }
            set { Status = (EwonStatus)Enum.Parse(typeof(EwonStatus), value, true); }
        }

        /// <summary>
        /// Communication status of hte eWON: Online or Offline
        /// </summary>
        public EwonStatus Status { get; set; }

        /// <summary>
        /// EncodedName: Same as Name but ready to be used in URLs.
        /// </summary>
        [DataMember(Name = "encodedName")]
        public string EncodedName;

        /// <summary>
        /// Description as in the Talk2M account
        /// </summary>
        [DataMember(Name = "description")]
        public string Description;

        /// <summary>
        /// The custom attributes of this eWON in the Talk2M account
        /// </summary>
        [DataMember(Name="customAttributes")]
        public string[] CustomAttributes;

        [DataMember(Name="m2webServer")]
        public string M2webServer;

        /// <summary>
        /// Username and Password: eWON login (as opposed to Talk2M login). e.g.: adm/adm
        /// This login information is necessary to communication with the eWON
        /// It is passed to the M2Web API as t2mdeviceusername & t2mdevicepassword query string
        /// parameters.
        /// </summary>
        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Firmware, FirmwareCodeName, SerialNumber: Information retrieved from the eWON
        /// using LoadInfo()
        /// </summary>
        public string Firmware { get; private set; }
        public string FirmwareCodeName { get; private set; }
        public string SerialNumber { get; private set; }

        /// <summary>
        /// IsEwon4000, IsFlexy, HasViewOn: Properties based on the Serial Number
        /// </summary>
        public bool IsEwon4000 { get; private set; }
        public bool IsFlexy { get; private set; }
        public bool HasViewOn { get { return IsEwon4000 || IsFlexy; } }

        /// <summary>
        /// List of Tags. Created using LoadTags()
        /// </summary>
        public IEnumerable<Tag> Tags { get; set; }

        /// <summary>
        /// LoadPage: Retrieves a page from the eWON.
        /// </summary>
        /// <param name="path">relative path to the page. e.g.: /usr/mypage.htm</param>
        /// <returns>The contents of the page as a string</returns>
        public string LoadPage(string path)
        {
            return M2web.GetAsString(M2webServer, CreateRelativeUrl(path));
        }

        /// <summary>
        /// Posts a request to an eWON and retrieves the repsonse as string
        /// </summary>
        /// <param name="path">relative path to the page. e.g.: /usr/mypage.htm</param>
        /// <param name="parameters">The contents of the data to be posted. Assumes standard HTML form format: application/x-www-form-urlencoded</param>
        /// <returns>the response of the eWON</returns>
        public string Post(string path, string parameters)
        {
            return M2web.PostAsString(M2webServer, CreateRelativeUrl(path), parameters);
        }

        /// <summary>
        /// LoadXmlPage(): Same as LoadPage() but in addition, this method deserializes the
        /// XML contents into an object of type T
        /// </summary>
        /// <typeparam name="T">Type of class that describes the XML top-level object in the page.</typeparam>
        /// <param name="path">See LoadPage()</param>
        /// <returns>An object of type T containing the contents of the XML page</returns>
        public T LoadXmlPage<T>(string path)
        {
            return M2web.GetXmlObject<T>(M2webServer, CreateRelativeUrl(path));
        }

        /// <summary>
        /// LoadExportBlockDescriptor(): Simlple wrapper around LoadPage that helps building the URL
        /// </summary>
        /// <param name="exportBlockDescriptor">e.g.: $dtTL to retrieve the list of tags</param>
        /// <returns>The contents of the page as a string</returns>
        public string LoadExportBlockDescriptor(string exportBlockDescriptor)
        {
            return LoadPage("rcgi.bin/ParamForm?AST_Param=$" + exportBlockDescriptor);
        }

        /// <summary>
        /// Retrieves eWON properties such as Firmware and SerialNumber
        /// Indirectly, lets the HasViewOn property correctly identify if vows Web services are
        /// available from this eWON, which lets refresh selected tags (instead of all tags only)
        /// </summary>
        public void LoadInfo()
        {
            var response = LoadExportBlockDescriptor("$dtES$flGenInfo");
            var properties = ParseConfig(response);
            SerialNumber = properties["SerNum"];
            var PCode = SerialNumber.Substring(Math.Max(SerialNumber.Length - 2, 0));
            IsEwon4000 = new string[] { "43", "45", "47", "49", "52", "54", "56", "58", "66", "67", "71", "73", "75", "88", "89", "91" }.Contains(PCode);
            IsFlexy = new string[] { "20", "21" }.Contains(PCode);
            Firmware = properties["FwrVerHi"] + "." + properties["FwrVerLo"];
            FirmwareCodeName = properties["CodeName"];
        }

        /// <summary>
        /// GetTags() loads the list of tags with their main properties and instant values.
        /// Note: The DataType is correctly identified only if fullTagInfo is true. Otherwise,
        /// all retrieved tags are assumed to be float.
        /// </summary>
        /// <param name="fullTagInfo"></param>
        /// <returns></returns>
        public IEnumerable<Tag> GetTags(bool fullTagInfo)
        {
            var tags = new List<Tag>();
            var ebd = fullTagInfo ? "$dtTL" : "$dtIV";

            var response = LoadExportBlockDescriptor(ebd + "$ftT");
            var lines = response.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1)) // Skip header line
            {
                var parts = SplitCsvLine(line);
                var tag = new Tag
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    description = fullTagInfo ? parts[2] : null,
                    Quality = Tag.GetQualityFromInt(int.Parse(parts[fullTagInfo ? 57 : 5]) & 0xFF),
                    AlarmStatus = Alarm.ParseStatus(parts[fullTagInfo ? 54 : 3]),
                    AlarmType = Alarm.ParseType(parts[fullTagInfo ? 58 : 4]),
                };
                if (fullTagInfo) tag.DataType = (DataType)Enum.Parse(typeof(DataType), parts[10]);
                tag.Value = parts[fullTagInfo ? 56 : 2]; // Must be set AFTER the DataType is set

                tags.Add(tag);
            }

            return tags;
        }

        /// <summary>
        /// LoadTags: Same as GetTags() but stores the tags in the Tags properties of this Ewon object
        /// </summary>
        public void LoadTags()
        {
            Tags = GetTags(true);
        }


        /// <summary>
        /// ReadTags(): Reads the instant value and alarm status of the said list of tags.
        /// Available only for eWON 4xxx and Flexy (Requries the vows web services)
        /// </summary>
        /// <param name="tags">list of tag names to read</param>
        /// <returns>List of tags. Note: All tags are assumed to be float. The type information is not retrieved</returns>
        public IEnumerable<Tag> ReadTags(string[] tags)
        {
            if (HasViewOn)
            {
                return ReadTagsVows(tags);
            }
            else
            {
                if (SerialNumber == null)
                {   // We don't know the PCode of the eWON, hence we are not sure whether it supports VieWON web services
                    throw new Exception("The eWON model is not identified. Please call LoadInfo() first.");
                }
                else
                {
                    throw new Exception("Individual tags reading not supported by this eWON model.");
                }
            }
        }

        /// <summary>
        /// ReadTag() : wrapper around ReadTags() for a single tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Tag ReadTag(string tag)
        {
            return ReadTags(new string[] { tag }).First();
        }

        /// <summary>
        /// WriteTag: Updates the value of said tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        public void WriteTag(string tag, object value)
        {
            string stringValue;
            if (value is IFormattable)
                stringValue = ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture); // e.g. if it's a double, make sure it's 0.5 instead of 0,5
            else
                stringValue = value.ToString();
            WriteTag(tag, stringValue);
        }

        public void WriteTag(string tag, bool value)
        {
            WriteTag(tag, value ? 1 : 0);
        }

        public void WriteTag(string tag, string value)
        {
            if (HasViewOn)
            {
                WriteTagVows(tag, value);
            }
            else
            {
                WriteTagUTF(tag, value);
            }
        }

        /// <summary>
        /// GetTagHistory: Retrieves the data history for say tag
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns>list of HistorySample containing timestamp and value</returns>
        public IEnumerable<Tag.HistorySample> GetTagHistory(string tagname)
        {
            // Reads the history file for the given tag
            var response = LoadExportBlockDescriptor("$dtHL$ftT$tn" + tagname);

            // Parses the file
            var samples = new List<Tag.HistorySample>();
            var lines = response.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1)) // Skip header line
            {
                var parts = SplitCsvLine(line);
                samples.Add(new Tag.HistorySample
                {
                    Timestamp = ParseDateTime(parts[1]),
                    IsInitValue = parts[2]!="0",
                    Value = parts[3],
                    Quality = (Quality)int.Parse(parts[4]),
                });
            }

            return samples;
        }

        /// <summary>
        /// GetRealtimeAlarms: Retrieves the list of live alarms including for each alarm the tag that
        /// that triggered it, the alarm status, timestamp and more...
        /// </summary>
        /// <returns>List of Alarm objects</returns>
        public IEnumerable<Alarm> GetRealtimeAlarms()
        {
            var alarms = new List<Alarm>();

            var response = LoadExportBlockDescriptor("$dtAR");
            var lines = response.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1)) // Skip header line
            {
                var parts = SplitCsvLine(line);
                alarms.Add(new Alarm
                {
                    Live = true,

                    TagId = int.Parse(parts[0]),
                    Tagname = parts[2],

                    StartTime = ParseDateTime(parts[1]),

                    StatusTime = ParseDateTime(parts[5]),
                    Status = Alarm.ParseStatus(parts[3]),

                    Type = Alarm.ParseType(parts[4]),

                    UserAck = parts[6],
                    Description = parts[7],
                });
            }

            return alarms;
        }

        /// <summary>
        /// GetAlarmsHistory: Retrieves the alarms history of this eWON.
        /// Note: Each alarm is wrapped into the same Alarm object as live alarms but some properties are
        /// not used for history alarms. See Alarm class definition for more info.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Alarm> GetAlarmsHistory()
        {
            var alarms = new List<Alarm>();

            var response = LoadExportBlockDescriptor("$dtAH");
            var lines = response.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1)) // Skip header line
            {
                var parts = SplitCsvLine(line);
                alarms.Add(new Alarm
                {
                    Live = false,

                    // TagId, StartTime: Not specified in alarms history
                    Tagname = parts[1],

                    StatusTime = ParseDateTime(parts[0]),
                    Status = Alarm.ParseStatus(parts[2]),

                    Type = Alarm.ParseType(parts[3]),

                    UserAck = parts[4],
                    Description = parts[5],
                });
            }

            return alarms;
        }

        /// <summary>
        /// RefreshTags: This overload loads or refreshed the Tags properties of this eWON object
        /// </summary>
        public void RefreshTags()
        {
            if (Tags == null)
            {   // Initial refresh. Let's start by loading the list of tags!
                LoadTags();
            }
            else
            {   // Refresh our own list of tags
                RefreshTags(Tags);
            }
        }

        /// <summary>
        /// RefreshTags: This overload refreshes the instant value of an existing set of Tag objects
        /// The interesting point is that if those tags where loaded using either LoadTags() or GetTags(true),
        /// the datatype of each is identified and not assumed to be float.
        /// Therefore, the update of the value respects the correct datatype of the tag.
        /// </summary>
        /// <param name="tags"></param>
        public void RefreshTags(IEnumerable<Tag> tags) // Reads the instant values file from eWON and updates the existing set of tags accordingly
        {
            if (HasViewOn)
            {
                RefreshTagsVows(tags);
                return;
            }

            // Reads the isntant values file from eWON
            var response = LoadExportBlockDescriptor("$dtIV$ftT");

            // Creates a temporary dictionary of tags (why? to optimize lookup of each existing tag vs each line in the file)
            var tempTags = new Dictionary<int, Tag>();
            var lines = response.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1)) // Skip header line
            {
                var parts = SplitCsvLine(line);// line.Split(';');
                tempTags[int.Parse(parts[0])] = new Tag
                {
                    Value = parts[2], // Assume tag type is float -> raw value converted to double -> we don't lose precision even itf it's an int
                    Quality = Tag.GetQualityFromInt(int.Parse(parts[5]) & 0xFF),
                    AlarmStatus = (AlarmStatus)int.Parse(parts[3]),
                    AlarmType = (AlarmType)int.Parse(parts[4]),
                };
            }

            // Update the contents of the existing tags
            Tag tempTag;
            foreach (var tag in tags)
            {
                if (tempTags.TryGetValue(tag.Id, out tempTag))
                {
                    tag.Value = tempTag.Value;
                    tag.Quality = tempTag.Quality;
                    tag.AlarmStatus = tempTag.AlarmStatus;
                    tag.AlarmType = tempTag.AlarmType;
                }
            }
        }

        /// <summary>
        /// Sends a wakeup SMS to the eWON to make it connect Talk2M
        /// </summary>
        public void Wakeup()
        {
            M2web.Wakeup(Name);
        }

        /// <summary>
        /// Sends a 'Go offline' command to the connected eWON to let it interrupt its triggered connection.
        /// </summary>
        public void SendOffline()
        {
            M2web.SendOffline(Name);
        }

        /// <summary>
        /// Retrieve the current status of the eWON from M2Web. Updates the Status property accordingly
        /// </summary>
        /// <returns>The new contents of the Status property</returns>
        public EwonStatus RefreshStatus()
        {
            Status = M2web.GetEwon(Id).Status;
            return Status;
        }

        public override string ToString()
        {
            return Name;
        }

        protected void RefreshTagsVows(IEnumerable<Tag> Tags)
        {
            if (HasViewOn)
            {
                string[] tagnames = Tags.Select(t => t.Name).ToArray();
                var readTags = ReadTagsVows(tagnames);
                foreach (var tag in readTags)
                {
                    Tags.Where(t => t.Name == tag.Name).First().Value = tag.Value;
                }
            }
            else
            {
                if (SerialNumber == null)
                {   // We don't know the PCode of the eWON, hence we are not sure whether it supports VieWON web services
                    throw new Exception("The eWON model is not identified. Please call LoadInfo() first.");
                }
                else
                {
                    throw new Exception("Individual tags reading not supported by this eWON model.");
                }
            }
        }

        protected string CreateRelativeUrl(string path)
        {
            var uri = string.Format("get/{0}/{1}", EncodedName, path.StartsWith("/") ? path.Substring(1) : path);
            if (!string.IsNullOrEmpty(Username))
            {
                var deviceCredentials = string.Format("t2mdeviceusername={0}&t2mdevicepassword={1}", Username, Password);
                uri += (uri.Contains('?') ? '&' : '?') + deviceCredentials;
            }

            return uri; // https://m2web.talk2m.com/t2mapi/<uri>
        }

        protected Dictionary<string, string> ParseConfig(string text) // Parses a set of <key>:<value> set such as comconfig.txt
        {
            var properties = new Dictionary<string, string>();
            var lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Trim().Split(new char[] { ':' }, 2);
                if (parts.Length == 2) properties[parts[0].Trim()] = parts[1].Trim();
            }

            return properties;
        }

        protected List<string> SplitCsvLine(string line)
        {   // fields are ; separated
            // strings are between "
            // supports ; embedded in strings and as much as possible " embedded in strings
            var fields = new List<string>();

            bool stringField = false;
            bool fieldStart = true;
            var field = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                char? nextCh = i == line.Length - 1 ? (char?)null : line[i + 1];

                // start of field ?
                if (fieldStart)
                {
                    fieldStart = false;
                    stringField = ch == '"';
                    if (stringField) continue; // skip opening "
                }

                // End of field ?
                if ((stringField && ch == '"' && (nextCh == null || nextCh == ';')) || (!stringField && ch == ';'))
                {
                    fields.Add(field.ToString());
                    fieldStart = true;
                    field = new StringBuilder();
                    if (stringField) i++;
                    continue;
                }

                // Unicode encoded char (filled in using a browser)
                if (ch == '&' && nextCh == '#')
                {   // e.g.: &#917; = Greek capital epsilon
                    var match = Regex.Match(line.Substring(i), @"^&#(\d{1,7});"); // Unicode codepoints expressed in decimal require max 7 digits
                    if (match.Success)
                    {
                        var codepoint = int.Parse(match.Groups[1].Value);
                        field.Append((char)codepoint);
                        i += match.Length - 1;
                        continue;
                    }
                }

                field.Append(ch);
            }

            if (!fieldStart)
            {   // flush last field
                fields.Add(field.ToString());
            }

            return fields;
        }

        protected IEnumerable<Tag> ReadTagsEBD(string[] tags)
        {
            return GetTags(false).Where(t => tags.Contains(t.Name));
        }

        protected static IEnumerable<Tag> ToTags(Responses.VowsTag[] vowsTags)
        {   // converts tag nodes received from the vows webservice to a tag
            return vowsTags.Select(vowsTag => new Tag { Name = vowsTag.name, Value = vowsTag.value, Quality = Tag.GetQualityFromInt(vowsTag.quality) });
        }

        protected IEnumerable<Tag> ReadTagsVows(string[] tags)
        {
            var response = LoadXmlPage<Responses.GetTags>("rcgi.bin/vows/readVars?_vars=" + string.Join(",", tags));
            return ToTags(response.tags);
        }

        protected Tag WriteTagVows(string tag, string value)
        {
            var response = LoadXmlPage<Responses.GetTags>(string.Format("rcgi.bin/vows/writeVar?_varName={0}&_varValue={1}", tag, value));
            return ToTags(response.tags).First();
        }

        static protected DateTime ParseDateTime(string dateTime)
        {   // textual timestamps in eWON data files are always in european format. 2-digit and 4-digit years are accepted. 
            return DateTime.ParseExact(dateTime, new string[] { "d/M/y H:m:s", "d/M/yyyy H:m:s" }, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        protected void WriteTagUTF(string tag, string value)
        {   // Writes a tag value using the UpdateTagForm form.
            // The drawback of this form is that the response is an HTML page
            var htmlResponse = LoadPage(string.Format("rcgi.bin/UpdateTagForm?TagName={0}&TagValue={1}", tag, value));

            if (htmlResponse.Contains("res_ok")) return; // OK (res_ok is the HTML class of the Success message)

            if (htmlResponse.Contains("res_error")) // res_error is the HTML class of the error message
            {   // the eWON returned an error
                Regex regex = new Regex("ERROR: (?:</span>)?(.*)<p"); // Extract the error message from the HTML contents.
                var match = regex.Match(htmlResponse);
                if (match.Success)
                {
                    throw new Exception(match.Groups[1].Value);
                }
            }

            throw new Exception("Unknown error.");
        }

    }
}
