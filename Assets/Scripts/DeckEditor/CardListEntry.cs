using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardListEntry : MonoBehaviour
{
    private string cardName;
    private int quantity;
    public GameObject nameObject;
    public GameObject quantityObject;
    private TMP_Text nameText;
    private TMP_Text quantityText;

    // Start is called before the first frame update
    void Awake()
    {
        cardName = "";
        quantity = 0;
        nameText = nameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        quantityText = quantityObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set the name and quantity properties
    public void setValues(string name, int num)
    {
      cardName = name;
      quantity = num;
      updateEntry();
    }

    // Pass name and quantity to the game objects
    public void updateEntry()
    {
      nameText.text = cardName;
      quantityText.text = quantity + "x";
    }
}
