using UnityEngine;

public class Lense : MonoBehaviour
{
    [Header("Objects that lense is built of")]
    public GameObject square;
    public GameObject leftSide;
    public GameObject rightSide;

    [Header("Radius of curve")]
    public float leftRadius;
    public float rightRadius;

    [Header("Scale of square")]
    public float squareX;
    public float squareY;

    public virtual float SquareX
    {
        get { return squareX; }
        set
        {
            squareX = value;
        }
    }

    public virtual float SquareY
    {
        get { return squareY; }
        set
        {
            squareY = value;
        }
    }

    public virtual float LeftRadius
    {
        get { return leftRadius; }
        set
        {
            leftRadius = value;
        }
    }

    public virtual float RightRadius
    {
        get { return rightRadius; }
        set
        {
            rightRadius = value;
        }
    }
}
