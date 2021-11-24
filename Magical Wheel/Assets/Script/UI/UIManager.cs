using System;
using System.Linq;
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
    End_Game = 5,
    Start_Game = 6
}
public class UIManager : MonoBehaviour
{
    private static readonly List<Action> waitingQueue = new List<Action>();
    private static readonly List<Action> executeQueue = new List<Action>();
    private static bool actionToExecuteOnMainThread = false;
    public static void AddUIAction(Action _action)
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
    public static void UpdateUI()
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

    public string userName;

    [SerializeField] private GameObject[] menus;
    [SerializeField] private InputField userNameInp;
    [SerializeField] private Button conBtn;
    [SerializeField] private InputField answerLetter;
    [SerializeField] private InputField answerWord;
    [SerializeField] private Button ansBtn;
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
    static public Dictionary<STATE, STATE[]> ValidTransfer;
    public STATE State
    {
        get { return (STATE)state; }
        set
        {
            Debug.Log($"{state} -> {value}");
            if (ValidTransfer[state].Contains(value))
            {
                Debug.Log($"V");
                switch (value)
                {
                    case STATE.Register:
                        AddUIAction(RegisterSetup);
                        break;
                    case STATE.Waiting_Server:
                        AddUIAction(WaitingSetup);
                        break;
                    case STATE.Start_Game:
                        AddUIAction(WaitingStart);
                        break;
                    case STATE.Play_Wait:
                        AddUIAction(PlayWaitSetup);
                        break;
                    case STATE.Play_Turn:
                        AddUIAction(PlayTurnSetup);
                        break;
                    case STATE.Disqualify:
                        AddUIAction(DisqualifySetup);
                        break;
                    case STATE.End_Game:
                        AddUIAction(EndGameSetup);
                        break;
                }
                state = value;
            }
        }
    }

    #region setupUI
    private void RegisterSetup()
    {
        ActivateMenu(0);
        conBtn.interactable = true;
        userNameInp.interactable = true;
    }
    private void WaitingSetup()
    {
        ActivateMenu(0);
        conBtn.interactable = false;
        userNameInp.interactable = false;

        answerLetter.text = "";
        answerWord.text = "";
    }
    private void WaitingStart()
    {
        ActivateMenu(0);
        conBtn.interactable = false;
        userNameInp.interactable = false;

        answerLetter.text = "";
        answerWord.text = "";
    }
    private void PlayWaitSetup()
    {
        ActivateMenu(1);
        ansBtn.interactable = false;
        answerLetter.interactable = false;
        answerWord.interactable = false;
        curTime = timeOut;

        curWord.text = curWordRev;
        timer.text = curTime.ToString();
        score.text = Score.ToString();
    }
    private void PlayTurnSetup()
    {
        ActivateMenu(1);
        ansBtn.interactable = true;
        answerLetter.interactable = true;
        answerWord.interactable = true;

        curWord.text = curWordRev;
        timer.text = curTime.ToString();
        score.text = Score.ToString();

        answerLetter.text = "";
        answerWord.text = "";
    }
    private void DisqualifySetup()
    {
        ActivateMenu(3);
    }
    private void EndGameSetup()
    {
        ActivateMenu(2);
    }
    private void ActivateMenu(int _choice)
    {
        for(int i=0; i<menus.Length; i++)
        {
            menus[i].SetActive(i == _choice);
        }
    }
    #endregion


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
    private void Start()
    {
        ValidTransfer = new Dictionary<STATE, STATE[]> {
            {STATE.Register, new STATE[]{ STATE.Register, STATE.Waiting_Server} },
            {STATE.Waiting_Server, new STATE[]{ STATE.Register, STATE.Waiting_Server, STATE.Start_Game } },
            {STATE.Start_Game, new STATE[]{ STATE.Register, STATE.Waiting_Server, STATE.Play_Turn, STATE.Play_Wait } },
            {STATE.Play_Turn, new STATE[]{ STATE.Register, STATE.Waiting_Server, STATE.Play_Turn, STATE.Play_Wait, STATE.Disqualify, STATE.End_Game } },
            {STATE.Play_Wait, new STATE[]{ STATE.Register, STATE.Waiting_Server, STATE.Play_Turn, STATE.Play_Wait, STATE.Disqualify, STATE.End_Game } },
            {STATE.Disqualify, new STATE[]{ STATE.Register, STATE.Waiting_Server, STATE.Disqualify, STATE.End_Game } },
            {STATE.End_Game, new STATE[]{ STATE.Register, STATE.Waiting_Server, STATE.End_Game, STATE.Start_Game } }
        };
        StartCoroutine(Timing());
    }
    private void Update()
    {
        UpdateUI();
    }

    public void DisplayServerMessage(string _msg)
    {
        AddUIAction(() =>
        {
            foreach (Text notice in notices)
            {
                notice.text = "Server: " + _msg;
            }
        });
    }
    public void GameStart(int _limit, string _hint, int _timeout)
    {
        wordLen = _limit;
        curTime = timeOut = _timeout;
        Score = 0;
        curWordRev = "";
        for(int i = 0; i<wordLen; i++)
        {
            curWordRev += "-";
        }

        AddUIAction(() =>
        {
            hint.text = _hint;
            answerWord.characterLimit = wordLen;
        });
        State = STATE.Start_Game;
    }
    public void updateAnswer(string newWord)
    {
        curWordRev = newWord;
        AddUIAction(() =>
        {
            curWord.text = curWordRev;
        });
    }

    public void ConnectingToServer()
    {
        userName = userNameInp.text;
        State = STATE.Waiting_Server;
        if(Client.instance.tcp.socket != null)
        {
            if (Client.instance.tcp.socket.Connected)
            {
                Client.instance.userName = userName;
                ClientSender.WelcomeReceived();
                return;
            }
        }
        Client.instance.ConnectedToServer();
    }
    public void Answer()
    {
        State = STATE.Play_Wait;
        if (answerLetter.text == null)
        {
            answerLetter.text = "";
        }
        if(answerWord.text == null)
        {
            answerWord.text = "";
        }
        ThreadManager.AddAction(() =>
        {
            ClientSender.Answer(answerLetter.text, answerWord.text);
        });
    }
    IEnumerator Timing()
    {
        yield return new WaitForSeconds(1);
        if(state == STATE.Play_Turn)
        {
            if (curTime == 0)
            {
                State = STATE.Play_Wait;
                ThreadManager.AddAction(() =>
                {
                    ClientSender.Answer("", "");
                });
            }
            else
            {
                curTime--;
            }
            AddUIAction(() =>
            {
                timer.text = curTime.ToString();
            });
        }
        StartCoroutine(Timing());
    }
}
