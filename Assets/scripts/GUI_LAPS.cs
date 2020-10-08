using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_LAPS : MonoBehaviour
{
    public int lap = 1;

    void Update()
    {
        this.GetComponentInChildren<Text>().text = lap + " / 3";
    }
}
