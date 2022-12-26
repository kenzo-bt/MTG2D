using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject deckObject;
    public GameObject handObject;
    public GameObject battlefieldObject;
    public GameObject gameStateObject;
    public GameObject graveObject;
    public GameObject exileObject;
    private Hand hand;
    private Deck deck;
    private Battlefield battlefield;
    private CardStack grave;
    private CardStack exile;
    private CardStack deckStack;
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
      grave = graveObject.GetComponent<CardStack>();
      exile = exileObject.GetComponent<CardStack>();
      deckStack = deckObject.GetComponent<CardStack>();
      gameState = gameStateObject.GetComponent<GameState>();
      mulligans = 0;
      initialHandSize = 7;
      // Initialize your deck and hand
      deck.initializeDeck();
      hand.initializeHand();
      battlefield.initializeBattlefield();
      grave.initializeStack();
      exile.initializeStack();
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
        CardInfo card = deck.drawCard();
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
      CardInfo card = deck.drawCard();
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

    // Move battlefield card from source to destination
    public void moveBattlefieldCard(string cardId, string sourceArea, string destination)
    {
      // Remove card from current area
      battlefield.removeCard(cardId, sourceArea);
      // Place card in given destination
      if (destination == "hand")
      {
        Debug.Log("Sending this card to hand...");
        CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(cardId);
        hand.addCard(targetCard);
      }
      else if (destination == "grave")
      {
        Debug.Log("Sending this card to the graveyard...");
        grave.addCard(cardId);
      }
      else if (destination == "exile")
      {
        Debug.Log("Sending this card to exile...");
        exile.addCard(cardId);
      }
      else if (destination == "deck")
      {
        // Currently putting on top of deck -> Need option for top/bottom/shuffled
        Debug.Log("Sending this card to deck...");
        deckStack.addCard(cardId);
      }
    }

    public void moveStackCard(string cardId, string source, string destination)
    {
      // Remove card from current stack
      if (source == "Deck")
      {
        deckStack.cards.Remove(cardId);
      }
      else if (source == "Grave")
      {
        grave.cards.Remove(cardId);
      }
      else if (source == "Exile")
      {
        exile.cards.Remove(cardId);
      }
      // Put card in destination area
      if (destination == "Battlefield")
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
        battlefield.addCard(card);
      }
      else if (destination == "Hand")
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
        hand.addCard(card);
        hand.orderHand();
      }
      else if (destination == "Deck")
      {
        // Currently putting on top of deck -> Need option for top/bottom/shuffled
        deckStack.cards.Add(cardId);
      }
      else if (destination == "Grave")
      {
        grave.cards.Add(cardId);
      }
      else if (destination == "Exile")
      {
        exile.cards.Add(cardId);
      }
    }
}
