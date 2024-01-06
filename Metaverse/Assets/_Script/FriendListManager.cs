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
        public string userEmail;
        public string FriendList;
        public string WaitList;
    }
    private string[] FriendList = new string[0];
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
            StartCoroutine(GetFriendList(PlayerPrefs.GetString("email")));
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
                Transform Holder = GameObject.FindWithTag("FriendListHolder").transform;
                GameObject obj = Instantiate(friendListObject, GameObject.FindWithTag("FriendListHolder").transform);
                TextMeshProUGUI nameText = obj.GetComponentInChildren<TextMeshProUGUI>();
                nameText.text = friend;
            }
        }
        else if(GameObject.Find("FriendListCanvas/Background/PendingList").activeSelf){
            Debug.Log("pending list");
        }
        else if(GameObject.Find("FriendListCanvas/Background/AddOrRemove").activeSelf){
            Debug.Log("add list");
        }
        else{
            Debug.Log("Error");
        }
    }

    IEnumerator GetFriendList(string email){
        WWWForm form = new WWWForm();
        form.AddField("userEmail",email);
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
                    Array.Sort(FriendList);
                    FriendListAction();
                }
                else{
                    Debug.Log("Login failed: " + www.downloadHandler.text);
                }
            }
        }
    }

    IEnumerator AddFriend(string userEmail,string friendName){
        WWWForm form = new WWWForm();
        form.AddField("userEmail",userEmail);
        form.AddField("friendName",friendName);
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