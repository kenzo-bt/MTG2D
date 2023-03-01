using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public GameObject deckObject;
    public GameObject handObject;
    public GameObject battlefieldObject;
    public GameObject graveObject;
    public GameObject exileObject;
    public GameObject highlightObject;
    public GameObject highlightBackObject;
    public GameObject lifeObject;
    public GameObject gameStateObject;
    public GameObject opponentObject;
    public GameObject browserObject;
    public GameObject loggerObject;
    public GameObject coinImage;
    public GameObject coinTime;
    public GameObject coinButton;
    private Hand hand;
    private Deck deck;
    private Battlefield battlefield;
    private CardStack grave;
    private CardStack exile;
    private CardStack deckStack;
    private WebCard highlightCard;
    private WebCard highlightBackCard;
    private TMP_Text lifeCounter;
    private GameState gameState;
    private Opponent opponent;
    private Hasher hasher;
    private int mulligans;
    public int initialHandSize;
    private int lifeTotal;
    public string log;
    public int coinToss;
    public string tossTime;

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
      highlightBackCard = highlightBackObject.GetComponent<WebCard>();
      lifeCounter = lifeObject.GetComponent<TMP_Text>();
      opponent = opponentObject.GetComponent<Opponent>();
      gameState = gameStateObject.GetComponent<GameState>();
      mulligans = 0;
      initialHandSize = 7;
      lifeTotal = 20;
      log = "";
      coinToss = 0;
      tossTime = "";
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
      logMessage("You drew a card (" + card.name + ")");
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
        logMessage("You performed a mulligan (Drew " + (initialHandSize - mulligans) + ")");
      }
    }

    // Shuffle deck
    public void shuffleDeck()
    {
      deck.shuffleDeck();
      logMessage("You shuffled your deck");
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
      myState.coinToss = coinToss;
      myState.tossTime = tossTime;
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

    // Remove card from hand at certain index
    public void removeCardFromHand(int index)
    {
      hand.removeCard(index);
      hand.orderHand();
    }

    public void insertCardInHand(int index, CardInfo card, bool visibility)
    {
      hand.insertCard(index, card, visibility);
      hand.orderHand();
    }

    // Move card to the battlefield
    public void dropCardInBattlefield(CardInfo card)
    {
      battlefield.addCard(card);
      logMessage("You dropped " + card.name + " to the battlefield");
    }

    // Create a card in the battlefield (token)
    public void addCardToBattlefield(CardInfo card)
    {
      battlefield.addCard(card);
      logMessage("You created a " + card.name + " token in the battlefield");
    }

    // Move battlefield card from source to destination
    public void moveBattlefieldCard(string cardId, int index, string sourceArea, string destination)
    {
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(cardId);
      // Remove card from current area
      battlefield.removeCard(index, sourceArea);
      // Place card in given destination
      if (!targetCard.isToken)
      {
        if (destination == "hand")
        {
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
        else if (destination == "deckBtm")
        {
          deckStack.addCardBottom(cardId, true);
        }
      }
      // Hide card highlight (OnPointerExit will not trigger when the card disappears from field)
      hideHighlightCard();
      logMessage("You moved " + targetCard.name + " from " + sourceArea + "to " + destination);
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
      else if (destination == "DeckBtm")
      {
        deckStack.addCardBottom(cardId, true);
      }
      else if (destination == "Grave")
      {
        grave.addCard(cardId, true);
      }
      else if (destination == "Exile")
      {
        exile.addCard(cardId, true);
      }
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(cardId);
      logMessage("You moved " + targetCard.name + " from " + source + "to " + destination);
    }

    // Show highlight card
    public void showHighlightCard(string cardId)
    {
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      highlightCard.texturizeCard(card);
      highlightObject.GetComponent<CanvasGroup>().alpha = 1f;
      if (card.layout != "split" && card.backId != "" & card.backId != null)
      {
        CardInfo backCard = PlayerManager.Instance.getCardFromLookup(card.backId);
        highlightBackCard.texturizeCard(backCard);
        highlightBackObject.GetComponent<CanvasGroup>().alpha = 1f;
        if (card.layout == "flip")
        {
          highlightBackCard.cardImageObject.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
          highlightBackCard.cardImageObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
      }
      else
      {
        highlightBackObject.GetComponent<CanvasGroup>().alpha = 0f;
      }
    }

    // Hide highlight card
    public void hideHighlightCard()
    {
      highlightObject.GetComponent<CanvasGroup>().alpha = 0f;
      highlightBackObject.GetComponent<CanvasGroup>().alpha = 0f;
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
      logMessage("Life total changed to " + lifeTotal.ToString());
    }

    // show a card in hand to opponent
    public void showHandCard(int index)
    {
      hand.cardVisibility[index] = true;
      logMessage("Showed " + hand.hand[index].name + " in hand to opponent");
    }

    // show a card in hand to opponent
    public void hideHandCard(int index)
    {
      hand.cardVisibility[index] = false;
      logMessage("Hid " + hand.hand[index].name + " in hand from opponent");
    }

    // Mill X cards
    public void mill(int numCards)
    {
      // Remove top X cards on deck and add them to the graveyard stack
      for (int i = 0; i < numCards; i++)
      {
        if (deckStack.cards.Count < 1)
        {
          return;
        }
        // Get it
        string cardId = deckStack.cards[deckStack.cards.Count - 1];
        // Remove it
        deckStack.removeCard(deckStack.cards.Count - 1);
        // Place it in grave
        grave.addCard(cardId, true);
      }
      logMessage("You milled " + numCards + " cards");
    }

    // Look at top X cards of library
    public void look(int numCards)
    {
      // Make top X cards in deck stack visible
      for (int i = 0; i < numCards; i++)
      {
        if (deckStack.cards.Count < 1)
        {
          return;
        }
        deckStack.cardsVisibility[deckStack.cardsVisibility.Count - (1 + i)] = true;
      }
      // Open the browser
      deckStack.showStack();
      logMessage("You looked at the top " + numCards + " cards of your deck");
    }

    // Make whole deck stack visible. Open Browser. Shuffle after search.
    public void search()
    {
      // Make deck stack visible
      for (int i = 0; i < deckStack.cardsVisibility.Count; i++)
      {
        deckStack.cardsVisibility[i] = true;
      }
      // Open Browser and have it shuffle the cards on close
      browserObject.GetComponent<CardBrowser>().shuffle = true;
      deckStack.showStack();
      logMessage("You searched your library for a card");
    }

    public void logMessage(string message)
    {
      string time = DateTime.Now.ToLongTimeString();
      message = time + " - " + message;
      loggerObject.GetComponent<Logger>().addToLogger(message);
    }

    public void tossCoin()
    {
      Image coin = coinImage.GetComponent<Image>();
      TMP_Text time = coinTime.GetComponent<TMP_Text>();
      tossTime = DateTime.Now.ToLongTimeString();
      coinToss = UnityEngine.Random.Range(0, 2);
      if (coinToss == 0) // Heads
      {
        Texture2D headTexture = Resources.Load("Images/coinHeads") as Texture2D;
        coin.sprite = Sprite.Create(headTexture, new Rect(0, 0, headTexture.width, headTexture.height), new Vector2(0.5f, 0.5f));
      }
      else if (coinToss == 1) // Tails
      {
        Texture2D tailTexture = Resources.Load("Images/coinTails") as Texture2D;
        coin.sprite = Sprite.Create(tailTexture, new Rect(0, 0, tailTexture.width, tailTexture.height), new Vector2(0.5f, 0.5f));
      }
      time.text = tossTime;
      StartCoroutine(disableCoinButtonTemporarily(3));
    }

    private IEnumerator disableCoinButtonTemporarily(int seconds)
    {
      coinButton.GetComponent<Button>().interactable = false;
      yield return new WaitForSeconds(3);
      coinButton.GetComponent<Button>().interactable = true;
    }
}
