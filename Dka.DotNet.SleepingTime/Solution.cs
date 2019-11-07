using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dka.DotNet.SleepingTime
{
    public class Solution
    {
        public int GetMaxSleepingTime(string meetingsTimes)
        {
            var replaceDictionary = new Dictionary<string, string>
            {
                {"Mon", "[1]"},
                {"Tue", "[2]"},
                {"Wed", "[3]"},
                {"Thu", "[4]"},
                {"Fri", "[5]"},
                {"Sat", "[6]"},
                {"Sun", "[7]"},
            };

            foreach (var (key, value) in replaceDictionary)
            {
                meetingsTimes = meetingsTimes.Replace(key, value);
            }

            var meetingTimesAsList = new List<string>();
            var meetingsTimesReader = new StringReader(meetingsTimes);

            while (meetingsTimesReader.ReadLine() is string meetingTime)
            {
                meetingTime = meetingTime.Trim(' ');

                if (string.IsNullOrWhiteSpace(meetingTime))
                {
                    continue;
                }

                meetingTimesAsList.Add(meetingTime);
            }

            meetingTimesAsList.Sort();

            if (meetingTimesAsList.Count == 0)
            {
                return 0;
            }
            
            const string regExStartTime = "([0-9]{2}:[0-9]{2})-";
            const string regExEndTime = "-([0-9]{2}:[0-9]{2})";
            
            var firstTime = Regex.Match(meetingTimesAsList[0], regExStartTime).Groups[1].Value;
            var maxSleepTime = GetMinutes("00:00", firstTime);

            var lastTime = Regex.Match(meetingTimesAsList[^1], regExEndTime).Groups[1].Value;
            var sleepTime = GetMinutes(lastTime, "23:59");
            maxSleepTime = Math.Max(sleepTime, maxSleepTime);
            
            for (var i = 1; i <= 7; i++)
            {
                var meetingsTimesInterval = meetingTimesAsList.Where(record => record.StartsWith($"[{i}]")).ToList();

                for (var j = 0; j < meetingsTimesInterval.Count-1; j++)
                {
                    var startTimeAsString = Regex.Match(meetingsTimesInterval[j], regExEndTime).Groups[1].Value;
                    var endTimeAsString = Regex.Match(meetingsTimesInterval[j+1], regExStartTime).Groups[1].Value;
                    sleepTime = GetMinutes(startTimeAsString, endTimeAsString);

                    maxSleepTime = Math.Max(sleepTime, maxSleepTime);
                }

                if (i < 7)
                {
                    var firstNextDay = meetingTimesAsList.First(record => record.StartsWith($"[{i + 1}]"));
                    var startTimeAsString = Regex.Match(meetingsTimesInterval[^1], regExEndTime).Groups[1].Value;
                    var endTimeAsString = "23:59";
                    var sleepTime1 = GetMinutes(startTimeAsString, endTimeAsString); 
                       
                    startTimeAsString = "00:00";
                    endTimeAsString = Regex.Match(firstNextDay, regExStartTime).Groups[1].Value;
                    
                    var sleepTime2 = GetMinutes(startTimeAsString, endTimeAsString);

                    sleepTime = sleepTime1 + sleepTime2;

                    maxSleepTime = Math.Max(sleepTime, maxSleepTime);
                }
            }
            
            return maxSleepTime;
        }

        private int GetMinutes(string startTimeAsString, string endTimeAsString)
        {
            if (startTimeAsString.Equals("24:00"))
            {
                return 0;
            }

            if (endTimeAsString.Equals("24:00"))
            {
                endTimeAsString = "23:59";
            }

            var startTime = DateTime.ParseExact(startTimeAsString, "HH:mm",
                CultureInfo.InvariantCulture);            
            
            var endTime = DateTime.ParseExact(endTimeAsString, "HH:mm",
                CultureInfo.InvariantCulture);

            var totalMinutes = (int)(endTime - startTime).TotalMinutes;

            if (endTimeAsString.Equals("23:59"))
            {
                totalMinutes++;
            }

            return totalMinutes;
        }
    }
}