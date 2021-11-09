using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> waitingQueue = new List<Action>();
    private static readonly List<Action> executeQueue = new List<Action>();
    private static bool actionToExecuteOnMainThread = false;

    private void Update()
    {
        UpdateMain();
    }

    public static void AddAction(Action _action)
    {
        if (_action == null)
        {
            return;
        }

        lock (waitingQueue)
        {
            waitingQueue.Add(_action);
            actionToExecuteOnMainThread = true;
        }
    }

    public static void UpdateMain()
    {
        if (actionToExecuteOnMainThread)
        {
            executeQueue.Clear();
            lock (waitingQueue)
            {
                executeQueue.AddRange(waitingQueue);
                waitingQueue.Clear();
                actionToExecuteOnMainThread = false;
            }

            for (int i = 0; i < executeQueue.Count; i++)
            {
                executeQueue[i]();
            }
        }
    }
}
