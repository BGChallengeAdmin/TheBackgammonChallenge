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
        playerDebugLogDirectory = Application.persistentDataPath + playerAppDataDirectory;
        playerDebugLogFilepath = Path.Combine(playerDebugLogDirectory, playerAppDebugLogFilename);
        playerDebugLogFilepath = playerDebugLogDirectory + playerAppDebugLogFilename;
        
        CreateDebugLogFolder();

        writer = new StreamWriter(playerDebugLogFilepath, true);
        debugLogQueue = new Queue();
        
        for (int bs = 0; bs < 5; bs++)
            WriteToDebugLogFile($" ");

        WriteToDebugLogFile($"INIT DEBUG LOG FILE");
    }

    internal void OnDestroy()
    {
        if (writer is not null)
            writer.Close();

        Debug.Log($"CLOSED WRITER");
    }

    private bool CreateDebugLogFolder()
    {
        // TEST IF DIRECTORY EXISTS
        if (!Directory.Exists(playerDebugLogDirectory))
        {
            Directory.CreateDirectory(playerDebugLogDirectory);
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

    public void DeleteOutputFile()
    {
        if (File.Exists(playerDebugLogFilepath))
        {
            if (writer is not null) writer.Close();
            File.Delete(playerDebugLogFilepath);
        }

        Init();
    }

    TextWriter writer;

    public void WriteToDebugLogFile(string logMessage)
    {
        var _logMessage = System.DateTime.Now.ToString();
        _logMessage += "  :  " + logMessage + $"  \n";

        debugLogQueue.Enqueue(_logMessage);

        //writer = new StreamWriter(playerDebugLogFilepath, true);
        //using TextWriter writer = new StreamWriter(playerDebugLogFilepath, true);
        while (debugLogQueue.Count > 0)
        {
            var timedLogMessage = System.DateTime.Now.ToString();
            timedLogMessage += debugLogQueue.Dequeue();

            if (writer is not null) writer.Write(timedLogMessage);
        }
    }
}
