using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaGauge : MonoBehaviour
{
    public GameObject fillObject;
    public GameObject bgObject;
    private RectTransform gauge;
    private RectTransform fill;
    private RectTransform bg;
    private float gaugeHeight;

    // Start is called before the first frame update
    void Awake()
    {
        gauge = GetComponent<RectTransform>();
        fill = fillObject.GetComponent<RectTransform>();
        bg = bgObject.GetComponent<RectTransform>();
        // Set bg/fill width and initial height
        fill.sizeDelta = new Vector2(fill.sizeDelta.x, 0);
        gaugeHeight = gauge.sizeDelta.y - 1;
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

    // Get the % of fill as a float
    public float getGaugePercent()
    {
      return (fill.sizeDelta.y / gaugeHeight);
    }
}
