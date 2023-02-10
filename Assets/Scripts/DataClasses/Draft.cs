using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Draft
{
  public int id;
  public int hostId;
  public int capacity;
  public string set;
  public List<int> players;
}
