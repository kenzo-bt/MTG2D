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
          GemsCounterObject.GetComponent<TMP_Text>().text = "" + myCurrencies.gems;
          CoinsCounterObject.GetComponent<TMP_Text>().text = "" + myCurrencies.coins;
          PlayerManager.Instance.lastGemAmount = myCurrencies.gems;
          PlayerManager.Instance.lastCoinAmount = myCurrencies.coins;
        }
      }
    }
}
