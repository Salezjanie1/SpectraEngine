using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPool : MonoBehaviour
{
    public static LightPool SharedInstance;
    public List<GameObject> singleLightObjects;
    public List<GameObject> prismLightObjects;
    public GameObject singleLight;
    public GameObject prismLight;
    public int amountToPoolSingleLight = 100;
    public int amountToPoolPrismLight = 50;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        singleLightObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPoolSingleLight; i++)
        {
            tmp = Instantiate(singleLight);
            tmp.SetActive(false);
            singleLightObjects.Add(tmp);
        }

        prismLightObjects = new List<GameObject>();
        for (int i = 0; i < amountToPoolPrismLight; i++)
        {
            tmp = Instantiate(prismLight);
            tmp.SetActive(false);
            prismLightObjects.Add(tmp);
        }
    }

    public GameObject GetSingleColorLightObject()
    {
        for (int i = 0; i < singleLightObjects.Count; i++) // Zmieni�em amountToPoolSingleLight na singleLightObjects.Count
        {
            if (!singleLightObjects[i].activeInHierarchy)
            {
                return singleLightObjects[i];
            }
        }

        // Je�li wszystkie obiekty s� u�ywane, zwi�ksz pul�
        return ExpandSingleLightPool();
    }

    private GameObject ExpandSingleLightPool()
    {
        Debug.Log("Expanding single light pool...");
        for (int i = 0; i < 100; i++) // Dodaj 100 nowych obiekt�w
        {
            GameObject tmp = Instantiate(singleLight);
            tmp.SetActive(false);
            singleLightObjects.Add(tmp);

            // Je�li jest to pierwszy dodany obiekt, zwr�� go (aby od razu m�c go u�y�)
            if (i == 0) return tmp;
        }

        // Teoretycznie, ten punkt nigdy nie powinien zosta� osi�gni�ty, ale zwr�� ostatni utworzony obiekt dla bezpiecze�stwa
        return singleLightObjects[singleLightObjects.Count - 1];
    }

    public GameObject GetPrismLightObject()
    {
        for (int i = 0; i < amountToPoolPrismLight; i++)
        {
            if (!prismLightObjects[i].activeInHierarchy)
            {
                return prismLightObjects[i];
            }
        }
        return null;
    }
}
