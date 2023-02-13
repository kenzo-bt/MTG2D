using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Draft
{
  public int id;
  public int hostId;
  public string hostName;
  public int capacity;
  public string set;
  public string setName;
  public List<int> players;
  public int started;
}
