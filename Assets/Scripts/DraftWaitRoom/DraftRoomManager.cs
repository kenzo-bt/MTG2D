using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class DraftRoomManager : MonoBehaviour
{
    public GameObject roomTitleObject;
    public GameObject statusMessageObject;
    public GameObject playerGridObject;
    public GameObject playerEntryPrefab;
    public GameObject startDraftButton;
    public List<int> playerIds;
    public string hostName;
    public string setName;
    public List<string> setCodes;

    // Start is called before the first frame update
    void Start()
    {
      playerIds = new List<int>();
      hostName = "";
      setName = "";
      setCodes = new List<string>();
      initializeRoom();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeRoom()
    {
      InvokeRepeating("updateRoomData", 0f, 3.0f);
    }

    public void returnToHub()
    {
      StartCoroutine(leaveDraftRoom());
    }

    public IEnumerator leaveDraftRoom()
    {
      // If you are host, then delete draft room
      if (PlayerManager.Instance.myID == PlayerManager.Instance.draftHostID)
      {
        string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.draftHostID;
        UnityWebRequest request = new UnityWebRequest(url);
        request.method = UnityWebRequest.kHttpVerbDELETE;
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else {
          SceneManager.LoadScene("Hub");
        }
      }
      else // Else -> remove yourself from draft in server and go back to hub
      {
        string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.draftHostID + "/players/" + PlayerManager.Instance.myID;
        UnityWebRequest request = new UnityWebRequest(url);
        request.method = UnityWebRequest.kHttpVerbDELETE;
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else {
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
      string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.draftHostID;
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
          if (serverJson.Trim() == "{\"Error\":\"Draft not found\"}")
          {
            SceneManager.LoadScene("Hub");
          }
          Draft draft = JsonUtility.FromJson<Draft>(serverJson);
          // Enter draft editor if draft has been started by host
          if (draft.started == 1)
          {
            SceneManager.LoadScene("DraftEditor");
          }
          // Update message
          if (draft.players.Count < draft.capacity)
          {
            statusMessageObject.GetComponent<TMP_Text>().text = "Waiting for players... (" + draft.players.Count + "/" + draft.capacity + ")";
            if (PlayerManager.Instance.myID == PlayerManager.Instance.draftHostID)
            {
              if (startDraftButton.activeSelf)
              {
                startDraftButton.SetActive(false);
              }
            }
          }
          else
          {
            if (PlayerManager.Instance.myID == PlayerManager.Instance.draftHostID)
            {
              if (!startDraftButton.activeSelf)
              {
                startDraftButton.SetActive(true);
              }
              statusMessageObject.GetComponent<TMP_Text>().text = "";
            }
            else
            {
              statusMessageObject.GetComponent<TMP_Text>().text = "Waiting for host to start draft";
            }
          }
          // Update set name
          if (setName == "")
          {
            setName = draft.setName;
          }
          // Update set codes
          if (setCodes.Count == 0)
          {
            setCodes.Add(draft.set1);
            setCodes.Add(draft.set2);
            setCodes.Add(draft.set3);
          }
          // Update title
          if (hostName != draft.hostName)
          {
            roomTitleObject.GetComponent<TMP_Text>().text = draft.hostName + "\'s Draft Room";
            hostName = draft.hostName;
          }
          // Populate room grid
          if (!playerIds.SequenceEqual(draft.players))
          {
            clearPlayerGrid();
            foreach (int playerID in draft.players)
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
            playerIds = new List<int>(draft.players);
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

    public void startDraft()
    {
      StartCoroutine(initializeDraftInServer());
    }

    public IEnumerator initializeDraftInServer()
    {
      // Generate booster packs for each player
      List<CardSet> draftSets = new List<CardSet>();
      foreach (string setCode in setCodes)
      {
        foreach (CardSet set in PlayerManager.Instance.cardCollection)
        {
          if (set.setCode == setCode)
          {
            draftSets.Add(set);
          }
        }
      }
      // Generate player packs
      foreach (int id in playerIds)
      {
        DraftPacks playerDraftPacks = new DraftPacks();
        playerDraftPacks.draftPacks = new List<Pack>();
        foreach (CardSet draftSet in draftSets)
        {
          Pack pack = new Pack();
          pack.cards = new List<string>(draftSet.getPack());
          playerDraftPacks.draftPacks.Add(pack);
        }
        // Upload player packs to server
        string packsJson = JsonUtility.ToJson(playerDraftPacks);
        string postUrl = PlayerManager.Instance.apiUrl + "users/" + id + "/draftPacks";
        byte[] bytes = Encoding.UTF8.GetBytes(packsJson);
        UnityWebRequest request = new UnityWebRequest(postUrl);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = new UploadHandlerRaw (bytes);
        request.uploadHandler.contentType = "application/json";
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        request.Dispose();
      }
      // Set draft start flag in server
      string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.draftHostID + "/start";
      UnityWebRequest startRequest = new UnityWebRequest(url);
      startRequest.method = UnityWebRequest.kHttpVerbPOST;
      yield return startRequest.SendWebRequest();
      if(startRequest.result == UnityWebRequest.Result.ConnectionError || startRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(startRequest.error);
      }
      else
      {
        SceneManager.LoadScene("DraftEditor");
      }
      startRequest.Dispose();
    }
}
