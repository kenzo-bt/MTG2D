using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Challenge
{
  public int challengerID;
  public int accepted; // -1 Declined 0 Pending 1 Accepted 2 Ready
}
