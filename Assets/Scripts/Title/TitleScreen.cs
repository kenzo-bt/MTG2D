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
    public GameObject currencyPanelObject;
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
    private int connectedToServer;
    private bool authenticating;
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
        connectedToServer = -1;
        authenticating = false;
        if (PlayerPrefs.HasKey("lastUser"))
        {
          username.text = PlayerPrefs.GetString("lastUser");
        }
        StartCoroutine(PingServer());
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

    public IEnumerator PingServer() {
      string url = PlayerManager.Instance.apiUrl + "ping";
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          connectedToServer = 0;
        }
        else {
          if (request.responseCode == 200) {
            connectedToServer = 1;
          }
        }
      }
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
      if (authenticating) {
        return;
      }
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
      authenticating = true;
      // Get the username list from the server
      StartCoroutine(authenticateWithServer());
    }

    IEnumerator authenticateWithServer()
    {
      string statusMessage = "Connecting to server";
      string delayMessage = "";
      status.color = normalColor;
      status.text = statusMessage;
      int ellipsisCounter = 0;
      while (connectedToServer == -1) {
        yield return new WaitForSeconds(1);
        string ellipsis = " ";
        for (int i = 0; i < (ellipsisCounter % 3) + 1; i++) {
          ellipsis += ".";
        }
        ellipsisCounter++;
        if (ellipsisCounter > 9) {
          statusMessage = "Server is sleeping. Waking up";
          delayMessage = "\n(This may take some time)";
        }
        status.text = statusMessage + ellipsis + delayMessage;
      }
      if (connectedToServer == 0) {
        status.color = alertColor;
        status.text = "Unable to connect to server\nTry submitting again";
        authenticating = false;
        yield break;
      }
      string url = PlayerManager.Instance.apiUrl + "users/auth/" + username.text.ToLower() + "/" + hashedPassword;
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          if (request.responseCode == 400)
          {
            status.color = alertColor;
            status.text = "Incorrect password";
          }
          else
          {
            status.color = alertColor;
            status.text = "Unable to connect to server\nTry submitting again";
          }
          authenticating = false;
        }
        else
        {
          string serverJson = request.downloadHandler.text;
          User user = JsonUtility.FromJson<User>(serverJson);
          PlayerManager.Instance.myID = user.id;
          PlayerManager.Instance.myName = username.text;
          PlayerManager.Instance.loggedIn = true;
          PlayerPrefs.SetString("lastUser", username.text);
          status.color = normalColor;
          if (request.responseCode == 200)
          {
            status.text = "Successfully authenticated\nLogging in...";
          }
          else if (request.responseCode == 201)
          {
            status.text = "New user created\nLogging in...";
          }

          friendsPanelObject.GetComponent<FriendsPanel>().loadFriends();

          yield return currencyPanelObject.GetComponent<CurrencyPanel>().fetchCurrenciesFromServer();
          yield return PlayerManager.Instance.fetchPlayerObjectivesFromServer();
          yield return PlayerManager.Instance.deleteChallenges();
          yield return PlayerManager.Instance.fetchPlayerCollectionFromServer();

          PlayerManager.Instance.loadPlayerDecks();
          PlayerManager.Instance.readStarterDecks();
          PlayerManager.Instance.readProDecks();

          PlayerManager.Instance.deletePlayerDrafts();
          PlayerManager.Instance.deletePlayerLobbies();

          StartCoroutine(FadeAllToZeroAlpha());
        }
      }
    }

    // Hide the screen after successful login
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
