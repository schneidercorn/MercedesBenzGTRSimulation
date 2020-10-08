using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    // components
    public Rigidbody rb;
    public AudioSource audiosrc;

    // inputs
    public float horizontalInput;
    public float verticalInput;
    private float steerAngle;
    public bool isManual = false;

    // Audio Inputs
    public AudioClip startup;
    public AudioClip loopEngine;

    // wheelColliders and wheelMeshes
    public WheelCollider frontLW, frontRW;
    public WheelCollider rearLW, rearRW;
    public Transform frontLT, frontRT;
    public Transform rearRT, rearLT;
    public float wheelRadius = 0.4f;

    // tail lights
    public GameObject tailLights;
    public GameObject tailLightL;
    public GameObject tailLightR;
    public Material[] tailLightsMat;
    Renderer rendTail;
    Renderer rendHead;
    Renderer rendHigh;

    // head lights and highbeams
    public GameObject headLights;
    public GameObject headLightL;
    public GameObject headLightR;

    public GameObject highbeams;
    public GameObject highbeamL;
    public GameObject highbeamR;
    public Material[] headLightsMat;

    // testing values
    public float maxSteer = 30f;
    public float motorForce = 4f;
    public float maxBrakeTorque = 10f;

    // outputs
    public int speed;
    public int RPM;
    public int gear = 1;
    public float[] gearRatios;
    public bool redline = false;

    // outputs (display)
    public int displaySpeed = 0;
    public float RPMDisplay;

    void Start()
    {
        // set up tail/head lights
        rendTail = tailLights.GetComponent<Renderer>();
        rendTail.enabled = true;
        rendTail.sharedMaterial = tailLightsMat[0];

        rendHead = headLights.GetComponent<Renderer>();
        rendHead.enabled = true;
        rendHead.sharedMaterial = headLightsMat[0];

        rendHigh = highbeams.GetComponent<Renderer>();
        rendHigh.enabled = true;
        rendHigh.sharedMaterial = headLightsMat[0];

        // set up components
        rb = GetComponent<Rigidbody>();
        audiosrc = GetComponent<AudioSource>();

        // start Coroutine(s)
        StartCoroutine(SpeedUpdater());

        // start audio
        audiosrc.clip = startup;
        audiosrc.Play();
        StartCoroutine(startEngineLoop());
    }

    public void GetInput()
    {
        // horizontal is A/D, Left/Right || vertical is W/S, Up/Down
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        // -steerAngle- is a subset of WheelCollider and controls angle of driving
        steerAngle = maxSteer * horizontalInput;
        frontLW.steerAngle = steerAngle;
        frontRW.steerAngle = steerAngle;
    }

    private void Accelerate()
    {
        if (verticalInput > 0 && redline == false)
        {
            frontLW.motorTorque = verticalInput * motorForce * 1000;
            frontRW.motorTorque = verticalInput * motorForce * 1000;
        }


        if (frontLW.rpm > 500)
            frontLW.motorTorque = 0;
        if (frontRW.rpm > 500)
            frontRW.motorTorque = 0;
    }

    private void updateWheelPos()
    {
        // these are using helper functions to control rotation and position to simulate actual
        // movements of the wheel/tire
        helperWheelPos(frontLW, frontLT);
        helperWheelPos(frontRW, frontRT);
        helperWheelPos(rearLW, rearLT);
        helperWheelPos(rearRW, rearRT);
    }

    // WheelCollider will access the
    private void helperWheelPos(WheelCollider collider, Transform transform)
    {
        // -Vector3- will deal with 3D vectors and points || Quaternion deals with rotation
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;


        // -out- modifies parameters and automatically updates the values
        // -GetWorldPose- is a subset of WheelCollider which deals with ground contact
        collider.GetWorldPose(out pos, out quat);

        // now the values are transferred over
        transform.position = pos;
        transform.rotation = quat;
    }

    public void getBrakes()
    {
        if (Input.GetKey(KeyCode.Space) && speed > 0 && gear != 0)
        {
            frontLW.brakeTorque = motorForce * 80;
            frontRW.brakeTorque = motorForce * 80;
            rearLW.brakeTorque = motorForce * 80;
            rearRW.brakeTorque = motorForce * 80;
        }
        else
        {
            frontLW.brakeTorque = 0f;
            frontRW.brakeTorque = 0f;
            rearLW.brakeTorque = 0f;
            rearRW.brakeTorque = 0f;
        }
        if (Input.GetKey(KeyCode.Space) || (gear == 0 && verticalInput < 0))
        {
            rendTail.sharedMaterial = tailLightsMat[1];
            tailLightL.GetComponent<Light>().enabled = true;
            tailLightR.GetComponent<Light>().enabled = true;
        }
        else
        {
            rendTail.sharedMaterial = tailLightsMat[0];
            tailLightL.GetComponent<Light>().enabled = false;
            tailLightR.GetComponent<Light>().enabled = false;
        }
    }

    public void getHeadlights()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            headLightL.GetComponent<Light>().enabled = !headLightL.GetComponent<Light>().enabled;
            headLightR.GetComponent<Light>().enabled = !headLightR.GetComponent<Light>().enabled;

            if (rendHead.sharedMaterial == headLightsMat[1])
                rendHead.sharedMaterial = headLightsMat[0];
            else
                rendHead.sharedMaterial = headLightsMat[1];
        }
    }

    public void getHighBeams()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            highbeamL.GetComponent<Light>().enabled = !highbeamL.GetComponent<Light>().enabled;
            highbeamR.GetComponent<Light>().enabled = !highbeamR.GetComponent<Light>().enabled;

            if (rendHigh.sharedMaterial == headLightsMat[1])
                rendHigh.sharedMaterial = headLightsMat[0];
            else
                rendHigh.sharedMaterial = headLightsMat[1];
        }
    }

    public void gearDealer()
    {
        if (isManual)
        {
            if (Input.GetKeyDown("u"))
                if (gear < (gearRatios.Length))
                    gear++;

            if (Input.GetKeyDown("j"))
                if (gear > 0)
                    gear--;

            if (gear == 1 && speed == 0 && Input.GetKeyDown(KeyCode.J))
                gear = 0;
        }
        else if (!isManual)
        {
            // 1 - 7
            if (RPMDisplay >= 6.6 && gear == 1)
                gear++;
            if (RPMDisplay >= 6.7 && gear == 2)
                gear++;
            if (RPMDisplay >= 6.9 && gear == 3)
                gear++;
            if (RPMDisplay >= 7.1 && gear == 4)
                gear++;
            if (RPMDisplay >= 7.2 && gear == 5)
                gear++;
            if (RPMDisplay >= 7.4 && gear == 6)
                gear++;

            // R
            if (speed == 0)
                gear = 0;
            if (gear == 0 && verticalInput > 0)
                gear++;
        }

        motorForce = 3f * gear * gear;
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && speed <= 3)
            RPMDisplay += 2.5f * Time.deltaTime;
        else
            RPMDisplay = RPM / 10000f;

        // 8.5 is redlined || absolute max RPM -- also makes pitch shifting easier
        if (RPMDisplay > 8.5)
        {
            RPMDisplay = 8.5f;
            redline = true;
            StartCoroutine(redlined());
        }

        // so car doesn't break down
        if (gear > 1 && RPMDisplay < 1)
            gear--;
        if (gear > 2 && RPMDisplay < 3)
            gear--;

        // in reverse
        if (gear == 0 && (verticalInput < 0))
        {
            Debug.Log("working");
            frontLW.motorTorque = -3000; // brake torque same as first gear torque
            frontRW.motorTorque = -3000;
        }

        ValtoAudioPitchShift(RPMDisplay);
    }

    // perform a ~MODIFIED~ affine transformation [0, 8500] to [1, 2]
    public void ValtoAudioPitchShift(float val)
    {
        // implementation in O(1) time | ( QUICK ) |
        audiosrc.pitch = 1 + (val / 11f);
    }


    private IEnumerator SpeedUpdater()
    {
        for (; ; )
        {
            if (displaySpeed < speed)
                displaySpeed++;
            if (displaySpeed > speed)
                displaySpeed--;

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator gearUpdater()
    {
        yield return new WaitForSeconds(.1f);
    }

    private IEnumerator redlined()
    {
        yield return new WaitForSeconds(.2f);
        redline = false;
    }

    private IEnumerator startEngineLoop()
    {
        while (true)
        {
            if (!audiosrc.isPlaying)
            {
                audiosrc.clip = loopEngine;
                audiosrc.Play();
                audiosrc.loop = true;
                StopCoroutine(startEngineLoop());
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void checkCarValues()
    {
        speed = 3 * Mathf.RoundToInt(rb.velocity.magnitude);

        if (gear > 0)
            RPM = Mathf.FloorToInt((rb.velocity.magnitude * 60) * (Mathf.PI * 2 * wheelRadius) * 12f * gearRatios[gear - 1]);
    }

    private void FixedUpdate()
  {
    GetInput();
    Steer();
    Accelerate();
    getBrakes();
    updateWheelPos();
    checkCarValues();
  }

  private void Update()
  {
    getHeadlights();
    getHighBeams();
    gearDealer();
  }
}