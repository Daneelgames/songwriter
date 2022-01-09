using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : Instrument
{

    public enum Wave
    {
        Sine, Square, Triangle
    }

    public enum Octave
    {
        OnlyLow, OnlyNormal, OnlyHigh, Choose
    }

    public Wave waveType = Wave.Sine;
    public Octave octaveMode = Octave.Choose;

    public double frequency = 440.0;
    private double increment;   
    private double phase;   
    private double samplingFreq = 48000.0;

    public float gain;
    public float volume = 0.1f;

    public float[] frequencies;
    public int currentFreq;

    void Start()
    {
        frequencies = new float[22];
        
        // A MAJOR SCALE
        frequencies[0] = 220; // A3
        frequencies[1] = 246.94f; // B3
        frequencies[2] = 277.18f; // C#4
        frequencies[3] = 293.66f; // D4
        frequencies[4] = 329.63f; // E4
        frequencies[5] = 369.99f; // F#4
        frequencies[6] = 415.30f; // G#4
        frequencies[7] = 440; // A4
        frequencies[8] = 493.88f; // B4
        frequencies[9] = 554.37f; // C#5
        frequencies[10] = 587.33f; // D5
        frequencies[11] = 659.25f; // E5
        frequencies[12] = 739.99f; // F#5
        frequencies[13] = 830.61f; // G#5
        frequencies[14] = 880; // A5
        frequencies[15] = 987.77f; // B5
        frequencies[16] = 1108.73f; // C#6
        frequencies[17] = 1174.66f; // D6
        frequencies[18] = 1318.51f; // E6
        frequencies[19] = 1479.98f; // F#6
        frequencies[20] = 1661.22f; // G#6
        frequencies[21] = 1760; // A6
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
        {
            SetFreq(0, 7, 14);
            gain = volume;
        }
        else if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
        {
            SetFreq(1, 8, 15);
            gain = volume;
        }
        else if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
        {
            SetFreq(2, 9, 16);
            gain = volume;
        }
        else if (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4))
        {
            SetFreq(3, 10, 17);
            gain = volume;
        }
        else if (Input.GetKey(KeyCode.Alpha5) || Input.GetKey(KeyCode.Keypad5))
        {
            SetFreq(4, 11, 18);
            gain = volume;
        }
        else if (Input.GetKey(KeyCode.Alpha6) || Input.GetKey(KeyCode.Keypad6))
        {
            SetFreq(5, 12, 19);
            gain = volume;
        }
        else if (Input.GetKey(KeyCode.Alpha7) || Input.GetKey(KeyCode.Keypad7))
        {
            SetFreq(6, 13, 20);
            gain = volume;
        }
        else if (Input.GetKey(KeyCode.Alpha8) || Input.GetKey(KeyCode.Keypad8))
        {
            SetFreq(7, 14, 21);
            gain = volume;
        }
        else
        {
            gain = 0;
        }
    }

    void SetFreq(int lowFreqIndex, int normFreqIndex, int highFreqIndex)
    {
        if (octaveMode == Octave.Choose)
        {
            if (Input.GetKey(KeyCode.X))
                frequency = frequencies[highFreqIndex];
            else if (Input.GetKey(KeyCode.Z))
                frequency = frequencies[lowFreqIndex];
            else
                frequency = frequencies[normFreqIndex];   
        }
        else
        {
            switch (octaveMode)
            {
                case Octave.OnlyHigh:
                    frequency = frequencies[highFreqIndex];
                    break;
                case Octave.OnlyNormal:
                    frequency = frequencies[normFreqIndex];
                    break;
                case Octave.OnlyLow:
                    frequency = frequencies[lowFreqIndex];
                    break;
            }
        }
    }
    
    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / samplingFreq;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            switch (waveType)
            {
             case Wave.Sine:
                 data[i] = (float)(gain * Mathf.Sin((float)phase));    
                 break;
             
             case Wave.Square:
                 if (gain * Mathf.Sin((float)phase) >= 0 * gain)
                 {
                     data[i] = (float)gain * 0.6f;
                 }
                 else
                 {
                     data[i] = -(float)gain * 0.6f;
                 }
                 break;
             
             case Wave.Triangle:
                 data[i] = (float)(gain * (double)Mathf.PingPong((float)phase, 1.0f));
                 break;
            }

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            if (phase > Mathf.PI * 2)
            {
                phase = 0.0;
            }
        }
    }
}
