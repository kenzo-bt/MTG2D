using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WebCard : MonoBehaviour
{
    public string cardName;
    public string cardId;
    private string cachePath;
    private string fileExtension;
    public GameObject cardImageObject;
    public GameObject imageMaskObject;
    public GameObject fullImageObject;
    public GameObject fullMaskObject;
    public GameObject fullImageBackObject;
    public GameObject fullMaskBackObject;
    private Image cardImage;
    private Image maskImage;
    private Image fullImage;
    private Image fullMask;
    private Image fullImageBack;
    private Image fullMaskBack;

    void Awake()
    {
      cardImage = cardImageObject.GetComponent<Image>();
      maskImage = imageMaskObject.GetComponent<Image>();
      fullImage = fullImageObject.GetComponent<Image>();
      fullMask = fullMaskObject.GetComponent<Image>();
      if (fullImageBackObject != null)
      {
        fullImageBack = fullImageBackObject.GetComponent<Image>();
        fullMaskBack = fullMaskBackObject.GetComponent<Image>();
      }
    }

    IEnumerator fetchCardFromServer(string serverUrl, string cardId)
    {
      UnityWebRequest request = UnityWebRequestTexture.GetTexture(serverUrl + cardId + fileExtension);
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
        Debug.Log("Failed to retrieve card with ID: " + cardId);
        Debug.Log("ServerURL: " + serverUrl);
      }
      else
      {
        Texture2D cardTexture = ((DownloadHandlerTexture) request.downloadHandler).texture as Texture2D;
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        saveImageToCache(cardTexture, cardId);
      }
    }

    IEnumerator fetchBackCardFromServer(string serverUrl, string cardId)
    {
      UnityWebRequest request = UnityWebRequestTexture.GetTexture(serverUrl + cardId + fileExtension);
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        Texture2D cardTexture = ((DownloadHandlerTexture) request.downloadHandler).texture as Texture2D;
        fullImageBack.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        saveImageToCache(cardTexture, cardId);
      }
    }

    // Apply texture to card
    public void texturizeCard(CardInfo card)
    {
      cardId = card.id;
      cardName = card.name;
      cachePath = Application.persistentDataPath + "/ImageCache/";
      fileExtension = PlayerManager.Instance.serverImageFileExtension;
      string set = card.set;
      // Use old border mask if card is of the Alpha set
      if (card.set == "LEA")
      {
        setOldBorder();
      }
      else
      {
        setNewBorder();
      }
      // Check if image already exists in cache
      if (File.Exists(cachePath + cardId + fileExtension))
      {
        Texture2D cardTexture = loadImageFromCache(cardId);
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      }
      else // If not in cache, proceed to download from server
      {
        // Set fallback image
        Texture2D cardTexture = Resources.Load("Images/cardFallback") as Texture2D;
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        string serverUrl = PlayerManager.Instance.serverUrl + set + "/";
        StartCoroutine(fetchCardFromServer(serverUrl, cardId));
      }
      // MDFC back side
      if (card.backId != "" && card.backId != null && fullImageBack)
      {
        if (File.Exists(cachePath + card.backId + fileExtension))
        {
          Texture2D cardTexture = loadImageFromCache(card.backId);
          fullImageBack.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
          Texture2D cardTexture = Resources.Load("Images/cardFallback") as Texture2D;
          fullImageBack.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
          string serverUrl = PlayerManager.Instance.serverUrl + set + "/";
          StartCoroutine(fetchBackCardFromServer(serverUrl, card.backId));
        }
      }
    }

    public void showFront()
    {
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      // Check if image already exists in cache
      if (File.Exists(cachePath + cardId + fileExtension))
      {
        Texture2D cardTexture = loadImageFromCache(cardId);
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      }
      else // If not in cache, proceed to download from server
      {
        // Set fallback image
        Texture2D cardTexture = Resources.Load("Images/cardFallback") as Texture2D;
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        string serverUrl = PlayerManager.Instance.serverUrl + card.set + "/";
        StartCoroutine(fetchCardFromServer(serverUrl, cardId));
      }
    }

    public void showBack()
    {
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      if (card.text.Contains("Morph"))
      {
        Texture2D cardTexture = Resources.Load("Images/WebCardDefault") as Texture2D;
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        return;
      }
      // Check if image already exists in cache
      if (File.Exists(cachePath + card.backId + fileExtension))
      {
        Texture2D cardTexture = loadImageFromCache(card.backId);
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      }
      else // If not in cache, proceed to download from server
      {
        // Set fallback image
        Texture2D cardTexture = Resources.Load("Images/cardFallback") as Texture2D;
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        fullImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        string serverUrl = PlayerManager.Instance.serverUrl + card.set + "/";
        StartCoroutine(fetchCardFromServer(serverUrl, card.backId));
      }
    }

    // Save image to cache
    private void saveImageToCache(Texture2D texture, string id) {
      string fileName = id + fileExtension;
      string savePath = cachePath;
      try {
        // If directory does not exist, create it
        if (!Directory.Exists(savePath)) {
          Directory.CreateDirectory(savePath);
        }
        File.WriteAllBytes(savePath + fileName, texture.EncodeToJPG(100));
      } catch (Exception e) {
        Debug.Log(e.Message);
      }
    }

    // Load image from cache
    private Texture2D loadImageFromCache(string id)
    {
      Texture2D texture = null;
      byte[] fileData;
      string fileName = id + fileExtension;
      string filePath = cachePath + fileName;

      if (File.Exists(filePath))     {
        fileData = File.ReadAllBytes(filePath);
        texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.LoadImage(fileData);
      }

      return texture;
    }

    // If image is Alpha set utilize the old border as mask
    private void setOldBorder()
    {
      Texture2D cardTexture = Resources.Load("Images/OldBorder") as Texture2D;
      maskImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      fullMask.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      if (fullMaskBack)
      {
        fullMaskBack.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      }
    }

    // If image is Alpha set utilize the old border as mask
    private void setNewBorder()
    {
      Texture2D cardTexture = Resources.Load("Images/NewBorder") as Texture2D;
      maskImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      fullMask.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      if (fullMaskBack)
      {
        fullMaskBack.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      }
    }

    // Make the card transparent
    public void makeTransparent()
    {
      /*
      Texture2D cardTexture = Resources.Load("Images/CardTransparent") as Texture2D;
      maskImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      */
      gameObject.SetActive(false);
    }

    public void makeVisible()
    {
      gameObject.SetActive(true);
    }

    // Enable/Disable the full sized card
    public void showFullSize()
    {
      fullMaskObject.SetActive(true);
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      if (card.layout != "split" && card.backId != "" && card.backId != null)
      {
        fullMaskBackObject.SetActive(true);
        if (card.layout == "flip")
        {
          fullMaskBackObject.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
          fullMaskBackObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
      }
      else
      {
        fullMaskBackObject.SetActive(false);
      }
    }

    public void hideFullSize()
    {
      fullMaskObject.SetActive(false);
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      if (card.layout != "split" && card.backId != "" && card.backId != null)
      {
        fullMaskBackObject.SetActive(false);
      }
    }

    public void bringToFront()
    {
      GetComponent<Canvas>().overrideSorting = true;
    }

    public void sendToBack()
    {
      GetComponent<Canvas>().overrideSorting = false;
    }

    public void highlightInHand()
    {
      bringToFront();
      transform.localPosition = new Vector3(transform.localPosition.x, 182f, transform.localPosition.z);
    }

    public void unHighlightInHand()
    {
      sendToBack();
      transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
    }
}
