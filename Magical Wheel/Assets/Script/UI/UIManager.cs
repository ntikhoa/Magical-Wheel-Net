using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public string userName;
    public GameObject startMenu;
    public InputField inp;
    static public UIManager instance;

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

    public void ConnectedToServer()
    {
        userName = inp.text;
        //startMenu.SetActive(false);
        Client.instance.ConnectedToServer();
    }
}
