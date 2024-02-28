using TMPro;
using UnityEngine;

public class LensePanel : Panel
{
    [Header("Special settings for lense")]
    public TMP_InputField scaleX;
    public TMP_InputField scaleY;
    public TMP_InputField leftRadius;
    public TMP_InputField rightRadius;

    private Lense script;
    private Glass glassScript;

    public override void LoadData(Transform heldObject)
    {
        base.LoadData(heldObject);

        script = heldObject.GetComponent<Lense>();
        glassScript = heldObject.GetComponent<Glass>();

        //Load first dataset
        scaleX.text = script.squareX.ToString();
        scaleY.text = script.squareY.ToString();
        leftRadius.text = script.leftRadius.ToString();
        rightRadius.text = script.rightRadius.ToString();
        refractiveIndex.text = glassScript.RefractiveIndex.ToString();

        //Lense settings
        scaleX.onValueChanged.AddListener((x) =>
        {
            if (float.TryParse(scaleX.text.Replace(".", ","), out float scaleXValue) && !scaleX.text.EndsWith(','))
            {
                script.SquareX = scaleXValue;
                scaleX.text = script.squareX.ToString();

                leftRadius.text = script.leftRadius.ToString();
                rightRadius.text = script.rightRadius.ToString();
            }
        });

        scaleY.onValueChanged.AddListener((x) =>
        {
            if (float.TryParse(scaleY.text.Replace(".", ","), out float scaleYValue) && !scaleY.text.EndsWith(','))
            {
                script.SquareY = scaleYValue;
                scaleY.text = script.squareY.ToString();
            }
        });

        leftRadius.onValueChanged.AddListener((x) =>
        {
            if (float.TryParse(leftRadius.text.Replace(".", ","), out float leftRadiusValue) && !leftRadius.text.EndsWith(','))
            {
                script.LeftRadius = leftRadiusValue;
                leftRadius.text = leftRadius.text == "-0" ? leftRadius.text : script.LeftRadius.ToString();
            }
        });

        rightRadius.onValueChanged.AddListener((x) =>
        {
            if (float.TryParse(rightRadius.text.Replace(".", ","), out float rightRadiusValue) && !rightRadius.text.EndsWith(','))
            {
                script.RightRadius = rightRadiusValue;
                rightRadius.text = rightRadius.text == "-0" ? rightRadius.text : script.RightRadius.ToString();
            }
        });
    }

}
