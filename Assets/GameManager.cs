using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    StreamWriter _inputStream;
    StreamReader _outputStream;
    [SerializeField]
    TMP_InputField inputfield;
    private string _outputData = "";
    
    //make singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get{return instance;}
    }
    
    private void Awake() {
        if(GameManager.Instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void Submmit()
    {
        Client.Instace.Communication(inputfield.text);
        inputfield.text = "";
    }
}
