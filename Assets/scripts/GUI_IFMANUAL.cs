using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_IFMANUAL : MonoBehaviour
{
    public carController car;

    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(toggleManual);
    }

    void toggleManual()
    {
        car.isManual = !car.isManual;
    }

    void Update()
    {
        if (car.isManual)
            this.GetComponentInChildren<Text>().text = "Manual";
        else
            this.GetComponentInChildren<Text>().text = "Automatic";
    }
}
