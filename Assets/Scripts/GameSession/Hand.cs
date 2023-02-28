using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<CardInfo> hand;
    public List<bool> cardVisibility;
    public GameObject cardPrefab;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeHand()
    {
      hand = new List<CardInfo>();
      cardVisibility = new List<bool>();
    }

    public void addCard(CardInfo card)
    {
      // Update data
      hand.Add(card);
      cardVisibility.Add(false);
      // Update physical cards
      GameObject cardInstance = Instantiate(cardPrefab, transform);
      cardInstance.GetComponent<WebCard>().texturizeCard(card);
      cardInstance.GetComponent<HandCard>().player = player;
      cardInstance.GetComponent<DragDrop>().player = player;
    }

    public void insertCard(int index, CardInfo card, bool visibility)
    {
      hand.Insert(index, card);
      cardVisibility.Insert(index, visibility);
      GameObject cardInstance = Instantiate(cardPrefab, transform);
      cardInstance.transform.SetSiblingIndex(index);
      cardInstance.GetComponent<WebCard>().texturizeCard(card);
      cardInstance.GetComponent<HandCard>().player = player;
      cardInstance.GetComponent<DragDrop>().player = player;
      if (visibility)
      {
        cardInstance.GetComponent<HandCard>().toggleShowCard();
      }
    }

    public void removeCard(int index)
    {
      hand.RemoveAt(index);
      cardVisibility.RemoveAt(index);
      DestroyImmediate(transform.GetChild(index).gameObject);
    }

    // Get number of cards in hand
    public int getNumberOfCards()
    {
      return hand.Count;
    }

    // Position cards in hand
    public void orderHand()
    {
      GetComponent<Container>().orderChildrenCenter();
    }

    // Empty hand
    public void emptyHand()
    {
      // Update data
      hand = new List<CardInfo>();
      cardVisibility = new List<bool>();
      // Update physical cards
      int numChildren = transform.childCount;
      for (int i = 0; i < numChildren; i++)
      {
        DestroyImmediate(transform.GetChild(0).gameObject);
      }
    }

    // Get ids of cards in hand
    public List<string> getHandIds()
    {
      List<string> handIds = new List<string>();
      int numCards = hand.Count;

      for (int i = 0; i < hand.Count; i++)
      {
        string cardID = hand[i].id;
        if (!cardVisibility[i])
        {
          cardID += "-H";
        }
        handIds.Add(cardID);
      }
      return handIds;
    }

    // Debug hand info
    public void debugPrintHand()
    {
      foreach (CardInfo card in hand)
      {
        Debug.Log(card.name);
      }
      Debug.Log("Cards in hand: " + hand.Count);
    }
}
