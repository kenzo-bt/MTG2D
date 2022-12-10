using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Enter decks scene
    public void enterDecks()
    {
      SceneManager.LoadScene("DeckBrowser");
    }

    // Enter user scene
    public void enterUser()
    {
      Debug.Log("enterUser() invoked");
    }

    // Enter play scene
    public void enterPlay()
    {
      SceneManager.LoadScene("GameSession");
    }
}
