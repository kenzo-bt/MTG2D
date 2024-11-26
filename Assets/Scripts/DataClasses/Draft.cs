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
  public string set1;
  public string set2;
  public string set3;
  public string setName;
  public List<int> players;
  public int started;
  public List<DraftPacks> cpu;
}
