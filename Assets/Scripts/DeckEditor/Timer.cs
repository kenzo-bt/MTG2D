using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timer;
    private int secondsRemaining;

    // Start is called before the first frame update
    void Start()
    {
        timer.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator startTimer()
    {
        secondsRemaining = 900;

        while (secondsRemaining > 0)
        {
            int minutes = secondsRemaining / 60;
            int seconds = secondsRemaining % 60;
            string timeRemaining = minutes.ToString() + ":" + seconds.ToString();
            timer.text = timeRemaining;
            yield return new WaitForSeconds(1f);
            secondsRemaining--;
        }
    }
}
