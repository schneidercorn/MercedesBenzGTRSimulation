using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public carController car;

    public static AudioClip startup, idle, slide;
    static AudioSource audioSrc;

    void Start()
    {
        startup = Resources.Load<AudioClip>("startup");
        slide = Resources.Load<AudioClip>("slide");

        audioSrc = GetComponent<AudioSource>();
    }

    public void checkSlide()
    {
        // if speed is over 20 mph, play sound
        // do this AFTER establishing the mph conversion and GUI for testing
        // but I like the sound so ill keep it...for now
        // do set this to not activate EVERY TIME though
        if (car.horizontalInput != 0)
            playSound("slide");
    }



    void FixedUpdate()
    {
        checkSlide();
    }
    
    public static void playSound (string clip)
    {
        switch (clip)
        {
            case "startup":
                audioSrc.PlayOneShot(startup);
                break;

            case "idle":
                audioSrc.PlayOneShot(idle);
                break;

            case "slide":
                audioSrc.PlayOneShot(slide);
                break;

        }
    }

}
