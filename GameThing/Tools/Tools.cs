using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public static class Tools
    {
        // PROPERTIES

        private const string ErrorLogFilePath = "./_error_log.txt";

        private static bool IsErrorLogStarted = false;


        // METHODS

        /// <summary>
        /// Wrapper function for serialising data into a JSON string.
        /// </summary>
        /// <param name="data">Raw data to convert to JSON.</param>
        /// <returns>JSON string.</returns>
        public static string SerialiseData(object data, bool beautify = true)
        {
            Formatting jsonFormatting = Formatting.None;
            if (beautify)
            {
                jsonFormatting = Formatting.Indented;
            }
            string serialisedData = JsonConvert.SerializeObject(data, jsonFormatting);
            return serialisedData;
        }


        /// <summary>
        /// Wrapper function for deserialising a JSON string into data.
        /// </summary>
        /// <typeparam name="T">The type to convert the JSON into.</typeparam>
        /// <param name="serialisedData">The JSON string to deserialise.</param>
        /// <returns>The converted data.</returns>
        public static T DeserialiseData<T>(string serialisedData)
        {
            T data = JsonConvert.DeserializeObject<T>(serialisedData);
            return data;
        }


        /// <summary>
        /// Log an exception in the error log.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public static void LogError(Exception ex)
        {
            string exceptionMessage = GetFullExceptionMessage(ex);
            LogError(exceptionMessage);
        }


        /// <summary>
        /// Log a message in the error log.
        /// </summary>
        /// <param name="errorMessage">The message to log.</param>
        public static void LogError(string errorMessage)
        {
            //Add timestamp to exception message
            errorMessage = DateTime.Now.ToString() + " --- " + errorMessage;
            //Add a begin line to the error log if nothing has been logged this session yet
            if (!IsErrorLogStarted)
            {
                File.AppendAllText(ErrorLogFilePath, "\r\n ----- SESSION START ----- \r\n");
                IsErrorLogStarted = true;
            }
            //Add error message to the log file
            File.AppendAllText(ErrorLogFilePath, errorMessage + "\r\n");
        }


        /// <summary>
        /// Get a full exception message string including all inner exception messages.
        /// </summary>
        /// <param name="exceptionPart">The exception to get the messages from.</param>
        /// <param name="innerDepth">Used for recursing through inner exceptions. Leave at zero.</param>
        /// <param name="topException">The top level exception object (for inner exceptions).</param>
        /// <returns>The full exception message.</returns>
        public static string GetFullExceptionMessage(Exception exceptionPart, int innerDepth = 0, Exception topException = null)
        {
            if (topException == null) topException = exceptionPart;

            string exceptionMessage = exceptionPart.Message;
            
            if (innerDepth > 0)
            {
                //Add indenting if this is an inner exception
                exceptionMessage = "\r\n    └" + new String('─', innerDepth * 3) + " " + exceptionPart.Message;
            }

            if (exceptionPart.InnerException != null)
            {
                //Recurse through inner exceptions to get the full message
                exceptionMessage += GetFullExceptionMessage(exceptionPart.InnerException, innerDepth + 1, topException);
            }
            else
            {
                //Add the strack trace to the end of the message
                exceptionMessage += "\r\n" + topException.StackTrace;
            }
            return exceptionMessage;
        }


        /// <summary>
        /// Convert a string to title case (Capitalised letters).
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>The converted title case string.</returns>
        public static string TitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return input; }
            string[] inputWords = input.Split(' ');
            string output = "";
            foreach (string word in inputWords)
            {
                //Add a space, except for the first word
                if (output != "")
                {
                    output += " ";
                }
                //Capitalise the first letter
                if (word == "") { continue; }
                output += word.First().ToString().ToUpper() + word.Substring(1);
            }
            return output;
        }


        /// <summary>
        /// Round a number to the nearest multiple of a given number.
        /// </summary>
        /// <param name="value">The number to round.</param>
        /// <param name="nearest">Round to the nearest multiple of this number.</param>
        /// <returns>The rounded number.</returns>
        public static int RoundToNearest(double value, int nearest)
        {
            return (int)Math.Round(value / nearest) * nearest;
        }


        /// <summary>
        /// Formats a date to a user-friendly string that is relative to the current day.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>The formatted date string.</returns>
        public static string DateRelative(DateTime? date)
        {
            if (date == null) return "Never";
            DateTime now = DateTime.Now;
            DateTime dateNonNull = date ?? now;
            string formattedDate = dateNonNull.ToString("d MMMM");
            if (dateNonNull.Date == now.Date)
            {
                formattedDate = "Today at " + dateNonNull.ToString("h:mm tt");
            }
            else if (dateNonNull.Date == now.Date.Subtract(TimeSpan.FromDays(1)))
            {
                formattedDate = "Yesterday";
            }
            else if (dateNonNull.Year != now.Year)
            {
                formattedDate += " " + dateNonNull.Year;
            }
            return formattedDate;
        }


        /// <summary>
        /// Formats a date to a string denoting how long ago (from the current time) it is.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>The formatted string.</returns>
        public static string DateFromNow(DateTime? date)
        {
            if (date == null) return "never";
            DateTime now = DateTime.Now;
            DateTime dateNonNull = date ?? now;
            TimeSpan dateDiff = now.Subtract(dateNonNull);

            string spanUnits = "hour";
            int spanAmount = 0;
            if (dateDiff.TotalHours < 1) return "recently";
            else if (dateDiff.TotalDays < 1)
            {
                spanUnits = "hour";
                spanAmount = (int)Math.Floor(dateDiff.TotalHours);
            }
            else if (dateDiff.TotalDays < 2) return "yesterday";
            else if (dateDiff.TotalDays < 31)
            {
                spanUnits = "day";
                spanAmount = (int)Math.Floor(dateDiff.TotalDays);
            }
            else if (dateDiff.TotalDays < 365)
            {
                spanUnits = "month";
                spanAmount = (int)Math.Floor(dateDiff.TotalDays / 30);
            }
            else
            {
                spanUnits = "year";
                spanAmount = (int)Math.Floor(dateDiff.TotalDays / 365);
            }

            string spanPlural = "s";
            if (spanAmount == 1) spanPlural = "";
            return $"{spanAmount} {spanUnits}{spanPlural} ago";
        }


        /// <summary>
        /// Converts an absolute file path to a relative file path.
        /// </summary>
        /// <param name="filePath">File path to convert to relative.</param>
        /// <param name="referencePath">Path that the output path will be relative to.</param>
        /// <returns>Relative file path.</returns>
        public static string GetRelativePath(string filePath, string referencePath = null)
        {
            if (referencePath == null)
            {
                referencePath = Assembly.GetEntryAssembly().Location;
            }
            
            Uri fileUri = new Uri(filePath);
            Uri referenceUri = new Uri(referencePath);
            string relativePath = referenceUri.MakeRelativeUri(fileUri).ToString();
            return Uri.UnescapeDataString(relativePath);
        }


    }
}
