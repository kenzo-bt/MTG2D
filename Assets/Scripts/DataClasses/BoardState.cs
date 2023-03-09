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
  public int life;
  public string hash;
  public int coinToss;
  public string tossTime;
  public bool coinVisible;

  public void debugState()
  {
    Debug.Log(JsonUtility.ToJson(this));
  }
}
