using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject deckObject;
    public GameObject handObject;
    public GameObject battlefieldObject;
    public GameObject gameStateObject;
    private Hand hand;
    private Deck deck;
    private Battlefield battlefield;
    private GameState gameState;
    private Hasher hasher;
    private int mulligans;
    public int initialHandSize;

    // Start is called before the first frame update
    void Start()
    {
      hasher = GetComponent<Hasher>();
      hand = handObject.GetComponent<Hand>();
      deck = deckObject.GetComponent<Deck>();
      battlefield = battlefieldObject.GetComponent<Battlefield>();
      gameState = gameStateObject.GetComponent<GameState>();
      mulligans = 0;
      initialHandSize = 7;
      // Initialize your deck and hand
      deck.initializeDeck();
      hand.initializeHand();
      battlefield.initializeBattlefield();
      // Draw 7 cards (Involves hand and deck -> Player handles this)
      drawCards(initialHandSize);
      // Initialize game state
      gameState.initializeState();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Draw N number of cards from deck and put them in hand
    public void drawCards(int num)
    {
      for (int i = 0; i < num; i++)
      {
        CardInfo card = deck.removeCard();
        if (card != null)
        {
          // Place card in hand
          hand.addCard(card);
        }
      }
      // Order hand
      hand.orderHand();
    }

    // Draw a card from deck and place in hand
    public void drawCard()
    {
      CardInfo card = deck.removeCard();
      if (card != null)
      {
        hand.addCard(card);
        // Order hand
        hand.orderHand();
      }
    }

    // Mulligan
    public void mulliganHand()
    {
      if (hand.getNumberOfCards() > 0)
      {
        mulligans += 1;
        // Empty hand
        hand.emptyHand();
        // Reset deck
        deck.initializeDeck();
        // Perform a mulligan
        drawCards(initialHandSize - mulligans);
      }
    }

    // Produce board state as JSON string
    public BoardState getBoardState()
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

      return myState;
    }

    // Restart Game
    public void restartGame()
    {
      hand.emptyHand();
      deck.initializeDeck();
      drawCards(initialHandSize);
    }

    // Move card to the battlefield
    public void dropCardInBattlefield(CardInfo card)
    {
      // Remove card from hand
      hand.removeCard(card);
      hand.orderHand();
      // Add card to battlefield
      Debug.Log("Adding to the battlefield...");
      battlefield.addCard(card);
    }

    // Move card from source to destination
    public void changeCardLocation(string cardId, string sourceArea, string destination)
    {
      // Remove card from current area
      battlefield.removeCard(cardId, sourceArea);
      // Place card in given destination
      if (destination == "hand")
      {
        Debug.Log("Sending this card to hand...");
      }
      else if (destination == "grave")
      {
        Debug.Log("Sending this card to the graveyard...");
      }
      else if (destination == "exile")
      {
        Debug.Log("Sending this card to exile...");
      }
      else if (destination == "deck")
      {
        Debug.Log("Sending this card to deck...");
      }
    }
}
