using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckActions : MonoBehaviour
{
    public GameObject player;
    public GameObject millMenu;
    public GameObject lookMenu;
    public GameObject millInput;
    public GameObject lookInput;
    public GameObject errorMessage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void show()
    {
      GetComponent<CanvasGroup>().alpha = 1f;
      GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void hide()
    {
      GetComponent<CanvasGroup>().alpha = 0f;
      GetComponent<CanvasGroup>().blocksRaycasts = false;
      millMenu.SetActive(false);
      lookMenu.SetActive(false);
      errorMessage.GetComponent<TMP_Text>().text = "";
    }

    public void showMillMenu()
    {
      show();
      millMenu.SetActive(true);
    }

    public void showLookMenu()
    {
      show();
      lookMenu.SetActive(true);
    }

    public void validateMill()
    {
      string inputText = millInput.GetComponent<TMP_InputField>().text;
      try
      {
        int numCards = Int32.Parse(inputText);
        if (numCards >= 0)
        {
          player.GetComponent<Player>().mill(numCards);
          hide();
        }
        else
        {
          errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer.";
        }
      }
      catch (FormatException)
      {
        errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer.";
      }
    }

    public void validateLook()
    {
      string inputText = lookInput.GetComponent<TMP_InputField>().text;
      try
      {
        int numCards = Int32.Parse(inputText);
        if (numCards >= 0)
        {
          player.GetComponent<Player>().look(numCards);
          hide();
        }
        else
        {
          errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer.";
        }
      }
      catch (FormatException)
      {
        errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer.";
      }
    }
}
