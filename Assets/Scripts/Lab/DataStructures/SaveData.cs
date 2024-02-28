using System.Collections.Generic;
[System.Serializable]
public class SaveData
{
    public float camX;
    public float camY;
    public float camZ;
    public float rotX;
    public float rotY;
    public float rotZ;
    public List<LightData> lights = new List<LightData>();
    public List<LenseData> lenses = new List<LenseData>();
    public List<PrismData> prisms = new List<PrismData>();
    public List<MirrorData> mirrors = new List<MirrorData>();
}
