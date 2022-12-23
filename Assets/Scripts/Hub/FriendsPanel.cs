using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FriendsPanel : MonoBehaviour
{
  public List<int> friendIDs;
  public List<string> friendNames;
  public GameObject friendPrefab;
  // Start is called before the first frame update
  void Start()
  {
      friendIDs = new List<int>();
      friendNames = new List<string>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void loadFriends()
  {
    StartCoroutine(getFriendsFromServer());
  }

  public IEnumerator getFriendsFromServer()
  {
    FriendList allFriends = new FriendList();
    AllUsers allUsers = new AllUsers();
    // Get ids
    string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/friends";
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
        allFriends = JsonUtility.FromJson<FriendList>(serverJson);
      }
    }
    // Get names
    url = PlayerManager.Instance.apiUrl + "users";
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
        allUsers = JsonUtility.FromJson<AllUsers>(serverJson);
      }
    }
    // Generate local data
    for (int i = 0; i < allFriends.friends.Count; i++)
    {
      int ID = allFriends.friends[i];
      friendIDs.Add(ID);
      friendNames.Add(allUsers.users[ID - 1].username);
      Debug.Log("Adding friend -> ID " + ID + " : " + allUsers.users[ID - 1].username);
    }
  }
}
