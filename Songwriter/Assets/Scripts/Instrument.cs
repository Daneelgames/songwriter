using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    public enum InstrumentType
    {
        Drums
    }

    public InstrumentType _InstrumentType = InstrumentType.Drums;
}