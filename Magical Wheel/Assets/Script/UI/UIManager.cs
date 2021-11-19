using System;
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

    public STATE State
    {
        get { return (STATE)state; }
        set
        {
            switch (value)
            {
                case STATE.Register:
                    AddUIAction(RegisterSetup);
                    break;
                case STATE.Waiting_Server:
                    AddUIAction(WaitingSetup);
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

    #region setupUI
    private void RegisterSetup()
    {
        ActivateMenu(0);
        conBtn.interactable = true;
        userNameInp.interactable = true;
        Debug.Log(userNameInp.interactable);
    }
    private void WaitingSetup()
    {
        ActivateMenu(0);
        conBtn.interactable = false;
        userNameInp.interactable = false;
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
        State = STATE.Play_Wait;
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
        Client.instance.ConnectedToServer();
    }
    public void Answer()
    {
        ClientSender.Answer(answerLetter.text, answerWord.text);
    }
    IEnumerator Timing()
    {
        yield return new WaitForSeconds(1);
        if(state == STATE.Play_Turn)
        {
            if (curTime == 0)
            {
                ClientSender.Answer("", "");
            }
            else
            {
                curTime--;
            }
            timer.text = curTime.ToString();
        }
    }
}
