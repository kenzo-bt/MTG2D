using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeCounter : MonoBehaviour
{
    public GameObject creaturesObject;
    public GameObject nonCreaturesObject;
    public GameObject landsObject;
    private TMP_Text creaturesText;
    private TMP_Text nonCreaturesText;
    private TMP_Text landsText;

    // Start is called before the first frame update
    void Start()
    {
      creaturesText = creaturesObject.GetComponent<TMP_Text>();
      nonCreaturesText = nonCreaturesObject.GetComponent<TMP_Text>();
      landsText = landsObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateCounts(Decklist deck)
    {
      int creatures = 0;
      int nonCreatures = 0;
      int lands = 0;

      for (int i = 0; i < deck.cards.Count; i++)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(deck.cards[i]);
        if (card.isLand())
        {
          lands += deck.cardFrequencies[i];
        }
        else if (card.types.Contains("Creature"))
        {
          creatures += deck.cardFrequencies[i];
        }
        else
        {
          nonCreatures += deck.cardFrequencies[i];
        }
      }

      creaturesText.text = creatures.ToString();
      nonCreaturesText.text = nonCreatures.ToString();
      landsText.text = lands.ToString();
    }
}
