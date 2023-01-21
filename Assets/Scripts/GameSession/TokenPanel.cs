using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenPanel : MonoBehaviour
{
    public GameObject searchObject;
    public GameObject tokenBrowser;
    public GameObject cardPrefab;
    public GameObject tokenAmount;
    public GameObject errorMessage;
    public GameObject player;
    public List<string> tokenIds;
    public List<string> filteredTokens;
    public static List<string> searchKeywords;
    // Start is called before the first frame update
    void Start()
    {
      tokenIds = new List<string>();
      filteredTokens = new List<string>();
      searchKeywords = new List<string>();
      loadTokens();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadTokens()
    {
      foreach (CardSet set in PlayerManager.Instance.cardCollection)
      {
        foreach (CardInfo card in set.cards)
        {
          // Dont include DFC backside
          if (card.isToken)
          {
            tokenIds.Add(card.id);
          }
        }
      }
    }

    public void filterSearch()
    {
      Debug.Log("Tokens: " + tokenIds.Count);
      string searchText = searchObject.GetComponent<TMP_InputField>().text;
      searchKeywords = new List<string>(searchText.Split(','));
      if (searchText != "")
      {
        filteredTokens = new List<string>(tokenIds.FindAll(matchesSearchText));
      }

      foreach (string id in filteredTokens)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(id);
        Debug.Log("Token name: " + card.name + " / ID: " + card.id);
      }

      // Eliminate all browser children
      int numChildren = tokenBrowser.transform.childCount;
      for (int i = 0; i < numChildren; i++)
      {
        DestroyImmediate(tokenBrowser.transform.GetChild(0).gameObject);
      }
      // Populate browser with filtered Tokens
      foreach (string id in filteredTokens)
      {
        CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
        GameObject token = Instantiate(cardPrefab, tokenBrowser.transform);
        token.GetComponent<WebCard>().texturizeCard(targetCard);
      }
    }

    private static bool matchesSearchText(string id)
    {
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
      foreach (string keyword in searchKeywords)
      {
        if (!targetCard.text.ToLower().Contains(keyword.ToLower()) && !targetCard.name.ToLower().Contains(keyword.ToLower()))
        {
          return false;
        }
      }
      return true;
    }

    public void processTokenSelection()
    {
      // Determine selected token from the browser
      string selectedId = "";
      foreach (Transform tokenCard in tokenBrowser.transform)
      {
        if (tokenCard.gameObject.GetComponent<BrowserToken>().selected)
        {
          selectedId = tokenCard.gameObject.GetComponent<WebCard>().cardId;
          break;
        }
      }
      if (selectedId == "")
      {
        errorMessage.GetComponent<TMP_Text>().text = "Please select a token";
        return;
      }

      // Determine how many tokens to be created
      string inputAmount = tokenAmount.GetComponent<TMP_InputField>().text;
      try
      {
        int numTokens = Int32.Parse(inputAmount);
        if (numTokens >= 0)
        {
          // Generate X tokens in the battlefield
          CardInfo selectedToken = PlayerManager.Instance.getCardFromLookup(selectedId);
          for (int i = 0; i < numTokens; i++)
          {
            player.GetComponent<Player>().addCardToBattlefield(selectedToken);
          }

          errorMessage.GetComponent<TMP_Text>().text = "";
          tokenAmount.GetComponent<TMP_InputField>().text = "";
          hideTokenPanel();
        }
        else
        {
          errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer.";
        }
      }
      catch (FormatException)
      {
        errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer.";
      }
    }

    public void showTokenPanel()
    {
      GetComponent<CanvasGroup>().alpha = 1;
      GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void hideTokenPanel()
    {
      GetComponent<CanvasGroup>().alpha = 0;
      GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void selectTokenAtIndex(int index)
    {
      foreach (Transform tokenCard in tokenBrowser.transform)
      {
        tokenCard.gameObject.GetComponent<BrowserToken>().deselectToken();
      }
      tokenBrowser.transform.GetChild(index).gameObject.GetComponent<BrowserToken>().selectToken();
    }
}
