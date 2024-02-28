using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Panel : MonoBehaviourPunCallbacks
{
    [Header("Default settings")]
    public TMP_InputField posX;
    public TMP_InputField posY;
    public TMP_InputField posZ;
    public TMP_InputField transmission;
    public TMP_InputField refractiveIndex;

    [Header("Inputs for Custom Object")]
    public Toggle reflectToggle;
    public Toggle refractToggle;

    public Button deleteButton;
    public Button closeButton;

    private Transform dataSource;
    private Glass glass;

    public virtual void LoadData(Transform heldObject)
    {
        dataSource = heldObject;

        //Load data
        dataSource.TryGetComponent<Glass>(out glass);
        if (glass != null)
        {
            refractiveIndex.text = glass.refractiveIndex.ToString();
            transmission.text = glass.transmission.ToString();
        }

        //Position
        posX.onValueChanged.AddListener((x) =>
        {
            if (dataSource.gameObject.GetComponent<PhotonView>() != null && posX.isFocused)
                dataSource.gameObject.GetComponent<PhotonView>().RequestOwnership();

            if (float.TryParse(posX.text, out float posXValue) && posX.isFocused)
                dataSource.transform.position = new Vector3(posXValue, dataSource.transform.position.y, dataSource.transform.position.z);
        });

        posY.onValueChanged.AddListener((x) =>
        {
            if (dataSource.gameObject.GetComponent<PhotonView>() != null && posY.isFocused)
                dataSource.gameObject.GetComponent<PhotonView>().RequestOwnership();

            if (float.TryParse(posY.text, out float posYValue) && posY.isFocused)
                dataSource.transform.position = new Vector3(dataSource.transform.position.x, posYValue, dataSource.transform.position.z);
        });

        posZ.onValueChanged.AddListener((x) =>
        {
            if (dataSource.gameObject.GetComponent<PhotonView>() != null && posZ.isFocused)
                dataSource.gameObject.GetComponent<PhotonView>().RequestOwnership();

            if (string.IsNullOrEmpty(posZ.text)) posZ.text = "0";

            if (float.TryParse(posZ.text, out float posZValue) && posZ.isFocused)
                dataSource.transform.rotation = Quaternion.Euler(dataSource.transform.eulerAngles.x, dataSource.transform.eulerAngles.y, posZValue);
        });

        if (transmission != null)
        {
            transmission.onValueChanged.AddListener((x) =>
            {
                if (float.TryParse(transmission.text, out float transmissionValue) && transmission.isFocused)
                {
                    glass.Transmission = transmissionValue;
                    transmission.text = glass.Transmission.ToString();
                }
            });
        }

        if (refractiveIndex != null)
        {
            refractiveIndex.onValueChanged.AddListener((x) =>
            {
                if (float.TryParse(refractiveIndex.text.Replace(".", ","), out float refractiveIndexValue) && !refractiveIndex.text.EndsWith(',') && refractiveIndex.text != "0")
                {
                    glass.RefractiveIndex = refractiveIndexValue;
                    refractiveIndex.text = glass.RefractiveIndex.ToString();
                }
            });
        }
        if (reflectToggle != null && refractToggle != null)
        {
            reflectToggle.onValueChanged.RemoveAllListeners();
            refractToggle.onValueChanged.RemoveAllListeners();
            reflectToggle.isOn = false;
            refractToggle.isOn = false;

            if (dataSource.tag == "reflect")
                reflectToggle.isOn = true;
            else if (dataSource.tag == "refract")
                refractToggle.isOn = true;

            reflectToggle.onValueChanged.AddListener(delegate { UpdateDataSourceTag(); });
            refractToggle.onValueChanged.AddListener(delegate { UpdateDataSourceTag(); });
        }

        deleteButton.onClick.AddListener(() =>
        {
            if (FindFirstObjectByType<EditManagerOnline>() == null)
                Delete();
            else
                DeleteOnline();
        });

        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
            if (FindFirstObjectByType<EditManagerOnline>() == null)
                Delete();
            else
                DeleteOnline();

        if (posX.isFocused || posY.isFocused || posZ.isFocused) return;

        posX.text = Math.Round(dataSource.transform.position.x, 2).ToString();
        posY.text = Math.Round(dataSource.transform.position.y, 2).ToString();
        posZ.text = Math.Round(dataSource.transform.rotation.eulerAngles.z, 2).ToString();
    }

    private void Delete()
    {
        Destroy(dataSource.gameObject);
        gameObject.SetActive(false);

        Resources.UnloadUnusedAssets();
    }

    private void DeleteOnline()
    {
        Debug.Log("t");
        FindFirstObjectByType<EditManagerOnline>().GetComponent<PhotonView>().RPC("CloseAll", RpcTarget.All);
        PhotonNetwork.Destroy(dataSource.gameObject);
        Resources.UnloadUnusedAssets();
    }

    public void UpdateDataSourceTag()
    {
        if (reflectToggle != null && refractToggle != null)
        {
            if (reflectToggle.isOn)
            {
                dataSource.tag = "reflect";
            }
            else if (refractToggle.isOn)
            {
                dataSource.tag = "refract";
            }
        }
    }
}
