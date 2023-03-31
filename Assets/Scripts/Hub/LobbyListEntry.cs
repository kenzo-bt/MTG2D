using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyListEntry : MonoBehaviour
{
  public GameObject hostObject;
  public GameObject capacityObject;
  public bool selected;
  public int hostID;
  // Start is called before the first frame update
  void Awake()
  {
    selected = false;
    hostID = -1;
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void setInfo(int hostId, string hostName, string capacity)
  {
    hostID = hostId;
    hostObject.GetComponent<TMP_Text>().text = hostName;
    capacityObject.GetComponent<TMP_Text>().text = capacity;
    deselectEntry();
  }

  public void selectEntry()
  {
    // Deselect all entries first
    int numEntries = transform.parent.childCount;
    for (int i = 0; i < numEntries; i++)
    {
      transform.parent.GetChild(i).gameObject.GetComponent<LobbyListEntry>().deselectEntry();
    }
    // Select this entry
    selected = true;
    GetComponent<Image>().color = new Color(0f, 0.92f, 1f, 0.5f);
  }

  public void deselectEntry()
  {
    selected = false;
    GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
  }

  public void toggleSelect()
  {
    if (selected)
    {
      deselectEntry();
    }
    else
    {
      selectEntry();
    }
    // Update selected draft in panel
    GameObject.Find("FreeForAllPanel").GetComponent<FreeForAllPanel>().updateSelection();
  }
}
