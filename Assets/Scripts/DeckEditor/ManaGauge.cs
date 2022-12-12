using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaGauge : MonoBehaviour
{
    public GameObject fillObject;
    public GameObject bgObject;
    private RectTransform fill;
    private float gaugeHeight;

    // Start is called before the first frame update
    void Start()
    {
        fill = fillObject.GetComponent<RectTransform>();
        fill.sizeDelta = new Vector2(fill.sizeDelta.x, 0);
        gaugeHeight = bgObject.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Sets the % of fill in this gauge
    public void setGaugePercent(float fillPercent)
    {
      fill.sizeDelta = new Vector2(fill.sizeDelta.x, gaugeHeight * fillPercent);
    }
}
