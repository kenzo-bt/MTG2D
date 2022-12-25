using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject deckObject;
    public GameObject handObject;
    private Hand hand;
    private Hasher hasher;

    // Start is called before the first frame update
    void Awake()
    {
      hasher = GetComponent<Hasher>();
      hand = handObject.GetComponent<Hand>();
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

    // Produce board state as JSON string
    public string getBoardState()
    {
      List<string> handIds = new List<string>();
      foreach (CardInfo card in hand.hand)
      {
        handIds.Add(card.id);
      }

      BoardState myState = new BoardState();
      myState.hand = handIds;
      myState.hash = ""; // Standardize to avoid garbage values before hashing
      myState.hash = hasher.getHash(JsonUtility.ToJson(myState));

      return JsonUtility.ToJson(myState);
    }
}
