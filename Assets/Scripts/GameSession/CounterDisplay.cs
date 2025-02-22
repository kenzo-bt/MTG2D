using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CounterDisplay : MonoBehaviour
{
    public GameObject counterUp;
    public GameObject counterDown;
    public GameObject counterManager;
    TMP_Text counterDisplay;

    // Start is called before the first frame update
    void Start()
    {
        counterDisplay = counterManager.GetComponent<TMP_Text>();
        counterDisplay.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseCounter()
    {
        string counter = counterDisplay.text;
        int countertwo = int.Parse(counter);
        countertwo++;
        counter = countertwo.ToString();
        counterDisplay.text = counter;
    }
    public void decreaseCounter()
    {
        string counter = counterDisplay.text;
        int countertwo = int.Parse(counter);
        countertwo--;
        counter = countertwo.ToString();
        counterDisplay.text = counter;
    }

}
