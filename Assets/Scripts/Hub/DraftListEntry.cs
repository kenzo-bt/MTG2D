using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftListEntry : MonoBehaviour
{
  public GameObject hostObject;
  public GameObject setObject;
  public GameObject capacityObject;
  public bool selected;
  // Start is called before the first frame update
  void Start()
  {
    selected = false;
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void setInfo(string hostName, string setName, string capacity)
  {
    hostObject.GetComponent<TMP_Text>().text = hostName;
    setObject.GetComponent<TMP_Text>().text = setName;
    capacityObject.GetComponent<TMP_Text>().text = capacity;
    deselectEntry();
  }

  public void selectEntry()
  {
    // Deselect all entries first
    int numEntries = transform.parent.childCount;
    for (int i = 0; i < numEntries; i++)
    {
      transform.parent.GetChild(i).gameObject.GetComponent<DraftListEntry>().deselectEntry();
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
    GameObject.Find("DraftPanel").GetComponent<DraftPanel>().updateSelection();
  }
}
