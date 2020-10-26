////////////////////////////////////Disclaimer////////////////////////////////////////////
//This library has been wrote by : Anas Reslan Bahsas  if you are going to use it		//	
//please dont remove this line .														//					
//you have to add this class to a asp.net web project to work well.					//		//	
//I will be grateful to receive any commments or suggestion to anasbahsas@hotmail.com	//								//	
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Web;
using System.Globalization;

//using System.Web;

namespace PinnaFace.Core
{
    /// <summary>
    /// Summary description for Dates.
    /// </summary>
    public class Dates
    {
        private readonly HttpContext _cur;

        private const int StartGreg = 1900;
        private const int EndGreg = 2100;
        private readonly string[] _allFormats = { "yyyy/MM/dd", "yyyy/M/d", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "yyyy-MM-dd", "yyyy-M-d", "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy", "yyyy MM dd", "yyyy M d", "dd MM yyyy", "d M yyyy", "dd M yyyy", "d MM yyyy" };
        private readonly CultureInfo _arCul;
        private readonly CultureInfo _enCul;
        private GregorianCalendar _g;

        public Dates()
        {
            _cur = HttpContext.Current;

            _arCul = new CultureInfo("ar-SA");
            _enCul = new CultureInfo("en-US");
            
            var h = new HijriCalendar();
            _g = new GregorianCalendar(GregorianCalendarTypes.USEnglish);

            _arCul.DateTimeFormat.Calendar = h;

        }

        /// <summary>
        /// Check if string is hijri date and then return true 
        /// </summary>
        /// <param name="hijri"></param>
        /// <returns></returns>
        public bool IsHijri(string hijri)
        {
            if (hijri.Length <= 0)
            {

                _cur.Trace.Warn("IsHijri Error: Date String is Empty");
                return false;
            }
            try
            {
                var tempDate = DateTime.ParseExact(hijri, _allFormats, _arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                if (tempDate.Year >= StartGreg && tempDate.Year <= EndGreg)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("IsHijri Error :" + hijri.ToString() + "\n" + ex.Message);
                return false;
            }

        }
        /// <summary>
        /// Check if string is Gregorian date and then return true 
        /// </summary>
        /// <param name="greg"></param>
        /// <returns></returns>
        public bool IsGreg(string greg)
        {
            if (greg.Length <= 0)
            {

                _cur.Trace.Warn("IsGreg :Date String is Empty");
                return false;
            }
            try
            {
                var tempDate = DateTime.ParseExact(greg, _allFormats, _enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                if (tempDate.Year >= StartGreg && tempDate.Year <= EndGreg)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("IsGreg Error :" + greg.ToString() + "\n" + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// Return Formatted Hijri date string 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string FormatHijri(string date, string format)
        {
            if (date.Length <= 0)
            {

                _cur.Trace.Warn("Format :Date String is Empty");
                return "";
            }
            try
            {
                var tempDate = DateTime.ParseExact(date, _allFormats, _arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, _arCul.DateTimeFormat);

            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("Date :\n" + ex.Message);
                return "";
            }

        }
        /// <summary>
        /// Returned Formatted Gregorian date string
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string FormatGreg(string date, string format)
        {
            if (date.Length <= 0)
            {

                _cur.Trace.Warn("Format :Date String is Empty");
                return "";
            }
            try
            {
                var tempDate = DateTime.ParseExact(date, _allFormats, _enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, _enCul.DateTimeFormat);

            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("Date :\n" + ex.Message);
                return "";
            }

        }
        /// <summary>
        /// Return Today Gregorian date and return it in yyyy/MM/dd format
        /// </summary>
        /// <returns></returns>
        public string GDateNow()
        {
            try
            {
                return DateTime.Now.ToString("yyyy/MM/dd", _enCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("GDateNow :\n" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// Return formatted today Gregorian date based on your format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string GDateNow(string format)
        {
            try
            {
                return DateTime.Now.ToString(format, _enCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("GDateNow :\n" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Return Today Hijri date and return it in yyyy/MM/dd format
        /// </summary>
        /// <returns></returns>
        public string HDateNow()
        {
            try
            {
                return DateTime.Now.ToString("yyyy/MM/dd", _arCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("HDateNow :\n" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// Return formatted today hijri date based on your format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>

        public string HDateNow(string format)
        {
            try
            {
                return DateTime.Now.ToString(format, _arCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("HDateNow :\n" + ex.Message);
                return "";
            }

        }

        /// <summary>
        /// Convert Hijri Date to it's equivalent Gregorian Date
        /// </summary>
        /// <param name="hijri"></param>
        /// <returns></returns>
        public string HijriToGreg(string hijri)
        {

            if (hijri.Length <= 0)
            {

                _cur.Trace.Warn("HijriToGreg :Date String is Empty");
                return "";
            }
            try
            {
                var tempDate = DateTime.ParseExact(hijri, _allFormats, _arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString("yyyy/MM/dd", _enCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("HijriToGreg :" + hijri.ToString() + "\n" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// Convert Hijri Date to it's equivalent Gregorian Date and return it in specified format
        /// </summary>
        /// <param name="hijri"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string HijriToGreg(string hijri, string format)
        {

            if (hijri.Length <= 0)
            {

                _cur.Trace.Warn("HijriToGreg :Date String is Empty");
                return "";
            }
            try
            {
                var tempDate = DateTime.ParseExact(hijri, _allFormats, _arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, _enCul.DateTimeFormat);

            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("HijriToGreg :" + hijri.ToString() + "\n" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// Convert Gregoian Date to it's equivalent Hijir Date
        /// </summary>
        /// <param name="greg"></param>
        /// <returns></returns>
        public string GregToHijri(string greg)
        {

            if (greg.Length <= 0)
            {

                _cur.Trace.Warn("GregToHijri :Date String is Empty");
                return "";
            }
            try
            {
                var tempDate = DateTime.ParseExact(greg, _allFormats, _enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString("yyyy/MM/dd", _arCul.DateTimeFormat);

            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("GregToHijri :" + greg.ToString() + "\n" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// Convert Hijri Date to it's equivalent Gregorian Date and return it in specified format
        /// </summary>
        /// <param name="greg"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string GregToHijri(string greg, string format)
        {

            if (greg.Length <= 0)
            {

                _cur.Trace.Warn("GregToHijri :Date String is Empty");
                return "";
            }
            try
            {

                var tempDate = DateTime.ParseExact(greg, _allFormats, _enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, _arCul.DateTimeFormat);

            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("GregToHijri :" + greg.ToString() + "\n" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Return Gregrian Date Time as digit stamp
        /// </summary>
        /// <returns></returns>
        public string GTimeStamp()
        {
            return GDateNow("yyyyMMddHHmmss");
        }
        /// <summary>
        /// Return Hijri Date Time as digit stamp
        /// </summary>
        /// <returns></returns>
        public string HTimeStamp()
        {
            return HDateNow("yyyyMMddHHmmss");
        }

        /// <summary>
        /// Compare if the Hijri date between  other financial year and return True if within , False in not within
        /// </summary>
        /// <param name="dt">Data Table contain start date and end date ,,start date is the first column in table , end date is the second column in table</param>
        /// <param name="fromDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        
        /// <summary>
        /// Compare two instaces of string date and return indication of thier values 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns>positive d1 is greater than d2,negative d1 is smaller than d2, 0 both are equal</returns>
        public int Compare(string d1, string d2)
        {
            try
            {
                var date1 = DateTime.ParseExact(d1, _allFormats, _arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                var date2 = DateTime.ParseExact(d2, _allFormats, _arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return DateTime.Compare(date1, date2);
            }
            catch (Exception ex)
            {
                _cur.Trace.Warn("Compare :" + "\n" + ex.Message);
                return -1;
            }
        }
    }
}
