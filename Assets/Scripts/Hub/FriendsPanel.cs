using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FriendsPanel : MonoBehaviour
{
  public List<int> friendIDs;
  public List<string> friendNames;
  public GameObject friendPrefab;
  public GameObject friendList;
  public GameObject addOverlayObject;
  private AddFriend addFriendOverlay;

  // Start is called before the first frame update
  void Start()
  {
      friendIDs = new List<int>();
      friendNames = new List<string>();
      addFriendOverlay = addOverlayObject.GetComponent<AddFriend>();
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
    // Delete previous entries in list
    int numChildrenInList = friendList.transform.childCount;
    int deleteOffset = 0;
    for (int i = 0; i < numChildrenInList; i++)
    {
      if (friendList.transform.GetChild(0 + deleteOffset).gameObject.name != "AddFriend")
      {
        DestroyImmediate(friendList.transform.GetChild(0 + deleteOffset).gameObject);
      }
      else {
        deleteOffset += 1;
      }
    }
    // Generate local data and populate friend panel
    for (int i = 0; i < allFriends.friends.Count; i++)
    {
      int ID = allFriends.friends[i];
      friendIDs.Add(ID);
      friendNames.Add(allUsers.users[ID - 1].username);
      Debug.Log("Adding friend -> ID " + ID + " : " + allUsers.users[ID - 1].username);
      GameObject friendInstance = Instantiate(friendPrefab, friendList.transform);
      friendInstance.GetComponent<FriendEntry>().setData(allUsers.users[ID - 1].username, ID);
    }
    // Hide the overlay
    addFriendOverlay.hide();
  }

  // Add a friend
  public void addFriend(string friendName)
  {
    // Check if already a friend
    if (friendNames.Contains(friendName))
    {
      addFriendOverlay.setAlert("Player is already your friend!");
    }
    else
    {
      StartCoroutine(addFriendFromServer(friendName));
    }
  }

  public IEnumerator addFriendFromServer(string friendName)
  {
    // Check if User exists in the server
    string url = PlayerManager.Instance.apiUrl + "users";
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
        AllUsers allUsers = JsonUtility.FromJson<AllUsers>(serverJson);
        bool userFound = false;
        int targetFriendID = 0;
        foreach (User user in allUsers.users)
        {
          if (user.username == friendName)
          {
            userFound = true;
            targetFriendID = int.Parse(user.id);
            break;
          }
        }
        if (userFound)
        {
          Debug.Log("Player found in server! Proceeding to add him to your friend list...");
          friendIDs.Add(targetFriendID);
          friendNames.Add(friendName);
          StartCoroutine(postNewFriendToServer(friendName));
        }
        else
        {
          addFriendOverlay.setAlert("Player not found!");
        }
      }
    }
  }

  // Update your friends list in the server
  public IEnumerator postNewFriendToServer(string friendName)
  {
    // POST to server
    string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/friends";
    FriendList newFriendList = new FriendList();
    newFriendList.friends = new List<int>(friendIDs);
    string newFriends = JsonUtility.ToJson(newFriendList);
    byte[] bytes = Encoding.UTF8.GetBytes(newFriends);
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
      Debug.Log("Friends list sent successfully to server");
    }
    // Dispose of the request to prevent memory leaks
    request.Dispose();
    // Rebuild the friends panel
    loadFriends();
  }
}
