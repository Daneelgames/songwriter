using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Editor
{
    public class PianoRollEditor : EditorWindow
    {
        [Serializable]
        enum GridSize
        {
            Four, Eight, Sixteen, ThirtyTwo
        }
        [SerializeField] private GridSize currentGridSize;
        [SerializeField] private int barsAmount = 8;
        [SerializeField] private CompositionData _compositionData;
        
        [MenuItem("Window/PianoRoll")]
        public static void ShowWindow()
        {
            GetWindow<PianoRollEditor>("PianoRoll");
        }

        void OnGUI()
        {
            currentGridSize = (GridSize)EditorGUILayout.EnumPopup("Grid Size:", currentGridSize);
            barsAmount = EditorGUILayout.IntField("Bars Amount: ", barsAmount);
            _compositionData = (CompositionData)EditorGUILayout.ObjectField(_compositionData, typeof(ScriptableObject), true);

            if (!_compositionData)
                return;
            
            if (GUI.Button(new Rect(20,65, 40,20), "Clear"))
            {
                ClearBars();
            }
            DrawPianoRollNotes();
        }

        void DrawPianoRollNotes()
        {
            int width = 7;

            switch (currentGridSize)
            {
                case GridSize.Sixteen:
                    width = 14;
                    break;
                case GridSize.Eight:
                    width = 28;
                    break;
                case GridSize.Four:
                    width = 56;
                    break;
            }
            
            for (int j = 0; j < 8; j++)
            {
                GUI.Label(new Rect(new Vector2(25, 100 + 30 * j), Vector2.one * 32), (7 - j).ToString());
            }
            
            int i = 0;

            //for (int x = 0; x < notesAmount; x++)
            for (int x = 0; x < 128; x++)
            {
                int bar = BarOfGlobalTimeStamp(x);
                for (int y = 0; y < 8; y++)
                {
                    if (i == 0)
                    {
                        if (_compositionData.Bars[bar].Timestamps[GlobalTimeStampToLocal(x)].Notes.Contains(y))
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.gray;
                        
                        if (GUI.Button(new Rect(50 + 7 * x, 100 + 30 * y, width, 30), Texture2D.whiteTexture))
                        {
                            ClickedOnNote(x, y);
                        }
                    }
                }
                
                if (currentGridSize != GridSize.ThirtyTwo)
                    i++;
                    
                if (currentGridSize == GridSize.Sixteen && i > 1)
                    i = 0;
                else if (currentGridSize == GridSize.Eight && i > 3)
                    i = 0;
                else if (currentGridSize == GridSize.Four && i > 7)
                    i = 0;

                if (x == 0 || x == 32 || x == 64 || x == 96 || x == 128 || x == 160 || x == 192 || x == 224)
                    DrawBox(new Rect(46 + 7 * x, 97, 6,255), Color.black);
            }   
        }

        void ClearBars()
        {
            for (int b = 0; b < _compositionData.Bars.Count; b++)
            {
                for (int t = 0; t < _compositionData.Bars[b].Timestamps.Count; t++)
                {
                    _compositionData.Bars[b].Timestamps[t].Notes.Clear();
                }
            }
        }

        int GlobalTimeStampToLocal(int global)
        {
            int barOfNote = BarOfGlobalTimeStamp(global);

            return  global - barOfNote * 32; 
        }

        int BarOfGlobalTimeStamp(int global)
        {
            //64 ==  bar 2
            int barOfNote = 0;
            if (global >= 32 && global < 64)
                barOfNote = 1;
            else if (global >= 64 && global < 96)
                barOfNote = 2;
            else if (global >= 96)
                barOfNote = 3;

            return barOfNote;
        }
        
        void ClickedOnNote(int globalTimeStamp, int note)
        {
            int barOfNote = BarOfGlobalTimeStamp(globalTimeStamp);
            int localTimeStamp = GlobalTimeStampToLocal(globalTimeStamp);
            
            //Add bars if there's not enough
            while (_compositionData.Bars.Count <= barOfNote)
            {
                _compositionData.Bars.Add(new Bar());
                _compositionData.Bars[_compositionData.Bars.Count - 1].Timestamps = new List<Timestamp>();
                for (int i = 0; i < 32; i++)
                {
                    _compositionData.Bars[_compositionData.Bars.Count - 1].Timestamps.Add(new Timestamp());
                    _compositionData.Bars[_compositionData.Bars.Count - 1]
                            .Timestamps[_compositionData.Bars[_compositionData.Bars.Count - 1].Timestamps.Count - 1].Notes = new List<int>();
                }
            }

            // ADD NOTE
            var notes = _compositionData.Bars[barOfNote].Timestamps[localTimeStamp].Notes;
            if (!notes.Contains(note))
                notes.Add(note);
            else
                notes.Remove(note);
        }
        
        void DrawBox (Rect rect, Color color) {
            Color oldColor = GUI.color;
 
            GUI.color = color;
            GUI.Box(rect, "");
 
            GUI.color = oldColor;
        }
    }
}
