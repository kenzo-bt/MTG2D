using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaCurveDisplay : MonoBehaviour
{
    private List<ManaGauge> gauges;
    private List<int> manaFrequencies;
    private List<float> gaugeValues;
    private Decklist deck;
    private int maxFrequency;

    // Start is called before the first frame update
    void Start()
    {
      gauges = new List<ManaGauge>();
      manaFrequencies = new List<int>();
      gaugeValues = new List<float>();
      deck = PlayerManager.Instance.selectedDeck;
      maxFrequency = 0;

      foreach (Transform child in transform)
      {
        ManaGauge gauge = child.gameObject.GetComponent<ManaGauge>();
        gauges.Add(gauge);
        gaugeValues.Add(0f);
        manaFrequencies.Add(0);
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void calculateGaugeValues()
    {
      maxFrequency = 0;
      for (int i = 0; i < manaFrequencies.Count; i++)
      {
        if (manaFrequencies[i] > maxFrequency)
        {
          maxFrequency = manaFrequencies[i];
        }
      }
      for (int i = 0; i < manaFrequencies.Count; i++)
      {
        gaugeValues[i] = (float)manaFrequencies[i] / (float)maxFrequency;
      }
    }

    private void fillGauges()
    {
      for (int i = 0; i < gauges.Count; i++)
      {
        gauges[i].setGaugePercent(gaugeValues[i]);
      }
    }

    // Update the mana curve gauges
    public void updateManaCurve(Decklist deck)
    {
      resetManaFrequencies();

      for (int i = 0; i < deck.cards.Count; i++)
      {
        int cmc = deck.cards[i].convertedManaCost;
        int frequency = deck.cardFrequencies[i];
        if (cmc > 0 && cmc < 6)
        {
          manaFrequencies[cmc - 1] += frequency;
        }
        else if(cmc >= 6)
        {
          manaFrequencies[5] += frequency;
        }
      }

      calculateGaugeValues();
      fillGauges();
    }

    // Reset manaFrequency list
    private void resetManaFrequencies()
    {
      for (int i = 0; i < manaFrequencies.Count; i++)
      {
        manaFrequencies[i] = 0;
      }
    }

    // Debug
    public void debugManaCurve()
    {
      for (int i = 0; i < gauges.Count; i++)
      {
        Debug.Log("Gauge " + (i + 1) + ": " + gauges[i].getGaugePercent());
      }
    }
}
