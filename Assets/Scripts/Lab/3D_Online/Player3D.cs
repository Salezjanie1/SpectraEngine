using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;

public class Player3D : MonoBehaviourPunCallbacks
{
    public List<MonoBehaviour> scripts;
    public MonoBehaviour[] scriptsToOffOnPause;
    public GameObject model;
    public Camera playerCamera;
    public AudioListener audioListener;
    public AudioSource audioSource;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            foreach (MonoBehaviour script in scripts)
                script.enabled = false;

            playerCamera.enabled = false;
            audioListener.enabled = false;
            audioSource.enabled = false;
        }
        else
        {
            model.SetActive(false);
            FindObjectOfType<EditManagerOnline>().disableScripts = scriptsToOffOnPause;
        }
    }
}
