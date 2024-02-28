using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LightPanel : Panel
{
    [Header("Special settings for light")]
    public TMP_InputField width;
    public TMP_InputField opacity;
    public TMP_InputField waveLength;

    public Toggle simulateColors;

    private LightPrism script;

    public override void LoadData(Transform heldObject)
    {
        base.LoadData(heldObject);

        script = heldObject.GetComponent<LightPrism>();

        //Load first dataset
        width.text = script.Width.ToString();
        opacity.text = script.Opacity.ToString();
        waveLength.text = script.WaveLength.ToString();
        simulateColors.isOn = script.whiteLight;

        //Light settings
        simulateColors.onValueChanged.AddListener((x) =>
        {
            script.whiteLight = simulateColors.isOn;
            script.WaveLength = script.WaveLength;
        });

        waveLength.onValueChanged.AddListener((x) =>
        {
            if (int.TryParse(waveLength.text, out int waveLengthValue))
                script.WaveLength = Mathf.Clamp(waveLengthValue, 380, 720);
        });

        width.onValueChanged.AddListener((x) =>
        {
            if (float.TryParse(width.text.Replace(".", ","), out float widthValue) && !width.text.EndsWith(',') && width.text != "0")
            {
                script.Width = Mathf.Clamp(widthValue, 0.09f, 1.0f);
                width.text = script.Width.ToString();
            }
        });

        opacity.onValueChanged.AddListener((x) =>
        {
            if (float.TryParse(opacity.text, out float opacityValue))
            {
                script.Opacity = Mathf.Clamp(opacityValue, 0.0f, 100.0f);
                opacity.text = script.Opacity.ToString();
            }
        });
    }
}
