using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeChallenge
{
  public int id;
  public string hostName;
  public string set;
  public List<TimeChallengePlayer> players;
  public bool started;
  public int objectiveId;
}
