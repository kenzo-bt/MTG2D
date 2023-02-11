using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DraftListEntry : MonoBehaviour
{
  public GameObject hostObject;
  public GameObject setObject;
  public GameObject capacityObject;
  // Start is called before the first frame update
  void Start()
  {

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
  }
}
