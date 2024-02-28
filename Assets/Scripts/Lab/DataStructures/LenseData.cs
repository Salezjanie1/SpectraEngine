[System.Serializable]
public class LenseData : TransformData
{
    public string type;

    public float leftRadius;
    public float rightRadius;

    public float squareX;
    public float squareY;

    public float refractiveIndex = 1.5f;
    public float transmission = 100.0f;
}
