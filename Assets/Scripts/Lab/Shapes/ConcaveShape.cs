using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcaveShape : Lense
{
    public ConcaveShape() : base()
    {
        leftRadius = -0.9f;
        rightRadius = -0.9f;

        squareX = 2.0f;
        squareY = 3.0f;
    }

    public override float SquareX 
    { 
        get => base.SquareX; 
        set
        {
            squareX = Mathf.Max(value, 2);
            square.transform.localScale = new Vector3(squareX, squareY, 1);

            leftSide.transform.localPosition = new Vector2(-square.transform.localScale.x / 2, leftSide.transform.localPosition.y);
            rightSide.transform.localPosition = new Vector2(square.transform.localScale.x / 2, rightSide.transform.localPosition.y);
        }
    }

    public override float SquareY
    {
        get => base.SquareY;
        set
        {
            squareY = Mathf.Max(value, 1);
            square.transform.localScale = new Vector3(squareX, squareY, 1);

            leftSide.transform.localPosition = new Vector3(-square.transform.localScale.x / 2, leftSide.transform.localPosition.y, 0);
            leftSide.transform.localScale = new Vector3(leftRadius * -2, squareY, 1);
            rightSide.transform.localPosition = new Vector3(square.transform.localScale.x / 2, rightSide.transform.localPosition.y, 0);
            rightSide.transform.localScale = new Vector3(rightRadius * -2, squareY, 1);
        }
    }

    public override float LeftRadius 
    { 
        get => base.LeftRadius;
        set 
        {
            if (value == 0 && rightRadius == 0)
                leftRadius = -0.1f;
            else
                leftRadius = Mathf.Clamp(value, -squareX / 2 + 0.05f, 0);
            leftSide.transform.localScale = new Vector3(leftRadius * -2, squareY, 1);
        } 
    }

    public override float RightRadius
    {
        get => base.RightRadius;
        set
        {
            if (value == 0 && leftRadius == 0)
                rightRadius = -0.1f;
            else
                rightRadius = Mathf.Clamp(value, -squareX / 2 + 0.05f, 0);
            rightSide.transform.localScale = new Vector3(rightRadius * -2, squareY, 1);
        }
    }
}
