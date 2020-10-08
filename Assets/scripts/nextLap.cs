using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextLap : MonoBehaviour
{
    public GUI_LAPS lapController;
    public GUI_TIME timeController;
    public carController car;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "car")
            lapController.lap++;
    }

    void checkGameEnd()
    {
        if (lapController.lap == 4)
            StartCoroutine(endGame());
    }

    IEnumerator endGame()
    {
        lapController.lap = 3;
        timeController.gameRunning = false;

        yield return new WaitForSeconds(1f);
        timeController.flashText();
    }

    void FixedUpdate()
    {
        checkGameEnd();
    }
}
