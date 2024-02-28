using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EditManagerOnline : EditManager
{
    [Header("Online")]
    public GameObject playerPrefab;

    public TMP_Text code;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
        }
        string firstHalf = PhotonNetwork.CurrentRoom.Name.Substring(0, 3);
        string secondHalf = PhotonNetwork.CurrentRoom.Name.Substring(3, 3);
        code.text = firstHalf + " " + secondHalf;
    }

    private void Awake()
    {
        //LoadData();
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (Input.GetKeyDown(KeyCode.Delete) && heldObject != null && heldObject.tag != "Untagged")
            Delete();

        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenu.activeSelf)
            PauseMenu();
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu.activeSelf && !helpMenu.activeSelf)
            PauseMenu();
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu.activeSelf && helpMenu.activeSelf)
            Help();

        if (Input.GetMouseButtonDown(0) && !pauseMenu.activeSelf)
        {
            // if (is3d)
            // {
            //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //     RaycastHit hit3d;
            //     if (Physics.Raycast(ray, out hit3d))
            //     {
            //         if (hit3d.collider != null && !EventSystem.current.IsPointerOverGameObject())
            //         {
            //             heldObject = hit3d.transform.root;
            //             HandleObjectClick(hit3d.transform.root.tag);
            //         }
            //     }else if (hit3d.collider == null && !EventSystem.current.IsPointerOverGameObject())
            // {
            //     //CloseAll();
            //     heldObject = null;
            // } 
            // }
            // else {
            // RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // if (hit.collider != null && !EventSystem.current.IsPointerOverGameObject())
            // {
            //     heldObject = hit.transform.root;
            //     isDragging = true;
            //     dragOffset = heldObject.position - mousePosition;

            //     heldObject.GetComponent<PhotonView>().RequestOwnership();

            //     HandleObjectClick(hit.transform.root.tag);
            // }
            // else if (hit.collider == null && !EventSystem.current.IsPointerOverGameObject())
            // {
            //     CloseAll();
            //     heldObject = null;
            // }
            // }




            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    heldObject = hit.transform.root;
                    if (!is3d)
                    {
                        isDragging = true;
                        dragOffset = heldObject.position - mousePosition;
                        heldObject.GetComponent<PhotonView>().RequestOwnership();
                    }


                    HandleObjectClick(hit.transform.root.tag);
                }
            }
            else
            {
                if (!is3d && !EventSystem.current.IsPointerOverGameObject())
                    CloseAll();
                heldObject = null;
            }
        }

        if (!pauseMenu.activeSelf)
        {
            if (Input.GetMouseButton(0) && isDragging && heldObject != null)
            {
                if (Input.GetKey(KeyCode.R))
                {
                    RotateObjectTowardsMouse(heldObject, mousePosition);
                    isRotating = true;
                }
                else if (Input.GetKey(KeyCode.S) && !heldObject.CompareTag("light"))
                {
                    if (!isScaling)
                    {
                        initialDistance = Vector2.Distance(heldObject.position, mousePosition);
                        initialScale = heldObject.localScale;
                        isScaling = true;
                    }
                    else
                    {
                        ScaleObject(heldObject, mousePosition, initialDistance, initialScale);
                    }
                }
                else
                {
                    if (!isRotating && !isScaling)
                    {
                        heldObject.transform.position = new Vector2(mousePosition.x, mousePosition.y) + new Vector2(dragOffset.x, dragOffset.y);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                isRotating = false;
                isScaling = false;
                if (is3d)
                    heldObject = null;
            }

            //Pan camera movment system
            if (Input.GetMouseButtonDown(2)) // Right mouse button clicked
            {
                originPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButton(2)) // Right mouse button held down
            {
                Vector3 difference = originPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += new Vector3(difference.x, difference.y, 0);
            }

            //Zoom camera system
            if (Camera.main.orthographic)
            {
                // For orthographic camera
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minZoom, maxZoom);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && helpMenu.activeSelf)
            Help();
    }

    [PunRPC]
    public void Inform(string inform)
    {
        Debug.Log(inform);
    }

    public new void Home()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    public void SpawnObjectOnline(GameObject prefab)
    {
        photonView.RPC("Spawn", RpcTarget.All, prefab.name, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0));
    }

    public void SpawnObject3DOnline(GameObject prefab)
    {
        photonView.RPC("SpawnObject3D", RpcTarget.All, prefab.name, Camera.main.transform.position, Camera.main.transform.forward);
    }

    [PunRPC]
    public void SpawnObject3D(string prefab, Vector3 pos, Vector3 dir)
    {
        Vector3 cameraPosition = pos;

        Vector3 cameraDirection = dir;

        Vector3 spawnPosition = cameraPosition + cameraDirection * 10;

        GameObject tool = PhotonNetwork.InstantiateRoomObject(prefab, spawnPosition, Quaternion.identity);
    }

    [PunRPC]
    public void Spawn(string prefab, Vector3 pos)
    {
        GameObject tool = PhotonNetwork.InstantiateRoomObject(prefab, pos, Quaternion.identity);
    }

    [PunRPC]
    public new void CloseAll()
    {

        lightPanel.SetActive(false);
        lensePanel.SetActive(false);
        prismPanel.SetActive(false);
        mirrorPanel.SetActive(false);
    }
}
