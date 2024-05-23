using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackdropRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      randomizeBackdrop();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void randomizeBackdrop() {
      int numBackdrops = 5;
      int randomInt = UnityEngine.Random.Range(1, numBackdrops + 1);
      string backdropName = "" + randomInt;
      Image backdropImage = transform.gameObject.GetComponent<Image>();
      Texture2D backdropTexture = Resources.Load("Images/Backdrops/" + backdropName) as Texture2D;
      backdropImage.sprite = Sprite.Create(backdropTexture, new Rect(0, 0, backdropTexture.width, backdropTexture.height), new Vector2(0.5f, 0.5f));
    }
}
