using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class LightPrism : MonoBehaviour
{
    [Header("Adjustable settings")]
    public float distance = 100f;
    public float width = 0.1f;
    public float opacity = 100;
    public int waveLength = 660;
    public bool whiteLight = true;
    public int maxRefracts = 10;

    [Header("Prefabs")]
    public GameObject lightPrefab;
    public GameObject prismColorsPrefab; 

    private LineRenderer lineRenderer;
    public bool isInside = false;

    private Vector3 lastHit;
    private Vector3 lastPosition;
    private bool lastWhiteLight;
    private float lastRefractiveIndex;
    private float lastTransmission;
    private GameObject lastGameObject;
    private string lastTag;

    private int refractCount = 0;

    public LightPrism root;
    private Vector3 shootPosition;
    private Vector3 shootDir;

    public int index = 0;

    [Header("Is 3D Light")]
    public bool is3D = false;

    private bool firstLoop = true;
    private int counter = 0;

    public float Width
    {
        get { return width; }
        set
        {
            width = value;

            if (lineRenderer != null)
            {
                lineRenderer.startWidth = value;
                lineRenderer.endWidth = value;
            }

            if (transform.childCount > 0)
            {
                int whichChild = is3D ? 1 : 0;

                if (transform.childCount > whichChild)
                    if(transform.GetChild(whichChild).GetComponent<LightPrism>() != null)
                        transform.GetChild(whichChild).GetComponent<LightPrism>().Width = value;
                    else
                        foreach (Transform prismPrefabChild in transform.GetChild(whichChild))
                            if (prismPrefabChild != null)
                                prismPrefabChild.GetComponent<LightPrism>().Width = value;
            }
        }
    }

    public float Opacity
    {
        get { return opacity; }
        set
        {
            opacity = value;

            if (lineRenderer != null)
            {
                lineRenderer.startColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, value / 100);
                lineRenderer.endColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, value / 100);
            }

            if (transform.childCount > 0)
            {
                int whichChild = is3D ? 1 : 0;

                if (transform.childCount > whichChild)
                    if (transform.GetChild(whichChild).GetComponent<LightPrism>() != null)
                        transform.GetChild(whichChild).GetComponent<LightPrism>().Opacity = value;
                    else
                        foreach (Transform prismPrefabChild in transform.GetChild(whichChild))
                            if (prismPrefabChild != null)
                                prismPrefabChild.GetComponent<LightPrism>().Opacity = value;
            }
        }
    }

    public int WaveLength
    {
        get { return waveLength; }
        set
        {
            waveLength = value;

            if (lineRenderer != null)
            {
                lineRenderer.startColor = whiteLight ? Color.white : GetColorFromWavelength(value);
                lineRenderer.endColor = whiteLight ? Color.white : GetColorFromWavelength(value);
            }

            if (transform.childCount > 0)
            {
                int whichChild = is3D ? 1 : 0;

                if (transform.childCount > whichChild)
                    if (transform.GetChild(whichChild).GetComponent<LightPrism>() != null)
                        transform.GetChild(whichChild).GetComponent<LightPrism>().WaveLength = value;
            }
        }
    }

    private NativeArray<RaycastCommand> _raycastCommands;
    private NativeArray<RaycastHit> _raycastHits;
    private JobHandle _jobHandle;

    void Awake()
    {
        _raycastCommands = new NativeArray<RaycastCommand>(1, Allocator.Persistent);
        _raycastHits = new NativeArray<RaycastHit>(1, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        _jobHandle.Complete();
        _raycastCommands.Dispose();
        _raycastHits.Dispose();
    }


    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //We are setting 2 because light has start and the end
        lineRenderer.positionCount = 2;

        //Starting options
        Width = width;
        WaveLength = waveLength;
        Opacity = opacity;
        lastWhiteLight = whiteLight;

        root = transform.root.GetComponent<LightPrism>();

        firstLoop = true;
    }

    private void Update()
    {
        shootDir = root.is3D ? transform.forward : transform.right;

        shootPosition = transform.tag == "light" ? transform.position + shootDir * 0.6f : transform.position;

        // First position on line renderer is always center of transform
        lineRenderer.SetPosition(0, shootPosition);

        //New
        _jobHandle.Complete();

        RaycastHit hit = _raycastHits[0];
        bool didHit = hit.collider != null;

        if (Physics.Raycast(shootPosition, shootDir, out hit, 300.0f, 5)) // Use direction for raycasting
        {
            lineRenderer.SetPosition(1, hit.point); // If ray hits something, stop there

            Glass hitGlass = hit.transform.root.GetComponent<Glass>();
            float objectRefractiveIndex = hitGlass != null ? hitGlass.RefractiveIndex : 1.5f;
            float transmission = hitGlass != null ? hitGlass.Transmission : 100.0f;

            // Check if a significant change has occurred
            if ((transform.hasChanged || 
                lastHit != hit.point ||
                lastPosition != transform.position ||
                lastWhiteLight != whiteLight ||
                firstLoop ||
                lastTag != hit.transform.root.tag ||
                lastTransmission != transmission ||
                lastRefractiveIndex != objectRefractiveIndex) && hit.collider != null)
            {
                //Switching between white and single light
                if (lastWhiteLight != whiteLight || lastGameObject != hit.transform.gameObject)
                    DestroyChildren();

                if (lastTransmission != transmission)
                    ChangeTransmission(transmission);

                // Handle the hit object based on its tag
                switch (hit.transform.root.tag)
                {
                    case "reflect":
                        Reflect(hit.point, shootDir, hit.normal, transmission);
                        break;
                    case "refract":
                        float n1 = isInside ? objectRefractiveIndex : 1;
                        float n2 = isInside ? 1 : objectRefractiveIndex;
                        if(root.is3D)
                            Refract3D(hit.point, hit.normal, shootDir, n1, n2, waveLength, transmission);
                        else
                            Refract(hit.point, hit.normal, shootDir, n1, n2, waveLength, transmission);
                        break;
                }

                // Update last hit and position
                lastHit = hit.point;
                lastPosition = transform.position;
                lastRefractiveIndex = objectRefractiveIndex;
                lastTransmission = transmission;
                lastGameObject = hit.transform.gameObject;
                lastTag = hit.transform.root.tag;
            }
        }
        else // If ray doesn't hit anything, go through
        {
            DestroyChildren();
            lineRenderer.SetPosition(1, transform.position + shootDir * distance); // Use direction for setting position
        }
        lastWhiteLight = whiteLight;
        transform.hasChanged = false;      
        lineRenderer.enabled = true;

        //Initialization done
        if (counter > 10)
            firstLoop = false;
        else
            counter += 1;

        _raycastCommands[0] = new RaycastCommand(shootPosition, shootDir);
        _jobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, 1);
    }

    private void CreateLight(Vector3 position, float zAngle, bool isInside, int waveLength, float width, float opacity, bool whiteLight, int refractCount)
    {
        GameObject light = Instantiate(lightPrefab, position, Quaternion.Euler(0, 0, zAngle), transform);
        LightPrism lightPrism = light.GetComponent<LightPrism>();
        LineRenderer lightLineRenderer = light.GetComponent<LineRenderer>();

        if (lightPrism != null)
        {
            lightPrism.isInside = isInside; //Pass the information is light inside of object or not
            lightPrism.waveLength = waveLength; 
            lightPrism.width = width; 
            lightPrism.opacity = opacity; 
            lightPrism.whiteLight = whiteLight; 
            lightPrism.refractCount = refractCount;
            lightPrism.index = index + 1;
            lightLineRenderer.sortingOrder = transform.GetComponent<LineRenderer>().sortingOrder;
        }
        
    }

    /// <summary>
    /// n1 - Refractive index of the medium that light is coming from
    /// n2 - Refractive index of the medium that light is coming to
    /// waveLength - wave length based on color
    /// </summary>
    private Vector3 CalculateRefractedDirection(Vector3 normal, Vector3 incident, float n1, float n2, int wavelength)
    {
        // Normalizacja wektor�w
        normal.Normalize();
        incident.Normalize();

        // Obliczenie k�ta padania przy u�yciu iloczynu skalarnego
        float cosThetaI = -Vector3.Dot(normal, incident);
        float sinThetaI2 = 1.0f - cosThetaI * cosThetaI;

        // Obliczenie sinusa k�ta za�amania przy u�yciu prawa Snella
        float sinThetaT2 = (n1 / n2) * (n1 / n2) * sinThetaI2;

        // Sprawdzenie warunku na ca�kowite wewn�trzne odbicie
        if (sinThetaT2 > 1.0f)
        {
            if (refractCount > maxRefracts)
                return incident;
            return Vector3.Reflect(incident, normal); // Odbicie je�li nie ma za�amania
        }

        float cosThetaT = Mathf.Sqrt(1.0f - sinThetaT2);

        // Oblicz wsp�czynnik na podstawie d�ugo�ci fali
        float wavelengthFactor = 1.0f / (1.0f + (wavelength - 500.0f) / 500.0f);

        // Obliczenie kierunku za�amanego promienia
        return n1 / n2 * incident + (n1 / n2 * cosThetaI - cosThetaT) * normal * wavelengthFactor;
    }
    float spawnDifference = 0.05f;
    private void Refract(Vector3 hit, Vector3 normal, Vector3 dir, float n1, float n2, int waveLength, float transmission)
    {
        Vector3 refractedDirection = CalculateRefractedDirection(normal, dir, n1, n2, waveLength);
        float zAngle = Mathf.Atan2(refractedDirection.y, refractedDirection.x) * Mathf.Rad2Deg;

        if (transform.childCount == 0)
        {
            if (!whiteLight)
                CreateLight(hit + dir.normalized * spawnDifference, zAngle, !isInside, waveLength, width, opacity * transmission / 100, whiteLight, refractCount + 1);
            else
            {
                GameObject prismColors = Instantiate(prismColorsPrefab, hit - dir.normalized * spawnDifference, transform.rotation, transform);

                //Set starting width and opacity
                foreach (Transform child in prismColors.transform)
                {
                    child.GetComponent<LightPrism>().width = width;
                    child.GetComponent<LightPrism>().opacity = opacity;
                }
            }
        }
        else
        {
            // Update existing children
            if (!whiteLight)
            {
                transform.GetChild(0).position = hit + dir.normalized * spawnDifference;
                transform.GetChild(0).rotation = Quaternion.Euler(0, 0, zAngle);
            }
            else
                foreach (Transform child in transform)
                {
                    child.position = hit - dir.normalized * spawnDifference;
                    child.rotation = transform.rotation;
                }
        }
    }

    private void Refract3D(Vector3 hit, Vector3 normal, Vector3 dir, float n1, float n2, int waveLength, float transmission)
    {
        Vector3 refractedDirection = CalculateRefractedDirection(normal, dir, n1, n2, waveLength);
        float zAngle = Mathf.Atan2(refractedDirection.y, refractedDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.LookRotation(refractedDirection);

        if ((transform.childCount == 1 && transform.tag == "light") || transform.childCount == 0)
        {
            if (!whiteLight)
                CreateLight(hit + dir.normalized * spawnDifference, zAngle, !isInside, waveLength, width, opacity * transmission / 100, whiteLight, refractCount + 1);
            else
            {
                GameObject prismColors = Instantiate(prismColorsPrefab, hit - dir.normalized * spawnDifference, Quaternion.LookRotation(shootDir), transform);

                foreach (Transform child in prismColors.transform)
                {
                    child.GetComponent<LightPrism>().width = width;
                    child.GetComponent<LightPrism>().opacity = opacity;
                }
            }
        }
        else
        {
            if (root.is3D && transform.tag == "light")
            {
                transform.GetChild(1).position = hit + (whiteLight ? -dir.normalized : dir.normalized) * spawnDifference;
                if(!whiteLight)
                    transform.GetChild(1).rotation = targetRotation;
            }
            else if(root.is3D && transform.childCount == 1)
            {
                transform.GetChild(0).position = hit + (whiteLight ? -dir.normalized : dir.normalized) * spawnDifference;
                if (!whiteLight)
                    transform.GetChild(0).rotation = targetRotation;
            }
            else
            {
                transform.GetChild(0).position = hit + dir.normalized * spawnDifference;
                transform.GetChild(0).rotation = targetRotation;
            }
        }
    }

    private Vector3 CalculateReflectedDirection(Vector3 incident, Vector3 normal)
    {
        // Reflect the incident vector (direction of incoming light) off the surface normal
        Vector3 reflectedDirection = Vector3.Reflect(incident.normalized, normal.normalized);
        return reflectedDirection;
    }

    private void Reflect(Vector3 hit, Vector3 dir, Vector3 normal, float transmission)
    {
        Vector3 reflectedDirection = CalculateReflectedDirection(dir, normal);
        float zAngle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg;
        
        Quaternion targetRotation = Quaternion.LookRotation(reflectedDirection);

        if (transform.childCount != 0)
            if(transform.GetChild(0).tag == "prismColors")
                Destroy(transform.GetChild(0).gameObject); //If we go to mirror from lense or prism directly there is no reason to hold prismcolors just create one white line

        if (transform.childCount == 0 || (root.is3D && transform.childCount == 1 && transform.tag == "light"))
            CreateLight(hit, zAngle, isInside, waveLength, width, opacity * transmission / 100, whiteLight, refractCount);
        else
        {
            // Update existing children
            if (root.is3D && transform.tag == "light")
            {
                transform.GetChild(1).position = hit;
                transform.GetChild(1).rotation = targetRotation;
            }
            else if (root.is3D && transform.childCount == 1)
            {
                transform.GetChild(0).position = hit;
                transform.GetChild(0).rotation = targetRotation;
            }
            else
            {
                transform.GetChild(0).position = hit;
                transform.GetChild(0).rotation = Quaternion.Euler(0, 0, zAngle);
            }
                
        }
    }

    private Color GetColorFromWavelength(float wavelength)
    {
        float gamma = 0.8f;
        float intensityMax = 1.0f;
        float factor = 0.0f;
        float R = 0.0f, G = 0.0f, B = 0.0f;

        if (wavelength == 0)
        {
            // White light for wavelength equal to 0
            R = G = B = 1.0f;
        }
        else if ((wavelength >= 380) && (wavelength < 440))
        {
            R = -(wavelength - 440) / (440 - 380);
            G = 0.0f;
            B = 1.0f;
        }
        else if ((wavelength >= 440) && (wavelength < 490))
        {
            R = 0.0f;
            G = (wavelength - 440) / (490 - 440);
            B = 1.0f;
        }
        else if ((wavelength >= 490) && (wavelength < 510))
        {
            R = 0.0f;
            G = 1.0f;
            B = -(wavelength - 510) / (510 - 490);
        }
        else if ((wavelength >= 510) && (wavelength < 580))
        {
            R = (wavelength - 510) / (580 - 510);
            G = 1.0f;
            B = 0.0f;
        }
        else if ((wavelength >= 580) && (wavelength < 645))
        {
            R = 1.0f;
            G = -(wavelength - 645) / (645 - 580);
            B = 0.0f;
        }
        else if ((wavelength >= 645) && (wavelength <= 750))
        {
            R = 1.0f;
            G = 0.0f;
            B = 0.0f;
        }

        // Adjust intensity
        if ((wavelength >= 380) && (wavelength <= 750))
        {
            if (wavelength < 645)
            {
                factor = 0.3f + 0.7f * (wavelength - 380) / (645 - 380);
            }
            else
            {
                factor = 0.3f + 0.7f * (750 - wavelength) / (750 - 645);
            }

            R = Mathf.Pow(R * factor, gamma);
            G = Mathf.Pow(G * factor, gamma);
            B = Mathf.Pow(B * factor, gamma);
        }
        else
        {
            R = G = B = 0.0f;
        }

        return new Color(R * intensityMax, G * intensityMax, B * intensityMax);
    }

    private void ChangeTransmission(float transmission)
    {
        if (transform.childCount > 0)
        {
            int whichChild = is3D ? 1 : 0;

            if (transform.childCount > whichChild)
                if (transform.GetChild(whichChild).GetComponent<LightPrism>() != null)
                    if (!transform.GetChild(whichChild).GetComponent<LightPrism>().isInside)
                        transform.GetChild(whichChild).GetComponent<LightPrism>().Opacity = transform.GetChild(whichChild).parent.GetComponent<LightPrism>().Opacity * transmission / 100;
                else
                    foreach (Transform prismPrefabChild in transform.GetChild(whichChild))
                        if (prismPrefabChild != null)
                            prismPrefabChild.GetComponent<LightPrism>().Opacity = transform.GetChild(whichChild).parent.GetComponent<LightPrism>().Opacity * transmission / 100;
        }
    }

    private void DestroyChildren()
    {
        if (transform.childCount > 0)
        {
            if (is3D && transform.childCount > 1)
            {
                DestroyImmediate(transform.GetChild(1).gameObject);
                if (transform.parent == null)
                    Resources.UnloadUnusedAssets();
            }
            else if(!is3D)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
                if (transform.parent == null)
                    Resources.UnloadUnusedAssets();
            }
        }
    }
}