using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace PsychImmersion.Experiment
{
    public static class DataRecorder {

        private static DateTime _startPoint;
        private static StringBuilder _builder;

        private static bool _initalized = false;

        static DataRecorder()
        {
            Initalize();
        }

        /// <summary>
        /// Call this prior to calling any other DataRecorder functions
        /// I guess this could just be in a static constructor?
        /// If this is called again, the previous data is discarded
        /// </summary>
        public static void Initalize()
        {
            if(_initalized) Debug.LogWarning("DataRecorder Re-Initalized; all previous data lost");
            //sets up int to keep track of number of trials
            if (!PlayerPrefs.HasKey("TrialNumber"))
            {
                PlayerPrefs.SetInt("TrialNumber", 1);
            }
            else
            {
                PlayerPrefs.SetInt("TrialNumber", PlayerPrefs.GetInt("TrialNumber") + 1);
            }
   
            _startPoint = DateTime.Now;
            _builder = new StringBuilder();
            WriteHeader();
            _initalized = true;
        }

        /// <summary>
        /// Call this when the actual experiment starts (the cage is placed)
        /// we don't do this on initalization because we want zero to be the start of the experiment
        /// </summary>
        public static void ResetTime()
        {
            _startPoint = DateTime.Now;
        }

        /// <summary>
        /// returns the name of the current platform (Hololens, PC, VR_[device type])
        /// </summary>
        /// <returns>The current platform name</returns>
        public static string GetPlatformName()
        {
#if UNITY_WSA
            return "Hololens";
#else
            if (UnityEngine.VR.VRSettings.isDeviceActive) {
                return "VR_" + UnityEngine.VR.VRSettings.loadedDeviceName;
            }
            return "PC";
#endif
        }

        /// <summary>
        /// Writes the header onto the top of the file
        /// </summary>
        private static void WriteHeader() 
        { 
            _builder.AppendFormat("Date:,{0}\n", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            _builder.AppendFormat("Platform:,{0}\n", GetPlatformName());
            _builder.AppendFormat("Trial:,{0}\n", GetTrialNumber());
            //TODO record animal selection
            _builder.AppendLine();
            _builder.AppendLine("Timestamp,Event Type,Value");
        }

        public static int GetTrialNumber()
        {
            return PlayerPrefs.GetInt("TrialNumber");
        }

        public static void ResetTrialNumber()
        {
            PlayerPrefs.SetInt("TrialNumber", 0);
        }

        /// <summary>
        /// Records an event
        /// </summary>
        /// <param name="eventType">the type of event to record</param>
        /// <param name="value">An optional value to record with the event</param>
        public static void RecordEvent(DataEvent eventType, int value = 0)
        {
            if(!_initalized) throw new Exception("DataRecorder not initalized!");
            if (eventType == DataEvent.AnxietyLevel)
            {
                _builder.AppendFormat("{0:F2},{1},{2:F1}\n", GetCurrentTimeStamp(), GetNameForEvent(eventType), value/2f);
            }
            else
            {
                _builder.AppendFormat("{0:F2},{1},{2}\n", GetCurrentTimeStamp(), GetNameForEvent(eventType), value);
            }
        }

        /// <summary>
        /// Returns the current timestep value
        /// </summary>
        /// <returns></returns>
        public static double GetCurrentTimeStamp()
        {
            if (!_initalized) throw new Exception("DataRecorder not initalized!");
            return (DateTime.Now - _startPoint).TotalSeconds;
        }

        /// <summary>
        /// Given an event, returns the name to use in the actual file
        /// I made this a method so we could maybe use nicer names later,
        /// for now it just returns the name of the event from the enum
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        private static string GetNameForEvent(DataEvent evnt)
        {
            return Enum.GetName(typeof(DataEvent), evnt);
        }

        /// <summary>
        /// Writes all recorded events to the file
        /// </summary>
        /// <returns>the name of the file written to (for UI use)</returns>
        public static string WriteFile()
        {
            if (!_initalized) throw new Exception("DataRecorder not initalized!");
            var filename = GetFileName();
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + filename,
                _builder.ToString());
            return filename;
        }

        /// <summary>
        /// Returns the name of the file to record
        /// </summary>
        /// <returns></returns>
        private static string GetFileName()
        {
            return string.Format("{0}_Trial{1}.csv", GetPlatformName(),PlayerPrefs.GetInt("TrialNumber"));
        }
    }

    public enum DataEvent
    {
        ExperimentStart,
        DifficultyLevelChanged,
        AnxietyLevel,
        ExperimentAborted,
        ExperimentEnded
    }
}
