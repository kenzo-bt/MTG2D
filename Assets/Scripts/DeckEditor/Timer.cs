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
            // Time formatting
            string minutesString =  minutes.ToString();
            string secondsString = seconds.ToString();
            if (minutes < 10)
            {
              minutesString = "0" + minutesString;
            }
            if (seconds < 10)
            {
              secondsString = "0" + secondsString;
            }
            string timeRemaining = minutesString + ":" + secondsString;
            timer.text = timeRemaining;
            yield return new WaitForSeconds(1f);
            secondsRemaining--;
        }
    }
}
