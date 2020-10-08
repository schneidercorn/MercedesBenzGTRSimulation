using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_RPM : MonoBehaviour
{
    public carController car;

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = "RPM: " + car.RPMDisplay.ToString();
    }
}
