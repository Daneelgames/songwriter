using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public enum Note
{
    E, F, Fd, G, Gd, A, Ad, B, C, Cd, D, Dd 
}

public class BpmManager : MonoBehaviour
{
    public enum Quantize
    {
        four, eight, sixteen, thirtyTwo
    }

    public static BpmManager Instance;
    public RecorderCanvasManager RecorderCanvasManager;
    
    [Range(40,260)]public int tempo = 120;
    public Quantize quantizeTo = Quantize.sixteen;

    [Header("Beat")]
    public Vector2 timeSignature = new Vector2(4, 4);
    public int currentBeat = 0;
    public int currentBar = 0;
    public List<Bar> Bars = new List<Bar>(4);
    [Range(0, 31)]public int currentTimestampInBar = 0;
    
    [Header("Click")]
    public AudioSource clickAu;
    public AudioClip clickMainFx;
    public AudioClip clickFx;

    [Header("Instruments")] 
    public DrumPlayerController drums;
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RecorderCanvasManager.Initialize(Bars);
        RecorderCanvasManager.UpdateCurrentQuantization(quantizeTo);
        PlayOnBarFromStart(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeTempo(5);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeTempo(-5);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var newBar = currentBar;
            newBar--;
            PlayOnBarFromStart(newBar);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var newBar = currentBar;
            newBar++;
            PlayOnBarFromStart(newBar);
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            ChangeQuantization(-1);
        }
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ChangeQuantization(1);
        }
    }

    void ChangeTempo(int change)
    {
        tempo += change;
        
        if (tempo < 40)
            tempo = 40;
        else if (tempo > 260)
            tempo = 260;
        
        RecorderCanvasManager.ChangeTempo(tempo);
    }

    void ChangeQuantization(int minusPlus)
    {
        switch (minusPlus)
        {
            case -1:
                if (quantizeTo == Quantize.thirtyTwo)
                    quantizeTo = Quantize.sixteen;
                else if (quantizeTo == Quantize.sixteen)
                    quantizeTo = Quantize.eight;
                else if (quantizeTo == Quantize.eight)
                    quantizeTo = Quantize.four;
                else
                    quantizeTo = Quantize.thirtyTwo;
                break;
            case 1:
                if (quantizeTo == Quantize.four)
                    quantizeTo = Quantize.eight;
                else if (quantizeTo == Quantize.eight)
                    quantizeTo = Quantize.sixteen;
                else if (quantizeTo == Quantize.sixteen)
                    quantizeTo = Quantize.thirtyTwo;
                else
                    quantizeTo = Quantize.four;
                break;
        }

        RecorderCanvasManager.UpdateCurrentQuantization(quantizeTo);
    }

    void TogglePlay()
    {
        if (clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
            clickCoroutine = null;
        }
        else
        {
            clickCoroutine = StartCoroutine(ClickCoroutine());
        }
    }
    
    void PlayOnBarFromStart(int bar)
    {
        if (clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
            clickCoroutine = null;
        }

        if (bar < 0) bar = 0;
        if (bar > Bars.Count)
            bar = Bars.Count - 1;
        
        currentBeat = 0;
        currentTimestampInBar = 0;

        currentBar = bar;
        RecorderCanvasManager.UpdateCurrentTimeStamp(currentBar, currentTimestampInBar);
        clickCoroutine = StartCoroutine(ClickCoroutine());
    }

    private Coroutine clickCoroutine;
    IEnumerator ClickCoroutine()
    {
        while (true)
        {
            clickAu.clip = currentBeat == 0 ? clickMainFx : clickFx;

            clickAu.Play();
            
            float beatsPerSecond = tempo * 1f / 60;
            float secondsPerBeat = 1 / beatsPerSecond;
            int timestampsPerBeat = 8;
            float secondsPerTimestamp = secondsPerBeat / timestampsPerBeat;
            for (int i = 0; i < timestampsPerBeat; i++)
            {
                if (Input.GetKey(KeyCode.Backspace))
                {
                    RecorderCanvasManager.ToggleEraserFeedback(true);
                    DeleteNotesAtTimeStamp();
                }
                else
                {
                    RecorderCanvasManager.ToggleEraserFeedback(false);
                    PlayNotesAtTimeStamp();
                }
                yield return new WaitForSeconds(secondsPerTimestamp);   
                currentTimestampInBar++;
                RecorderCanvasManager.UpdateCurrentTimeStamp(currentBar, currentTimestampInBar);
            }

            currentBeat++;
            if (currentBeat >= timeSignature.x) // next bar
            {
                currentBeat = 0;
                currentTimestampInBar = 0;

                if (currentBar >= Bars.Count - 1)
                {
                    // no more bars
                    // START PART OVER
                    currentBar = 0;
                }
                else
                {
                    currentBar++;
                }
            }
        }        
    }

    
    void DeleteNotesAtTimeStamp()
    {
        Bars[currentBar].Timestamps[currentTimestampInBar].Notes.Clear();
        RecorderCanvasManager.RemoveNotesAtTimeStamp(currentBar,currentTimestampInBar);
    }
    
    void PlayNotesAtTimeStamp()
    {
        /*
        if (currentBar >= Bars.Count)
            return;*/
        
        for (int i = 0; i < Bars[currentBar].Timestamps[currentTimestampInBar].Notes.Count; i++)
        {
            drums.PlayDrumNote(Bars[currentBar].Timestamps[currentTimestampInBar].Notes[i]);
        }
    }
    
    public void SaveNoteToPianoRoll(int note, Instrument.InstrumentType instrumentType)
    {
        // save the note to currently selected instrument's piano roll at current time
        // and then update piano roll
        
        // в пиано ролле количество строчек равное количеству ступеней в двух октавах выбранной гаммы
        
        // get current bar
        // get current timestamp in that bar
        // each bar contains dictionary timestamps:  int indexOf32x, IntVector3(instrument, note, octave)
        var tempCurrentTimeStampAndBar = QuantizedTimeStamp(currentTimestampInBar, currentBar);
        var timeStamp = Bars[currentBar].Timestamps[tempCurrentTimeStampAndBar.x];
        if (!timeStamp.Notes.Contains(note))
        {
            if (currentTimestampInBar >= tempCurrentTimeStampAndBar.x)
                timeStamp.Notes.Add(note);
            else
                StartCoroutine(SaveNoteWhenBarIsOver(timeStamp, note));
                

            RecorderCanvasManager.PlaceNote(tempCurrentTimeStampAndBar.y, tempCurrentTimeStampAndBar.x, note,  2);
        }
    }

    IEnumerator SaveNoteWhenBarIsOver(Timestamp timeStamp, int note)
    {
        int oldBar = currentBar;
        
        while (currentBar == oldBar)
        {
            yield return null;
        }
        timeStamp.Notes.Add(note);
    }
    
    Vector2Int QuantizedTimeStamp(int timeStampToQuantize, int bar)
    {
        int result = timeStampToQuantize;
        switch (quantizeTo)
        {
            case Quantize.four:
                if (result < 3)
                    result = 0;
                else if (result < 8)
                    result = 8;
                else if (result < 11)
                    result = 8;
                else if (result < 16)
                    result = 16;
                else if (result < 19)
                    result = 16;
                else if (result < 24)
                    result = 24;
                else if (result < 27)
                    result = 24;
                else
                {
                    result = 0;
                    bar++;
                }
                break;
            
            case Quantize.eight:
                if (result < 2)
                    result = 0;
                else if (result < 4)
                    result = 4;
                else if (result < 6)
                    result = 4;
                else if (result < 8)
                    result = 8;
                else if (result < 10)
                    result = 8;
                else if (result < 12)
                    result = 12;
                else if (result < 14)
                    result = 12;
                else if (result < 16)
                    result = 16;
                else if (result < 18)
                    result = 16;
                else if (result < 20)
                    result = 20;
                else if (result < 22)
                    result = 20;
                else if (result < 24)
                    result = 24;
                else if (result < 26)
                    result = 24;
                else if (result < 28)
                    result = 28;
                else if (result < 30)
                    result = 28;
                else
                {
                    result = 0;
                    bar++;
                }
                break;
            
            case Quantize.sixteen:
                // QUANTIZE to 1/16
                if (result % 2 != 0)
                    result--;
                break;
        }

        if (bar >= Bars.Count)
            bar = 0;
        
        return new Vector2Int(result, bar);
    }
}

[Serializable]
public class Bar
{
    public List<Timestamp> Timestamps = new List<Timestamp>(32);
}

[Serializable]
public class Timestamp
{
    public List<int> notes;

    public List<int> Notes
    {
        get { return notes; }
        set
        {
            notes = value;
        }
    }
    public List<RectTransform> notesVisuals;
}
