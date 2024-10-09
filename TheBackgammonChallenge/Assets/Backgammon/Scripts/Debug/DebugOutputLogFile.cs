using System.Collections;
using System.IO;
using UnityEngine;

public class DebugOutputLogFile
{
    private string playerDebugLogDirectory = string.Empty;
    private string playerDebugLogFilepath = string.Empty;

    private string playerAppDataDirectory = "/BackgammonChallenge/DebugLog";
    private string playerAppDebugLogFilename = "/DebugLogFile.txt";
    
    public bool DebugLogFileLoaded = false;
    private Queue debugLogQueue;

    public void Init()
    {
        Debug.Log($"***** INIT DEBUG LOG FILE *****");

        playerDebugLogDirectory = Application.persistentDataPath + playerAppDataDirectory;
        playerDebugLogFilepath = Path.Combine(playerDebugLogDirectory, playerAppDebugLogFilename);
        playerDebugLogFilepath = playerDebugLogDirectory + playerAppDebugLogFilename;

        debugLogQueue = new Queue();

        Debug.Log($"FILE EXISTS {CreateDebugLogFile()}");

        for (int bs = 0; bs < 5; bs++)
            WriteToDebugLogFile($" ");

        WriteToDebugLogFile($"INIT DEBUG LOG FILE");
    }

    private bool CreateDebugLogFile()
    {
        // TEST IF DIRECTORY EXISTS
        if (!Directory.Exists(playerDebugLogDirectory))
        {
            Directory.CreateDirectory(playerDebugLogDirectory);

            if (Directory.Exists(playerDebugLogDirectory))
            {
                DebugLogFileLoaded = CreateDebugLogFileInDirectory();
            }
        }
        else
        {
            DebugLogFileLoaded = CreateDebugLogFileInDirectory();
        }

        return DebugLogFileLoaded;
    }

    private bool CreateDebugLogFileInDirectory()
    {
        if (!File.Exists(playerDebugLogFilepath))
        {
            var fs = new FileStream(playerDebugLogFilepath, FileMode.Create);
            fs.Dispose();
        }

        return File.Exists(playerDebugLogFilepath);
    }

    public void WriteToDebugLogFile(string logMessage)
    {
        var _logMessage = System.DateTime.Now.ToString();
        _logMessage += "  :  " + logMessage + $"  \n";

        debugLogQueue.Enqueue(_logMessage);

        using TextWriter writer = new StreamWriter(playerDebugLogFilepath, true);
        while (debugLogQueue.Count > 0)
        {
            var timedLogMessage = System.DateTime.Now.ToString();
            timedLogMessage += debugLogQueue.Dequeue();

            writer.Write(timedLogMessage);
        }
    }
}
