using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleObject;
    public GameObject loginObject;
    public GameObject userObject;
    public GameObject passObject;
    public GameObject statusObject;
    public GameObject objectGroup;
    public GameObject friendsPanelObject;
    private TMP_InputField username;
    private TMP_InputField password;
    private TMP_Text status;
    private Image title;
    private Image login;
    private Color alertColor;
    private Color normalColor;
    private JsonNetworking serverCommunicator;
    private Hasher hasher;
    private string hashedPassword;
    private int userID;
    // Start is called before the first frame update
    void Start()
    {
      if (!PlayerManager.Instance.loggedIn)
      {
        title = titleObject.GetComponent<Image>();
        login = loginObject.GetComponent<Image>();
        username = userObject.GetComponent<TMP_InputField>();
        password = passObject.GetComponent<TMP_InputField>();
        status = statusObject.GetComponent<TMP_Text>();
        alertColor = new Color(1f, 0.9f, 0f, 1f);
        normalColor = new Color(1f, 1f, 1f, 1f);
        serverCommunicator = GetComponent<JsonNetworking>();
        hasher = GetComponent<Hasher>();
        if (PlayerPrefs.HasKey("lastUser"))
        {
          username.text = PlayerPrefs.GetString("lastUser");
        }
        StartCoroutine(FadeTitleToFullAlpha(2f, title));
      }
      else {
        if (gameObject.activeSelf)
        {
          friendsPanelObject.GetComponent<FriendsPanel>().loadFriends();
          gameObject.SetActive(false);
        }
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FadeTitleToFullAlpha(float t, Image i)
    {
        yield return new WaitForSeconds(1);
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeTitleToZeroAlpha(2f, i));
    }
 
    public IEnumerator FadeTitleToZeroAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        titleObject.SetActive(false);
        loginObject.SetActive(true);
        StartCoroutine(FadeToFullAlpha(1.5f, login));
    }

    public IEnumerator FadeToFullAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public void authenticate()
    {
      // Reset status message
      status.text = "";

      if (username.text == "")
      {
        status.color = alertColor;
        status.text = "Please enter a valid username";
        return;
      }
      if (password.text == "")
      {
        status.color = alertColor;
        status.text = "Please enter a valid password";
        return;
      }
      hashedPassword = hasher.getHash(password.text);
      // Get the username list from the server
      StartCoroutine(authenticateWithServer());
    }

    IEnumerator authenticateWithServer()
    {
      string url = PlayerManager.Instance.apiUrl + "users";
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else
        {
          string serverJson = request.downloadHandler.text;
          AllUsers allUsers = JsonUtility.FromJson<AllUsers>(serverJson);
          bool userFound = false;
          string serverHashPassword = "";
          foreach (User user in allUsers.users)
          {
            if (user.username.ToLower() == username.text.ToLower())
            {
              userFound = true;
              serverHashPassword = user.password;
              userID = user.id;
              break;
            }
          }
          if (userFound)
          {
            // User was found in the server
            if (hashedPassword == serverHashPassword)
            {
              status.color = normalColor;
              status.text = "Successfully authenticated\nLogging in...";
              PlayerPrefs.SetString("lastUser", username.text);
              PlayerManager.Instance.loggedIn = true;
              PlayerManager.Instance.myName = username.text;
              yield return new WaitForSeconds(3);
              StartCoroutine(setUserID(username.text));
              hideTitleScreen();
            }
            else
            {
              status.color = alertColor;
              status.text = "Incorrect password";
            }
          }
          else
          {
            // Create new user and send to server
            User newUser = new User();
            newUser.username = username.text;
            newUser.password = hashedPassword;
            string serverUrl = PlayerManager.Instance.apiUrl + "users";
            string jsonUser = JsonUtility.ToJson(newUser);
            serverCommunicator.sendJson(serverUrl, jsonUser);
            // Update status and hide title screen
            status.color = normalColor;
            status.text = "New user created\nLogging in...";
            PlayerPrefs.SetString("lastUser", username.text);
            PlayerManager.Instance.loggedIn = true;
            PlayerManager.Instance.myName = username.text;
            yield return new WaitForSeconds(3);
            StartCoroutine(setUserID(username.text));
            hideTitleScreen();
          }
        }
      }
    }

    IEnumerator setUserID(string username)
    {
      string url = PlayerManager.Instance.apiUrl + "users";
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else
        {
          string serverJson = request.downloadHandler.text;
          AllUsers allUsers = JsonUtility.FromJson<AllUsers>(serverJson);
          foreach (User user in allUsers.users)
          {
            if (username == user.username)
            {
              PlayerManager.Instance.myID = user.id;
              Debug.Log("My ID: " + PlayerManager.Instance.myID);
              friendsPanelObject.GetComponent<FriendsPanel>().loadFriends();
              break;
            }
          }
          // Delete all challenges for this user
          url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges";
          UnityWebRequest deleteRequest = new UnityWebRequest(url);
          deleteRequest.method = UnityWebRequest.kHttpVerbDELETE;
          yield return deleteRequest.SendWebRequest();
          if (deleteRequest.result == UnityWebRequest.Result.ConnectionError || deleteRequest.result == UnityWebRequest.Result.ProtocolError)
          {
            Debug.Log(deleteRequest.error);
          }
          else
          {
            Debug.Log("Deleted all challenges for this user...");
          }
        }
      }
    }

    // Hide the screen after successful login
    public void hideTitleScreen()
    {
      StartCoroutine(FadeAllToZeroAlpha());
    }

    public IEnumerator FadeAllToZeroAlpha()
    {
        float t = 1f;
        CanvasGroup group = objectGroup.GetComponent<CanvasGroup>();
        float alpha = 1f;
        group.alpha = alpha;
        while ((group.alpha) > 0.0f)
        {
            group.alpha = group.alpha - (Time.deltaTime / t);
            yield return null;
        }

        t = 4f;
        Image titleScreen = GetComponent<Image>();
        titleScreen.color = new Color(1, 1, 1, 1);
        while (titleScreen.color.a > 0.0f)
        {
            titleScreen.color = new Color(1, 1, 1, titleScreen.color.a - (Time.deltaTime / t));
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
