using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using FishNet.Object;

public class FriendListManager : NetworkBehaviour
{
    [SerializeField] GameObject friendListObject;
    FriendListData friendListData;

    [System.Serializable]
    public class FriendListData{
        public string userName;
        public string FriendList;
        public string waitlist;
    }
    private string[] FriendList = new string[0];
    private string[] WaitList = new string[0];
    [SerializeField] string FriendListURL = "http://127.0.0.1:3000/user/FriendList";
    [SerializeField] string AddFriendURL = "http://127.0.0.1:3000/user/AddFriend";
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
    }
    private void Start() {
        Debug.Log("Start");
        GameObject.Find("FriendListCanvas/Background").gameObject.SetActive(false);
    }
    public void OnListOpen(){
        if(!GameObject.Find("FriendListCanvas/Background")){
            return;
        }
        if(GameObject.Find("FriendListCanvas/Background").activeSelf){
            StartCoroutine(GetFriendList(PlayerPrefs.GetString("name")));
        }
        
    }
    public void FriendListAction(){
        if(!GameObject.Find("FriendListCanvas/Background").activeSelf){
            return;
        }
        if(GameObject.Find("FriendListCanvas/Background/FriendList").activeSelf){
           foreach (Transform child in GameObject.FindWithTag("FriendListHolder").transform)
                Destroy(child.gameObject); 
            if(FriendList.Length==0){
                Debug.Log("empty");
                return;
            }
            foreach(string friend in FriendList){
                if(friend==""){
                    continue;
                }
                Transform Holder = GameObject.FindWithTag("FriendListHolder").transform;
                GameObject obj = Instantiate(friendListObject, GameObject.FindWithTag("FriendListHolder").transform);
                TextMeshProUGUI nameText = obj.GetComponentInChildren<TextMeshProUGUI>();
                nameText.text = friend;
            }
        }
        else if(GameObject.Find("FriendListCanvas/Background/PendingList").activeSelf){
            foreach (Transform child in GameObject.FindWithTag("FriendListHolder").transform)
                Destroy(child.gameObject); 
            if(WaitList.Length==0){
                Debug.Log("empty");
                return;
            }
            foreach(string friend in WaitList){
                if(friend==""){
                    continue;
                }
                Transform Holder = GameObject.FindWithTag("FriendListHolder").transform;
                GameObject obj = Instantiate(friendListObject, GameObject.FindWithTag("FriendListHolder").transform);
                TextMeshProUGUI nameText = obj.GetComponentInChildren<TextMeshProUGUI>();
                nameText.text = friend;
                StartCoroutine(AddFriend(PlayerPrefs.GetString("name"),friend,1));
            }
        }
        else if(GameObject.Find("FriendListCanvas/Background/AddOrRemove").activeSelf){
            foreach (Transform child in GameObject.FindWithTag("FriendListHolder").transform)
                Destroy(child.gameObject); 
            if(FriendList.Length==0){
                Debug.Log("empty");
                return;
            }
            foreach(string friend in FriendList){
                if(friend==""){
                    continue;
                }
                Transform Holder = GameObject.FindWithTag("FriendListHolder").transform;
                GameObject obj = Instantiate(friendListObject, GameObject.FindWithTag("FriendListHolder").transform);
                TextMeshProUGUI nameText = obj.GetComponentInChildren<TextMeshProUGUI>();
                nameText.text = friend;
            }
        }
        else{
            Debug.Log("Error");
        }
    }
    public void searchFriend(){
        StartCoroutine(AddFriend(PlayerPrefs.GetString("name"),GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add/InputField").GetComponent<TMP_InputField>().text,0));
    }
    IEnumerator GetFriendList(string name){
        WWWForm form = new WWWForm();
        form.AddField("userName",name);
        using(UnityWebRequest www = UnityWebRequest.Post(FriendListURL, form)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.ConnectionError){
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                if(www.responseCode == 200){
                    var responseData = JsonUtility.FromJson<FriendListData>(www.downloadHandler.text);
                    FriendList = responseData.FriendList.Split(",");
                    WaitList = responseData.waitlist.Split(",");
                    Array.Sort(FriendList);
                    FriendListAction();
                }
                else{
                    Debug.Log("Login failed: " + www.downloadHandler.text);
                }
            }
        }
    }

    IEnumerator AddFriend(string userName,string friendName,int mode){
        WWWForm form = new WWWForm();
        form.AddField("userName",userName);
        form.AddField("friendName",friendName);
        form.AddField("mode",mode);
        using(UnityWebRequest www = UnityWebRequest.Post(AddFriendURL, form)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.ConnectionError){
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                if(www.responseCode == 200){
                    var responseData = JsonUtility.FromJson<FriendListData>(www.downloadHandler.text);
                }
                else{
                    Debug.Log("Login failed: " + www.downloadHandler.text);
                } 
            }
        }
    }
}