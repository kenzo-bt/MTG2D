using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningDisplay : MonoBehaviour
{
    public GameObject blackFadeObject;
    public GameObject canvasGroupingObject;
    public GameObject cardContainer;
    public GameObject cardPrefab;
    public GameObject currencyPanel;
    public GameObject backToStoreButton;
    private string boosterSetCode;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator fadeToBlack()
    {
      yield return blackFadeObject.GetComponent<ImageFade>().FadeToFullAlpha(1f);
      yield return new WaitForSeconds(0.5f);
      canvasGroupingObject.GetComponent<CanvasGroup>().alpha = 1;
      canvasGroupingObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
      yield return showCardsAnimation();
    }

    private IEnumerator fadeToZero()
    {
      yield return hideCardsAnimation();
      yield return new WaitForSeconds(1);
      canvasGroupingObject.GetComponent<CanvasGroup>().alpha = 0;
      canvasGroupingObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
      yield return blackFadeObject.GetComponent<ImageFade>().FadeToZeroAlpha(1f);
      gameObject.SetActive(false);
    }

    private IEnumerator showCardsAnimation() {
      // Show cards one by one with a delay
      yield return new WaitForSeconds(1);
      int numInstances = cardContainer.transform.childCount;
      for (int i = 0; i < numInstances; i++)
      {
        GameObject cardObject = cardContainer.transform.GetChild(i).gameObject;
        float fadeTime = 0.5f;
        if (i == (numInstances - 1))
        {
          fadeTime = 1.5f;
        }
        else if (i >= (numInstances - 4))
        {
          fadeTime = 1f;
        }
        yield return cardObject.GetComponent<CanvasGroupFade>().FadeToFullAlpha(fadeTime);
        StartCoroutine(cardObject.GetComponent<OpeningDisplayCard>().shrink(3f));
        if (i == (numInstances - 2))
        {
          yield return new WaitForSeconds(1.0f);
        }
        else if (i >= (numInstances - 5))
        {
          yield return new WaitForSeconds(0.5f);
        }
      }
      backToStoreButton.GetComponent<CanvasGroup>().alpha = 1;
      backToStoreButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    private IEnumerator reduceContainerSpacing(float time)
    {
      HorizontalLayoutGroup containerLayout = cardContainer.GetComponent<HorizontalLayoutGroup>();
      float targetSpacing = -210f;
      while (containerLayout.spacing > targetSpacing)
      {
          containerLayout.spacing = containerLayout.spacing - (Time.deltaTime / time);
          yield return null;
      }
    }

    private IEnumerator hideCardsAnimation() {
      // Show cards one by one with a delay
      yield return new WaitForSeconds(0.1f);
      int numInstances = cardContainer.transform.childCount;
      for (int i = (numInstances - 1); i >= 0; i--)
      {
        GameObject cardObject = cardContainer.transform.GetChild(i).gameObject;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(cardObject.GetComponent<CanvasGroupFade>().FadeToZeroAlpha(0.5f));
      }
    }

    private IEnumerator updateDuplicateCoins(int numCoins)
    {
      yield return PlayerManager.Instance.addPlayerCurrenciesInServer(numCoins, 0);
      currencyPanel.GetComponent<CurrencyPanel>().setCurrency();
    }

    public void openPack(string setCode) {
      cardContainer.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
      cardContainer.GetComponent<HorizontalLayoutGroup>().spacing = -200;
      boosterSetCode = setCode;
      show();
      // Cleanup
      int numInstances = cardContainer.transform.childCount;
      for (int i = 0; i < numInstances; i++)
      {
        DestroyImmediate(cardContainer.transform.GetChild(0).gameObject);
      }
      // Open pack
      foreach (CardSet set in PlayerManager.Instance.cardCollection)
      {
        if (set.setCode == setCode)
        {
          Pack pack = new Pack();
          pack.cards = new List<string>(set.getPack());
          Dictionary<string, int> playerCollection = PlayerManager.Instance.collectedCards;
          int duplicateCoins = 0;
          for (int i = (pack.cards.Count - 1); i >= 0; i--)
          {
            CardInfo card = PlayerManager.Instance.getCardFromLookup(pack.cards[i]);
            // Instantiate the cards
            GameObject cardInstance = Instantiate(cardPrefab, cardContainer.transform);
            // Set the target scale (smallest first) [1 -> 1.5]
            float decrementStep = 0.5f / (pack.cards.Count - 1);
            float targetScale = 1.5f - (decrementStep  * i);
            cardInstance.GetComponent<OpeningDisplayCard>().targetScale = targetScale;
            // TODO : Adhere to scryfall request limits (10/s)
            cardInstance.GetComponent<WebCard>().texturizeCard(PlayerManager.Instance.getCardFromLookup(card.id));
            // Determine duplicate credits
            if (playerCollection.ContainsKey(card.id))
            {
              int numCoins = 0;
              if (card.rarity == "common")
              {
                numCoins = 1;
                duplicateCoins += 1;
              }
              else if (card.rarity == "uncommon")
              {
                numCoins = 5;
                duplicateCoins += 5;
              }
              else
              {
                numCoins = 10;
                duplicateCoins += 10;
              }
              cardInstance.GetComponent<OpeningDisplayCard>().showDuplicateCoins(numCoins);
            }
            else
            {
              // Add to collection
              playerCollection.Add(card.id, 1);
            }
          }
          // Update collection in server
          StartCoroutine(PlayerManager.Instance.addPackToPlayerCollectionInServer(pack));
          // Update currency in server
          StartCoroutine(updateDuplicateCoins(duplicateCoins));
          break;
        }
      }
    }

    public void openColourPack(string colour)
    {
      cardContainer.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
      cardContainer.GetComponent<HorizontalLayoutGroup>().spacing = -200;
      show();
      // Cleanup
      int numInstances = cardContainer.transform.childCount;
      for (int i = 0; i < numInstances; i++)
      {
        DestroyImmediate(cardContainer.transform.GetChild(0).gameObject);
      }
      // Open pack
      Pack pack = PlayerManager.Instance.getColourPack(colour);
      Dictionary<string, int> playerCollection = PlayerManager.Instance.collectedCards;
      int duplicateCoins = 0;
      for (int i = (pack.cards.Count - 1); i >= 0; i--)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(pack.cards[i]);
        // Instantiate the cards
        GameObject cardInstance = Instantiate(cardPrefab, cardContainer.transform);
        // Set the target scale (smallest first) [1 -> 1.5]
        float decrementStep = 0.5f / (pack.cards.Count - 1);
        float targetScale = 1.5f - (decrementStep  * i);
        cardInstance.GetComponent<OpeningDisplayCard>().targetScale = targetScale;
        // TODO : Adhere to scryfall request limits (10/s)
        cardInstance.GetComponent<WebCard>().texturizeCard(PlayerManager.Instance.getCardFromLookup(card.id));
        // Determine duplicate credits
        if (playerCollection.ContainsKey(card.id))
        {
          int numCoins = 0;
          if (card.rarity == "common")
          {
            numCoins = 1;
            duplicateCoins += 1;
          }
          else if (card.rarity == "uncommon")
          {
            numCoins = 5;
            duplicateCoins += 5;
          }
          else
          {
            numCoins = 10;
            duplicateCoins += 10;
          }
          cardInstance.GetComponent<OpeningDisplayCard>().showDuplicateCoins(numCoins);
        }
        else
        {
          // Add to collection
          playerCollection.Add(card.id, 1);
        }
      }
      // Update collection in server
      StartCoroutine(PlayerManager.Instance.addPackToPlayerCollectionInServer(pack));
      // Update currency in server
      StartCoroutine(updateDuplicateCoins(duplicateCoins));
    }

    public void openLegendaryPack()
    {
      cardContainer.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
      cardContainer.GetComponent<HorizontalLayoutGroup>().spacing = 200;
      show();
      // Cleanup
      int numInstances = cardContainer.transform.childCount;
      for (int i = 0; i < numInstances; i++)
      {
        DestroyImmediate(cardContainer.transform.GetChild(0).gameObject);
      }
      // Open pack
      Pack pack = PlayerManager.Instance.getLegendaryPack();
      Dictionary<string, int> playerCollection = PlayerManager.Instance.collectedCards;
      int duplicateCoins = 0;
      for (int i = (pack.cards.Count - 1); i >= 0; i--)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(pack.cards[i]);
        // Instantiate the cards
        GameObject cardInstance = Instantiate(cardPrefab, cardContainer.transform);
        float targetScale = 1.4f;
        cardInstance.GetComponent<OpeningDisplayCard>().targetScale = targetScale;
        // TODO : Adhere to scryfall request limits (10/s)
        cardInstance.GetComponent<WebCard>().texturizeCard(PlayerManager.Instance.getCardFromLookup(card.id));
        // Determine duplicate credits
        if (playerCollection.ContainsKey(card.id))
        {
          int numCoins = 0;
          if (card.rarity == "common")
          {
            numCoins = 1;
            duplicateCoins += 1;
          }
          else if (card.rarity == "uncommon")
          {
            numCoins = 5;
            duplicateCoins += 5;
          }
          else
          {
            numCoins = 10;
            duplicateCoins += 10;
          }
          cardInstance.GetComponent<OpeningDisplayCard>().showDuplicateCoins(numCoins);
        }
        else
        {
          // Add to collection
          playerCollection.Add(card.id, 1);
        }
      }
      // Update collection in server
      StartCoroutine(PlayerManager.Instance.addPackToPlayerCollectionInServer(pack));
      // Update currency in server
      StartCoroutine(updateDuplicateCoins(duplicateCoins));
    }

    // Show / Hide overlay
    public void hide()
    {
      backToStoreButton.GetComponent<CanvasGroup>().alpha = 0;
      backToStoreButton.GetComponent<CanvasGroup>().blocksRaycasts = false;
      if (gameObject.activeSelf)
      {
        StartCoroutine(fadeToZero());
      }
    }

    private void show()
    {
      gameObject.SetActive(true);
      StartCoroutine(fadeToBlack());
    }
}
