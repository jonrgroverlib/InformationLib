//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InformationLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// InformationLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InformationLib.  If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using InformationLib.Testing;         // for Here
using System;                         // for DateTime
using System.Data.SqlTypes;           // for SqlDateTime
using System.Text.RegularExpressions; // for Regex
using System.Xml;                     // for XmlNode

namespace InformationLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InformationLib.Testing references
{
    // ------------------------------------------------------------------------------------------------
    /// <!-- Date -->
    /// <summary>
    ///      Combines three date formats using their strengths and hiding their weaknesses:
    ///      - CLRformat - good for local/UTC, date pickers, computation, fails with time zones and Nulls
    ///      - SQLformat - good for Null date/times,                      fails with time zones and local/UTC
    ///      - ISOformat - good for XML, local/UTC and time zones,        fails with Nulls and computation
    ///      CLR means DateTime, SQL means SqlDateTime, ISO means ISO8601 string (standard XML format)
    /// </summary>
    /// <example>
    ///      Five standard examples for setting a Date:
    ///      Date date = new Date();
    ///      date.CLRformat = DateTime.UtcNow;
    ///      date.SQLFormat = SqlDateTime.MaxDate;
    ///      date.ISOformat = "2008-10-02T11:59:00-05:00";
    ///      date = new Date(DateTime.Now, DateTimeKind.Local);
    ///      date = Date.Null;
    /// </example>
    /// <remarks>
    ///      Design:
    ///      The Date class is designed to favor 'get' performance over 'set' performance
    ///      The class could have been designed the other way but was was designed to favor
    ///      get performance over set performance, Dates are kept internally in all three
    ///      formats at all times and all conversion processing happens during set,
    ///      If you set by any of these formats, the class handles the other two formats
    /// 
    ///      The CLR format:
    ///      means DateTime, the problem with the DateTime class out of the box is that
    ///      the Equals, Parse and TryParse methods are broken
    ///      Equals: does not take Kind into account
    ///      Parse and TryParse: handle ISO dates improperly
    /// 
    ///      The ISO format:
    ///      is weighted toward the XML DateTimes allowable in SQL Server, so fractional seconds
    ///      are removed
    /// 
    ///      The main problem with the Date class design is that accuracy greater than 1 second is
    ///      not included, this is done to favor SQL server which has problems with accuracies
    ///      greater than one second in ISO formatted dates in the XML data type :(
    /// 
    ///      34 public things
    ///      28 private things
    /// 
    ///      This namespace is a primitive so it should use nothing but System references
    ///      
    /// 
    ///      deprecated but production ready
    /// </remarks>
    public class TimeDate_old
    {

        #region members and constructors


        // ----------------------------------------------------------------------------------------
        /// <!-- IsParsed -->
        private bool _isParsed;
        /// <summary>
        /// 
        /// </summary>
        public bool IsParsed
        {
            get { return _isParsed; }
            set { _isParsed = value; }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CLRFormat -->
        private DateTime _clrFormat;
        /// <summary>
        ///      Sets or gets the datetime in the CLR DateTime format (native to C#)
        /// </summary>
        public DateTime CLRFormat
        {
            get { return _clrFormat; }
            set
            {
                _clrFormat = value;


                // ---------------------------------------------------------------------90
                //  the simple approach fails -- 11:59:59.9999999 can become 12:00:00
                //  SQL has 150 ticks per second and CLR has 10,000,000
                // ---------------------------------------------------------------------90
                DateTime value2 = value;
                if (value2 < DateTime.MaxValue)
                    value2.AddTicks(1L);

                if (_clrFormat >= MinSqlDateTime)
                    _sqlFormat = (SqlDateTime)value2;
                else _sqlFormat = SqlDateTime.Null;


                _isoFormat = CLRtoISO(_clrFormat);
                _isParsed = true;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ISOformat -->
        private String _isoFormat;
        /// <summary>
        ///      Sets or gets the datetime in the ISO 8601 format (the native format of XML)
        /// </summary>
        public String ISOformat
        {
            get { return _isoFormat; }
            set
            {
                _isParsed = false;
                if (String.IsNullOrEmpty(value)) { _isParsed = true;  SetDateNull(); return; }
                if (IsLocalIsoDate(value))  // local ISO
                {
                    _isoFormat = value;
                    _isParsed = true;
                    _clrFormat = StrToCLR(_isoFormat);
                    _sqlFormat = CLRtoSQL(_clrFormat);
                }
                else if (IsCompleteIsoDate(value)) // universal ISO
                {
                    _clrFormat = StrToCLR(value);
                    _isoFormat = CLRtoISO(_clrFormat);
                    _sqlFormat = CLRtoSQL(_clrFormat);
                }
                else
                {
                    _clrFormat = StrToCLR(value);
                    _isoFormat = CLRtoISO(_clrFormat);
                    _sqlFormat = CLRtoSQL(_clrFormat);
                }
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SQLFormat -->
        private SqlDateTime _sqlFormat;
        /// <summary>
        ///      Sets or gets the datetime in the SqlDateTime format (native to SQL Server)
        /// </summary>
        public SqlDateTime SQLFormat
        {
            get { return _sqlFormat; }
            set
            {
                if (value.IsNull) { SetDateNull(); return; }
                _sqlFormat = value;
                _clrFormat = DateTime.SpecifyKind(_sqlFormat.Value, DateTimeKind.Local);
                _isoFormat = _clrFormat.ToString("yyyy-MM-ddTHH:mm:ss");
                _isParsed = true;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Date constructors -->
        /// <summary>
        ///      Sets time to a 'null' value
        /// </summary>
        public TimeDate_old() { SetDateNull(); }


        // ----------------------------------------------------------------------------------------
        //  One input constructors: date/time
        // ----------------------------------------------------------------------------------------
        public TimeDate_old(String      date) { ISOformat = date; }
        public TimeDate_old(DateTime    date) { CLRFormat = date; }
        public TimeDate_old(SqlDateTime date) { SQLFormat = date; }
        public TimeDate_old(object date)
        {
            if (Is.Null(date))  SetDateNull();
            else switch (date.GetType().Name)
            {
                case "String"     : ISOformat = (String)date;      break;
                case "DateTime"   : CLRFormat = (DateTime)date;    break;
                case "SqlDateTime": SQLFormat = (SqlDateTime)date; break;
                default           : ISOformat = date.ToString();   break;
            }
        }


        // ----------------------------------------------------------------------------------------
        //  Two input constructors: date/time, kind
        // ----------------------------------------------------------------------------------------
        public TimeDate_old(DateTime    date, DateTimeKind kind) { CLRFormat = date; SetKind(kind); }
        public TimeDate_old(SqlDateTime date, DateTimeKind kind) { SQLFormat = date; SetKind(kind); }
        public TimeDate_old(String      date, DateTimeKind kind) { ISOformat = date; SetKind(kind); }


        // ----------------------------------------------------------------------------------------
        //  Three input constructors: date, time, kind
        // ----------------------------------------------------------------------------------------
        public TimeDate_old(string dateOnly, string timeOnly, DateTimeKind kind)
        {
            ISOformat = timeOnly + " " + dateOnly;
            if (this.IsNull)  ISOformat = dateOnly;
            SetKind(kind);
        }
        public TimeDate_old(DateTime dateOnly, string timeOnly, DateTimeKind kind)
        {
            ISOformat = timeOnly + " " + dateOnly.ToString("MM/dd/yyyy");
            if (this.IsNull)  CLRFormat = dateOnly;
            SetKind(kind);
        }
        public TimeDate_old(DateTime dateOnly, TimeSpan timeOnly, DateTimeKind kind)
        {
            ISOformat = timeOnly.ToString() + " " + dateOnly.ToString("MM/dd/yyyy");
            if (this.IsNull)  CLRFormat = dateOnly.Date.AddMilliseconds(timeOnly.Milliseconds);
            SetKind(kind);
        }
        public TimeDate_old(TimeDate_old dateOnly, TimeSpan timeOnly, DateTimeKind kind)
        {
            DateTime date = dateOnly._clrFormat;
            CLRFormat = date.Add(timeOnly);
            SetKind(kind);
        }
        public TimeDate_old(string dateOnly, TimeSpan timeOnly, DateTimeKind kind)
        {
            ISOformat = timeOnly.ToString() + " " + dateOnly;
            if (this.IsNull)  ISOformat = dateOnly;
            SetKind(kind);
        }
        public TimeDate_old(DateTime dateOnly, int hours, int minutes, int seconds, DateTimeKind kind)
        {
            CLRFormat = dateOnly.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            SetKind(kind);
        }


        #endregion members and constructors

        #region public methods


        // ----------------------------------------------------------------------------------------
        /// <!-- AddHours -->
        /// <summary>
        ///      Returns a new Date made from the old one with hours adjusted
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public TimeDate_old AddHours(double hours)
        {
            TimeDate_old date = this.Copy();
            if (!date.IsNull)
                date.CLRFormat = date._clrFormat.AddHours(hours);
            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Best -->
        /// <summary>
        ///      Returns the secondary date if the primary date is null
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns></returns>
        public static TimeDate_old Best(TimeDate_old primary, TimeDate_old secondary)
        {
            if (primary == null || primary.IsNull && !secondary.IsNull) return secondary;
            else return primary;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ClrDate -->
        /// <summary>
        ///      Attempts to convert what is sent to it to a DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ClrDate(TimeDate_old date)
        {
            DateTime clrDate;
            if (date == null || date.IsNull)
                clrDate = TimeDate_old.MinClrValue.SetKind(DateTimeKind.Utc).CLRFormat;
            else
                clrDate = date.Local().CLRFormat;
            return clrDate;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ClrDate -->
        /// <summary>
        ///      Attempts to convert what is sent to it to a DateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ClrDate(object obj, DateTime defaultValue)
        {
            if (Is.Null(obj))
                return defaultValue;
            else
            {
                Type type = obj.GetType();
                if (type == typeof(DateTime))
                    return (DateTime)obj;
                if (type == typeof(SqlDateTime) && !((SqlDateTime)obj).IsNull)
                    return (DateTime)(SqlDateTime)obj;

                    
                TimeDate_old date = new TimeDate_old(obj);
                return date.CLRFormat;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Copy -->
        /// <summary>
        ///      Returns an exact copy of this Date by copying its four members
        /// </summary>
        /// <returns></returns>
        public TimeDate_old Copy()
        {
            TimeDate_old date = new TimeDate_old(this._clrFormat);
                                         date._clrFormat = this._clrFormat;
                                         date._isoFormat = this._isoFormat;
            if (this._sqlFormat.IsNull)  date._sqlFormat = SqlDateTime.Null;
            else                         date._sqlFormat = _sqlFormat.Value;
                                         date._isParsed  = this._isParsed;
            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- DateOnly -->
        /// <summary>
        ///      Returns the date portion of the the date, not the time portion or null if null
        /// </summary>
        public TimeDate_old DateOnly
        {
            get
            {
                if (IsNull) return TimeDate_old.Null;
                else return (new TimeDate_old(_clrFormat.Date));
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Tests whether two Dates are equal with a specified tolerance
        ///      defaulting to about a second
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) { return Equals(obj, 1002.0); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Tests whether two Dates are equal with a specified tolerance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msTolerance">tolerance in miliseconds</param>
        /// <returns></returns>
        public bool Equals(object obj, double tolerance)
        {
            if (obj == null)                return false;
            if (ReferenceEquals(this, obj)) return true;
            Type type = obj.GetType();
            switch (type.Name)
            {
                case "DateTime"     :  return Equals(new TimeDate_old((DateTime)obj), tolerance);
                case "Date"         :  return Equals((TimeDate_old)obj,               tolerance);
                case "SqlDateTime"  :
                    if (((SqlDateTime)obj).IsNull && !InSqlRange(_clrFormat)) return true;
                    else return Equals(new TimeDate_old((SqlDateTime)obj), tolerance);
                default : string str = obj.ToString(); return Equals(new TimeDate_old(str), tolerance);
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Compares two Dates with a particular tolerance
        /// </summary>
        /// <param name="date"></param>
        /// <param name="msTolerance">tolerance in miliseconds</param>
        /// <returns></returns>
        public bool Equals(TimeDate_old date, double tolerance)
        {
            if (date == null)                              return false;
            if (ReferenceEquals(this, date))               return true;
            if (this.IsNull && date.IsNull)                return true;
            if (IsoEquality(_isoFormat, date._isoFormat))  return true;


            bool equal = false;
            SqlBoolean extremeMatch = ExtremeClrMatch(_clrFormat, date._clrFormat, tolerance);
            if (extremeMatch.IsNull)
                equal = (_clrFormat.AddMilliseconds(-tolerance) <= date._clrFormat)
                    && (date._clrFormat <= _clrFormat.AddMilliseconds(tolerance));
            else equal = (bool)extremeMatch;
            return equal;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- GetHashCode -->
        /// <summary>
        ///      A really, really, really horrible hash algorithm but it works
        ///      So please, please, please don't use this as a key for any big hash tables
        /// 
        ///      I might hook one up to the SQL format at some point since it has the lowest
        ///      granularity, the problem is boundaries between dates,
        ///      a date on a boundary is hard enough to determine equality,
        ///      carrying this over to the hash algorithm is beyond my ability at the moment,
        ///      I would have to have the hash boundaries the same as the equals boundaries, 
        ///      and this is impossible until I get the equals method settled down, 
        ///      and may still then be extremely difficult
        ///      (note that Equals has a somewhat arbitrary number for tolerance)
        /// 
        ///      Maybe if I used Gray codes :)
        ///      Nope, the problem is that time is a continuum, and there will always be arbitrary
        ///      boundaries between times.
        /// 
        ///      What if I use the finest granularity possible?, say CLR ticks?
        ///      Will there ever be the same times with a different number of CLR ticks?
        ///      yes, so it fails
        /// 
        ///      I need something in between, based on ticks but as large grained as the tolerance specifies.
        ///      That arbitrary tolerance number would have to be factored in to any algorithm.
        ///      This might work, although I will have to do a lot of testing.
        /// 
        ///      What if I based the hash code on the minute of the year and had a special
        ///      hash value for transitions between minutes? I would have to figure out what
        ///      times were 'in' a minute and what times were at a transition. There are about 500,000
        ///      minutes in a year. 1 of every 60 Date objects would be a transition at the current specified tolerance.
        ///      - since Dates are really designed to handle things down to a second level.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 1; }


        // ----------------------------------------------------------------------------------------
        /// <!-- IsNull -->
        /// <summary>
        ///      Returns true if either SQL or CLR formats are null or have values which can be
        ///      interpreted as null
        /// </summary>
        public bool IsNull
        { get { return (_clrFormat <= DateTime.MinValue.AddYears(2)
            || _sqlFormat.IsNull && _clrFormat > MinSqlDateTime); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- MinValue -->
        /// <summary>
        /// 
        /// </summary>
        public static TimeDate_old MinClrValue { get { return new TimeDate_old(DateTime.MinValue,    DateTimeKind.Utc); } }
        public static TimeDate_old MinSqlValue { get { return new TimeDate_old(SqlDateTime.MinValue, DateTimeKind.Utc); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- Now -->
        /// <summary>
        ///      Returns local time Now
        /// </summary>
        public static TimeDate_old Now
        { get { return (new TimeDate_old(DateTime.Now, DateTimeKind.Local)); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- Now_Utc -->
        /// <summary>
        ///      Returns Utc time Now
        /// </summary>
        public static TimeDate_old Now_Utc
        { get { return (new TimeDate_old(DateTime.Now, DateTimeKind.Utc)).Utc(); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- Null -->
        /// <summary>
        ///      Returns a null Date
        /// </summary>
        public static TimeDate_old Null
        {
            get { return (new TimeDate_old()).SetDateNull(); }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Sane -->
        /// <summary>
        /// 
        /// </summary>
        public bool Sane
        {
            get
            {
                bool e1 = SqlNearlyEqual(_sqlFormat, _clrFormat);
                bool e2 = IsoNearlyEqual(_isoFormat, _clrFormat);
                return (e1 && e2);
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SetKind -->
        /// <summary>
        ///      Sets the Kind of the date - does NOT do conversions from local to utc etc.
        /// </summary>
        /// <param name="kind"></param>
        public TimeDate_old SetKind(DateTimeKind kind)
        {
            if (this.IsNull)
            {
                _clrFormat = DateTime.SpecifyKind(_clrFormat, kind);
                if (kind == DateTimeKind.Utc && _isoFormat == "0001-01-01T00:00:00")
                    _isoFormat = "0001-01-01T00:00:00+00:00";
                if (kind == DateTimeKind.Local && _isoFormat == "0001-01-01T00:00:00+00:00")
                    _isoFormat = "0001-01-01T00:00:00";
            }
            else
                CLRFormat = DateTime.SpecifyKind(_clrFormat, kind);
            return this;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SetDateNull -->
        /// <summary>
        ///      Sets the date to null
        /// </summary>
        public TimeDate_old SetDateNull()
        {
            _sqlFormat = SqlDateTime.Null;
            _clrFormat = DateTime.MaxValue;  // safer because this is the same date in both SQL and CLR
            _isoFormat = "";
            _isParsed = true;
            return this;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SqlDate -->
        /// <summary>
        ///      Tries to get a date from a simple node's inner text
        /// </summary>
        /// <param name="node"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static SqlDateTime SqlDate(XmlNode node, SqlDateTime defaultValue)
        {
            if (node == null)  return defaultValue;
            else               return SqlDate(node.InnerText, defaultValue);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SqlDate -->
        /// <summary>
        ///      Attempts to convert what is sent to it to a DateTime, allowing nulls because it
        ///      returns a SqlDateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue">SqlDate.Null allowed</param>
        /// <returns></returns>
        public static SqlDateTime SqlDate(object obj, SqlDateTime defaultValue)
        {
            if (Is.Null(obj))
                return defaultValue;
            else if (obj.GetType() == typeof(SqlDateTime))
                return (SqlDateTime)obj;
            else if (obj.GetType() == typeof(TimeDate_old))
                return ((TimeDate_old)obj).SQLFormat;
            else
            {
                TimeDate_old date = new TimeDate_old(obj);
                if (date.IsParsed)
                    return date.SQLFormat;
                else
                    return defaultValue;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Converts the CLR version of the the date to a string or blank if it is null
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.IsNull) return "";
            if (_clrFormat.Kind == DateTimeKind.Utc)   return _clrFormat.ToString() + " Utc";
            if (_clrFormat.Kind == DateTimeKind.Local) return _clrFormat.ToString() + " Local";
            return _clrFormat.ToString();
        }
        public string ToString(string format)
        { if (this.IsNull) return ""; else return _clrFormat.ToString(format); }


        // ----------------------------------------------------------------------------------------
        /// <!-- TruncateToSecond -->
        /// <summary>
        ///      Removes anything below the accuracy of a second
        /// </summary>
        /// <returns></returns>
        public TimeDate_old TruncateToSecond()
        {
            TimeDate_old date;
            if (this.IsNull)
                date = Null;
            else
            {
                date = new TimeDate_old();
                DateTime dt = CLRFormat;
                date = new TimeDate_old(dt.Date, dt.Hour, dt.Minute, dt.Second, dt.Kind);
            }
            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Local -->
        /// <summary>
        ///      Returns a copy of the local time version of this object, if it is already local it
        ///      makes no change, if it is utc or unspecified, assumes that it is utc and returns local
        /// </summary>
        /// <returns>self</returns>
        public TimeDate_old Local()
        {
            TimeDate_old date = this.Copy();
            if (!date.IsNull && date._clrFormat.Kind != DateTimeKind.Local)
                date.CLRFormat = UtcToLocal(_clrFormat);
            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Utc -->
        /// <summary>
        ///      Returns the utc version of this object, if it is already utc it makes no change,
        ///      if it is local or unspecified, assumes that it is local and returns utc
        /// </summary>
        /// <returns>self</returns>
        public TimeDate_old Utc()
        {
            DateTime utcDateTime;
            TimeDate_old date = this.Copy();
            if (this.IsNull)
                return TimeDate_old.Null;
            if (date._clrFormat.Kind != DateTimeKind.Utc)
            {
                TimeSpan span = TimeZone.CurrentTimeZone.GetUtcOffset(_clrFormat);
                if (_clrFormat.Ticks - span.Ticks < DateTime.MaxValue.Ticks)
                    utcDateTime = _clrFormat - span;
                else utcDateTime = DateTime.MaxValue;
                date.CLRFormat = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            return date;
        }


        // ----------------------------------------------------------------------------------------
        //  Various Operators
        // ----------------------------------------------------------------------------------------
        public static bool operator <=(TimeDate_old lhs, TimeDate_old rhs) { return ((lhs == rhs) || (lhs < rhs)); }
        public static bool operator >=(TimeDate_old lhs, TimeDate_old rhs) { return ((lhs == rhs) || (lhs > rhs)); }
        public static bool operator !=(TimeDate_old lhs, TimeDate_old rhs) { return (!(lhs == rhs)); }


        public static bool operator < (TimeDate_old lhs, TimeDate_old rhs)
        {
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;
            else return (lhs.CLRFormat < rhs.CLRFormat);
        }
        public static bool operator > (TimeDate_old lhs, TimeDate_old rhs)
        {
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;
            else return (lhs.CLRFormat > rhs.CLRFormat);
        }
        public static bool operator ==(TimeDate_old lhs, TimeDate_old rhs)
        {
            if (ReferenceEquals(lhs, null))
                return (ReferenceEquals(rhs, null));
            else return lhs.Equals(rhs);
        }


        #endregion public methods

        #region private methods


        // ----------------------------------------------------------------------------------------
        /// <!-- Best -->
        /// <summary>
        ///      Returns the date that is not null
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns></returns>
        public static SqlDateTime Best(SqlDateTime primary, SqlDateTime secondary)
        {
            if (primary.IsNull
                || primary.Value == SqlDateTime.MinValue && !secondary.IsNull)
                return secondary;
            else
                return primary;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CLRtoISO -->
        /// <summary>
        ///      Converts a regular DateTime (CLR) to an ISO 8601 date time
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static String CLRtoISO(DateTime date)
        {
            String iso = "";
            iso = date.ToString("yyyy-MM-ddTHH:mm:ss");
            if (date.Kind == DateTimeKind.Utc)
                iso += "+00:00";
            
            return iso;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CLRtoSQL -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrFormat"></param>
        /// <returns></returns>
        private static SqlDateTime CLRtoSQL(DateTime clrFormat)
        {
            SqlDateTime sql;
            if (clrFormat.Year >= 1753)
                sql = (SqlDateTime)clrFormat;
            else sql = SqlDateTime.Null;
            return sql;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ExtremeClrMatch -->
        /// <summary>
        ///      Returns null unless both dates are very close to a DateTime extreme
        /// </summary>
        /// <remarks>
        ///      I stuck the Clr in the name so the tests scripts would be able to
        ///      distinguish it from ExtremeSqlMatch
        /// </remarks>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static SqlBoolean ExtremeClrMatch(DateTime A, DateTime B, double millisecondOffset)
        {
            SqlBoolean extremeMatch = SqlBoolean.Null;


            // -------------------------------------------------------------------------90
            //  Check for both dates close to max date
            // -------------------------------------------------------------------------90
            DateTime max = DateTime.MaxValue.AddMilliseconds(-Math.Abs(millisecondOffset));
            if (A >= max && B >= max) extremeMatch = true;
            else if (A >= max || B >= max) extremeMatch = false;


            // -------------------------------------------------------------------------90
            //  Check for both dates close to min date
            // -------------------------------------------------------------------------90
            DateTime min = DateTime.MinValue.AddMilliseconds(Math.Abs(millisecondOffset));
            if (A <= min && B <= min) extremeMatch = true;
            else if (A <= min || B <= min) extremeMatch = false;


            return extremeMatch;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ExtremeSqlMatch -->
        /// <summary>
        ///      Returns null unless both dates are very close to a SqlDateTime extreme
        /// </summary>
        /// <remarks>
        ///      I stuck the Sql in the name so the tests scripts would be able to
        ///      distinguish it from ExtremeClrMatch
        /// </remarks>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static SqlBoolean ExtremeSqlMatch(SqlDateTime A, SqlDateTime B, double millisecondOffset)
        {
            SqlBoolean extremeMatch = SqlBoolean.Null;


            // -------------------------------------------------------------------------90
            //  Check for both dates close to Sql max date
            // -------------------------------------------------------------------------90
            DateTime dmax = ((DateTime)SqlDateTime.MaxValue);
            SqlDateTime max = (SqlDateTime)dmax.AddMilliseconds(-Math.Abs(millisecondOffset)-7);
            if (A >= max && B >= max) extremeMatch = true;
            else if (A >= max || B >= max) extremeMatch = false;


            // -------------------------------------------------------------------------90
            //  Check for both dates close to Sql min date
            // -------------------------------------------------------------------------90
            SqlDateTime min = (SqlDateTime)MinSqlDateTime.AddMilliseconds(Math.Abs(millisecondOffset)+7);
            if (A <= min && B <= min) extremeMatch = true;
            else if (A <= min || B <= min) extremeMatch = false;
            return extremeMatch;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- InOrder -->
        /// <summary>
        ///      Returns whether A, B and C are in order  (A <= B <= C)
        /// </summary>
        /// <param name="lo"></param>
        /// <param name="mid"></param>
        /// <param name="hi"></param>
        /// <returns></returns>
        public static bool InOrder(long lo, long mid, long hi) { return (lo <= mid && mid <= hi); }


        // ----------------------------------------------------------------------------------------
        /// <!-- InSqlRange -->
        /// <summary>
        ///      Determines whether a DateTime is in the proper range for a SqlDateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static bool InSqlRange(DateTime date)
        { return (MinSqlDateTime <= date && date.Ticks < ((DateTime)SqlDateTime.MaxValue).Ticks + 30000); }


        private static bool IsCompleteIsoDate(string date) { return IsoDateOk(date) && IsoTimeOk(date) && IsoZoneOk(date) && Regex.IsMatch(date, @"^.{19}(\.[0-9]+)?(Z|[-+]..:..)$"); }
        private static bool IsLocalIsoDate   (string date) { return IsoDateOk(date) && IsoTimeOk(date) && Regex.IsMatch(date, "^.{19}$"); }
        private static bool IsSomeIsoDate    (string date) { return IsoDateOk(date) && IsoTimeOk(date) && IsoZoneOk(date) && Regex.IsMatch(date, @"^.{19}(\.[0-9]+)?(Z|[-+]..:..)?$"); }
        private static bool IsUtcIsoDate     (string date) { return IsoDateOk(date) && IsoTimeOk(date) && IsoZoneOk(date) && Regex.IsMatch(date, @"^.{19}(\.[0-9]+)?(Z|[-+]00:00)$"); }
        private static bool IsoDateOk        (string date) { bool ok = Regex.IsMatch(date, "^(000[1-9]|00[1-9][0-9]|0[1-9][0-9]{2}|[1-9][0-9]{3})-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])"); return ok; }
        private static bool IsoTimeOk        (string date) { bool ok = Regex.IsMatch(date, @"^.{10}T([01][0-9]|2[0123]):[0-5][0-9]:[0-5][0-9](\.[0-9]+)?"); return ok; }
        private static bool IsoZoneOk        (string date) { bool ok = Regex.IsMatch(date, @"^.{19}(\.[0-9]+)?(Z|[-+]0[0-9]:[0-5][0-9]|[-+]1[0-4]:[0-5][0-9])?$"); return ok; }


        // ----------------------------------------------------------------------------------------
        /// <!-- IsoEquality -->
        /// <summary>
        ///      Determines whether two iso dates are equal
        /// </summary>
        /// <param name="isoDateTime"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsoEquality(String isoDate1, String isoDate2)
        {
            if (!IsCompleteIsoDate(isoDate1) || !IsCompleteIsoDate(isoDate2))
                return (isoDate1 == isoDate2);
            DateTime date1 = ISOtoCLR(isoDate1);
            DateTime date2 = ISOtoCLR(isoDate2);
            return date2.Equals(date1);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ISOtoCLR -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isoDateTime"></param>
        /// <returns></returns>
        private static DateTime ISOtoCLR(String isoDateTime)
        {
            // -------------------------------------------------------------------------90
            //  Calculate simple (unspecified time zone) date
            // -------------------------------------------------------------------------90
            int years   = TreatAs.IntValue(isoDateTime.Substring(0, 4), 1);
            int months  = TreatAs.IntValue(isoDateTime.Substring(5, 2), 1);
            int days    = TreatAs.IntValue(isoDateTime.Substring(8, 2), 1);
            int hours   = TreatAs.IntValue(isoDateTime.Substring(11, 2), 1);
            int minutes = TreatAs.IntValue(isoDateTime.Substring(14, 2), 1);
            int seconds = TreatAs.IntValue(isoDateTime.Substring(17, 2), 1);


            DateTime date = new DateTime(years, months, days, hours, minutes, seconds);


            if (IsCompleteIsoDate(isoDateTime))
            {
                // -------------------------------------------------------------------------90
                //  Calculate span
                // -------------------------------------------------------------------------90
                string zone = ISOtoTimeZone(isoDateTime);
                TimeSpan span = TimeZoneToSpan(zone);
                if (!(-900 < span.TotalMinutes && span.TotalMinutes < 900)) // 899: maximum allowable number of minutes for a time zone offset from UTC in ISO
                    throw new ArgumentOutOfRangeException("invalid timezone: " + zone);


                // -------------------------------------------------------------------------90
                //  Add span
                // -------------------------------------------------------------------------90
                long ticks = date.Ticks - span.Ticks;
                if (ticks > DateTime.MaxValue.Ticks) date = DateTime.MaxValue;
                else if (ticks > 0) date = date.Add(-span);
                else date = DateTime.MinValue;


                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }
            else
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);


            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ISOtoUTCtoCLR -->
        /// <summary>
        ///      Converts an ISO date/time string to a Utc DateTime
        /// </summary>
        /// <param name="isoDate"></param>
        /// <returns></returns>
        private static DateTime ISOtoUTCtoCLR(String isoDate)
        {
            string isoBody = Regex.Replace(isoDate, "(Z|[-+][01][0-9]:[012345][0-9])$", "");
            string isoZone = ISOtoTimeZone(isoDate);
            TimeSpan span = TimeZoneToSpan(isoZone);
            DateTime date1 = DateTime.Parse(isoBody).Add(-span);
            DateTime date2 = DateTime.SpecifyKind(date1, DateTimeKind.Utc);
            return date2;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ISOtoTimeZone -->
        /// <summary>
        ///      Returns the time zone from the ISO datetime string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ISOtoTimeZone(String value)
        {
            string timeZone;
            if (IsSomeIsoDate(value))
                timeZone = Regex.Replace(value, @"^.{19}(\.[0-9]+)?", "");
            else
                timeZone = "";
            return timeZone;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ISOtoTimeZoneSpan -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isoDate"></param>
        /// <returns></returns>
        private static TimeSpan ISOtoTimeZoneSpan(String isoDate)
        {
            TimeSpan ts = TimeZoneToSpan(ISOtoTimeZone(isoDate));
            if (ts == TimeSpan.MaxValue || ts == TimeSpan.MinValue)
                ts = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Parse(isoDate));
            return ts;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- LowSqlDateTime -->
        /// <summary>
        /// 
        /// </summary>
        //private static DateTime LowSqlDateTime
        //{ get { return ((DateTime)SqlDateTime.MinValue).AddYears(2); } }
        private static DateTime MinSqlDateTime
        { get { return (DateTime)SqlDateTime.MinValue; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- IsoNearlyEqual -->
        /// <summary>
        ///      Check to see if String (ISO) and clr date times are within a second
        /// </summary>
        /// <param name="isoDateTime"></param>
        /// <param name="clrDateTime"></param>
        /// <returns></returns>
        private static bool IsoNearlyEqual(String isoDateTime, DateTime clrDateTime)
        {
            if (string.IsNullOrEmpty(isoDateTime) && (clrDateTime == DateTime.MaxValue))
                return true;
            if (!IsSomeIsoDate(isoDateTime))
                throw new FormatException("This is not an ISO formatted datetime: " + isoDateTime);
            bool equal = false;


            // -------------------------------------------------------------------------90
            //  Convert the iso date to DateTime
            // -------------------------------------------------------------------------90
            DateTime iso = ISOtoCLR(isoDateTime);
            switch (clrDateTime.Kind)
            {
                case DateTimeKind.Local : if (iso.Kind == DateTimeKind.Utc)   iso = iso.ToLocalTime();     break;
                case DateTimeKind.Utc :   if (iso.Kind == DateTimeKind.Local) iso = iso.ToUniversalTime(); break;
                case DateTimeKind.Unspecified :                                                            break;
            }


            // -------------------------------------------------------------------------90
            //  Perform the comparison
            // -------------------------------------------------------------------------90
            long isoTicks = iso.Ticks;
            long clrTicks = clrDateTime.Ticks;
            long offset = 10000001;
            equal = (InOrder(isoTicks - offset, clrTicks, isoTicks + offset));


            return equal;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- MiddleTime -->
        /// <summary>
        ///      Exactly half way between min and max dateTime
        /// </summary>
        public static DateTime MiddleTime
        { get { return new DateTime((DateTime.MaxValue - DateTime.MinValue).Ticks / 2L); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- SqlNearlyEqual -->
        /// <summary>
        ///      Determines whether a SqlDateTime value and a CLR DateTime are essentially equal
        /// </summary>
        /// <remarks>
        ///      Because CLR DateTime has a much higher precision than SqlDateTime,
        ///      A CLR DateTime has 10,000 ticks per second while a SqlDateTime has 150,
        ///      This can amount to a comparison rounding error of as much as 6.7 milliseconds,
        ///      For example DateTime 11:59:59.9999999 can become SqlDateTime 12:00:00
        /// 
        ///      Needs work - is showing equality at times it shouldn't
        /// </remarks>
        /// <param name="sqlDateTime"></param>
        /// <param name="clrDateTime"></param>
        /// <returns></returns>
        private static bool SqlNearlyEqual(SqlDateTime sqlDateTime, DateTime clrDateTime)
        {
            if (clrDateTime < MinSqlDateTime) return sqlDateTime.IsNull;
            if (clrDateTime == DateTime.MaxValue && sqlDateTime.IsNull)  return true;
            double offset = 6.6; // milliseconds


            switch (ExtremeSqlMatch(sqlDateTime, (SqlDateTime)clrDateTime, offset).ToString())
            {
                case "True"  : return true;
                case "False" : return false;
                case "Null"  : //  normal case
                    bool highEnough = (DateTime)sqlDateTime >= clrDateTime.AddMilliseconds(-offset);
                    bool lowEnough  = (DateTime)sqlDateTime <= clrDateTime.AddMilliseconds(offset);
                    return (highEnough && lowEnough);
            }


            throw new NotSupportedException("this line will never run");
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- StrToCLR -->
        /// <summary>
        ///      Converts a string to a DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private DateTime StrToCLR(String value)
        {
            _isParsed = true;
            DateTime date;
            if      (IsCompleteIsoDate(value))  date = ISOtoUTCtoCLR(value);
            else if (IsSomeIsoDate(value))      date = ISOtoCLR(value);
            else
            {
                //string timeZone = TimeZoneOfIso(value);
                if (Regex.IsMatch(value, "^[0-2][0-9][0-9][0-9][01][0-9][0-3][0-9]$"))
                    value = Regex.Replace(value, "^(....)(..)(..)$", "$1-$2-$3T00:00:00");
                if (DateTime.TryParse(value, out date))
                {
                    date = DateTime.SpecifyKind(date, DateTimeKind.Local);
                    _isParsed = true;
                }
                else
                {
                    date = DateTime.MinValue;
                    _isParsed = false;
                }
            }
            return date;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- TimeZoneToSpan -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tz"></param>
        /// <returns></returns>
        private static TimeSpan TimeZoneToSpan(String isoTimeZone)
        {
            if (isoTimeZone.Length > 6)
                throw new ArgumentException("TimeZoneToSpan error"
                    + " - this is not a time zone in a valid format: " + isoTimeZone);
            TimeSpan ts = new TimeSpan(0);
            if (string.IsNullOrEmpty(isoTimeZone) || isoTimeZone == "Z" || isoTimeZone == "+00:00" || isoTimeZone == "-00:00")
                ts = new TimeSpan(0);
            if (Regex.IsMatch(isoTimeZone, "^[-+][0-9][0-9]:[0-9][0-9]$"))
            {
                int hours   = TreatAs.IntValue(Regex.Replace(isoTimeZone, "^[-+](..):..$", "$1"), 0);
                int minutes = TreatAs.IntValue(Regex.Replace(isoTimeZone, "^[-+]..:(..)$", "$1"), 0);
                int seconds = 0;
                ts = new TimeSpan(hours, minutes, seconds);
                if (Regex.IsMatch(isoTimeZone, "^-[0-9][0-9]:[0-9][0-9]$"))
                    ts = -ts;
            }
            return ts;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- UtcToLocal -->
        /// <summary>
        ///      Converts a datetime from utc time to local time
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <returns></returns>
        private static DateTime UtcToLocal(DateTime utcDateTime)
        {
            DateTime localDateTime;
            if (utcDateTime > DateTime.MinValue.AddHours(24))
            {
                DateTime temp = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Local);
                TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(temp);
                localDateTime = utcDateTime + offset;
                localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
            }
            else
                localDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Local);
            return localDateTime;
        }


        #endregion private methods


        // ----------------------------------------------------------------------------------------
        ///  Datecode generators
        // ----------------------------------------------------------------------------------------


        // ----------------------------------------------------------------------------------------
        /// <!-- DateCode -->
        /// <summary>
        ///      Encodes a date for embedding into a file or batch name
        /// </summary>
        /// <param name="date">a date to encode</param>
        /// <param name="yearCycle">how long before a code repeats, generally 25, 50 or 10000</param>
        /// <param name="numCharacters">generally 4, 6 or 14</param>
        /// <returns></returns>
        public static string DateCode(DateTime date, int yearCycle, int numCharacters)
        {
            if      (yearCycle == 50    && numCharacters == 4)  return DateCode50h4(date);
            else if (yearCycle == 25    && numCharacters == 4)  return DateCode25h4(date);
            else if (yearCycle == 50    && numCharacters == 6)  return DateCode50m6(date);
            else if (yearCycle == 10000 && numCharacters == 14) return DateCode10000s14(date);
            else throw new NotImplementedException("this combination not supported yet: "
                + yearCycle + " years, " + numCharacters + " characters.");
        }


        public static string DateCode10000s14(DateTime date)
        {
            string year   = date.Year.ToString().PadLeft(4, '0');
            string month  = date.Month.ToString().PadLeft(2, '0');
            string day    = date.Day.ToString().PadLeft(2, '0');
            string hour   = date.Hour.ToString().PadLeft(2, '0');
            string minute = date.Minute.ToString().PadLeft(2, '0');
            string second = date.Second.ToString().PadLeft(2, '0');
            return year + month + day + hour + minute + second;
        }
        /// <remarks>
        ///      First Letter:
        ///        A 2000,2025,2050,2075      B 2001,2026,2051,2076     C 2002, 2027, 2052, 2077
        ///        ...
        ///        Y 2024,2049,2074,2099
        /// 
        ///      Second Letter: (the 2nd letter of the pair is for days after the 26th)
        ///        AB January    CD February   EF March      GH April      IJ May        KL June
        ///        MN July       OP August     QR Sept       ST Oct        UV Nov        WX Dec
        /// 
        ///      Third Letter (16):
        ///        A 1st,17th    B 2nd,18th    C 3rd,19th    D 4th,20th    E 5th, 21sh
        ///        F 6th,22nd    G 7th,23rd    H 8th,24th    I 9th,25th    J 10th,26th
        ///        K 11th,27th   L 12th,28th   M 13th,29th   N 14th,30th   O 15th,31st   P 16th   
        /// 
        ///      Third Letter (27):
        ///        A 1st,27th    B 2nd,28th    C 3rd,29th    D 4th,30th    E 5th, 31st
        ///        F 6th     G 7th     H 8th     I 9th     J 10th    K 11th    L 12th    M 13th
        ///        N 14th    O 15th    P 16th    Q 17th    R 18th    S 19th    T 20th    U 21st
        ///        V 22nd    W 23rd    X 24th    Y 25th    Z 26th
        /// 
        ///      Fourth Letter:
        ///        A 0:00    B 1:00    C 2:00    D 3:00    E 4:00
        ///        F 5:00    G 6:00    H 7:00    I 8:00    J 9:00    K 10:00   L 11:00   M 12:00
        ///        N 13:00   O 14:00   P 15:00   Q 16:00   R 17:00   S 18:00   T 19:00   U 20:00
        ///        V 21:00   W 22:00   X 23:00
        /// 
        ///      So 3/17/2011 3:30 PM would be  LEQP , so would 3/17/2036 3:10 PM
        /// </remarks>
        public static string DateCode25h4(DateTime date)
        {
            int wrapday = 16;  /// must be between 16 and 26
            char year  = (char)(65 +  date.Year % 25);
            char month = (char)(63 +  date.Month * 2 + ((date.Day - 1) / wrapday)); /// two letters per month
            char day   = (char)(65 + (date.Day - 1) % wrapday);                     /// the end of the month wraps back to A,B,C...
            char hour  = (char)(65 +  date.Hour);
            string id = "" + year + month + day + hour;
            return id;
        }
        public static string DateCode50h4(DateTime date)
        {
            char year  = (char)(65 +      (date.Year % 50) / 2);                      /// A-Y, 25 two year periods in 50 years
            char month = (char)(64 + 12 * (date.Year % 2) + date.Month);              /// A-X, 24 months over two years period
            char day   = NumUnder36toChar (date.Day);                                 /// 1-9, then A-V
            char hour  = (char)(65 +       date.Hour);                                /// A-X, 24 hours
            string id = "" + year + month + day + hour;                               /// returns a different code for each hour in ia fifty year period
            return id;
        }
        public static string DateCode50m6(DateTime date)
        {
            char year   = (char)(65 +      (date.Year % 50) / 2);                     /// A-Y, 25 two year periods in 50 years
            char month  = (char)(64 + 12 * (date.Year % 2) + date.Month);             /// A-X, 24 months over two years period
            char day    = NumUnder36toChar (date.Day);                                /// 1-9, then A-V
            char hour   = (char)(65 +       date.Hour);                               /// A-X, 24 hours
            char minute = NumUnder36toChar(1 + date.Minute/2);                        /// 1-9, then A-U
            char second = NumUnder36toChar(1 + 15 * (date.Minute%2) + date.Second/4); /// 1-9, then A-U
            string id = "" + year + month + day + hour + minute + second;             /// returns a different code for each 4 second period in a fifty year period
            return id;
        }


        public static char NumUnder36toChar(int NumUnder36)
        {
            if (NumUnder36 >= 36 || NumUnder36 < 0)
                throw new ArgumentOutOfRangeException("The number " + NumUnder36 + " is out of range 0-35");
            string str = NumUnder36.ToString();                                /// 1-9, first nine numbers
            if (str.Length > 1)
                str = "" + (char)(65 + (NumUnder36 - 10));                  /// A-Z, numbers the first nine
            char cha = str[0];
            return cha;
        }

    }
}
