using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if UNITY_WSA && ENABLE_WINMD_SUPPORT
    using Windows.ApplicationModel;
#endif
namespace BrowserDesign.log
{
    public class LogUnity : MonoBehaviour
    {
        [Tooltip("Filename withour extension")]
        [SerializeField]
        private string logFileName="log";

        private string logFile;
        private string sessionID; 
        private string logPath; //path to the current log session folder
        private string logFolder; //path to log folder that stores all generated session folders
        private FileStream logStream;

        public string currentVersionNumber = "UnityDefault";
        public string StorageFolder
        {
            get { return logPath; }
        }
        public void Init()
        {
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
            logFolder = Path.Combine(DataFolders.tempDirIO.Path, "Log");
#else
            logFolder = Path.Combine(DataFolders.tempDirIO, "Log");
#endif

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            //create session id that is unique to every application startup
            var random = new System.Random();
            sessionID = System.DateTime.UtcNow.Year.ToString()
                + System.DateTime.UtcNow.Month.ToString()
                + System.DateTime.UtcNow.Day.ToString()
                + System.DateTime.UtcNow.Hour.ToString()
                + System.DateTime.UtcNow.Minute.ToString()
                + random.Next(10, 100);

            //create session folder with session id
            logPath = Path.Combine(logFolder, "Session" + sessionID);
            Directory.CreateDirectory(logPath);

            //when enable, send log message to log function
            Application.logMessageReceived += Log;
            //every time it will clean the directory that older than 60 days
            Task.Run(CleanDirectory).ConfigureAwait(false);

        }

        /// <summary>
        /// manually triggered method, that clean all logs files in the log folder
        /// TODO: add button that clear and delete all logs.
        /// </summary>
        void DeleteAllLogs()
        {
            if (Directory.Exists(logFolder))
            {
                foreach(var dir in Directory.GetDirectories(logFolder))
                {
                    Directory.Delete(dir, true);
                }
            }
        }
        /// <summary>
        /// Clear and delete the log files that older than 60 days in the log folder
        /// </summary>
        void CleanDirectory()
        {
            var directories = Directory.GetDirectories(logFolder, "*", SearchOption.TopDirectoryOnly);
            foreach(var d in directories)
            {
                if ((System.DateTime.UtcNow - Directory.GetCreationTimeUtc(d)).TotalDays > 60)
                {
                    Directory.Delete(d, true);
                }
            }
        }

        /// <summary>
        /// create a log file for current session
        /// </summary>
        void CreateLogFile()
        {
            string fileName = logFileName
                + System.DateTime.UtcNow.Hour.ToString()
                + System.DateTime.UtcNow.Minute.ToString()
                + System.DateTime.UtcNow.Millisecond.ToString();
            logFile = Path.Combine(logPath, fileName+".txt");
            using (var filestream = File.Create(logFile))
            {
                Debug.Log($"Successful create log file {logFile}");
            }
            //logStream = new FileStream(logFile, FileMode.Append, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// log string should be sent to this method
        /// </summary>
        /// <param name="mess"></param>
        public async void LogMessage(string mess)
        {
            //write message and it need to wait until other process
            while (true)
            {
                try
                {
                    using (FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write,FileShare.None))
                    {
                        byte[] buffer = new UnicodeEncoding().GetBytes(mess);
                        await fs.WriteAsync(buffer,0,buffer.Length);
                    }
                    break;
                }
                catch(System.IO.IOException ex) //io exception is generally race condition and we wait for other thread finish the filestream
                {
                    await Task.Delay(100);
                }
                catch(System.Exception ex) // other types of exceptions cannot be resolved by repeating the request
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Unity Log system event callback function
        /// </summary>
        /// <param name="logString"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        public void Log(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                //check whether there is a log file
                if (logFile == null)
                {
                    //create logfile
                    CreateLogFile();
                }
                //log message
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Time: {0}", System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                sb.Append(System.Environment.NewLine);
                sb.Append("-----------------------------------------------------------");
                sb.Append(System.Environment.NewLine);
                sb.Append(string.Format("Message: {0}", logString));
                sb.Append(System.Environment.NewLine);
                sb.Append(string.Format("StackTrace: {0}", stackTrace));
                sb.Append(System.Environment.NewLine);
                sb.Append("-----------------------------------------------------------");
                sb.Append(System.Environment.NewLine);
                LogMessage(sb.ToString());
            }
        }


        
    }
}
