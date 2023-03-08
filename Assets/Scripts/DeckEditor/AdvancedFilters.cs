using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedFilters : MonoBehaviour
{
    public GameObject cardCollectionObject;
    private RectTransform rect;
    private Vector3 targetPosition;
    private float speed;
    private float hiddenPos;
    private float shownPos;
    private List<string> rarities;
    private List<int> manaFilters;
    private List<string> colours;
    private List<string> types;
    private List<string> supertypes;
    private List<string> sets;

    // Start is called before the first frame update
    void Start()
    {
      speed = 1000f;
      rect = GetComponent<RectTransform>();
      hiddenPos = rect.localPosition.y;
      shownPos = rect.localPosition.y + rect.sizeDelta.y;
      rarities = new List<string>();
      manaFilters = new List<int>();
      colours = new List<string>();
      types = new List<string>();
      supertypes = new List<string>();
      sets = new List<string>();
      hide();
    }

    // Update is called once per frame
    void Update()
    {
      var step =  speed * Time.deltaTime; // calculate distance to move
      transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step);
    }

    public void show()
    {
      targetPosition = new Vector3(rect.localPosition.x, shownPos, rect.localPosition.z);
    }

    public void hide()
    {
      targetPosition = new Vector3(rect.localPosition.x, hiddenPos, rect.localPosition.z);
    }

    public void toggleMana(int value)
    {
      if (manaFilters.Contains(value))
      {
        manaFilters.Remove(value);
      }
      else {
        manaFilters.Add(value);
      }
    }

    public void toggleRarity(string rarity)
    {
      if (rarities.Contains(rarity))
      {
        rarities.Remove(rarity);
      }
      else {
        rarities.Add(rarity);
      }
    }

    public void toggleColour(string colour)
    {
      if (colours.Contains(colour))
      {
        colours.Remove(colour);
      }
      else {
        colours.Add(colour);
      }
    }

    public void toggleType(string type)
    {
      if (types.Contains(type))
      {
        types.Remove(type);
      }
      else {
        types.Add(type);
      }
    }

    public void toggleSupertype(string type)
    {
      if (supertypes.Contains(type))
      {
        supertypes.Remove(type);
      }
      else {
        supertypes.Add(type);
      }
    }

    public void toggleSet(string set)
    {
      if (sets.Contains(set))
      {
        sets.Remove(set);
      }
      else {
        sets.Add(set);
      }
    }

    public void sendFiltersToCollection()
    {
      CardCollection collection = cardCollectionObject.GetComponent<CardCollection>();
      collection.addRarities(string.Join(",", rarities));
      collection.addManaValues(string.Join(",", manaFilters));
      collection.addColours(string.Join(",", colours));
      collection.addTypes(string.Join(",", types));
      collection.addSupertypes(string.Join(",", supertypes));
      collection.addSets(string.Join(",", sets));
      collection.filterCollection();
    }
}
