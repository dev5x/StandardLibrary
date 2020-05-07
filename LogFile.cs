using System;
using System.IO;
using System.Reflection;

/* dev5x.com (c) 2020
 * 
 * LogFile Class
 * Handles general log file needs
*/

namespace dev5x.StandardLibrary
{
    public class LogFile : BaseClass
    {
        private readonly string _logPath = string.Empty;
        private readonly string _logFile = string.Empty;
        private readonly string tab = "\t";

        #region Constructor
        public LogFile(string LogFileName)
        {
            try
            {
                _logFile = LogFileName;

                // Make sure the Logs subdirectory exists
                _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");

                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }

                // Check the size and create archive when file >5mb
                FileInfo fi = new FileInfo(Path.Combine(_logPath, _logFile));
                if (fi.Exists && fi.Length > 5120000)
                {
                    string archiveName = Path.Combine(_logPath, Path.GetFileNameWithoutExtension(_logFile) + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + Path.GetExtension(_logFile));
                    fi.CopyTo(archiveName);
                    fi.Delete();
                    Post("Previous log archived to " + Path.GetFileName(archiveName));
                }
            }
            catch { }
        }
        #endregion

        #region Properties
        public string FileName
        {
            get { return _logFile; }
        }

        public string FilePath
        {
            get { return _logPath; }
        }
        #endregion

        #region Public Methods
        public void Post(string Message)
        {
            try
            {
                // Write the message to the log file
                using (FileStream fs = new FileStream(Path.Combine(_logPath, _logFile), FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString() + tab + Message);
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Log File Post - " + ex.Message);
            }
        }

        public string Read()
        {
            try
            {
                // Read the contents of the log file
                using (StreamReader sr = new StreamReader(Path.Combine(_logPath, _logFile)))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Log File Read - " + ex.Message);
                return string.Empty;
            }
        }

        public void Open()
        {
            try
            {
                // Open the log file in associated app
                Utility.OpenByAssociation(Path.Combine(_logPath, _logFile));
            }
            catch (Exception ex)
            {
                SetErrorMessage("Log File Open - " + ex.Message);
            }
        }

        //public void WriteApplicationEventLog(string Source, string Message, EventLogEntryType EntryType, int EventID)
        //{
        //    try
        //    {
        //        // Write an event to the Application log
        //        const string logName = "Application";

        //        // Register source
        //        if (!EventLog.SourceExists(Source))
        //        {
        //            EventLog.CreateEventSource(Source, logName);
        //        }

        //        // Write event entry
        //        using (EventLog eventLog = new EventLog(logName, Environment.MachineName, Source))
        //        {
        //            eventLog.WriteEntry(Message, EntryType, EventID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SetErrorMessage("Write Application EventLog - " + ex.Message);
        //    }
        //}
        #endregion
    }
}