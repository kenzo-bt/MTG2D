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
      float cardWidth = cardPrefab.GetComponent<RectTransform>().sizeDelta.x * cardPrefab.GetComponent<RectTransform>().localScale.x;
      float allCardsWidth = cardWidth * transform.childCount;
      float areaWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
      float spacingOffset = 0f;
      if (allCardsWidth > areaWidth)
      {
        allCardsWidth = areaWidth;
        spacingOffset = (cardWidth - (allCardsWidth / transform.childCount)) / transform.childCount;
      }
      float individualSpace = (allCardsWidth / transform.childCount)- spacingOffset;
      float positionX = 0 - (allCardsWidth / 2);

      foreach (Transform child in transform)
      {
        child.localPosition = new Vector3(positionX, child.localPosition.y, child.localPosition.z);
        positionX += individualSpace;
      }
    }
}
