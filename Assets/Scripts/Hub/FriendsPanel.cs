using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendsPanel : MonoBehaviour
{
  public GameObject friendPrefab;
  public GameObject friendList;
  public GameObject addOverlayObject;
  public GameObject panelObject;
  public GameObject expanderObject;
  private AddFriend addFriendOverlay;
  private float speed;
  private float showPanelY;
  private float hidePanelY;
  private Vector3 targetPosition;

  // Start is called before the first frame update
  void Start()
  {
      addFriendOverlay = addOverlayObject.GetComponent<AddFriend>();
      // Panel Movement
      speed = 500f;
      showPanelY = panelObject.transform.localPosition.y;
      hidePanelY = panelObject.transform.localPosition.y - panelObject.GetComponent<RectTransform>().sizeDelta.y;
      panelObject.transform.localPosition = new Vector3(0f, hidePanelY, 0f);
      targetPosition = panelObject.transform.localPosition;
  }

  // Update is called once per frame
  void Update()
  {
    // Move panel if show/hide
    var step =  speed * Time.deltaTime;
    panelObject.transform.localPosition = Vector3.MoveTowards(panelObject.transform.localPosition, targetPosition, step);
  }

  public void showPanel()
  {
    targetPosition = new Vector3(0f, showPanelY, 0f);
    expanderObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
  }

  public void hidePanel()
  {
    targetPosition = new Vector3(0f, hidePanelY, 0f);
    expanderObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
  }

  public void loadFriends()
  {
    StartCoroutine(getFriendsFromServer());
  }

  public void loadFriendsLocal()
  {
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
    // Generate list locally
    for (int i = 0; i < PlayerManager.Instance.friendIDs.Count; i++)
    {
      GameObject friendInstance = Instantiate(friendPrefab, friendList.transform);
      friendInstance.GetComponent<FriendEntry>().setData(PlayerManager.Instance.friendNames[i], PlayerManager.Instance.friendIDs[i]);
      friendInstance.GetComponent<FriendEntry>().setPanelObject(gameObject);
    }
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
    PlayerManager.Instance.friendIDs = new List<int>();
    PlayerManager.Instance.friendNames = new List<string>();
    for (int i = 0; i < allFriends.friends.Count; i++)
    {
      int ID = allFriends.friends[i];
      PlayerManager.Instance.friendIDs.Add(ID);
      PlayerManager.Instance.friendNames.Add(allUsers.users[ID - 1].username);
      // Debug.Log("Adding friend -> ID " + ID + " : " + allUsers.users[ID - 1].username);
      GameObject friendInstance = Instantiate(friendPrefab, friendList.transform);
      friendInstance.GetComponent<FriendEntry>().setData(allUsers.users[ID - 1].username, ID);
      friendInstance.GetComponent<FriendEntry>().setPanelObject(gameObject);
      // Check if challenge exists to turn them into sent entries
      url = PlayerManager.Instance.apiUrl + "users/" + ID + "/challenges/" + PlayerManager.Instance.myID;
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
          if (serverJson.Trim() != "{}")
          {
            Debug.Log("Turning to sent...");
            friendInstance.GetComponent<FriendEntry>().turnToSent();
          }
        }
      }
    }
    // Hide the overlay
    addFriendOverlay.hide();
  }

  // Add a friend
  public void addFriend(string friendName)
  {
    // Check if already a friend
    if (PlayerManager.Instance.friendNames.Contains(friendName))
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
            targetFriendID = user.id;
            break;
          }
        }
        if (userFound)
        {
          Debug.Log("Player found in server! Proceeding to add him to your friend list...");
          PlayerManager.Instance.friendIDs.Add(targetFriendID);
          PlayerManager.Instance.friendNames.Add(friendName);
          StartCoroutine(postFriendsToServer());
        }
        else
        {
          addFriendOverlay.setAlert("Player not found!");
        }
      }
    }
  }

  // Update your friends list in the server
  public IEnumerator postFriendsToServer()
  {
    // POST to server
    string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/friends";
    FriendList newFriendList = new FriendList();
    newFriendList.friends = new List<int>(PlayerManager.Instance.friendIDs);
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

  // Remove friend
  public void removeFriend(int removeID)
  {
    // Remove locally
    for (int i = 0; i < PlayerManager.Instance.friendIDs.Count; i++)
    {
      if (PlayerManager.Instance.friendIDs[i] == removeID)
      {
        PlayerManager.Instance.friendIDs.RemoveAt(i);
        PlayerManager.Instance.friendNames.RemoveAt(i);
        break;
      }
    }
    // Remove remotely
    StartCoroutine(postFriendsToServer());
  }

  // Turn a specific friend into the idle state
  public void turnToIdle(int id)
  {
    foreach (Transform child in friendList.transform)
    {
      if (child.gameObject.name != "AddFriend")
      {
        FriendEntry friend = child.gameObject.GetComponent<FriendEntry>();
        if (friend.friendID == id)
        {
          friend.turnToIdle();
          break;
        }
      }
    }
  }
}
