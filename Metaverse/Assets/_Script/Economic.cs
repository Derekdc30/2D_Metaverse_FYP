using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;


public class Economic : MonoBehaviour
{
    private TextMeshProUGUI MoneyText; 
    [SerializeField] string MoneyURL = "http://127.0.0.1:3000/user/Money";

    public class MoneyData{
        public string name;
        public string money;

    }
    private void Start() {
        MoneyText = GameObject.FindWithTag("MoneyText").GetComponent<TextMeshProUGUI>();
    }
    public void UpdateMoneyText(string value){
        Debug.Log(value);
        String text = "$" + value;
        MoneyText.text = text;
    }
    public void SyncMoneyroutine(string value, string mode, string name){
        StartCoroutine(SyncMoney(value,mode,name));
    }
    // mode 1: add money
    // mode 2: minus money
    // mode 3: get money
    IEnumerator SyncMoney (string value, string mode, string name){
        WWWForm form = new WWWForm();
        form.AddField("userName",name);
        form.AddField("value",value);
        form.AddField("mode",mode);
        using (UnityWebRequest www = UnityWebRequest.Post(MoneyURL, form)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.ConnectionError){
                Debug.Log(www.error);
            }
            else{
                Debug.Log(www.downloadHandler.text);
                if(www.responseCode == 200){
                    var responseData = JsonUtility.FromJson<MoneyData>(www.downloadHandler.text);
                    if(mode == "3"){
                        UpdateMoneyText(responseData.money);
                    }
                }
                else{
                    Debug.Log("Login failed: " + www.downloadHandler.text);
                }
            }
        }
    }
}
