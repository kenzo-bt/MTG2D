using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Decklist selectedDeck;
    public List<Decklist> allDecks;

    private void Awake()
    {
      if (Instance != null)
      {
          Destroy(gameObject);
          return;
      }
      Instance = this;
      DontDestroyOnLoad(gameObject);

      // Load the decks from disk
      //TextAsset listFile = Resources.Load("Decklist") as TextAsset;
      Debug.Log(Application.persistentDataPath);
    }
}
