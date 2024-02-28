using UnityEngine;
using Photon.Pun;

public class LenseSync : MonoBehaviourPunCallbacks, IPunObservable
{
    private Lense lense;
    private EditManagerOnline editManager;

    private void Awake()
    {
        lense = GetComponent<Lense>();
        editManager = FindFirstObjectByType<EditManagerOnline>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Update variables
        if(stream.IsWriting)
        {
            stream.SendNext(lense.squareX);
            stream.SendNext(lense.squareY);
            stream.SendNext(lense.leftRadius);
            stream.SendNext(lense.rightRadius);
        }
        else if(stream.IsReading)
        {
            lense.SquareX = (float)stream.ReceiveNext();
            lense.SquareY = (float)stream.ReceiveNext();
            lense.LeftRadius = (float)stream.ReceiveNext();
            lense.RightRadius = (float)stream.ReceiveNext();

            //Update UI
            if (editManager.heldObject != null)
                editManager.HandleObjectClick(editManager.heldObject.tag);
        }
    }
}
