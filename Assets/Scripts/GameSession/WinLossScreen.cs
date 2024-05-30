using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinLossScreen : MonoBehaviour
{
    public GameObject gems;
    public GameObject gemsText;
    public GameObject coins;
    public GameObject coinsText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showScreen()
    {
      GetComponent<CanvasGroup>().alpha = 1;
      GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void hideScreen()
    {
      GetComponent<CanvasGroup>().alpha = 0;
      GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void setGemAmount(int amount)
    {
      gems.SetActive(true);
      gemsText.GetComponent<TMP_Text>().text = "" + amount;
    }

    public void setCoinAmount(int amount)
    {
      coins.SetActive(true);
      coinsText.GetComponent<TMP_Text>().text = "" + amount;
    }

    public void returnToHub()
    {
      SceneManager.LoadScene("Hub");
    }
}
