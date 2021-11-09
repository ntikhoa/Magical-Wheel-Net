using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int id = 0;
}
