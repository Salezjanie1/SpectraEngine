using Photon.Pun;
using UnityEngine;

public class draganddrop : MonoBehaviour
{
    private bool _mouseState;
    private bool _rotateState;
    private bool _scaleState; // Dodana zmienna stanu dla skalowania
    public GameObject Target;
    public Vector3 screenSpace;
    public Vector3 offset;
    public float rotationSpeed = 4f; // Prędkość obrotu
    public float scaleSpeed = 0.1f; // Prędkość skalowania

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            if (Target = GetClickedObject(out hitInfo))
            {
                if (Target.GetComponent<PhotonView>() != null)
                    Target.GetComponent<PhotonView>().RequestOwnership();

                _mouseState = true;
                screenSpace = Camera.main.WorldToScreenPoint(Target.transform.position);
                offset = Target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _mouseState = false;
        }

        if (Input.GetKey(KeyCode.R) && _mouseState)
        {
            _rotateState = true;
            GetComponent<FreeFlyCamera>().enabled = false;
        }
        else
        {
            _rotateState = false;
            GetComponent<FreeFlyCamera>().enabled = true;
        }

        if (Input.GetKey(KeyCode.Q) && _mouseState) // Dodana obsługa skalowania przy przytrzymaniu klawisza "S"
        {
            _scaleState = true;
            GetComponent<FreeFlyCamera>().enabled = false;

        }
        else
        {
            _scaleState = false;
            GetComponent<FreeFlyCamera>().enabled = true;

        }


        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // Wyłącz skrypt FreeFlyCamera
            GetComponent<FreeFlyCamera>().enabled = false;

            // Odblokuj kursor, aby umożliwić interakcję z UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Włącz skrypt FreeFlyCamera
            GetComponent<FreeFlyCamera>().enabled = true;

            // Zablokuj kursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


if (_mouseState && Target != null)
{
    var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
    var curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
    Target.transform.position = curPosition;
    // Obróć obiekt w stronę kamery
    if(Target.GetComponent<Canvas>()){
        Target.transform.LookAt(Camera.main.transform);
        Target.transform.Rotate(0, 180, 0);
    }
}

        if (_rotateState && Target != null)
        {
            float rotateX = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotateY = Input.GetAxis("Mouse Y") * rotationSpeed;
            Target.transform.Rotate(Vector3.up, rotateX, Space.World);
            Target.transform.Rotate(Vector3.right, -rotateY, Space.World);

        }

        if (_scaleState && Target != null) // Obsługa skalowania
        {
            float scaleAmount = Input.GetAxis("Mouse Y") * scaleSpeed;
            Vector3 newScale = Target.transform.localScale + Vector3.one * scaleAmount;
            if (newScale.y >= 0.1 && newScale.x >= 0.1 && newScale.z >= 0.1)
                Target.transform.localScale = newScale;
        }
    }

    GameObject GetClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            target = hit.transform.root.gameObject;
        }
        return target;
    }
}
