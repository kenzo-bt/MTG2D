using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WebCard : MonoBehaviour
{
    private string cardName;
    private string cachePath;
    private string fileExtension;

    void Awake()
    {

    }

    IEnumerator fetchCardFromServer(string serverUrl, string cardId)
    {
      Image cardImage = GetComponent<Image>();
      UnityWebRequest request = UnityWebRequestTexture.GetTexture(serverUrl + cardId + fileExtension);
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
        // Set fallback image
        Texture2D cardTexture = Resources.Load("Images/cardFallback.png") as Texture2D;
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      }
      else
      {
        Texture2D cardTexture = ((DownloadHandlerTexture) request.downloadHandler).texture as Texture2D;
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
        saveImageToCache(cardTexture, cardId);
      }
    }

    // Apply texture to card
    public void texturizeCard(CardInfo card)
    {
      cachePath = Application.persistentDataPath + "/ImageCache/";
      fileExtension = PlayerManager.Instance.serverImageFileExtension;
      string cardId = card.id;
      string set = card.set;
      // Check if image already exists in cache
      if (File.Exists(cachePath + cardId + fileExtension))
      {
        Image cardImage = GetComponent<Image>();
        Texture2D cardTexture = loadImageFromCache(cardId);
        cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
      }
      else // If not in cache, proceed to download from server
      {
        string serverUrl = PlayerManager.Instance.serverUrl + set + "/";
        StartCoroutine(fetchCardFromServer(serverUrl, cardId));
      }
    }

    // Save image to cache
    private void saveImageToCache(Texture2D texture, string id) {
      Debug.Log("Saving image to cache...");
      string fileName = id + fileExtension;
      string savePath = cachePath;
      try {
        // If directory does not exist, create it
        if (!Directory.Exists(savePath)) {
          Directory.CreateDirectory(savePath);
        }
        File.WriteAllBytes(savePath + fileName, texture.EncodeToJPG());
      } catch (Exception e) {
        Debug.Log(e.Message);
      }
    }

    // Load image from cache
    private Texture2D loadImageFromCache(string id)
    {
      Debug.Log("Loading image from cache...");
      Texture2D texture = null;
      byte[] fileData;
      string fileName = id + fileExtension;
      string filePath = cachePath + fileName;

      if (File.Exists(filePath))     {
        fileData = File.ReadAllBytes(filePath);
        texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
      }
      return texture;
    }
}
