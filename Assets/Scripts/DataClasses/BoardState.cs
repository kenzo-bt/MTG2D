using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardState
{
  public List<string> hand;
  public List<string> deck;
  public List<string> grave;
  public List<string> exile;
  public List<string> creatures;
  public List<string> lands;
  public List<string> others;
  public string hash;

  public void debugState()
  {
    foreach (string cardID in hand)
    {
      Debug.Log("OppCard: " + cardID);
    }
  }
}
