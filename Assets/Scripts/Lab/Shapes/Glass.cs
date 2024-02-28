using System;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    public float refractiveIndex = 1.5f;
    public float transmission = 100.0f;
    public bool isCustom = false;

    [Header("3D Lenses")]
    public bool isLense3D = false;
    public string typeOfLense3D;

    public float RefractiveIndex
    {
        get{ return refractiveIndex; }
        set
        {
            refractiveIndex = Mathf.Clamp(value, 0.5f, 2.5f);
        }
    }

    public float Transmission
    {
        get { return transmission; }
        set
        {
            transmission = Mathf.Clamp(value, 0.0f, 100.0f);
        }
    }
}
