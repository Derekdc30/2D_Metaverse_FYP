using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class Register : MonoBehaviour
{
    public GameObject emailInput;
    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject Re_passwordInput;
    public Button registerButton;
    public Button goToLoginButton;

    [SerializeField] string registerURL = "localhost:3000/user/register";

    [System.Serializable]
    public class UserData{
        public string username;
        public string password;
        public string email;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(register("user02","user02@users.com","123456"));
        goToLoginButton.onClick.AddListener(moveTologin);
        registerButton.onClick.AddListener(register);
    }

    void moveTologin(){
        SceneManager.LoadScene("LoginScene");
    }
    void register(){
        string email = emailInput.GetComponent<TMP_InputField>().text;
        string username = usernameInput.GetComponent<TMP_InputField>().text;
        string password = passwordInput.GetComponent<TMP_InputField>().text;
        string re_password = Re_passwordInput.GetComponent<TMP_InputField>().text;
        if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(re_password)){
            Debug.Log("Empty field"); //action on empty field
        }
        else{
            if(password == re_password){
                StartCoroutine(registerroute(username,email,password));
            }
            else{
                Debug.Log("password not match"); //action on password not match
            }
        }

        
    }
    IEnumerator registerroute (string name, string email, string password){
    WWWForm form = new WWWForm();
    form.AddField("name", name);
    form.AddField("email", email);
    form.AddField("password", password);

    using (UnityWebRequest www = UnityWebRequest.Post(registerURL, form)){
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            moveTologin();
        }
    }
}
}

