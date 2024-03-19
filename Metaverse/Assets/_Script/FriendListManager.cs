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

    [System.Serializable]
    public class FriendListData{        // Data structure of Friend list
        public string userName;
        public string FriendList;
        public string waitlist;
    }
    public class Message{               // Message data structure
        public string message;
    }
    private string[] FriendList = new string[0];        // store current user friend list 
    private string[] WaitList = new string[0];          // store current user waiting list
    [Header("Route URL")]
    [SerializeField] private string FriendListURL = "http://127.0.0.1:3000/user/FriendList";    // route URL
    [SerializeField] private string AddFriendURL = "http://127.0.0.1:3000/user/AddFriend";      // route URL
    [Header("Pre-define mode")]
    private int MakeRequest = 0;
    private int AcceptRequest = 1;
    private int DeclineRequest = 2;
    private int Remove = 3;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        GameObject.Find("FriendListCanvas/Background").gameObject.SetActive(false); // turn off friend list
    }
    public void OnListOpen(){       // this function trigger by the open friend list button
        if(!GameObject.Find("FriendListCanvas/Background")){
            return;
        }
        if(GameObject.Find("FriendListCanvas/Background").activeSelf){
            StartCoroutine(GetFriendList(PlayerPrefs.GetString("name")));           // get the latest friend list
        }
        
    }
    public void FriendListAction(){     // some action relate to different tab of the friend list
        if(!GameObject.Find("FriendListCanvas/Background").activeSelf){     // make sure the friend list is open
            return;
        }
        if(GameObject.Find("FriendListCanvas/Background/FriendList").activeSelf){       // the friend list tab action
           foreach (Transform child in GameObject.FindWithTag("FriendListHolder").transform)        // clear current item
                Destroy(child.gameObject); 
            if(FriendList.Length==0){       // avoid action on empty friend list
                return;
            }
            foreach(string friend in FriendList){       // append friend list item to the list 
                if(friend==""){
                    continue;
                }
                Transform Holder = GameObject.FindWithTag("FriendListHolder").transform;
                GameObject obj = Instantiate(friendListObject, GameObject.FindWithTag("FriendListHolder").transform);
                TextMeshProUGUI nameText = obj.GetComponentInChildren<TextMeshProUGUI>();
                nameText.text = friend;
            }
        }
        else if(GameObject.Find("FriendListCanvas/Background/PendingList").activeSelf){         // the pending list tab action
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
                obj.GetComponent<Button>().onClick.AddListener(()=>{        // add listener on each friend list item
                    PendingResponse(PlayerPrefs.GetString("name"),friend);  // post the response 
                });
            }
        }
        else if(GameObject.Find("FriendListCanvas/Background/AddOrRemove").activeSelf){     // the add or remove tab action
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
                obj.GetComponent<Button>().onClick.AddListener(()=>{        // add listener to all item
                    RemoveFriend(PlayerPrefs.GetString("name"),friend);     // post response 
                });
            }
        }
        else{
            Debug.Log("Error");
        }
    }
    public void searchFriend(){     // this function trigger by the tick button on add or remove tab
        StartCoroutine(FriendHandler(PlayerPrefs.GetString("name"),GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add/InputField").GetComponent<TMP_InputField>().text,MakeRequest));
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add/InputField").GetComponent<TMP_InputField>().text = "";
    }
    public void PendingResponse(string userName, string friendName){        // mode 1 => add to friend list, mode 2 => decline request
        GameObject.Find("FriendListCanvas/Background/PendingList/Response").SetActive(true);
        GameObject.Find("FriendListCanvas/Background/PendingList/Panel").SetActive(false);
        GameObject.Find("FriendListCanvas/Background/PendingList/Response/AcceptButton").GetComponent<Button>().onClick.AddListener(()=>{
            StartCoroutine(FriendHandler(userName,friendName,AcceptRequest));
            GameObject.Find("FriendListCanvas/Background/PendingList/Response").SetActive(false);
            GameObject.Find("FriendListCanvas/Background/PendingList/Panel").SetActive(true);
        });
        GameObject.Find("FriendListCanvas/Background/PendingList/Response/DeclineButton").GetComponent<Button>().onClick.AddListener(()=>{
            StartCoroutine(FriendHandler(userName,friendName,DeclineRequest));
            GameObject.Find("FriendListCanvas/Background/PendingList/Response").SetActive(false);
            GameObject.Find("FriendListCanvas/Background/PendingList/Panel").SetActive(true);
        });
    }
    public void RemoveFriend(string userName, string friendName){           // mode 3 => remove friend
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Response").SetActive(true);
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Panel").SetActive(false);
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Add").SetActive(false);
        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Response/ConfirmButton").GetComponent<Button>().onClick.AddListener(()=>{
            StartCoroutine(FriendHandler(userName,friendName,Remove));
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
    IEnumerator GetFriendList(string name){         // get the current friend list, if success refresh the friend list
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
                    Debug.Log("split: "+responseData);
                    FriendList = responseData.FriendList.Split(",");
                    WaitList = responseData.waitlist.Split(",");
                    Array.Sort(FriendList);
                    FriendListAction();     //refresh friend list
                }
                else{
                    Debug.Log("Error " + www.downloadHandler.text);
                }
            }
        }
    }
    IEnumerator FriendHandler(string userName,string friendName,int mode){      // handle action of different friend list tab action
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
                if(www.responseCode == 200){        // special response code indicate action success
                    var responseData = JsonUtility.FromJson<FriendListData>(www.downloadHandler.text);
                    
                }
                else if (www.responseCode == 501){      // special response code for return message
                    var responseData = JsonUtility.FromJson<Message>(www.downloadHandler.text);
                    if(GameObject.Find("FriendListCanvas/Background/AddOrRemove").activeSelf){
                        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Message").SetActive(true);
                        GameObject.Find("FriendListCanvas/Background/AddOrRemove/Message/MessageContent").GetComponent<TextMeshProUGUI>().text = responseData.message;
                    }
                }
                else{
                    Debug.Log("Error " + www.downloadHandler.text);
                } 
                if(GameObject.Find("FriendListCanvas/Background").activeSelf){      // refresh the friend list
                    StartCoroutine(GetFriendList(PlayerPrefs.GetString("name")));
                }
            }
        }
    }
}