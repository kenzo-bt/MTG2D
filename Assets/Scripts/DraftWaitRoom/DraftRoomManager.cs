using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
      // Set the room name
      // Populate the player list
      // Invoke the status checker
    }

    public void returnToHub()
    {
      // Remove yourself from draft in server and go back to hub
    }
}
