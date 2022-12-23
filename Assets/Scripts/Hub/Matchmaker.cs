using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Matchmaker : MonoBehaviour
{
    // Once the user has logged in, we can ping the server every X seconds to check if we have open challenges (Idle)
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
            newChallenge.accepted = false;
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
}
