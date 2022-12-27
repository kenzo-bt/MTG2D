using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldArea : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addCard(CardInfo card)
    {
      GameObject cardInstance = Instantiate(cardPrefab, transform);
      cardInstance.GetComponent<WebCard>().texturizeCard(card);
      cardInstance.GetComponent<BattlefieldCard>().player = player;
      orderArea();
    }

    public void removeCard(string cardId)
    {
      int numCards = transform.childCount;
      for (int i = 0; i < numCards; i++)
      {
        WebCard card = transform.GetChild(i).gameObject.GetComponent<WebCard>();
        if (card.cardId == cardId)
        {
          DestroyImmediate(transform.GetChild(i).gameObject);
          orderArea();
          break;
        }
      }
    }

    // Position cards in area. Cards should have a left-aligned x-anchor.
    public void orderArea()
    {
      GetComponent<Container>().orderChildrenCenter();
    }
}
