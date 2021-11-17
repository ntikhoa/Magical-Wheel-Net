using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum STATE
{
    Register = 0,
    Waiting_Server = 1,
    Play_Wait = 2,
    Play_Turn = 3,
    Disqualify = 4,
    End_Game = 5
}
public class UIManager : MonoBehaviour
{
    public string userName;

    [SerializeField] private GameObject[] menus;
    [SerializeField] private InputField userNameInp;
    [SerializeField] private GameObject conBtn;
    [SerializeField] private InputField answerLetter;
    [SerializeField] private InputField answerWord;
    [SerializeField] private GameObject ansBtn;
    [SerializeField] private Text curWord;
    [SerializeField] private Text timer;
    [SerializeField] private Text hint;
    [SerializeField] private Text score;
    [SerializeField] private Text[] notices;


    private int timeOut;
    private int wordLen;
    private int curTime;
    private string curWordRev;
    private STATE state = 0;

    public int Score;
    static public UIManager instance;

    public STATE State
    {
        get { return (STATE)state; }
        set
        {
            state = value;
            
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public void DisplayServerMessage(string _msg)
    {
        foreach(Text notice in notices)
        {
            notice.text = "Server: " + _msg;
        }
    }
    public void GameStart(int _limit, string _hint, int _timeout)
    {
        wordLen = _limit;
        hint.text = _hint;
        timeOut = _timeout;
        State = STATE.Play_Wait;
    }
    public void updateAnswer(string newWord)
    {
        curWordRev = newWord;
    }

    public void ConnectingToServer()
    {
        userName = userNameInp.text;
        State = STATE.Waiting_Server;
        Client.instance.ConnectedToServer();
    }
}
