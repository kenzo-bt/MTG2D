using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public string myRole;
    public GameObject statusObject;
    public GameObject versusScreenObject;
    public GameObject yourName;
    public GameObject opponentName;
    public GameObject yourDisplay;
    public GameObject opponentDisplay;
    public GameObject oppDraftStatus;
    public GameObject yourDraftStatus;
    private CanvasGroup vsScreen;
    private TMP_Text status;
    private Decklist opponentDeck;
    // Start is called before the first frame update
    void Start()
    {
      status = statusObject.GetComponent<TMP_Text>();
      myRole = PlayerManager.Instance.role;
      vsScreen = versusScreenObject.GetComponent<CanvasGroup>();
      yourName.GetComponent<TMP_Text>().text = PlayerManager.Instance.myName;
      opponentName.GetComponent<TMP_Text>().text = PlayerManager.Instance.opponentName;
      // Update deck in loading screen
      showYourDeck();
      // Upload selected deck to server
      StartCoroutine(uploadActiveDeckToServer());
      // Clean my state in the server
      StartCoroutine(deletePreviousState());
      if (myRole == "challenger")
      {
        PlayerManager.Instance.chooseGameStart = true;
        InvokeRepeating("checkIfGuestReady", 3.0f, 3.0f);
      }
      else if (myRole == "guest")
      {
        PlayerManager.Instance.chooseGameStart = false;
        StartCoroutine(setReadyAndWait());
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator deletePreviousState()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/state";
      UnityWebRequest deleteRequest = new UnityWebRequest(url);
      deleteRequest.method = UnityWebRequest.kHttpVerbDELETE;
      yield return deleteRequest.SendWebRequest();
      if (deleteRequest.result == UnityWebRequest.Result.ConnectionError || deleteRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(deleteRequest.error);
      }
      else
      {
        Debug.Log("Previous state deleted.");
      }
    }

    private void checkIfGuestReady()
    {
      StartCoroutine(checkGuestReadyInServer());
    }

    IEnumerator checkGuestReadyInServer()
    {
      // Get the challenge from server to check if accept value has changed
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.opponentID + "/challenges/" + PlayerManager.Instance.myID;
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
          if (serverJson.Trim() == "{}") // User has delted your challenge request
          {
            Debug.Log("Opponent has declined your challenge");
            CancelInvoke();
            status.text = "Opponent has declined your challenge\nReturning to hub...";
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene("Hub");
          }
          else
          {
            Challenge myChallenge = JsonUtility.FromJson<Challenge>(serverJson);
            if (myChallenge.accepted == 2)
            {
              Debug.Log("Opponent is now on the loading screen...");
              // Change accept to 3 (POST)
              myChallenge.accepted = 3;
              string updatedChallenge = JsonUtility.ToJson(myChallenge);
              byte[] bytes = Encoding.UTF8.GetBytes(updatedChallenge);
              UnityWebRequest postRequest = new UnityWebRequest(url);
              postRequest.method = UnityWebRequest.kHttpVerbPOST;
              postRequest.uploadHandler = new UploadHandlerRaw (bytes);
              postRequest.uploadHandler.contentType = "application/json";
              yield return postRequest.SendWebRequest();
              // Debug the results
              if(postRequest.result == UnityWebRequest.Result.ConnectionError || postRequest.result == UnityWebRequest.Result.ProtocolError)
              {
                Debug.Log(postRequest.error);
              }
              else
              {
                Debug.Log("Accepted flag set to 3...");
                CancelInvoke();
                // Proceed to game session...
                enterGameSession();
              }
              // Dispose of the request to prevent memory leaks
              postRequest.Dispose();
            }
            else
            {
              Debug.Log("Opponent not yet in loading screen...");
            }
          }
        }
      }
    }

    IEnumerator setReadyAndWait()
    {
      // Set challenge flag to 2 and post to server
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges/" + PlayerManager.Instance.opponentID;
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
          Challenge oppChallenge = JsonUtility.FromJson<Challenge>(serverJson);
          oppChallenge.accepted = 2;
          string updatedChallenge = JsonUtility.ToJson(oppChallenge);
          byte[] bytes = Encoding.UTF8.GetBytes(updatedChallenge);
          UnityWebRequest postRequest = new UnityWebRequest(url);
          postRequest.method = UnityWebRequest.kHttpVerbPOST;
          postRequest.uploadHandler = new UploadHandlerRaw (bytes);
          postRequest.uploadHandler.contentType = "application/json";
          yield return postRequest.SendWebRequest();
          if(postRequest.result == UnityWebRequest.Result.ConnectionError || postRequest.result == UnityWebRequest.Result.ProtocolError)
          {
            Debug.Log(postRequest.error);
          }
          else
          {
            Debug.Log("Accepted flag set to 2...");
            InvokeRepeating("waitForReadiness", 3.0f, 3.0f);
          }
          postRequest.Dispose();
        }
      }
    }

    private void waitForReadiness()
    {
      StartCoroutine(waitForReadinessInServer());
    }

    IEnumerator waitForReadinessInServer()
    {
      // Wait until the challenge accepted flag is set to 3
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges/" + PlayerManager.Instance.opponentID;
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
          Challenge oppChallenge = JsonUtility.FromJson<Challenge>(serverJson);
          if (oppChallenge.accepted == 3)
          {
            CancelInvoke();
            // Remove the challenge and proceed to game session
            StartCoroutine(removeChallengeAndEnterGame());
          }
          else
          {
            Debug.Log("Challenger is not ready yet.");
          }
        }
      }
    }

    IEnumerator removeChallengeAndEnterGame()
    {
      // Make a delete request to the server for the current challenge
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges/" + PlayerManager.Instance.opponentID;
      UnityWebRequest deleteRequest = new UnityWebRequest(url);
      deleteRequest.method = UnityWebRequest.kHttpVerbDELETE;
      yield return deleteRequest.SendWebRequest();
      if (deleteRequest.result == UnityWebRequest.Result.ConnectionError || deleteRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(deleteRequest.error);
      }
      else
      {
        enterGameSession();
      }
    }

    public void enterGameSession()
    {
      // Fetch opponent active deck and update the play animation
      StartCoroutine(fetchOpponentDeckFromServer());
      // VS screen fade-in-out and change scene to game session
      StartCoroutine(readyToPlayAnimation());
    }

    IEnumerator readyToPlayAnimation()
    {
      float t = 2f;
      status.text = "";
      yield return new WaitForSeconds(1);
      vsScreen.alpha = 0;
      while (vsScreen.alpha < 1.0f)
      {
          vsScreen.alpha = vsScreen.alpha + (Time.deltaTime / t);
          yield return null;
      }
      yield return new WaitForSeconds(4);
      while (vsScreen.alpha > 0f)
      {
          vsScreen.alpha = vsScreen.alpha - (Time.deltaTime / t);
          yield return null;
      }
      yield return new WaitForSeconds(1);
      SceneManager.LoadScene("GameSession");
    }

    private IEnumerator uploadActiveDeckToServer()
    {
      Decklist activeDeck = PlayerManager.Instance.selectedDeck;
      string deckJson = JsonUtility.ToJson(activeDeck);
      byte[] bytes = Encoding.UTF8.GetBytes(deckJson);
      string postUrl = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/activeDeck";
      UnityWebRequest postRequest = new UnityWebRequest(postUrl);
      postRequest.method = UnityWebRequest.kHttpVerbPOST;
      postRequest.uploadHandler = new UploadHandlerRaw (bytes);
      postRequest.uploadHandler.contentType = "application/json";
      yield return postRequest.SendWebRequest();
      if(postRequest.result == UnityWebRequest.Result.ConnectionError || postRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(postRequest.error);
      }
      else
      {
        Debug.Log("Selected deck successfully uploaded to server");
      }
      postRequest.Dispose();
    }

    private IEnumerator fetchOpponentDeckFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.opponentID + "/activeDeck";
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
          opponentDeck = JsonUtility.FromJson<Decklist>(serverJson);
          showOpponentDeck();
          Debug.Log("Successfully fetched opponent deck from server");
        }
      }
    }

    private void showYourDeck()
    {
      Decklist yourDeck = PlayerManager.Instance.selectedDeck;
      yourDisplay.GetComponent<DeckDisplay>().setDisplayData(yourDeck.name, PlayerManager.Instance.getCardFromLookup(yourDeck.coverId));
      if (yourDeck.isDraft)
      {
        yourDraftStatus.SetActive(true);
      }
    }

    private void showOpponentDeck()
    {
      opponentDisplay.GetComponent<DeckDisplay>().setDisplayData(opponentDeck.name, PlayerManager.Instance.getCardFromLookup(opponentDeck.coverId));
      if (opponentDeck.isDraft)
      {
        oppDraftStatus.SetActive(true);
      }
    }
}
