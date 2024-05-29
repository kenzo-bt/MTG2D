using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timer;
    private AudioSource timerAlarm;
    private int secondsRemaining;
    private Color mediumAlert;
    private Color highAlert;

    // Start is called before the first frame update
    void Start()
    {
        timerAlarm = GetComponent<AudioSource>();
        timer.text = "";
        mediumAlert = new Color(1f, 0.9f, 0f, 1f);
        highAlert = new Color(1f, 0f, 0f, 1f);
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
        // Visual alerts of time remaining
        if (secondsRemaining == 30)
        {
          StartCoroutine(flashTimer());
          timerAlarm.Play(0);
        }
        if (secondsRemaining <= 60)
        {
          GetComponent<Image>().color = highAlert;
          timer.color = highAlert;
        }
        else if (secondsRemaining <= 300)
        {
          GetComponent<Image>().color = mediumAlert;
          timer.color = mediumAlert;
        }

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

    private IEnumerator flashTimer()
    {
      while (true)
      {
        yield return timer.GetComponent<CanvasGroupFade>().FadeToZeroAlpha(0.5f);
        yield return timer.GetComponent<CanvasGroupFade>().FadeToFullAlpha(0.5f);
      }
    }
}
