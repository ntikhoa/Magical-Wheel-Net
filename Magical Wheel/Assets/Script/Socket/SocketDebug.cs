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
        if (File.Exists(fileName))
        {
            File.WriteAllText(DateTime.Now + ": " + fileName, msg + "\n\n");
        }
        else
        {
            File.AppendAllText(DateTime.Now + ": " + fileName, msg + "\n\n");
        }
    }
}
