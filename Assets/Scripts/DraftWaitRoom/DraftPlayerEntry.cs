using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftPlayerEntry : MonoBehaviour
{
    public GameObject usernameObject;
    public GameObject statusObject;
    public bool ready;
    public string username;
    // Start is called before the first frame update
    void Awake()
    {
      ready = false;
      username = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setUsername(string name)
    {
      username = name;
      usernameObject.GetComponent<TMP_Text>().text = username;
    }

    public void setReady()
    {
      ready = true;
      Image statusCircle = statusObject.GetComponent<Image>();
      Texture2D readyTexture = Resources.Load("Images/doneCircle") as Texture2D;
      statusCircle.sprite = Sprite.Create(readyTexture, new Rect(0, 0, readyTexture.width, readyTexture.height), new Vector2(0.5f, 0.5f));
    }

    public void setUnready()
    {
      ready = false;
      Image statusCircle = statusObject.GetComponent<Image>();
      Texture2D loadingTexture = Resources.Load("Images/loadingCircle") as Texture2D;
      statusCircle.sprite = Sprite.Create(loadingTexture, new Rect(0, 0, loadingTexture.width, loadingTexture.height), new Vector2(0.5f, 0.5f));
    }
}
