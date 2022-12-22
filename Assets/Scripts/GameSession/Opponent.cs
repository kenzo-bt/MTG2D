using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public GameObject hand;
    public GameObject cardPrefab;
    public BoardState state;

    // Start is called before the first frame update
    void Start()
    {
      state = new BoardState();
      state.hand = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateBoard()
    {
      updateHand();
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
        GameObject cardInstance = Instantiate(cardPrefab, hand.transform);
        cardInstance.GetComponent<WebCard>().texturizeCard(targetCard);
      }
      // Order cards
      hand.GetComponent<Container>().orderChildrenCenter();
    }
}
