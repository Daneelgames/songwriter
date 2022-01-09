using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecorderCanvasManager : MonoBehaviour
{
    public GameObject currentTimeStampVisual;
    public GameObject eraserFeedback;
    public RectTransform noteVisualRef;
    public Text tempoFeedback;
    public Text quantizeFeedback;
    public List<Transform> notesRows;
    static float timeStampCanvasPixelWidth = 4.5f;

    private List<Bar> bars;
    
    public void Initialize(List<Bar> _bars)
    {
        bars = new List<Bar>(_bars);
    }
    
    public void PlaceNote(int currentBar, int timeStampInBarToSave, int noteIndex, float noteLength)
    {
        var newNote = Instantiate(noteVisualRef);
        newNote.transform.parent = notesRows[noteIndex];
        newNote.sizeDelta = new Vector2 ( timeStampCanvasPixelWidth * noteLength, newNote.sizeDelta.y);

//        Debug.Log("currentBar: "+ currentBar + "; timeStampInBarToSave " + timeStampInBarToSave);
        float newXpos = (32 * timeStampCanvasPixelWidth) * currentBar + timeStampInBarToSave * timeStampCanvasPixelWidth;
        newNote.localPosition = new Vector3(newXpos, 0, 0);
        bars[currentBar].Timestamps[timeStampInBarToSave].notesVisuals.Add(newNote);
    }

    public void RemoveNotesAtTimeStamp(int currentBar, int timeStamp)
    {
        if (bars[currentBar].Timestamps[timeStamp].notesVisuals.Count <= 0)
            return;
        
        for (int i = bars[currentBar].Timestamps[timeStamp].notesVisuals.Count - 1; i >= 0; i--)
        {
            Destroy(bars[currentBar].Timestamps[timeStamp].notesVisuals[i].gameObject);
            bars[currentBar].Timestamps[timeStamp].notesVisuals.RemoveAt(i);
        }
    }

    public void UpdateCurrentTimeStamp(int currentBar, int currentTimeStampInBar)
    {
        float newXpos = (32 * timeStampCanvasPixelWidth) * currentBar + currentTimeStampInBar * timeStampCanvasPixelWidth;
        currentTimeStampVisual.transform.localPosition = new Vector3(newXpos, 0, 0);
    }
    
    public void UpdateCurrentQuantization(BpmManager.Quantize quantize)
    {
        switch (quantize)
        {
            case BpmManager.Quantize.four:
                quantizeFeedback.text = "Current quantization: " + "1/4";
                break;
            case BpmManager.Quantize.eight:
                quantizeFeedback.text = "Current quantization: " + "1/8";
                break;
            case BpmManager.Quantize.sixteen:
                quantizeFeedback.text = "Current quantization: " + "1/16";
                break;
            case BpmManager.Quantize.thirtyTwo:
                quantizeFeedback.text = "Current quantization: " + "1/32";
                break;
        }
    }

    public void ChangeTempo(int newTempo)
    {
        tempoFeedback.text = "Tempo: " + newTempo;
    }

    public void ToggleEraserFeedback(bool toggle)
    {
        if (eraserFeedback.activeInHierarchy != toggle)
            eraserFeedback.SetActive(toggle);
    }
}