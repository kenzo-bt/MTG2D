using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public GameObject creatureArea;
    public GameObject landArea;
    public GameObject otherArea;
    public List<string> creatures;
    public List<string> lands;
    public List<string> others;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeBattlefield()
    {
      creatures = new List<string>();
      lands = new List<string>();
      others = new List<string>();
    }

    public void addCard(CardInfo card)
    {
      GameObject targetArea = null;
      List<string> targetList = null;
      // Determine correct area
      if (card.types.Contains("Land"))
      {
        targetArea = landArea;
        targetList = lands;
      }
      else if (card.types.Contains("Creature"))
      {
        targetArea = creatureArea;
        targetList = creatures;
      }
      else {
        targetArea = otherArea;
        targetList = others;
      }
      // Update data
      targetList.Add(card.id);
      // Update physical cards
      targetArea.GetComponent<BattlefieldArea>().addCard(card);
    }

    public void removeCard(int index, string area)
    {
      if (area == "Creatures")
      {
        creatures.RemoveAt(index);
        creatureArea.GetComponent<BattlefieldArea>().removeCard(index);
      }
      else if (area == "Lands")
      {
        lands.RemoveAt(index);
        landArea.GetComponent<BattlefieldArea>().removeCard(index);
      }
      else if (area == "Others")
      {
        others.RemoveAt(index);
        otherArea.GetComponent<BattlefieldArea>().removeCard(index);
      }
    }

    public List<string> getCreatures()
    {
      List<string> allCreatures = new List<string>();
      int numChildren = creatureArea.transform.childCount;
      for (int i = 0; i < numChildren; i++)
      {
        BattlefieldCard card = creatureArea.transform.GetChild(i).gameObject.GetComponent<BattlefieldCard>();
        string cardId = creatures[i];
        if (card.tapped)
        {
          cardId += "-T";
        }
        allCreatures.Add(cardId);
      }
      return allCreatures;
    }

    public List<string> getLands()
    {
      List<string> allLands = new List<string>();
      int numChildren = landArea.transform.childCount;
      for (int i = 0; i < numChildren; i++)
      {
        BattlefieldCard card = landArea.transform.GetChild(i).gameObject.GetComponent<BattlefieldCard>();
        string cardId = lands[i];
        if (card.tapped)
        {
          cardId += "-T";
        }
        allLands.Add(cardId);
      }
      return allLands;
    }

    public List<string> getOthers()
    {
      List<string> allOthers = new List<string>();
      int numChildren = otherArea.transform.childCount;
      for (int i = 0; i < numChildren; i++)
      {
        BattlefieldCard card = otherArea.transform.GetChild(i).gameObject.GetComponent<BattlefieldCard>();
        string cardId = others[i];
        if (card.tapped)
        {
          cardId += "-T";
        }
        allOthers.Add(cardId);
      }
      return allOthers;
    }
}
