using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    public GameObject focus;
    public float distance = 5f;
    public float height = 2f;
    public float dampening = 1f;

    public float rotationSpeed = 10f;
    public string setting = "follow";

    void followCar()
    {
        transform.position = Vector3.Lerp(transform.position, focus.transform.position + focus.transform.TransformDirection(new Vector3(0f, height, -distance)), dampening * Time.deltaTime);
    }

    void turntable()
    {
        Vector3 current = focus.transform.position;
        transform.Rotate(current, rotationSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Lerp() interpolates between two vectors (vector1, vector2, time)
        // -distance because behind car
        // TransformDistance adjusts for car coordinates
        // for smoother movements
        switch (setting)
        {
            case "follow" :
                followCar();
                break;

            case "turntable":
                turntable();
                break;
        }
        

      // LookAt() rotates transform to vector
      transform.LookAt(focus.transform);
    }
}
