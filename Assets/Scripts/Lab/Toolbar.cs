using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Toolbar : MonoBehaviourPunCallbacks
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ShowToolbar()
    {
        anim.SetTrigger("Show");
    }

    public void HideToolbar()
    {
        anim.SetTrigger("Hide");
    }
    
    public void SpawnObject(GameObject prefab)
    {
        Vector3 screenCenter = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        GameObject tool = Instantiate(prefab, screenCenter, Quaternion.identity);
    }

    [PunRPC]
    public void SpawnObjectOnline(GameObject prefab)
    {
        Vector3 screenCenter = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        GameObject tool = PhotonNetwork.InstantiateRoomObject(prefab.name, screenCenter, Quaternion.identity);
    }

    public void SpawnObject3D(GameObject prefab)
    {
        // Pobierz pozycję kamery
        Vector3 cameraPosition = Camera.main.transform.position;

        // Pobierz kierunek, w którym kamera jest skierowana
        Vector3 cameraDirection = Camera.main.transform.forward;

        // Ustal miejsce, w którym chcesz stworzyć obiekt
        Vector3 spawnPosition = cameraPosition + cameraDirection * 10; // distance to be defined

        // Stwórz obiekt
        GameObject tool = Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}
