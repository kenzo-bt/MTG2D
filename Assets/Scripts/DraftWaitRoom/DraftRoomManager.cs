using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeRoom()
    {
      updateRoomData();
      // Invoke the status checker
    }

    public void returnToHub()
    {
      // Remove yourself from draft in server and go back to hub
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
          Draft draft = JsonUtility.FromJson<Draft>(serverJson);
          // Update title
          roomTitleObject.GetComponent<TMP_Text>().text = draft.hostName + "\'s Draft Room";
          // Populate room grid
          clearPlayerGrid();
          foreach (int playerID in draft.players)
          {
            // TODO: Add player names in draft data scheme. Use List<Player> instead of List<int> for players
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
}
