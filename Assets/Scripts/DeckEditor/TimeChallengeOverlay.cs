using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChallengeOverlay : MonoBehaviour
{
    public GameObject cardObject1;
    public GameObject cardObject2;
    public GameObject cardObject3;

    // Start is called before the first frame update
    void Start()
    {
      loadCards();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadCards()
    {
      CardInfo card1 = PlayerManager.Instance.getCardFromLookup(PlayerManager.Instance.timeChallengeRares[0]);
      cardObject1.GetComponent<WebCard>().texturizeCard(card1);
      CardInfo card2 = PlayerManager.Instance.getCardFromLookup(PlayerManager.Instance.timeChallengeRares[1]);
      cardObject2.GetComponent<WebCard>().texturizeCard(card2);
      CardInfo card3 = PlayerManager.Instance.getCardFromLookup(PlayerManager.Instance.timeChallengeRares[2]);
      cardObject3.GetComponent<WebCard>().texturizeCard(card3);
    }
}
