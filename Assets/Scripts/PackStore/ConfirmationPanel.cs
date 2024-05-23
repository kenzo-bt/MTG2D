using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class ConfirmationPanel : MonoBehaviour
{
    public GameObject boosterNameObject;
    public GameObject boosterImageObject;
    public GameObject gemValueObject;
    public GameObject coinValueObject;
    public GameObject errorTextObject;
    public GameObject currencyPanelObject;
    public GameObject openingDisplayObject;
    private string boosterSetCode;
    private int gemCost;
    private int coinCost;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void openPanel(string encodedString)
    {
      string[] parameters = encodedString.Split('-');
      boosterSetCode = parameters[0];
      gemCost = 1;
      coinCost = 50;
      if (parameters.Length > 1)
      {
        gemCost = Int32.Parse(parameters[1]);
        coinCost = Int32.Parse(parameters[2]);
      }
      setBooster();
      show();
    }

    private void setBooster()
    {
      // Determine full set name
      string expansionName = "";
      foreach (CardSet set in PlayerManager.Instance.cardCollection)
      {
        if (set.setCode == boosterSetCode)
        {
          expansionName = set.setName;
          break;
        }
      }
      boosterNameObject.GetComponent<TMP_Text>().text = expansionName;
      // Set booster image
      Image boosterImage = boosterImageObject.GetComponent<Image>();
      Texture2D boosterTexture = Resources.Load("Images/Boosters/" + boosterSetCode) as Texture2D;
      boosterImage.sprite = Sprite.Create(boosterTexture, new Rect(0, 0, boosterTexture.width, boosterTexture.height), new Vector2(0.5f, 0.5f));
      // Set booster cost values
      gemValueObject.GetComponent<TMP_Text>().text = "" + gemCost;
      coinValueObject.GetComponent<TMP_Text>().text = "" + coinCost;
    }

    public void purchaseWithCoins()
    {
      errorTextObject.GetComponent<TMP_Text>().text = "";
      StartCoroutine(requestBoosterPurchase("coins", coinCost));
    }

    public void purchaseWithGems()
    {
      errorTextObject.GetComponent<TMP_Text>().text = "";
      StartCoroutine(requestBoosterPurchase("gems", gemCost));
    }

    private IEnumerator requestBoosterPurchase(string currency, int cost)
    {
      string url = PlayerManager.Instance.apiUrl + "store/booster/" + currency + "/" + cost + "/" + PlayerManager.Instance.myID;
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          if (request.responseCode == 400)
          {
            string serverJson = request.downloadHandler.text;
            GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(serverJson);
            errorTextObject.GetComponent<TMP_Text>().text = serverError.Error;
          }
        }
        else
        {
          hide();
          // Refresh currencies
          currencyPanelObject.GetComponent<CurrencyPanel>().setCurrency();
          // Open pack
          openingDisplayObject.GetComponent<OpeningDisplay>().openPack(boosterSetCode);

        }
      }
    }

    // Show / Hide overlay
    public void hide()
    {
      if (gameObject.activeSelf)
      {
        gameObject.SetActive(false);
      }
    }

    public void show()
    {
      gameObject.SetActive(true);
      errorTextObject.GetComponent<TMP_Text>().text = "";
    }
}
