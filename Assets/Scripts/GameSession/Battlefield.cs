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

    public void removeCard(string cardId, string area)
    {
      if (area == "Creatures")
      {
        creatures.Remove(cardId);
        creatureArea.GetComponent<BattlefieldArea>().removeCard(cardId);
      }
      else if (area == "Lands")
      {
        lands.Remove(cardId);
        landArea.GetComponent<BattlefieldArea>().removeCard(cardId);
      }
      else if (area == "Others")
      {
        others.Remove(cardId);
        otherArea.GetComponent<BattlefieldArea>().removeCard(cardId);
      }
    }
}
