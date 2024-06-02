using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CurrencyPanel : MonoBehaviour
{
    public GameObject GemsCounterObject;
    public GameObject CoinsCounterObject;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerManager.Instance.loggedIn)
        {
          GemsCounterObject.GetComponent<TMP_Text>().text = "" + PlayerManager.Instance.lastGemAmount;
          CoinsCounterObject.GetComponent<TMP_Text>().text = "" + PlayerManager.Instance.lastCoinAmount;
          setCurrency();
        }
        else {
          GemsCounterObject.GetComponent<TMP_Text>().text = "";
          CoinsCounterObject.GetComponent<TMP_Text>().text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setCurrency()
    {
      StartCoroutine(fetchCurrenciesFromServer());
    }

    public IEnumerator fetchCurrenciesFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/currency";
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else
        {
          string serverJson = request.downloadHandler.text;
          PlayerCurrencies myCurrencies = JsonUtility.FromJson<PlayerCurrencies>(serverJson);
          /*
            GemsCounterObject.GetComponent<TMP_Text>().text = "" + myCurrencies.gems;
            CoinsCounterObject.GetComponent<TMP_Text>().text = "" + myCurrencies.coins;
            PlayerManager.Instance.lastGemAmount = myCurrencies.gems;
            PlayerManager.Instance.lastCoinAmount = myCurrencies.coins;
          */
          StartCoroutine(currencyGradualAdjust(myCurrencies.gems, PlayerManager.Instance.lastGemAmount, GemsCounterObject.GetComponent<TMP_Text>(), "gems"));
          StartCoroutine(currencyGradualAdjust(myCurrencies.coins, PlayerManager.Instance.lastCoinAmount, CoinsCounterObject.GetComponent<TMP_Text>(), "coins"));
        }
      }
    }

    public IEnumerator currencyGradualAdjust(int updatedCurrency, int previousCurrency, TMP_Text textObject, string currencyType)
    {
      double currencyDouble = Convert.ToDouble(previousCurrency);
      int difference = Math.Abs(updatedCurrency - previousCurrency);
      double increment = Convert.ToDouble(difference / (1f / Time.deltaTime));
      float totalTime = 0f;
      if (updatedCurrency > previousCurrency)
      {
        while (Math.Floor(currencyDouble) < updatedCurrency)
        {
          currencyDouble += increment;
          textObject.text = "" + Math.Floor(currencyDouble);
          totalTime += Time.deltaTime;
          yield return null;
        }
      }
      else if (updatedCurrency < previousCurrency)
      {
        while (Math.Floor(currencyDouble) > updatedCurrency)
        {
          currencyDouble -= increment;
          textObject.text = "" + Math.Floor(currencyDouble);
          totalTime += Time.deltaTime;
          yield return null;
        }
      }
      textObject.text = "" + updatedCurrency;
      if (currencyType == "gems")
      {
        PlayerManager.Instance.lastGemAmount = updatedCurrency;
      }
      else
      {
        PlayerManager.Instance.lastCoinAmount = updatedCurrency;
      }
    }
}
