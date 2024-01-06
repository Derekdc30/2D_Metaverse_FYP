using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class Login : MonoBehaviour
{
    public GameObject emailInput;
    public GameObject passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    UserData userData;
    [SerializeField] string loginURL = "http://127.0.0.1:3000/user/login";

    [System.Serializable]
    public class UserData{
        public string name;
        public string email;
    }


    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(login);
        goToRegisterButton.onClick.AddListener(moveToRegister);
    }



    // Update is called once per frame
    void login()
    {
        string email = emailInput.GetComponent<TMP_InputField>().text;
        string password = passwordInput.GetComponent<TMP_InputField>().text;
        if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)){
            Debug.Log("Empty field");
        }
        else{
            StartCoroutine(ValidateUser(email,password));
        }
    }

    void moveToRegister()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    void loadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    IEnumerator ValidateUser(string email, string password){
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(loginURL, form)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.ConnectionError){
                Debug.Log(www.error);
            }
            else{
                Debug.Log(www.downloadHandler.text);
                if(www.responseCode == 200){
                    Debug.Log("Login successful");
                    var responseData = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                    string playerName = responseData.name;
                    string playerEmail = responseData.email;
                    PlayerPrefs.SetString("name", playerName);
                    PlayerPrefs.SetString("email", playerEmail);
                    loadMainScene();
                }
                else{
                    Debug.Log("Login failed: " + www.downloadHandler.text);
                }
            }
                
        }
    }
}

