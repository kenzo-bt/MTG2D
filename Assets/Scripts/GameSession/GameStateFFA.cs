using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class GameStateFFA : MonoBehaviour
{
    public GameObject playerObject;
    private Player player;
    private string lastHash;
    private int playerID;
    public List<GameObject> opponentObjects;
    private List<int> opponentIds;
    private List<string> lastOpponentHashes;
    public GameObject playerName;
    public List<GameObject> opponentNameObjects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeState()
    {
      lastHash = "";
      player = playerObject.GetComponent<Player>();
      playerID = PlayerManager.Instance.myID;
      opponentIds = new List<int>(PlayerManager.Instance.lobbyOpponents);
      lastOpponentHashes = new List<string>();
      for (int i = 0; i < opponentIds.Count; i++)
      {
        lastOpponentHashes.Add("");
        opponentObjects[i].GetComponent<Opponent>().initializeOpponent();
      }
      // Get names
      StartCoroutine(fetchOpponentNamesFromServer());
      // Send state
      InvokeRepeating("sendState", 0.5f, 1f);
      // Start listening for changes in opponent state
      InvokeRepeating("getOpponentStates", 1.5f, 1f);
    }

    public void getOpponentStates()
    {
      foreach (int oppId in opponentIds)
      {
        StartCoroutine(getOpponentStateFromServer(oppId));
      }
    }

    public void sendState()
    {
      StartCoroutine(sendStateToServer());
    }

    IEnumerator sendStateToServer()
    {
      // Get JSON string and encode it as UTF8 bytes to send as POST body
      BoardState myState = player.getBoardState();
      // Compare to see if last hash is same as new hash
      if (myState.hash != lastHash)
      {
        string state = JsonUtility.ToJson(myState);
        string url = PlayerManager.Instance.apiUrl + "users/" + playerID + "/state";
        // Send post request to update my state in the server
        byte[] bytes = Encoding.UTF8.GetBytes(state);
        // Create the web request and set the correct properties
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
        // Save the hash
        lastHash = myState.hash;
        // Dispose of the request to prevent memory leaks
        request.Dispose();
      }
    }

    IEnumerator getOpponentStateFromServer(int opponentID)
    {
      // Get only hash first and compare to last hash. If different, fetch new state
      int opponentIndex = opponentIds.IndexOf(opponentID);
      string url = PlayerManager.Instance.apiUrl + "users/" + opponentID + "/state/hash";
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else
        {
          string opponentHash = request.downloadHandler.text;
          if (opponentHash != lastOpponentHashes[opponentIndex])
          {
            // Fetch new state
            url = PlayerManager.Instance.apiUrl + "users/" + opponentID + "/state";

            using (UnityWebRequest stateRequest = UnityWebRequest.Get(url))
            {
              yield return stateRequest.SendWebRequest();
              if (stateRequest.result == UnityWebRequest.Result.ConnectionError || stateRequest.result == UnityWebRequest.Result.ProtocolError)
              {
                Debug.Log(stateRequest.error);
              }
              else
              {
                string serverJson = stateRequest.downloadHandler.text;
                BoardState oppState = JsonUtility.FromJson<BoardState>(serverJson);
                Opponent opponent = opponentObjects[opponentIndex].GetComponent<Opponent>();
                opponent.prevState = opponent.state;
                opponent.state = oppState;
                opponent.state.debugState();
                opponent.updateBoard();
              }
            }
            // Update lastOpponentHash
            lastOpponentHashes[opponentIndex] = opponentHash;
          }
        }
      }
    }

    private IEnumerator fetchOpponentNamesFromServer()
    {
      List<string> oppNames = new List<string>();
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
            oppNames.Add(user.username);
          }
        }
      }
      // Update names
      playerName.GetComponent<TMP_Text>().text = PlayerManager.Instance.myName;
      for (int i = 0; i < oppNames.Count; i++)
      {
        opponentNameObjects[i].GetComponent<TMP_Text>().text = oppNames[i];
      }
    }
}
