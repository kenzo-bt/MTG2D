using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyRoomManager : MonoBehaviour
{
    public GameObject roomTitleObject;
    public GameObject statusMessageObject;
    public GameObject playerGridObject;
    public GameObject playerEntryPrefab;
    public GameObject startDraftButton;
    public List<int> playerIds;
    public string hostName;

    // Start is called before the first frame update
    void Start()
    {
      playerIds = new List<int>();
      hostName = "";
      initializeRoom();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeRoom()
    {
      StartCoroutine(uploadActiveDeckToServer());
      InvokeRepeating("updateRoomData", 0f, 3.0f);
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

    public void returnToHub()
    {
      StartCoroutine(leaveLobbyRoom());
    }

    public IEnumerator leaveLobbyRoom()
    {
      // If you are host, then delete room
      if (PlayerManager.Instance.myID == PlayerManager.Instance.lobbyHostID)
      {
        string url = PlayerManager.Instance.apiUrl + "lobbies/" + PlayerManager.Instance.lobbyHostID;
        UnityWebRequest request = new UnityWebRequest(url);
        request.method = UnityWebRequest.kHttpVerbDELETE;
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else {
          Debug.Log("Successfully deleted lobby. Returning to Hub...");
          SceneManager.LoadScene("Hub");
        }
      }
      else // Else -> remove yourself from lobby in server and go back to hub
      {
        string url = PlayerManager.Instance.apiUrl + "lobbies/" + PlayerManager.Instance.lobbyHostID + "/players/" + PlayerManager.Instance.myID;
        UnityWebRequest request = new UnityWebRequest(url);
        request.method = UnityWebRequest.kHttpVerbDELETE;
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else {
          Debug.Log("Successfully left lobby. Returning to Hub...");
          SceneManager.LoadScene("Hub");
        }
      }
    }

    public void updateRoomData()
    {
      StartCoroutine(getRoomInfoFromServer());
    }

    public IEnumerator getRoomInfoFromServer()
    {
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
          // Exit if the room has been deleted
          if (serverJson.Trim() == "{\"Error\":\"Lobby not found\"}")
          {
            Debug.Log("Lobby has been deleted! Returning to Hub...");
            SceneManager.LoadScene("Hub");
          }
          Lobby lobby = JsonUtility.FromJson<Lobby>(serverJson);
          if (lobby.started == 1)
          {
            playerIds = new List<int>(lobby.players);
            PlayerManager.Instance.lobbyOpponents = new List<int>();
            foreach (int id in playerIds)
            {
              if (id != PlayerManager.Instance.myID)
              {
                PlayerManager.Instance.lobbyOpponents.Add(id);
              }
            }
            SceneManager.LoadScene("FFALoading");
          }
          // Update message
          if (lobby.players.Count < 4)
          {
            statusMessageObject.GetComponent<TMP_Text>().text = "Waiting for players... (" + lobby.players.Count + "/" + 4 + ")";
            if (PlayerManager.Instance.myID == PlayerManager.Instance.lobbyHostID)
            {
              if (lobby.players.Count == 3)
              {
                if (!startDraftButton.activeSelf)
                {
                  startDraftButton.SetActive(true);
                }
              }
              else
              {
                if (startDraftButton.activeSelf)
                {
                  startDraftButton.SetActive(false);
                }
              }
            }
          }
          else
          {
            if (PlayerManager.Instance.myID == PlayerManager.Instance.lobbyHostID)
            {
              if (!startDraftButton.activeSelf)
              {
                startDraftButton.SetActive(true);
              }
              statusMessageObject.GetComponent<TMP_Text>().text = "";
            }
            else
            {
              statusMessageObject.GetComponent<TMP_Text>().text = "Waiting for host to start...";
            }
          }
          // Update title
          if (hostName != lobby.hostName)
          {
            roomTitleObject.GetComponent<TMP_Text>().text = lobby.hostName + "\'s Free-For-All Room";
            hostName = lobby.hostName;
          }
          // Populate room grid
          if (!playerIds.SequenceEqual(lobby.players))
          {
            clearPlayerGrid();
            foreach (int playerID in lobby.players)
            {
              // Instantiate the entry
              GameObject playerEntry = Instantiate(playerEntryPrefab, playerGridObject.transform);
              // Name the entry
              url = PlayerManager.Instance.apiUrl + "users/" + playerID;
              using (UnityWebRequest nameRequest = UnityWebRequest.Get(url))
              {
                yield return nameRequest.SendWebRequest();
                if (nameRequest.result == UnityWebRequest.Result.ConnectionError || nameRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                  Debug.Log(nameRequest.error);
                }
                else
                {
                  serverJson = nameRequest.downloadHandler.text;
                  User user = JsonUtility.FromJson<User>(serverJson);
                  playerEntry.GetComponent<DraftPlayerEntry>().setUsername(user.username);
                  playerEntry.GetComponent<DraftPlayerEntry>().setReady();
                }
              }
            }
            playerIds = new List<int>(lobby.players);
          }
        }
      }
    }

    public void clearPlayerGrid()
    {
      int numPlayers = playerGridObject.transform.childCount;
      for (int i = 0; i < numPlayers; i++)
      {
        DestroyImmediate(playerGridObject.transform.GetChild(0).gameObject);
      }
    }

    public void startGame()
    {
      StartCoroutine(initializeLobbyInServer());
    }

    public IEnumerator initializeLobbyInServer()
    {
      PlayerManager.Instance.lobbyOpponents = new List<int>();
      foreach (int id in playerIds)
      {
        if (id != PlayerManager.Instance.myID)
        {
          PlayerManager.Instance.lobbyOpponents.Add(id);
        }
      }
      // Set lobby start flag in server
      string url = PlayerManager.Instance.apiUrl + "lobbies/" + PlayerManager.Instance.lobbyHostID + "/start";
      UnityWebRequest startRequest = new UnityWebRequest(url);
      startRequest.method = UnityWebRequest.kHttpVerbPOST;
      yield return startRequest.SendWebRequest();
      if(startRequest.result == UnityWebRequest.Result.ConnectionError || startRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(startRequest.error);
      }
      else
      {
        Debug.Log("Successfully started the lobby in the server.");
        SceneManager.LoadScene("FFALoading");
      }
      startRequest.Dispose();
    }
}
