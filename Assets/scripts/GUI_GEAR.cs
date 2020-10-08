using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_GEAR : MonoBehaviour
{
    public carController car;

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = "Gear: " + car.gear.ToString();
    }
}
