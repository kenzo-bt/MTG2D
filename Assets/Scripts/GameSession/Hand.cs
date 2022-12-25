using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<CardInfo> hand;
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
    }

    public void addCard(CardInfo card)
    {
      // Update data
      hand.Add(card);
      // Update physical cards
      GameObject cardInstance = Instantiate(cardPrefab, transform);
      cardInstance.GetComponent<WebCard>().texturizeCard(card);
      cardInstance.GetComponent<DragDrop>().player = player;
    }

    public void removeCard(CardInfo card)
    {
      // Update data
      hand.Remove(card);
      // Update physical cards
      int numChildren = transform.childCount;
      for(int i = 0; i < numChildren; i++)
      {
        if (transform.GetChild(i).gameObject.GetComponent<WebCard>().cardName == card.name)
        {
          DestroyImmediate(transform.GetChild(i).gameObject);
          break;
        }
      }
    }

    // Get number of cards in hand
    public int getNumberOfCards()
    {
      return hand.Count;
    }

    // Position cards in hand
    public void orderHand()
    {
      float cardWidth = cardPrefab.GetComponent<RectTransform>().sizeDelta.x;
      float handWidth = cardWidth * hand.Count;
      float maxHandWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
      if (handWidth >= maxHandWidth)
      {
        handWidth = maxHandWidth;
      }
      float individualSpace = handWidth / hand.Count;
      float positionX = 0 - (handWidth / 2) + (individualSpace / 2);

      foreach (Transform child in transform)
      {
        child.localPosition = new Vector3(positionX, child.localPosition.y, child.localPosition.z);
        positionX += individualSpace;
      }
    }

    // Empty hand
    public void emptyHand()
    {
      // Update data
      hand = new List<CardInfo>();
      // Update physical cards
      int numChildren = transform.childCount;
      for (int i = 0; i < numChildren; i++)
      {
        DestroyImmediate(transform.GetChild(0).gameObject);
      }
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
