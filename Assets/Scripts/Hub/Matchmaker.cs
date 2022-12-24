using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Matchmaker : MonoBehaviour
{
    private bool waiting;
    private bool opponentAccepted;
    public GameObject playMenu;
    // Once the user has logged in, we can ping the server every X seconds to check if we have open challenges (Idle)
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
              friend.turnToIdle();
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
      // Get challenges object from target player
      string url = PlayerManager.Instance.apiUrl + "users/" + targetID + "/challenges";
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
          AllChallenges targetChallenges = JsonUtility.FromJson<AllChallenges>(serverJson);
          // Debug.Log("User with ID->" + targetID + " has " + targetChallenges.challenges.Count + " active challenges");
          // Add new challenge if not already in existence
          bool challengeExists = false;
          foreach (Challenge ch in targetChallenges.challenges)
          {
            //Debug.Log("---> ID: " + ch.challengerID + " -> " + (ch.accepted ? " accepted" : " pending"));
            if (ch.challengerID == PlayerManager.Instance.myID){
              challengeExists = true;
              break;
            }
          }
          if (!challengeExists)
          {
            Challenge newChallenge = new Challenge();
            newChallenge.challengerID = PlayerManager.Instance.myID;
            newChallenge.accepted = 0;
            targetChallenges.challenges.Add(newChallenge);
            // POST to server
            url = PlayerManager.Instance.apiUrl + "users/" + targetID + "/challenges";
            string updatedChallenges = JsonUtility.ToJson(targetChallenges);
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
      opponentAccepted = false;
      waiting = true;
      while (waiting)
      {
        InvokeRepeating("checkIfOpponentAccepted", 5.0f, 5.0f);
      }
      // What to do once the opponent has accepted/rejected my challenge
      if (opponentAccepted)
      {
        Debug.Log("Opponent has accepted my challenge!");
        Debug.Log("Opening play planel");
        // Show play panel with opponent name and enable play button?
        playMenu.GetComponent<PlayMenu>().show();
      }
      else {
        Debug.Log("Opponent has actively refused your challenge");
      }
    }

    IEnumerator checkIfOpponentAccepted()
    {
      // Get opponent challenges
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.opponentID + "/challenges";
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
          AllChallenges oppChallenges = JsonUtility.FromJson<AllChallenges>(serverJson);
          // Check if my challenge has been accepted
          foreach (Challenge ch in oppChallenges.challenges)
          {
            if (ch.challengerID == PlayerManager.Instance.myID)
            {
              if (ch.accepted == 1)
              {
                waiting = false;
                opponentAccepted = true;
              }
              else if (ch.accepted == -1)
              {
                waiting = false;
                opponentAccepted = false;
              }
              else
              {
                Debug.Log("Opponent has not yet accepted the challenge");
              }
              break;
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
          bool acceptedChallenge = false;
          foreach (Challenge ch in myChallenges.challenges)
          {
            if (ch.challengerID == id)
            {
              ch.accepted = 1;
              acceptedChallenge = true;
              break;
            }
          }
          if (acceptedChallenge)
          {
            Debug.Log("Challenge was accepted.");
            // POST updated challenges to server
            url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/challenges";
            string updatedChallenges = JsonUtility.ToJson(myChallenges);
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
              Debug.Log("Challenges updated in server.");
              // Show play panel with opponent name and enable play button?
              playMenu.GetComponent<PlayMenu>().show();
            }
            // Dispose of the request to prevent memory leaks
            postRequest.Dispose();
          }
          else
          {
            Debug.Log("Tried to accept challenge, but it was not found.");
          }
        }
      }
    }
}
