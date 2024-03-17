using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginSceneController : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI text;
    
    [SerializeField] TMP_InputField inputfield;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("address"))
        {
            inputfield.text = PlayerPrefs.GetString("address");
            SetIP();
        }
    }
    
    public void SetKorean()
    {
        PlayerPrefs.SetString("language", "kr");
        SceneManager.LoadScene("ChatScene");
    }

    public void SetJapanese()
    {
        PlayerPrefs.SetString("language", "jp");
        SceneManager.LoadScene("ChatScene");
    }

    public void SetIP()
    {
        PlayerPrefs.SetString("address", inputfield.text);
        StartCoroutine(TestServer());
    }
    
    IEnumerator TestServer()
    {
        text.text = "Loading";
        string url = $"http://{PlayerPrefs.GetString("address")}:5000/";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // 요청 보내기 및 응답 대기
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                text.text = "Success";
            }
            else
            {
                Debug.Log("Error: " + www.error);
                text.text = "Error: " + www.error;
            }
        }
        
    }
}
