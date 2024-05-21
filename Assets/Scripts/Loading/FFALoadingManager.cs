using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class FFALoadingManager : MonoBehaviour
{
  private int opponentFetchIndex;
  private List<Decklist> opponentDecks;
  private List<string> opponentNames;
  public GameObject displayScreen;
  public GameObject yourDisplay;
  public GameObject yourName;
  public List<GameObject> opponentDisplays;
  public List<GameObject> opponentNameObjects;

  void Start()
  {
    opponentFetchIndex = 0;
    opponentDecks = new List<Decklist>();
    opponentNames = new List<string>();
    StartCoroutine(deletePreviousState());
    StartCoroutine(fetchOpponentNamesFromServer());
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
  }

  private IEnumerator fetchOpponentNamesFromServer()
  {
    foreach (int playerID in PlayerManager.Instance.lobbyOpponents)
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + playerID;
      using (UnityWebRequest nameRequest = UnityWebRequest.Get(url))
      {
        yield return nameRequest.SendWebRequest();
        if (nameRequest.result == UnityWebRequest.Result.ConnectionError || nameRequest.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(nameRequest.error);
        }
        else
        {
          string serverJson = nameRequest.downloadHandler.text;
          User user = JsonUtility.FromJson<User>(serverJson);
          opponentNames.Add(user.username);
        }
      }
    }
    StartCoroutine(fetchOpponentDecksFromServer());
  }

  private IEnumerator fetchOpponentDecksFromServer()
  {
    string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.lobbyOpponents[opponentFetchIndex] + "/activeDeck";
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
        opponentDecks.Add(JsonUtility.FromJson<Decklist>(serverJson));
        opponentFetchIndex++;
        if (opponentFetchIndex < PlayerManager.Instance.lobbyOpponents.Count)
        {
          StartCoroutine(fetchOpponentDecksFromServer());
        }
        else
        {
          PlayerManager.Instance.lobbyOpponentDecks = new List<Decklist>(opponentDecks);
          updateDeckDisplays();
        }
      }
    }
  }

  private void updateDeckDisplays()
  {
    Decklist myDeck = PlayerManager.Instance.selectedDeck;
    yourDisplay.GetComponent<DeckDisplay>().setDisplayData(myDeck.name, PlayerManager.Instance.getCardFromLookup(myDeck.coverId));
    yourName.GetComponent<TMP_Text>().text = PlayerManager.Instance.myName;
    for (int i = 0; i < opponentDecks.Count; i++)
    {
      opponentDisplays[i].GetComponent<DeckDisplay>().setDisplayData(opponentDecks[i].name, PlayerManager.Instance.getCardFromLookup(opponentDecks[i].coverId));
      opponentDisplays[i].GetComponent<CanvasGroup>().alpha = 1;
      opponentNameObjects[i].GetComponent<TMP_Text>().text = opponentNames[i];
      opponentNameObjects[i].GetComponent<CanvasGroup>().alpha = 1;
    }
    StartCoroutine(readyToPlayAnimation());
  }

  IEnumerator readyToPlayAnimation()
  {
    CanvasGroup vsScreen = displayScreen.GetComponent<CanvasGroup>();
    float t = 2f;
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
    SceneManager.LoadScene("FFASession");
  }
}
