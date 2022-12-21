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
    public GameObject deckObject;
    public GameObject handObject;

    private string playerID;
    private string opponentID;

    // Start is called before the first frame update
    void Start()
    {
      playerID = PlayerManager.Instance.myID;
      opponentID = PlayerManager.Instance.opponentID;
      //InvokeRepeating("fetchData", 0f, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Restart Game
    public void restartGame()
    {
      // Reset deck
      Deck deck = deckObject.GetComponent<Deck>();
      deck.resetDeck();
      // Reset hand
      Hand hand = handObject.GetComponent<Hand>();
      hand.resetHand();
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
      // Construct URL based on API requirements
      string url = PlayerManager.Instance.apiUrl + "users/" + playerID + "/state";
      // Get JSON string and encode it as UTF8 bytes to send as POST body
      string state = playerObject.GetComponent<Player>().getBoardState();
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
      else
      {
        Debug.Log("State sent successfully to server");
      }
      // Dispose of the request to prevent memory leaks
      request.Dispose();
    }

    IEnumerator getOpponentStateFromServer()
    {
      // Construct URL based on API requirements
      string url = PlayerManager.Instance.apiUrl + "users/" + opponentID + "/state";

      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else
        {
          Debug.Log("Successfully fetched opponent state from server");

          string serverJson = request.downloadHandler.text;
          BoardState oppState = JsonUtility.FromJson<BoardState>(serverJson);
          Debug.Log(serverJson);
          oppState.debugState();
        }
      }
    }
}
