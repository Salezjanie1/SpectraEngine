using UnityEngine;
using Photon.Pun;
public class LightSync : MonoBehaviourPunCallbacks, IPunObservable
{
    private LightPrism lightPrism;
    private EditManagerOnline editManager;

    private void Awake()
    {
        lightPrism = GetComponent<LightPrism>();
        editManager = FindFirstObjectByType<EditManagerOnline>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(lightPrism.distance);
            stream.SendNext(lightPrism.width);
            stream.SendNext(lightPrism.opacity);
            stream.SendNext(lightPrism.waveLength);
            stream.SendNext(lightPrism.whiteLight);
        }
        else if(stream.IsReading)
        {
            //Update variables
            lightPrism.distance = (float)stream.ReceiveNext();
            lightPrism.Width = (float)stream.ReceiveNext();
            lightPrism.Opacity = (float)stream.ReceiveNext();
            lightPrism.WaveLength = (int)stream.ReceiveNext();
            lightPrism.whiteLight = (bool)stream.ReceiveNext();

            //Update UI
            if (editManager.heldObject != null)
                editManager.HandleObjectClick(editManager.heldObject.tag);
        }
    }
}
