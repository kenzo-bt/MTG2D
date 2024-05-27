using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChallengeOverlay : MonoBehaviour
{
    public GameObject[] cardObjects;
    public GameObject decklistPanel;

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
      for (int i = 0; i < cardObjects.Length; i++)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(PlayerManager.Instance.timeChallengeRares[i]);
        cardObjects[i].GetComponent<WebCard>().texturizeCard(card);
      }
    }

    public void selectCard(int index)
    {
      string id = PlayerManager.Instance.getCardFromLookup(PlayerManager.Instance.timeChallengeRares[index]).id;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(id);
      PlayerManager.Instance.timeChallengeSelectedRare = id;
      // PlayerManager.Instance.selectedDeck.cards[0] = id;
      // PlayerManager.Instance.selectedDeck.cardFrequencies[0] = 1;
      // PlayerManager.Instance.selectedDeck.timeChallengeCardColours = card.colourIdentity;
      decklistPanel.GetComponent<DeckListPanel>().updatePanel();
      StartCoroutine(GetComponent<CanvasGroupFade>().FadeToZeroAlpha(1f));
    }
}
