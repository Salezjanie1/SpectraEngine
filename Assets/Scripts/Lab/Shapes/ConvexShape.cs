using System;
using Unity.VisualScripting;
using UnityEngine;

public class ConvexShape : Lense
{
    [Header("Masks for both of circles")]
    public GameObject maskForLeft;
    public GameObject maskForRight;

    public ConvexShape() : base()
    {
        leftRadius = 0.5f;
        rightRadius = 0.5f;

        squareX = 0.1f;
        squareY = 3.0f;
    }

    public override float SquareX
    {
        get => base.SquareX;
        set
        {
            squareX = Mathf.Max(value, 0);
            square.transform.localScale = new Vector3(squareX, squareY, 1);

            leftSide.transform.localPosition = new Vector2(-square.transform.localScale.x / 2, leftSide.transform.localPosition.y);
            rightSide.transform.localPosition = new Vector2(square.transform.localScale.x / 2, rightSide.transform.localPosition.y);

            maskForLeft.transform.localPosition = new Vector2(square.transform.localScale.x / 2 + maskForLeft.transform.localScale.x / 2, square.transform.position.y);

            maskForRight.transform.localPosition = new Vector2(-square.transform.localScale.x / 2 - maskForLeft.transform.localScale.x / 2, square.transform.position.y);
        }
    }

    public override float SquareY
    {
        get => base.SquareY;
        set
        {
            squareY = Mathf.Max(value, 1);
            square.transform.localScale = new Vector3(squareX, squareY, 1);

            leftSide.transform.localPosition = new Vector2(-square.transform.localScale.x / 2, leftSide.transform.localPosition.y);
            rightSide.transform.localPosition = new Vector2(square.transform.localScale.x / 2, rightSide.transform.localPosition.y);

            maskForLeft.transform.localPosition = new Vector2(square.transform.localScale.x / 2 + maskForLeft.transform.localScale.x / 2, square.transform.position.y);

            maskForRight.transform.localPosition = new Vector2(-square.transform.localScale.x / 2 - maskForLeft.transform.localScale.x / 2, square.transform.position.y);

            rightSide.transform.localScale = new Vector3(rightRadius * -2, squareY, 1);
            leftSide.transform.localScale = new Vector3(leftRadius * -2, squareY, 1);
        }
    }

    public override float LeftRadius
    {
        get => base.LeftRadius;
        set
        {
            if (value == 0 && rightRadius == 0)
                leftRadius = 0.1f;
            else
                leftRadius = Mathf.Clamp(value, 0, squareY / 2);
            leftSide.transform.localScale = new Vector3(leftRadius * 2, squareY, 1);
        }
    }

    public override float RightRadius
    {
        get => base.RightRadius;
        set
        {
            if (value == 0 && leftRadius == 0)
                rightRadius = 0.1f;
            else
                rightRadius = Mathf.Clamp(value, 0, squareY / 2);
            rightSide.transform.localScale = new Vector3(rightRadius * 2, squareY, 1);
        }
    }
}
