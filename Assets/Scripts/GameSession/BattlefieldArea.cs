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

    public void removeCard(int index)
    {
      DestroyImmediate(transform.GetChild(index).gameObject);
      orderArea();
    }

    // Position cards in area. Cards should have a left-aligned x-anchor.
    public void orderArea()
    {
      GetComponent<Container>().orderChildrenCenter();
    }
}