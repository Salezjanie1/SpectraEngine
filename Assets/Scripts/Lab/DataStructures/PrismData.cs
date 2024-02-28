[System.Serializable]
public class PrismData : TransformData
{
    public string type;

    public float refractiveIndex = 1.5f;
    public float transmission = 100.0f;
    public bool isLense3D = false;
    public string typeOfLense3D;
}
