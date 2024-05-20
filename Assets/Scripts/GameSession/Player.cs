using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
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
    public GameObject diceImage;
    public GameObject diceText;
    public GameObject diceTime;
    public GameObject diceButton;
    private Hand hand;
    private Deck deck;
    private Battlefield battlefield;
    private CardStack grave;
    private CardStack exile;
    private CardStack deckStack;
    private WebCard highlightCard;
    private WebCard highlightBackCard;
    private TMP_Text lifeCounter;
    private Opponent opponent;
    private Hasher hasher;
    private int mulligans;
    public int initialHandSize;
    private int lifeTotal;
    public string log;
    public int coinToss;
    public string tossTime;
    public bool coinVisible;
    public int diceRoll;
    public string rollTime;
    public bool diceVisible;
    public List<string> eventLog;

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
      mulligans = 0;
      initialHandSize = 7;
      lifeTotal = 20;
      log = "";
      coinToss = 0;
      tossTime = "";
      coinVisible = false;
      diceRoll = 0;
      rollTime = "";
      diceVisible = false;
      eventLog = new List<string>();
      // Initialize your life counter in the UI
      updateLifeTotal();
      // Initialize your deck and hand
      deck.initializeDeck();
      hand.initializeHand();
      battlefield.initializeBattlefield();
      grave.initializeStack();
      exile.initializeStack();
      // TODO remove this -> opponent.initializeOpponent();
      // Draw 7 cards (Involves hand and deck -> Player handles this)
      drawCards(initialHandSize);
      // Initialize game state
      if (gameStateObject.GetComponent<GameState>() != null)
      {
        gameStateObject.GetComponent<GameState>().initializeState();
      }
      else
      {
        gameStateObject.GetComponent<GameStateFFA>().initializeState();
      }
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
      registerEvent(PlayerManager.Instance.myName + " drew a card");
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
        registerEvent(PlayerManager.Instance.myName + " performed a mulligan (Drew " + (initialHandSize - mulligans) + ")");
      }
    }

    // Shuffle deck
    public void shuffleDeck()
    {
      deck.shuffleDeck();
      logMessage("You shuffled your deck");
      registerEvent(PlayerManager.Instance.myName + " shuffled their deck");
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
      myState.coinVisible = coinVisible;
      myState.diceRoll = diceRoll;
      myState.rollTime = rollTime;
      myState.diceVisible = diceVisible;
      myState.events = new List<string>(eventLog);
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
      registerEvent(PlayerManager.Instance.myName + " dropped " + card.name + " to the battlefield");
      // Check if daily objectives should be updated
      DailyObjectives dailies = PlayerManager.Instance.dailyObjectives;
      string objectiveIndexes = "";
      for (int i = 0; i < dailies.objectives.Count; i++)
      {
        Objective objective = dailies.objectives[i];
        if (objective.progress >= objective.quantity) {
          continue;
        }
        if (objective.target == "Instant/Sorcery")
        {
          if (card.types.Contains("Instant") || card.types.Contains("Sorcery"))
          {
            if (card.colours.Contains(objective.colour))
            {
              objectiveIndexes += ("" + i + " ");
              objective.progress += 1;
            }
          }
        }
        else if (objective.target == "Land")
        {
          if (card.types.Contains("Land"))
          {
            if (card.colourIdentity.Contains(objective.colour))
            {
              objectiveIndexes += ("" + i + " ");
              objective.progress += 1;
            }
          }
        }
        else {
          if (card.types.Contains(objective.target))
          {
            if (card.colourIdentity.Contains(objective.colour))
            {
              objectiveIndexes += ("" + i + " ");
              objective.progress += 1;
            }
          }
        }
      }
      // Send objective indexes to server if any objectives need to be updated
      if (objectiveIndexes != "") {
        objectiveIndexes = objectiveIndexes.Trim();
        StartCoroutine(progressDailyObjectives(objectiveIndexes));
      }
    }

    // Create a card in the battlefield (token)
    public void addCardToBattlefield(CardInfo card)
    {
      battlefield.addCard(card);
      logMessage("You created a " + card.name + " token");
      registerEvent(PlayerManager.Instance.myName + " created a " + card.name + " token");
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
      registerEvent(PlayerManager.Instance.myName + " moved " + targetCard.name + " from " + sourceArea + "to " + destination);
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
      logMessage("You moved " + targetCard.name + " from " + source + " to " + destination);
      registerEvent(PlayerManager.Instance.myName + " moved a card from their " + source + " into their " + destination);
    }

    // Show highlight card
    public void showHighlightCard(string cardId)
    {
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      highlightCard.texturizeCard(card);
      highlightObject.GetComponent<CanvasGroup>().alpha = 1f;
      if (card.hasBackSide())
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
      registerEvent(PlayerManager.Instance.myName + " changed life total to " + lifeTotal.ToString());
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
      registerEvent(PlayerManager.Instance.myName + " milled " + numCards + " cards");
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
      registerEvent(PlayerManager.Instance.myName + " looked at the top " + numCards + " cards of their deck");
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
      registerEvent(PlayerManager.Instance.myName + " is searching their library");
    }

    public void logMessage(string message)
    {
      string time = DateTime.Now.ToLongTimeString();
      message = time + " - " + message;
      loggerObject.GetComponent<Logger>().addToLogger(message);
    }

    public void logOpponentEvent(string eventMessage)
    {
      loggerObject.GetComponent<Logger>().addToLogger(eventMessage);
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
        logMessage("You tossed the coin. It landed on HEADS");
        registerEvent(PlayerManager.Instance.myName + " tossed the coin. It landed on HEADS");
      }
      else if (coinToss == 1) // Tails
      {
        Texture2D tailTexture = Resources.Load("Images/coinTails") as Texture2D;
        coin.sprite = Sprite.Create(tailTexture, new Rect(0, 0, tailTexture.width, tailTexture.height), new Vector2(0.5f, 0.5f));
        logMessage("You tossed the coin. It landed on TAILS");
        registerEvent(PlayerManager.Instance.myName + " tossed the coin. It landed on TAILS");
      }
      time.text = tossTime;
      coinVisible = true;
      showCoin();
      StartCoroutine(disableCoinButtonTemporarily(5));
    }

    private IEnumerator disableCoinButtonTemporarily(int seconds)
    {
      coinButton.GetComponent<Button>().interactable = false;
      yield return new WaitForSeconds(seconds);
      coinButton.GetComponent<Button>().interactable = true;
      coinVisible = false;
      hideCoin();
    }

    public void hideCoin()
    {
      coinImage.GetComponent<CanvasGroup>().alpha = 0;
      coinImage.GetComponent<CanvasGroup>().blocksRaycasts = false;
      coinTime.GetComponent<TMP_Text>().text = "";
    }

    public void showCoin()
    {
      coinImage.GetComponent<CanvasGroup>().alpha = 1;
      coinImage.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void rollDice()
    {
      rollTime = DateTime.Now.ToLongTimeString();
      diceTime.GetComponent<TMP_Text>().text = rollTime;
      diceRoll = UnityEngine.Random.Range(1, 21);
      diceText.GetComponent<TMP_Text>().text = "" + diceRoll;
      diceVisible = true;
      showDice();
      StartCoroutine(disableDiceButtonTemporarily(5));
      logMessage("You rolled a " + diceRoll);
      registerEvent(PlayerManager.Instance.myName + " rolled a " + diceRoll);
    }

    private IEnumerator disableDiceButtonTemporarily(int seconds)
    {
      diceButton.GetComponent<Button>().interactable = false;
      yield return new WaitForSeconds(seconds);
      diceButton.GetComponent<Button>().interactable = true;
      diceVisible = false;
      hideDice();
    }

    public void hideDice()
    {
      diceImage.GetComponent<CanvasGroup>().alpha = 0;
      diceImage.GetComponent<CanvasGroup>().blocksRaycasts = false;
      diceTime.GetComponent<TMP_Text>().text = "";
      diceText.GetComponent<TMP_Text>().text = "";
    }

    public void showDice()
    {
      diceImage.GetComponent<CanvasGroup>().alpha = 1;
      diceImage.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void registerEvent(string eventDescription)
    {
      string time = DateTime.Now.ToLongTimeString();
      eventDescription = "<color=#FF8400>" + time + " - " + eventDescription + "</color>";
      eventLog.Add(eventDescription);
    }

    private IEnumerator progressDailyObjectives(string indexes)
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/dailies/update";
      ObjectiveUpdate newObjectiveUpdate = new ObjectiveUpdate();
      newObjectiveUpdate.indexes = indexes;
      byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(newObjectiveUpdate));
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      // Debug the results
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        Debug.Log("Updated daily objectives in server: (" + indexes + ")");
      }
      // Dispose of the request to prevent memory leaks
      request.Dispose();
    }
}
