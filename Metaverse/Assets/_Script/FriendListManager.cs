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
    public class Message{
        public string message;
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
                obj.GetComponent<Button>().onClick.AddListener(()=>{
                    PendingResponse(PlayerPrefs.GetString("name"),friend);
                });
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
                obj.GetComponent<Button>().onClick.AddListener(()=>{
                    RemoveFriend(PlayerPrefs.GetString("name"),friend);
                });
            }
        }
        else{
            Debug.Log("Error");
        }
    }
    public void searchFriend(){
        StartCoroutine(FriendHandler(PlayerPrefs.GetString("name"),GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add/InputField").GetComponent<TMP_InputField>().text,0));
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add/InputField").GetComponent<TMP_InputField>().text = "";
    }
    public void PendingResponse(string userName, string friendName){
        GameObject.Find("FriendListCanvas/Background/PendingList/Response").SetActive(true);
        GameObject.Find("FriendListCanvas/Background/PendingList/Panel").SetActive(false);
        GameObject.Find("FriendListCanvas/Background/PendingList/Response/AcceptButton").GetComponent<Button>().onClick.AddListener(()=>{
            StartCoroutine(FriendHandler(userName,friendName,1));
            GameObject.Find("FriendListCanvas/Background/PendingList/Response").SetActive(false);
            GameObject.Find("FriendListCanvas/Background/PendingList/Panel").SetActive(true);
        });
        GameObject.Find("FriendListCanvas/Background/PendingList/Response/DeclineButton").GetComponent<Button>().onClick.AddListener(()=>{
            StartCoroutine(FriendHandler(userName,friendName,2));
            GameObject.Find("FriendListCanvas/Background/PendingList/Response").SetActive(false);
            GameObject.Find("FriendListCanvas/Background/PendingList/Panel").SetActive(true);
        });
    }
    public void RemoveFriend(string userName, string friendName){
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Response").SetActive(true);
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Panel").SetActive(false);
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add").SetActive(false);
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Response/ConfirmButton").GetComponent<Button>().onClick.AddListener(()=>{
            StartCoroutine(FriendHandler(userName,friendName,3));
            GameObject.Find("FriendListCanvas/Background/AddOrRemove/Response").SetActive(false);
            GameObject.Find("FriendListCanvas/Background/AddOrRemove/Panel").SetActive(true);
            GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add").SetActive(true);
        });
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Response/CancelButton").GetComponent<Button>().onClick.AddListener(()=>{
            GameObject.Find("FriendListCanvas/Background/AddOrRemove/Response").SetActive(false);
            GameObject.Find("FriendListCanvas/Background/AddOrRemove/Panel").SetActive(true);
            GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add").SetActive(true);
            return;
        });
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
                    Debug.Log("Error " + www.downloadHandler.text);
                }
            }
        }
    }

    IEnumerator FriendHandler(string userName,string friendName,int mode){
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
                if(www.responseCode == 200){
                    var responseData = JsonUtility.FromJson<FriendListData>(www.downloadHandler.text);
                    
                }
                else if (www.responseCode == 501){
                    var responseData = JsonUtility.FromJson<Message>(www.downloadHandler.text);
                    if(GameObject.Find("FriendListCanvas/Background/AddOrRemove").activeSelf){
                        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Message").SetActive(true);
                        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Message/MessageContent").GetComponent<TextMeshProUGUI>().text = responseData.message;
                    }
                }
                else{
                    Debug.Log("Error " + www.downloadHandler.text);
                } 
            }
        }
    }
}