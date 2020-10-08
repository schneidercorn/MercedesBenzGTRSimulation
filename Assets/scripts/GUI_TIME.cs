using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_TIME : MonoBehaviour
{
    public GUI_LAPS lapController;

    public float time;
    public float minutes;
    public float seconds;

    public bool gameRunning = true;

    void Start()
    {
        time = 0;
    }

    private string floatToIntStr(float f)
    {
        return Mathf.FloorToInt(f).ToString();
    }

    IEnumerator flashHelper()
    {
        this.GetComponent<Text>().enabled = !this.GetComponent<Text>().enabled;
        yield return new WaitForSeconds(.8f);
        flashText();
    }

    public void flashText()
    {
        StartCoroutine(flashHelper());
    }

    void FixedUpdate()
    {
        if (gameRunning == true)
        {
            time += Time.deltaTime;

            minutes = time / 60;
            seconds = time % 60;

            this.GetComponent<Text>().text = ((minutes > 10f) ? floatToIntStr(minutes) : ("0" + floatToIntStr(minutes))) + ":" + ((seconds > 10f) ? floatToIntStr(seconds) : ("0" + floatToIntStr(seconds)));
        }

        if (gameRunning == false)
        {
            this.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            this.GetComponent<Text>().text = ((minutes > 10f) ? floatToIntStr(minutes) : ("0" + floatToIntStr(minutes))) + ":" + ((seconds > 10f) ? floatToIntStr(seconds) : ("0" + floatToIntStr(seconds)));
        }
    }
}
