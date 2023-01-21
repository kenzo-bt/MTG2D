using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrowserToken : MonoBehaviour
{
    public GameObject selectedHalo;
    public bool selected;
    // Start is called before the first frame update
    void Awake()
    {
      selected = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void selectToken()
    {
      selectedHalo.SetActive(true);
      selected = true;
    }

    public void deselectToken()
    {
      selectedHalo.SetActive(false);
      selected = false;
    }

    public void handleClick()
    {
      transform.parent.parent.parent.gameObject.GetComponent<TokenPanel>().selectTokenAtIndex(transform.GetSiblingIndex());
    }
}
