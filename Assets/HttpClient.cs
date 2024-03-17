using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HttpClient : MonoBehaviour
{ 
    [SerializeField] AudioSource audioSource;
    [SerializeField] TextMeshProUGUI textfield;
    [SerializeField] TextMeshProUGUI ttsbuttontext;
    [SerializeField] Button button;
    [SerializeField] TMP_InputField inputfield;

    private bool _isNewAnswer;

    private void Start()
    {
        _isNewAnswer = true;
        string text;
        if (PlayerPrefs.GetString("language") == "jp")
        {
            text = "こんにちは、ドクター！";
        }
        else
        {
            text = "안녕하세요 박사님!";
        }
        ShowText(text);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("LoginScene");
        }
    }

    public void SendMessage()
    {
        if (inputfield.text == "")
        {
            return;
        }
        StartCoroutine(SendAskaiAPI(inputfield.text));
        inputfield.text = "";
    }

    public void PlayAudio()
    {
        if (_isNewAnswer)
        {
            StartCoroutine(SendTTSAPI(textfield.text));
            _isNewAnswer = false;
        }
        else
        {
            PlayAudioAgain();
        }
    }

    IEnumerator SendAskaiAPI(string text)
    {
        string url = "";
        if (PlayerPrefs.GetString("language") == "jp")
        {
            url = $"http://{PlayerPrefs.GetString("address")}:5000/askaijp";
            
        }
        else
        {
            url = $"http://{PlayerPrefs.GetString("address")}:5000/askai";
        }
        Debug.Log(url);
        // JSON 형태의 데이터 생성
        // JSON 데이터 생성
        Textdata data = new Textdata();
        data.text = text;
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log(jsonData);
        
        // UnityWebRequest 객체 생성 및 설정
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기 및 응답 대기
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string answer = DecodeUnicodeEscapeCharacters(www.downloadHandler.text);
            Debug.Log(answer);
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(answer);
            ShowText(responseData.result);
            _isNewAnswer = true;
        }
        else
        {
            Debug.Log("Error: " + www.error);
            ShowText("Error: " + www.error);
        }
    }
    
    IEnumerator SendTTSAPI(string text)
    {
        button.interactable = false;
        ttsbuttontext.text = "생성중";
        string url = "";
        if (PlayerPrefs.GetString("language") == "jp")
        {
            url = $"http://{PlayerPrefs.GetString("address")}:5000/ttsjp";
        }
        else
        {
            url = $"http://{PlayerPrefs.GetString("address")}:5000/tts";
        }

        Debug.Log(url);
        // JSON 형태의 데이터 생성
        // JSON 데이터 생성
        Textdata data = new Textdata();
        data.text = text;
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log(jsonData);
        
        // UnityWebRequest 객체 생성 및 설정
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerAudioClip(www.url, AudioType.WAV);
        www.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기 및 응답 대기
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = myClip;
            audioSource.Play();
        }

        ttsbuttontext.text = "음성듣기";
        button.interactable = true;
    }
    
    private void ShowText(string text)
    {
        text = text.Replace("ー", "-");
        textfield.text = text;
    }

    public void PlayAudioAgain()
    {
        // //초기상황
        // if (textfield.text == "안녕하세요. 박사님")
        // {
        //     //생성된거 넣자
        //     return;
        // }
        audioSource.time = 0;
        audioSource.Play();
    }
    
    public string DecodeUnicodeEscapeCharacters(string input)
    {
        return Regex.Replace(input, @"\\u(?<Value>[a-zA-Z0-9]{4})", m =>
        {
            return ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
        });
    }
    
    //tts버튼 나중에 따로 뺴기
    
}

[System.Serializable]
public class Textdata
{
    public string text;
}

[System.Serializable]
public class ResponseData
{
    public string result;
}