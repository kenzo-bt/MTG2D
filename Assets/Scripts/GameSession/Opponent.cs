using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
      // Destroy the hand objects
      int cardCount = hand.transform.childCount;
      for (int i = 0; i < cardCount; i++)
      {
        DestroyImmediate(hand.transform.GetChild(0).gameObject);
      }
      // Create card objects
      foreach (string cardID in state.hand)
      {
        CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(cardID);
        GameObject cardInstance = Instantiate(handCardPrefab, hand.transform);
        cardInstance.GetComponent<WebCard>().texturizeCard(targetCard);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
      }
      // Order cards
      hand.GetComponent<Container>().orderChildrenCenter();
    }

    public void updateBattlefield()
    {
      // Destroy all cards in battlefield
      for (int i = 0; i < creatureArea.transform.childCount; i++)
      {
        DestroyImmediate(creatureArea.transform.GetChild(0).gameObject);
      }
      for (int i = 0; i < landArea.transform.childCount; i++)
      {
        DestroyImmediate(landArea.transform.GetChild(0).gameObject);
      }
      for (int i = 0; i < otherArea.transform.childCount; i++)
      {
        DestroyImmediate(otherArea.transform.GetChild(0).gameObject);
      }
      // Repopulate battlefield with new state
      foreach (string card in state.creatures)
      {
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, creatureArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(card);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
      }
      creatureArea.GetComponent<Container>().orderChildrenCenter();
      foreach (string card in state.lands)
      {
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, landArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(card);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
      }
      landArea.GetComponent<Container>().orderChildrenCenter();
      foreach (string card in state.others)
      {
        GameObject cardInstance = Instantiate(battlefieldCardPrefab, otherArea.transform);
        CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(card);
        cardInstance.GetComponent<WebCard>().texturizeCard(cardInfo);
        cardInstance.GetComponent<OppBattlefieldCard>().player = player;
      }
      otherArea.GetComponent<Container>().orderChildrenCenter();
    }

    public void updateStacks()
    {
      deck.cards = new List<string>();
      deck.cardsVisibility = new List<bool>();
      foreach (string card in state.deck)
      {
        deck.addCard(card, false);
      }
      grave.cards = new List<string>();
      grave.cardsVisibility = new List<bool>();
      foreach (string card in state.grave)
      {
        grave.addCard(card, true);
      }
      exile.cards = new List<string>();
      exile.cardsVisibility = new List<bool>();
      foreach (string card in state.exile)
      {
        exile.addCard(card, true);
      }
    }

    public void updateLife()
    {
      life.text = state.life.ToString();
    }
}
