using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class SocketDebug
{
    public static void Log(string msg)
    {
        string fileName = @"\Debug.txt";
        if (!File.Exists(fileName))
        {
            Debug.Log(msg);
            File.WriteAllText(fileName, DateTime.Now + ": " + msg + "\n\n");
        }
        else
        {
            Debug.Log(msg);
            File.AppendAllText(fileName, DateTime.Now + ": " + msg + "\n\n");
        }
    }
}
