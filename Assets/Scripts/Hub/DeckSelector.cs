using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelector : MonoBehaviour
{
    private CanvasGroup deckSelector;
    // Start is called before the first frame update
    void Start()
    {
        deckSelector = GetComponent<CanvasGroup>();
        deckSelector.alpha = 0;
        deckSelector.blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Make deck browser + browse arrows visible
    public void show()
    {
      deckSelector.alpha = 1;
      deckSelector.blocksRaycasts = true;
    }

    // Hide deck browser + browse arrows visible
    public void hide()
    {
      deckSelector.alpha = 0;
      deckSelector.blocksRaycasts = false;
    }
}
