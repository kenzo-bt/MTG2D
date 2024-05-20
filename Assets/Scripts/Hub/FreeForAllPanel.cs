using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class FreeForAllPanel : MonoBehaviour
{
    public GameObject entryList;
    public GameObject entryPrefab;
    public GameObject createOverlay;
    public GameObject joinButton;
    public GameObject selectPanel;
    public GameObject deckSelectJoinButton;
    public GameObject deckSelectCreateButton;
    public GameObject deckBrowser;
    public GameObject deckDisplay;
    public GameObject objectivesPanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showPanel()
    {
      refreshEntries();
      CanvasGroup cg = GetComponent<CanvasGroup>();
      cg.alpha = 1;
      cg.blocksRaycasts = true;
    }

    public void hidePanel()
    {
      CanvasGroup cg = GetComponent<CanvasGroup>();
      cg.alpha = 0;
      cg.blocksRaycasts = false;
      closeDeckSelect();
      closeDeckBrowser();
      objectivesPanel.GetComponent<ObjectivesPanel>().showPanel();
    }

    public void openDeckSelect()
    {
      selectPanel.GetComponent<CanvasGroup>().alpha = 1;
      selectPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
      if (PlayerManager.Instance.selectedDeck.cards.Count <= 0)
      {
        deckDisplay.SetActive(false);
      }
      else
      {
        updateSelectedDeck();
      }
      selectPanel.GetComponent<MultiplayerMenu>().showPanel();
    }

    public void closeDeckSelect()
    {
      selectPanel.GetComponent<CanvasGroup>().alpha = 0;
      selectPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
      selectPanel.GetComponent<MultiplayerMenu>().hidePanel();
    }

    public void deckSelectCreateMode()
    {
      deckSelectCreateButton.SetActive(true);
      deckSelectJoinButton.SetActive(false);
      openDeckSelect();
    }

    public void deckSelectJoinMode()
    {
      deckSelectJoinButton.SetActive(true);
      deckSelectCreateButton.SetActive(false);
      openDeckSelect();
    }

    public void updateSelectedDeck()
    {
      Decklist selectedDeck = PlayerManager.Instance.selectedDeck;
      CardInfo coverCard = PlayerManager.Instance.getCardFromLookup(selectedDeck.coverId);
      deckDisplay.GetComponent<DeckDisplay>().setDisplayData(selectedDeck.name, coverCard);
      deckDisplay.SetActive(true);
    }

    public void closeDeckBrowser()
    {
      deckBrowser.GetComponent<DeckBrowser>().hideBrowser();
    }

    public void createLobby()
    {
      if (PlayerManager.Instance.selectedDeck.cards.Count > 0)
      {
        StartCoroutine(createLobbyInServer());
      }
    }

    public IEnumerator createLobbyInServer()
    {
      Lobby lobby = new Lobby();
      lobby.hostId = PlayerManager.Instance.myID;
      lobby.hostName = PlayerManager.Instance.myName;
      lobby.players = new List<int>();
      lobby.players.Add(lobby.hostId);
      lobby.started = 0;
      // Send to lobby to server
      string lobbyJson = JsonUtility.ToJson(lobby);
      byte[] bytes = Encoding.UTF8.GetBytes(lobbyJson);
      string url = PlayerManager.Instance.apiUrl + "lobbies";
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      // Debug the results
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        // Send to draft room
        Debug.Log("Lobby created successfully in server. Proceeding to wait room...");
        PlayerManager.Instance.lobbyHostID = PlayerManager.Instance.myID;
        SceneManager.LoadScene("LobbyWaitRoom");
      }
      // Dispose of the request to prevent memory leaks
      request.Dispose();
    }

    public void refreshEntries()
    {
      deleteEntries();
      StartCoroutine(fetchLobbiesFromServer());
    }

    public IEnumerator fetchLobbiesFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "lobbies";
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
          AllLobbies allLobbies = JsonUtility.FromJson<AllLobbies>(serverJson);
          foreach (Lobby lobby in allLobbies.lobbies)
          {
            int hostId = lobby.hostId;
            string hostName = lobby.hostName;
            string capacity = "" + lobby.players.Count + " / 4";
            GameObject entry = Instantiate(entryPrefab, entryList.transform);
            entry.GetComponent<LobbyListEntry>().setInfo(hostId, hostName, capacity);
          }
        }
      }
    }

    public void deleteEntries()
    {
      int numEntries = entryList.transform.childCount;
      for (int i = 0; i < numEntries; i++)
      {
        DestroyImmediate(entryList.transform.GetChild(0).gameObject);
      }
    }

    public void updateSelection()
    {
      bool lobbySelected = false;
      foreach (Transform entry in entryList.transform)
      {
        if (entry.gameObject.GetComponent<LobbyListEntry>().selected)
        {
          lobbySelected = true;
          PlayerManager.Instance.lobbyHostID = entry.gameObject.GetComponent<LobbyListEntry>().hostID;
          break;
        }
      }
      if (lobbySelected)
      {
        joinButton.SetActive(true);
      }
      else
      {
        joinButton.SetActive(false);
      }
    }

    public void joinLobbyRoom()
    {
      if (PlayerManager.Instance.selectedDeck.cards.Count > 0)
      {
        StartCoroutine(sendLobbyJoinRequest());
      }
    }

    public IEnumerator sendLobbyJoinRequest()
    {
      // Check if room has capacity
      string url = PlayerManager.Instance.apiUrl + "lobbies/" + PlayerManager.Instance.lobbyHostID;
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
          Lobby lobby = JsonUtility.FromJson<Lobby>(serverJson);
          if (lobby.players.Count >= 4)
          {
            Debug.Log("Couldn't join " +  lobby.hostName + "\'s lobby. Room is full.");
          }
          else
          {
            if (!lobby.players.Contains(PlayerManager.Instance.myID))
            {
              StartCoroutine(joinLobbyInServer());
            }
            else
            {
              Debug.Log("Couldn't join " + lobby.hostName + "\'s lobby. You are already in the room (?)");
            }
          }
        }
      }
    }

    public IEnumerator joinLobbyInServer()
    {
      // Add yourself to lobby
      string url = PlayerManager.Instance.apiUrl + "lobbies/" + PlayerManager.Instance.lobbyHostID + "/players/" + PlayerManager.Instance.myID;
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      yield return request.SendWebRequest();
      // Debug the results
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        Debug.Log("Successfully entered lobby room in server...");
        SceneManager.LoadScene("LobbyWaitRoom");
      }
      request.Dispose();
    }
}
