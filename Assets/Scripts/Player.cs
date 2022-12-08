using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject deckObject;
    public GameObject handObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void mulliganHand()
    {
      Hand hand = handObject.GetComponent<Hand>();
      if (hand.getNumberOfCards() > 0)
      {
        // Reset deck
        Deck deck = deckObject.GetComponent<Deck>();
        deck.resetDeck();
        // Perform a mulligan
        hand.mulligan();
      }
    }
}
