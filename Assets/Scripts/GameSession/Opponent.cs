using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private OppCardStack deck;
    private OppCardStack grave;
    private OppCardStack exile;
    public BoardState prevState;
    public BoardState state;
    private TMP_Text life;

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
    }

    public void updateBoard()
    {
      updateHand();
      updateBattlefield();
      updateStacks();
      updateLife();
    }

    public void updateHand()
    {
      // Determine if previous hand is different from new hand
      if (prevState.hand != null)
      {
        if (prevState.hand.SequenceEqual(state.hand))
        {
          Debug.Log("Hand has not changed. Skipping...");
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
          Debug.Log("Creature area has not changed. Skipping...");
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
        string onlyCard = card.Split("-T")[0];
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, creatureArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(onlyCard);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
        if (card.Split("-T").Length > 1)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().tapCard();
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
          Debug.Log("Lands area has not changed. Skipping...");
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
        string onlyCard = card.Split("-T")[0];
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, landArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(onlyCard);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
        if (card.Split("-T").Length > 1)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().tapCard();
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
          Debug.Log("Others area has not changed. Skipping...");
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
        string onlyCard = card.Split("-T")[0];
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, otherArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(onlyCard);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
        if (card.Split("-T").Length > 1)
        {
          cardInstance.GetComponent<OppBattlefieldCard>().tapCard();
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
          Debug.Log("Deck has not changed. Skipping...");
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
          Debug.Log("Grave has not changed. Skipping...");
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
          Debug.Log("Exile has not changed. Skipping...");
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
    }
}
