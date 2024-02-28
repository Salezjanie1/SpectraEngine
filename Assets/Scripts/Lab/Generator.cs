using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject lamp;

    public int density = 2;
    private int lastDensity;

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        for (int i = 400; i <= 710; i += density)
        {
            GameObject light = Instantiate(lamp, transform.position, Quaternion.identity);
            light.GetComponent<LightPrism>().whiteLight = false;
            light.GetComponent<LightPrism>().WaveLength = i;
            light.GetComponent<LineRenderer>().sortingOrder = -i;
            light.transform.SetParent(transform);
        }
    }
}
