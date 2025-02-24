using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Opponent : MonoBehaviour
{
    public GameObject player;
    public GameObject hand;
    public GameObject battlefieldCardPrefab;
    public GameObject handCardPrefab;
    public GameObject lifeObject;
    public GameObject creatureArea;
    public GameObject otherArea;
    public GameObject landArea;
    public GameObject deckObject;
    public GameObject graveObject;
    public GameObject exileObject;
    public GameObject coinToss;
    public GameObject tossTime;
    public GameObject diceImage;
    public GameObject diceRoll;
    public GameObject rollTime;
    private OppCardStack deck;
    private OppCardStack grave;
    private OppCardStack exile;
    public BoardState prevState;
    public BoardState state;
    private TMP_Text life;
    private int lastEventIndex;
    private bool defeated;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeOpponent()
    {
      prevState = new BoardState();
      state = new BoardState();
      life = lifeObject.GetComponent<TMP_Text>();
      deck = deckObject.GetComponent<OppCardStack>();
      grave = graveObject.GetComponent<OppCardStack>();
      exile = exileObject.GetComponent<OppCardStack>();
      deck.initializeStack();
      grave.initializeStack();
      exile.initializeStack();
      lastEventIndex = -1;
      defeated = false;
    }

    public void updateBoard()
    {
      updateHand();
      updateBattlefield();
      updateStacks();
      updateLife();
      updateCoin();
      updateDice();
      updateOpponentEvents();
    }

    public void updateHand()
    {
      // Determine if previous hand is different from new hand
      if (prevState.hand != null)
      {
        if (prevState.hand.SequenceEqual(state.hand))
        {
          return;
        }
      }
      // Destroy the hand objects
      int cardCount = hand.transform.childCount;
      for (int i = 0; i < cardCount; i++)
      {
        DestroyImmediate(hand.transform.GetChild(0).gameObject);
      }
      // Create card objects
      foreach (string cardID in state.hand)
      {
        string onlyCard = cardID.Split("-H")[0];
        CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(onlyCard);
        GameObject cardInstance = Instantiate(handCardPrefab, hand.transform);
        cardInstance.GetComponent<WebCard>().texturizeCard(targetCard);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
        if (cardID.Split("-H").Length > 1)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().hideCard();
        }
      }
      // Order cards
      hand.GetComponent<Container>().orderChildrenCenter();
    }

    public void updateBattlefield()
    {
      updateCreatures();
      updateLands();
      updateOthers();
    }

    public void updateCreatures()
    {
      // Check if area has changed from previous state
      if (prevState.creatures != null)
      {
        if (prevState.creatures.SequenceEqual(state.creatures))
        {
          return;
        }
      }
      // Destroy previous objects
      int numCards = creatureArea.transform.childCount;
      for (int i = 0; i < numCards; i++)
      {
        DestroyImmediate(creatureArea.transform.GetChild(0).gameObject);
      }
      // Populate with new objects
      foreach (string card in state.creatures)
      {
        bool isTapped = card.Contains("--T");
        bool isFlipped = card.Contains("--F");
        bool isFlipped180 = card.Contains("--F180");
        bool hasCounters = card.Contains("--%");
        string onlyCard = card.Split("--")[0];
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, creatureArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(onlyCard);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
        if (isTapped)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().tapCard();
        }
        if (isFlipped)
        {
          cardInstance.GetComponent<WebCard>().showBack();
          // Hide highlight if morph
          if (cardInfo.text.Contains("Morph") || cardInfo.text.Contains("Disguise {"))
          {
            cardInstance.GetComponent<OppBattlefieldCard>().turnFaceDown();
          }
        }
        if (isFlipped180)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().flip180();
        }
        if (hasCounters)
        {
          try
          {
            int startingIndex = card.IndexOf("--%") + 3;
            Debug.Log(card.Substring(startingIndex));
            int counterAmount = Int32.Parse(card.Substring(startingIndex));
            cardInstance.GetComponent<OppBattlefieldCard>().showCounters(counterAmount);
          }
          catch (Exception e)
          {
            Debug.Log(e.Message);
          }
        }
      }
      creatureArea.GetComponent<Container>().orderChildrenCenter();
    }

    public void updateLands()
    {
      // Check if area has changed from previous state
      if (prevState.lands != null)
      {
        if (prevState.lands.SequenceEqual(state.lands))
        {
          return;
        }
      }
      // Destroy previous objects
      int numCards = landArea.transform.childCount;
      for (int i = 0; i < numCards; i++)
      {
        DestroyImmediate(landArea.transform.GetChild(0).gameObject);
      }
      // Populate with new objects
      foreach (string card in state.lands)
      {
        bool isTapped = card.Contains("--T");
        bool isFlipped = card.Contains("--F");
        bool isFlipped180 = card.Contains("--F180");
        bool hasCounters = card.Contains("--%");
        string onlyCard = card.Split("--")[0];
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, landArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(onlyCard);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
        if (isTapped)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().tapCard();
        }
        if (isFlipped)
        {
          cardInstance.GetComponent<WebCard>().showBack();
        }
        if (hasCounters)
        {
          try
          {
            int startingIndex = card.IndexOf("--%") + 3;
            Debug.Log(card.Substring(startingIndex));
            int counterAmount = Int32.Parse(card.Substring(startingIndex));
            cardInstance.GetComponent<OppBattlefieldCard>().showCounters(counterAmount);
          }
          catch (Exception e)
          {
            Debug.Log(e.Message);
          }
        }
      }
      landArea.GetComponent<Container>().orderChildrenCenter();
    }

    public void updateOthers()
    {
      // Check if area has changed from previous state
      if (prevState.others != null)
      {
        if (prevState.others.SequenceEqual(state.others))
        {
          return;
        }
      }
      // Destroy previous objects
      int numCards = otherArea.transform.childCount;
      for (int i = 0; i < numCards; i++)
      {
        DestroyImmediate(otherArea.transform.GetChild(0).gameObject);
      }
      // Populate with new objects
      foreach (string card in state.others)
      {
        bool isTapped = card.Contains("--T");
        bool isFlipped = card.Contains("--F");
        bool isFlipped180 = card.Contains("--F180");
        bool hasCounters = card.Contains("--%");
        string onlyCard = card.Split("--")[0];
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, otherArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(onlyCard);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
        if (isTapped)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().tapCard();
        }
        if (isFlipped)
        {
          cardInstance.GetComponent<WebCard>().showBack();
        }
        if (hasCounters)
        {
          try
          {
            int startingIndex = card.IndexOf("--%") + 3;
            Debug.Log(card.Substring(startingIndex));
            int counterAmount = Int32.Parse(card.Substring(startingIndex));
            cardInstance.GetComponent<OppBattlefieldCard>().showCounters(counterAmount);
          }
          catch (Exception e)
          {
            Debug.Log(e.Message);
          }
        }
      }
      otherArea.GetComponent<Container>().orderChildrenCenter();
    }

    public void updateStacks()
    {
      updateDeck();
      updateGrave();
      updateExile();
    }

    public void updateDeck()
    {
      if (prevState.deck != null)
      {
        if (prevState.deck.SequenceEqual(state.deck))
        {
          return;
        }
      }
      deck.cards = new List<string>();
      deck.cardsVisibility = new List<bool>();
      foreach (string card in state.deck)
      {
        string onlyCard = card.Split("-H")[0];
        if (card.Split("-H").Length > 1)
        {
          deck.addCard(onlyCard, false);
        }
        else
        {
          deck.addCard(onlyCard, true);
        }
      }
    }

    public void updateGrave()
    {
      if (prevState.grave != null)
      {
        if (prevState.grave.SequenceEqual(state.grave))
        {
          return;
        }
      }
      grave.cards = new List<string>();
      grave.cardsVisibility = new List<bool>();
      foreach (string card in state.grave)
      {
        string onlyCard = card.Split("-H")[0];
        if (card.Split("-H").Length > 1)
        {
          grave.addCard(onlyCard, false);
        }
        else
        {
          grave.addCard(onlyCard, true);
        }
      }
    }

    public void updateExile()
    {
      if (prevState.exile != null)
      {
        if (prevState.exile.SequenceEqual(state.exile))
        {
          return;
        }
      }
      exile.cards = new List<string>();
      exile.cardsVisibility = new List<bool>();
      foreach (string card in state.exile)
      {
        string onlyCard = card.Split("-H")[0];
        if (card.Split("-H").Length > 1)
        {
          exile.addCard(onlyCard, false);
        }
        else
        {
          exile.addCard(onlyCard, true);
        }
      }
    }

    public void updateLife()
    {
      life.text = state.life.ToString();
      if (!defeated && state.life <= 0)
      {
        defeated = true;
        StartCoroutine(PlayerManager.Instance.registerWin(PlayerManager.Instance.myID));
      }
    }

    public void updateCoin()
    {
      Image coin = coinToss.GetComponent<Image>();
      TMP_Text time = tossTime.GetComponent<TMP_Text>();
      if (state.coinVisible)
      {
        if (state.coinToss == 0) // Heads
        {
          Texture2D headTexture = Resources.Load("Images/coinHeads") as Texture2D;
          coin.sprite = Sprite.Create(headTexture, new Rect(0, 0, headTexture.width, headTexture.height), new Vector2(0.5f, 0.5f));
        }
        else if (state.coinToss == 1) // Tails
        {
          Texture2D tailTexture = Resources.Load("Images/coinTails") as Texture2D;
          coin.sprite = Sprite.Create(tailTexture, new Rect(0, 0, tailTexture.width, tailTexture.height), new Vector2(0.5f, 0.5f));
        }
        time.text = state.tossTime;
        showCoin();
      }
      else
      {
        hideCoin();
      }
    }

    public void hideCoin()
    {
      coinToss.GetComponent<CanvasGroup>().alpha = 0;
      coinToss.GetComponent<CanvasGroup>().blocksRaycasts = false;
      tossTime.GetComponent<TMP_Text>().text = "";
    }

    public void showCoin()
    {
      coinToss.GetComponent<CanvasGroup>().alpha = 1;
      coinToss.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void updateDice()
    {
      if (state.diceVisible)
      {
        TMP_Text time = rollTime.GetComponent<TMP_Text>();
        TMP_Text roll = diceRoll.GetComponent<TMP_Text>();
        time.text = state.rollTime;
        roll.text = "" + state.diceRoll;
        showDice();
      }
      else
      {
        hideDice();
      }
    }

    public void hideDice()
    {
      diceImage.GetComponent<CanvasGroup>().alpha = 0;
      diceImage.GetComponent<CanvasGroup>().blocksRaycasts = false;
      rollTime.GetComponent<TMP_Text>().text = "";
      diceRoll.GetComponent<TMP_Text>().text = "";
    }

    public void showDice()
    {
      diceImage.GetComponent<CanvasGroup>().alpha = 1;
      diceImage.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void updateOpponentEvents()
    {
      if ((state.events.Count - 1) > lastEventIndex)
      {
        for (int i = lastEventIndex + 1; i < state.events.Count; i++)
        {
          player.GetComponent<Player>().logOpponentEvent(state.events[i]);
        }
        lastEventIndex = state.events.Count - 1;
      }
    }
}
