using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterTracker : MonoBehaviour
{
    public GameObject counterTextObject;
    private TMP_Text counterText;

    // Start is called before the first frame update
    void Awake()
    {
        counterText = counterTextObject.GetComponent<TMP_Text>();
        counterText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeCounterValue(int changeAmount)
    {
      try
      {
        int value = Int32.Parse(counterText.text) + changeAmount;
        counterText.text = value.ToString();
      }
      catch (Exception e)
      {
        Debug.Log(e.Message);
      }
    }

    public void setCounterValue(int value)
    {
      counterText.text = value.ToString();
    }

    public int getCounterValue()
    {
      try
      {
        return Int32.Parse(counterText.text);
      }
      catch (Exception e)
      {
        Debug.Log(e.Message);
        return 0;
      }
    }
}
