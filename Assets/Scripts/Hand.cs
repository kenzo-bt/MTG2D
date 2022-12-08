using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private List<string> hand = new List<string>();
    private int handSize = 7;
    public GameObject deckObject;
    public GameObject cardPrefab;
    private Deck deck;
    public bool isInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
      deck = deckObject.GetComponent<Deck>();
    }

    // Update is called once per frame
    void Update()
    {
      if (!isInitialized && deck.isInitialized)
      {
        generateStartingHand();
        showHand();
        isInitialized = true;
      }
    }

    // Draw 7 cards from deck
    private void generateStartingHand()
    {
      for (int i = 0; i < handSize; i++)
      {
        hand.Add(deck.drawCard());
      }
    }

    // Show hand (Generate the UI card images)
    private void showHand()
    {
      foreach (string card in hand)
      {
        GameObject cardInstance = Instantiate(cardPrefab, transform);
        Card cardScript = cardInstance.GetComponent<Card>();
        cardScript.texturizeCard(card);
      }

      orderHand();
    }

    // Position cards in hand
    private void orderHand()
    {
      float handWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
      float individualSpace = handWidth / hand.Count;
      float positionX = 0 - (handWidth / 2) + (individualSpace / 2);

      foreach (Transform child in transform)
      {
        //Debug.Log(positionX);
        child.localPosition = new Vector3(positionX, child.localPosition.y, child.localPosition.z);
        positionX += individualSpace;
      }

      //Debug.Log("Height: " + GetComponent<Rect>().height);
    }

    // Empty hand
    private void emptyHand()
    {
      hand = new List<string>();
      int numChildren = transform.childCount;

      for (int i = 0; i < numChildren; i++)
      {
        DestroyImmediate(transform.GetChild(0).gameObject);
      }
    }

    // Reset hand
    public void resetHand()
    {
      emptyHand();
      generateStartingHand();
      showHand();
    }

    // Debug hand info
    private void debugPrintHand()
    {
      foreach (var card in hand)
      {
        Debug.Log(card);
      }

      Debug.Log("Cards in hand: " + hand.Count);
    }
}
