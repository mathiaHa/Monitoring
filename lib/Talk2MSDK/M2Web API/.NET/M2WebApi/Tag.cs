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

namespace M2WebLibrary
{
    public enum Quality { BAD, Uncertain, Invalid, Good }
    public enum DataType { Bool, Float, Int, UInt }

    public class Tag
    {
        public int Id;
        public string Name;

        double rawValue = double.NaN;
        public object Value
        {
            get
            {
                switch (DataType)
                {
                    case M2WebLibrary.DataType.Bool: return rawValue != 0;
                    case M2WebLibrary.DataType.Float: return (float)rawValue;
                    case M2WebLibrary.DataType.Int: return (int)rawValue;
                    case M2WebLibrary.DataType.UInt: return (uint)rawValue;
                    default: throw new InvalidOperationException("Unknown tag DataType");
                }
            }

            set
            {
                if (value is double)
                {   // In case we copy a tag value into another one, this ensures we don't lose any precision in possible type conversions
                    rawValue = (double)value;
                }
                else
                {
                    if (Double.IsNaN(rawValue))
                    {   // Tag was never set before: It's datatype may not have been set either
                        // -> Infer the tag data type from the value written
                        if (value is string && (string.Compare((string)value, Boolean.TrueString, true) == 0 || string.Compare((string)value, "false", true) == 0))
                        {   // Tag is bool
                            DataType = M2WebLibrary.DataType.Bool;
                        }
                    }

                    switch (DataType)
                    {
                        case M2WebLibrary.DataType.Bool:
                            bool b;
                            if (value is bool) b = (bool)value;
                            else if (value is string) b = string.Compare((string)value, Boolean.TrueString, true)==0 || (string)value == "1";
                            else b = Convert.ToBoolean(value);
                            rawValue = b ? 1 : 0;
                            break;

                        case M2WebLibrary.DataType.Float:
                            rawValue = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
                            break;

                        case M2WebLibrary.DataType.Int:
                            rawValue = Convert.ToInt32(value);
                            break;

                        case M2WebLibrary.DataType.UInt:
                            rawValue = Convert.ToUInt32(value);
                            break;

                        default: throw new InvalidOperationException("Unknown tag DataType");
                    }
                }
            }
        }

        /// <summary>
        /// IntValue: Retrieves the value of a tag of type Int as... an int. Saves you a cast + a datatype check
        /// </summary>
        public int IntValue
        {
            get 
            { 
                if (DataType!=M2WebLibrary.DataType.Int) throw new InvalidOperationException("Tag is not an int");
                return (int)Value;
            }
        }

        public uint UIntValue
        {
            get 
            { 
                if (DataType!=M2WebLibrary.DataType.UInt) throw new InvalidOperationException("Tag is not an unsigned int");
                return (uint)Value;
            }
        }

        public float FloatValue
        {
            get
            {
                if (DataType != M2WebLibrary.DataType.Float) throw new InvalidOperationException("Tag is not a float");
                return (float)Value;
            }
        }

        public bool BoolValue
        {
            get
            {
                if (DataType != M2WebLibrary.DataType.Bool) throw new InvalidOperationException("Tag is not a bool");
                return (bool)Value;
            }
        }


        public Quality Quality { get;set; }

        public AlarmStatus AlarmStatus;
        public AlarmType AlarmType;

        public string description;

        public DataType DataType = DataType.Float;

        
        public static Quality GetQualityFromInt(int quality)
        {
            return (Quality)( (quality & 0xC0) >> 6 );
        }

        static int GetIntFromQuality(Quality quality)
        {
            return ((int)quality) << 6;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            if (Id != 0) sb.AppendFormat("({0})", Id);
            sb.Append(" - " + DataType + ": ");
            if (Quality != Quality.Good)
                sb.Append("*" + Quality + "*");
            else
                sb.Append(rawValue!=double.NaN ? Value.ToString() : "<not set>");
            sb.Append(" - Alarm: " + AlarmStatus);
            if (AlarmStatus != M2WebLibrary.AlarmStatus.Normal) sb.Append(" (" + AlarmType + ")");
            if (!string.IsNullOrEmpty(description)) sb.Append(" - " + description);

            return sb.ToString();
        }

        public struct HistorySample
        {
            public DateTime Timestamp;
            public bool IsInitValue;
            public string Value;
            public Quality Quality;

            public override string ToString()
            {
                if (Quality!=M2WebLibrary.Quality.Good)
                    return string.Format("{0} *{1}*", Timestamp, Quality);
                else
                    return string.Format("{0} {1} {2}", Timestamp, Value, IsInitValue ? "(Initial value)" : "");
            }
        }
    }
}
