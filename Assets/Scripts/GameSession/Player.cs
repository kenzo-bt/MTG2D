using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public GameObject deckObject;
    public GameObject handObject;
    public GameObject battlefieldObject;
    public GameObject graveObject;
    public GameObject exileObject;
    public GameObject highlightObject;
    public GameObject lifeObject;
    public GameObject gameStateObject;
    public GameObject opponentObject;
    private Hand hand;
    private Deck deck;
    private Battlefield battlefield;
    private CardStack grave;
    private CardStack exile;
    private CardStack deckStack;
    private WebCard highlightCard;
    private TMP_Text lifeCounter;
    private GameState gameState;
    private Opponent opponent;
    private Hasher hasher;
    private int mulligans;
    public int initialHandSize;
    private int lifeTotal;

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
      highlightCard = highlightObject.GetComponent<WebCard>();
      lifeCounter = lifeObject.GetComponent<TMP_Text>();
      opponent = opponentObject.GetComponent<Opponent>();
      gameState = gameStateObject.GetComponent<GameState>();
      mulligans = 0;
      initialHandSize = 7;
      lifeTotal = 20;
      // Initialize your life counter in the UI
      updateLifeTotal();
      // Initialize your deck and hand
      deck.initializeDeck();
      hand.initializeHand();
      battlefield.initializeBattlefield();
      grave.initializeStack();
      exile.initializeStack();
      opponent.initializeOpponent();
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
      BoardState myState = new BoardState();
      myState.hand = hand.getHandIds();
      myState.deck = deckStack.getStackIds();
      myState.grave = grave.getStackIds();
      myState.exile = exile.getStackIds();
      myState.creatures =  battlefield.getCreatures();
      myState.lands =  battlefield.getLands();
      myState.others =  battlefield.getOthers();
      myState.life = lifeTotal;
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
      battlefield.addCard(card);
    }

    // Move battlefield card from source to destination
    public void moveBattlefieldCard(string cardId, int index, string sourceArea, string destination)
    {
      // Remove card from current area
      battlefield.removeCard(index, sourceArea);
      // Place card in given destination
      if (destination == "hand")
      {
        CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(cardId);
        hand.addCard(targetCard);
        hand.orderHand();
      }
      else if (destination == "grave")
      {
        grave.addCard(cardId, true);
      }
      else if (destination == "exile")
      {
        exile.addCard(cardId, true);
      }
      else if (destination == "deck")
      {
        // Currently putting on top of deck -> Need option for top/bottom/shuffled
        deckStack.addCard(cardId, true);
      }
      // Hide card highlight (OnPointerExit will not trigger when the card disappears from field)
      hideHighlightCard();
    }

    public void moveStackCard(string cardId, int index, string source, string destination)
    {
      // Remove card from current stack
      if (source == "Deck")
      {
        deckStack.removeCard(index);
      }
      else if (source == "Grave")
      {
        grave.removeCard(index);
      }
      else if (source == "Exile")
      {
        exile.removeCard(index);
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
        deckStack.addCard(cardId, true);
      }
      else if (destination == "Grave")
      {
        grave.addCard(cardId, true);
      }
      else if (destination == "Exile")
      {
        exile.addCard(cardId, true);
      }
    }

    // Show highlight card
    public void showHighlightCard(string cardId)
    {
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      highlightCard.texturizeCard(card);
      highlightObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    // Hide highlight card
    public void hideHighlightCard()
    {
      highlightObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    // Reduce life total by 1
    public void reduceLife()
    {
      lifeTotal = lifeTotal - 1;
      updateLifeTotal();
    }

    // Increase life total by 1
    public void increaseLife()
    {
      lifeTotal = lifeTotal + 1;
      updateLifeTotal();
    }

    // Update total life in the UI
    public void updateLifeTotal()
    {
      lifeCounter.text = lifeTotal.ToString();
    }

    // show a card in hand to opponent
    public void showHandCard(int index)
    {
      hand.cardVisibility[index] = true;
    }

    // show a card in hand to opponent
    public void hideHandCard(int index)
    {
      hand.cardVisibility[index] = false;
    }
}
