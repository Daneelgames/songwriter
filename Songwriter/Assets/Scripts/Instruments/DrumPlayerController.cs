using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumPlayerController : Instrument
{
    
    public AudioSource kickSource;
    public AudioSource snareSource;
    public AudioSource hatSource;
    public AudioSource crashSource;
    void Update()
    {
        // KICK
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            kickSource.Play();
            BpmManager.Instance.SaveNoteToPianoRoll(0, _InstrumentType);
        }
        
        // SNARE
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            snareSource.Play();
            BpmManager.Instance.SaveNoteToPianoRoll(1, _InstrumentType);
        }
        
        // HAT
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            hatSource.Play();
            BpmManager.Instance.SaveNoteToPianoRoll(2, _InstrumentType);
        }
        
        // CRASH
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            crashSource.Play();
            BpmManager.Instance.SaveNoteToPianoRoll(3, _InstrumentType);
        }
    }

    public void PlayDrumNote(int note)
    {
        switch (note)
        {
            case 0:
                kickSource.Play();
                break;
            case 1:
                snareSource.Play();
                break;
            case 2:
                hatSource.Play();
                break;
            case 3:
                crashSource.Play();
                break;
        }
    }
}