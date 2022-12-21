using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardState
{
  public List<string> hand;

  public void debugState()
  {
    foreach (string cardID in hand)
    {
      Debug.Log("OppCard: " + cardID);
    }
  }
}
