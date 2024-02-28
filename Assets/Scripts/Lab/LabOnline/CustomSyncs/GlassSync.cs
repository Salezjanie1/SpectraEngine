using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GlassSync : MonoBehaviourPunCallbacks, IPunObservable
{
    private Glass glass;
    private EditManagerOnline editManager;

    private void Awake()
    {
        glass = GetComponent<Glass>();
        editManager = FindFirstObjectByType<EditManagerOnline>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(glass.refractiveIndex);
            stream.SendNext(glass.transmission);
        }
        else if(stream.IsReading)
        {
            //Update variables
            glass.RefractiveIndex = (float)stream.ReceiveNext();
            glass.Transmission = (float)stream.ReceiveNext(); 
            
            //Update UI
            if (editManager.heldObject != null)
                editManager.HandleObjectClick(editManager.heldObject.tag);
        }
    }
}
