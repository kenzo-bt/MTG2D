using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Matchmaker : MonoBehaviour
{
    public GameObject playMenu;
    // Once the user has logged in, we can ping the server every X seconds to check if we have open challenges (Idle)
    // Start is called before the first frame update
    void Start()
    {
      startChallengeChecking();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startChallengeChecking()
    {
      if (PlayerManager.Instance.myID != -1)
      {
        InvokeRepeating("checkForChallenges", 0.5f, 2f);
      }
    }

    public void checkForChallenges()
    {
      StartCoroutine(checkForChallengesInServer());
    }

    IEnumerator checkForChallengesInServer()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges";
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
          AllChallenges myChallenges = JsonUtility.FromJson<AllChallenges>(serverJson);
          Debug.Log("Successfully fetched challenges from server");
          // If the challenge comes from one of my friends, turn the friend entry into play mode
          GameObject friendList = GetComponent<FriendsPanel>().friendList;
          foreach (Transform child in friendList.transform)
          {
            if (child.gameObject.name != "AddFriend")
            {
              FriendEntry friend = child.gameObject.GetComponent<FriendEntry>();
              if (friend.sentRequest)
              {
                friend.turnToSent();
              }
              else
              {
                friend.turnToIdle();
              }
              foreach (Challenge ch in myChallenges.challenges)
              {
                if (ch.challengerID == friend.friendID)
                {
                  friend.turnToChallenger();
                  break;
                }
              }
            }
          }
        }
      }
    }

    public void sendChallenge(int targetID)
    {
      StartCoroutine(sendChallengeToServer(targetID));
    }

    IEnumerator sendChallengeToServer(int targetID)
    {
      // Get my challenge information from target player
      string url = PlayerManager.Instance.apiUrl + "users/" + targetID + "/challenges/" + PlayerManager.Instance.myID;
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
          Debug.Log(serverJson);
          // Add new challenge if not already in existence
          if (serverJson.Trim() == "{}")
          {
            Debug.Log("Challenge did not previously exist.");
            Challenge newChallenge = new Challenge();
            newChallenge.challengerID = PlayerManager.Instance.myID;
            newChallenge.accepted = 0;
            // POST to server
            url = PlayerManager.Instance.apiUrl + "users/" + targetID + "/challenges/" + PlayerManager.Instance.myID;
            string updatedChallenges = JsonUtility.ToJson(newChallenge);
            byte[] bytes = Encoding.UTF8.GetBytes(updatedChallenges);
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
              Debug.Log("Challenge successfully sent and updated in server.");
              PlayerManager.Instance.opponentID = targetID;

              waitOnChallengee();
            }
            // Dispose of the request to prevent memory leaks
            postRequest.Dispose();
          }
          else {
            Debug.Log("Challenge already exists on target player");
          }
        }
      }
    }

    public void waitOnChallengee()
    {
      InvokeRepeating("checkIfAccepted", 1.0f, 3.0f);
    }

    public void checkIfAccepted()
    {
      StartCoroutine(checkIfOpponentAccepted());
    }

    IEnumerator checkIfOpponentAccepted()
    {
      // Get my challenge from opponent challenges
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
            Debug.Log("Challenge no longer exists.");
            CancelInvoke();
            // Turn friend entry back to the idle state
            GetComponent<FriendsPanel>().turnToIdle(PlayerManager.Instance.opponentID);
          }
          else
          {
            Challenge myChallenge = JsonUtility.FromJson<Challenge>(serverJson);
            if (myChallenge.accepted == 1 || myChallenge.accepted == 2)
            {
              Debug.Log("Opponent has accepted my challenge!");
              Debug.Log("Opening play planel");
              PlayerManager.Instance.role = "challenger";
              // Hide friends panel
              GetComponent<FriendsPanel>().hidePanel();
              // Turn friend entry back to the idle state
              GetComponent<FriendsPanel>().turnToIdle(PlayerManager.Instance.opponentID);
              // Show play panel with opponent name
              playMenu.GetComponent<PlayMenu>().show();
              CancelInvoke();
            }
            else
            {
              Debug.Log("Opponent has not yet accepted the challenge");
            }
          }
        }
      }
    }

    public void acceptChallenge(int id)
    {
      StartCoroutine(acceptChallengeInServer(id));
    }

    IEnumerator acceptChallengeInServer(int id)
    {
      // Get challenges object from target player
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges/" + id;
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
          if (serverJson.Trim() == "{}")
          {
            Debug.Log("Challenge from user with ID " + id + " was not found in your challenge list.");
          }
          else
          {
            Challenge opponentChallenge = JsonUtility.FromJson<Challenge>(serverJson);
            opponentChallenge.accepted = 1;
            Debug.Log("Challenge was accepted.");
            // POST updated challenges to server
            url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges/" + id;
            string updatedChallenge = JsonUtility.ToJson(opponentChallenge);
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
              Debug.Log("Challenges updated in server.");
              // Hide friends panel
              GetComponent<FriendsPanel>().hidePanel();
              // Show play panel with opponent name
              PlayerManager.Instance.role = "guest";
              playMenu.GetComponent<PlayMenu>().show();
            }
            // Dispose of the request to prevent memory leaks
            postRequest.Dispose();
          }
        }
      }
    }

    public void cancelChallenge(int id)
    {
      StartCoroutine(cancelChallengeInServer(id));
    }

    IEnumerator cancelChallengeInServer(int id)
    {
      // Send a DELETE http request to the API for the given challenge
      string url = PlayerManager.Instance.apiUrl + "users/" + id + "/challenges/" + PlayerManager.Instance.myID;
      UnityWebRequest deleteRequest = new UnityWebRequest(url);
      deleteRequest.method = UnityWebRequest.kHttpVerbDELETE;
      yield return deleteRequest.SendWebRequest();
      if (deleteRequest.result == UnityWebRequest.Result.ConnectionError || deleteRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(deleteRequest.error);
      }
      else
      {
        Debug.Log("Challenge successfully deleted");
      }
      // Stop searching for acceptance?
    }
}
