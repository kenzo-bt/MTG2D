using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lobby
{
  public int id;
  public int hostId;
  public string hostName;
  public List<int> players;
  public int started;
}
