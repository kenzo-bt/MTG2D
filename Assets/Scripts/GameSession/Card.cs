using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private string cardName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Texture card
    public void texturizeCard(CardInfo card)
    {
      cardName = card.name;
      Image cardImage = GetComponent<Image>();
      Texture2D cardTexture = Resources.Load("Images/Cards/" + card.set + "/" + card.id) as Texture2D;
      cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
    }
}
