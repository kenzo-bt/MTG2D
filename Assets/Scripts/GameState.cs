using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public GameObject deckObject;
    public GameObject handObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Restart Game
    public void restartGame()
    {
      // Reset deck
      Deck deck = deckObject.GetComponent<Deck>();
      deck.resetDeck();
      // Reset hand
      Hand hand = handObject.GetComponent<Hand>();
      hand.resetHand();
    }
}
