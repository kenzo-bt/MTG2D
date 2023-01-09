using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameState : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject opponentObject;
    private Player player;
    private Opponent opponent;
    private string lastOpponentHash;
    private string lastHash;
    private int playerID;
    private int opponentID;

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
      lastOpponentHash = "";
      player = playerObject.GetComponent<Player>();
      opponent = opponentObject.GetComponent<Opponent>();
      playerID = PlayerManager.Instance.myID;
      opponentID = PlayerManager.Instance.opponentID;
      // Send state
      InvokeRepeating("sendState", 1f, 3f);
      // Start listening for changes in opponent state
      InvokeRepeating("getOpponentState", 3f, 3f);
    }

    public void getOpponentState()
    {
      StartCoroutine(getOpponentStateFromServer());
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
          player.logMessage("Failed sending new state to server");
        }
        else
        {
          Debug.Log("State sent successfully to server");
          player.logMessage("(POST) State sent successfully to server");
        }
        // Save the hash
        lastHash = myState.hash;
        // Dispose of the request to prevent memory leaks
        request.Dispose();
      }
      else {
        Debug.Log("Your state has NOT changed. Wont send it to server.");
        player.logMessage("Your state has NOT changed. Wont update server");
      }
    }

    IEnumerator getOpponentStateFromServer()
    {
      // Get only hash first and compare to last hash. If different, fetch new state
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
          Debug.Log("Successfully fetched opponent state hash value.");
          string opponentHash = request.downloadHandler.text;
          if (opponentHash != lastOpponentHash)
          {
            // Fetch new state
            url = PlayerManager.Instance.apiUrl + "users/" + opponentID + "/state";

            using (UnityWebRequest stateRequest = UnityWebRequest.Get(url))
            {
              yield return stateRequest.SendWebRequest();
              if (stateRequest.result == UnityWebRequest.Result.ConnectionError || stateRequest.result == UnityWebRequest.Result.ProtocolError)
              {
                Debug.Log(stateRequest.error);
                player.logMessage("Failed retrieving opponent state from server");
              }
              else
              {
                Debug.Log("Successfully fetched new opponent state from server");
                player.logMessage("(GET) Successfully fetched new opponent state from server");
                string serverJson = stateRequest.downloadHandler.text;
                BoardState oppState = JsonUtility.FromJson<BoardState>(serverJson);
                opponent.prevState = opponent.state;
                opponent.state = oppState;
                Debug.Log("Previous opponent state:");
                opponent.prevState.debugState();
                Debug.Log("New opponent state:");
                opponent.state.debugState();
                opponent.updateBoard();
              }
            }
            // Update lastOpponentHash
            lastOpponentHash = opponentHash;
          }
          else {
            Debug.Log("Hashes matched. Not fetching full opponent state...");
            player.logMessage("Hashes matched. Not fetching full opponent state");
          }
        }
      }
    }
}
