using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
          Debug.Log("Successfully deleted draft room. Returning to Hub...");
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
          Debug.Log("Successfully left draft room. Returning to Hub...");
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
          if (serverJson.Trim() == "{\"Error\":\"Draft not found\"}")
          {
            Debug.Log("Draft room has been deleted! Returning to Hub...");
            SceneManager.LoadScene("Hub");
          }
          else
          {
            Draft draft = JsonUtility.FromJson<Draft>(serverJson);
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
      // Set draft start flag in server
      string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.draftHostID + "/start";
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        // Should we generate all the packs and upload to server? Or each player generates their packs when entering deck creator?
      }
    }
}
