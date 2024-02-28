using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    private void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
            transform.position = mousePosition;
        }
    }
}
