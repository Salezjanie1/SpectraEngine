using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Menu : MonoBehaviour
{
    public int Scene2DId = 1;

    public GameObject credits;
    public GameObject help;

    [Header("Saves 2D")]
    public GameObject savesPanel2D;
    public ScrollRect scrollRect;
    public Transform saveContent;
    public GameObject saveButtonPrefab;

    [Header("Saves 3D")]
    public GameObject savesPanel3D;
    public ScrollRect scrollRect3D;
    public Transform saveContent3D;
    public GameObject saveButtonPrefab3D;

    [Header("Room Panel")]
    public GameObject roomPanel;

    [Header("Slides")]
    private Animator animator;
    public List<Texture2D> slides;
    public Image slide;
    private int lastSlideID;

    private void Awake()
    {
        //Load saves if any exists
        if (Directory.Exists($"{Application.dataPath}\\saves"))
        {
            if(!Directory.Exists($"{Application.dataPath}\\saves\\2D"))
                Directory.CreateDirectory($"{Application.dataPath}\\saves\\2D");

            if (!Directory.Exists($"{Application.dataPath}\\saves\\3D"))
                Directory.CreateDirectory($"{Application.dataPath}\\saves\\3D");

            string[] saves2D = ListDirectories($"{Application.dataPath}\\saves\\2D");
            string[] saves3D = ListDirectories($"{Application.dataPath}\\saves\\3D");

            if (saves2D.Length > 0)
            {
                foreach (string save in saves2D)
                {
                    GameObject saveButton = Instantiate(saveButtonPrefab);
                    saveButton.transform.SetParent(saveContent);
                    int indexBeforeLast = Mathf.Max(saveContent.childCount - 2, 0);
                    saveButton.transform.SetSiblingIndex(indexBeforeLast);
                    saveButton.transform.localScale = new Vector3(1, 1, 1);

                    SaveButton saveButtonComponent = saveButton.GetComponent<SaveButton>();
                    saveButtonComponent.date = save;
                    saveButtonComponent.mode = "2D";
                    saveButtonComponent.saveName.text = ConvertToDate(save);
                    saveButtonComponent.saveImage.sprite = GetImage($"{Application.dataPath}\\saves\\2D\\{save}\\{save}_Spectra.png");
                }
            }

            if (saves3D.Length > 0)
            {
                foreach (string save in saves3D)
                {
                    GameObject saveButton = Instantiate(saveButtonPrefab3D);
                    saveButton.transform.SetParent(saveContent3D);
                    int indexBeforeLast = Mathf.Max(saveContent3D.childCount - 2, 0);
                    saveButton.transform.SetSiblingIndex(indexBeforeLast);
                    saveButton.transform.localScale = new Vector3(1, 1, 1);

                    SaveButton saveButtonComponent = saveButton.GetComponent<SaveButton>();
                    saveButtonComponent.date = save;
                    saveButtonComponent.mode = "3D";
                    saveButtonComponent.saveName.text = ConvertToDate(save);
                    saveButtonComponent.saveImage.sprite = GetImage($"{Application.dataPath}\\saves\\3D\\{save}\\{save}_Spectra.png");
                }
            }
        }
        else
            Directory.CreateDirectory($"{Application.dataPath}\\saves");
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void Update()
    {
        if (Input.anyKey && !credits.activeSelf)
            animator.SetTrigger("stop");

        if (Input.anyKey && credits.activeSelf)
            Credits();

        if (Input.anyKey && help.activeSelf)
            Help();
    }

    private bool isFirstCall = true;
    public void SelectRandomSlide()
    {
        int loadID;
        if (isFirstCall)
        {
            // Start with slide number 8 on the first call
            loadID = 7; // Assuming slides are 0-indexed, slide number 8 would be at index 7
            isFirstCall = false; // Ensure that this condition is only true for the first call
        }
        else
        {
            do
            {
                loadID = Random.Range(0, slides.Count);
            } while (loadID == lastSlideID);
        }

        Texture2D slideTexture = slides[loadID];
        Sprite newSlide = Sprite.Create(slideTexture, new Rect(0.0f, 0.0f, slideTexture.width, slideTexture.height), new Vector2(0.5f, 0.5f));
        slide.sprite = newSlide;

        lastSlideID = loadID;
    }

    public void ShowSaves2D()
    {
        CloseAll();
        savesPanel2D.SetActive(!savesPanel2D.activeSelf);
    }

    public void ShowSaves3D()
    {
        CloseAll();
        savesPanel3D.SetActive(!savesPanel3D.activeSelf);
    }

    public void ShowRoomPanel()
    {
        CloseAll();
        roomPanel.SetActive(true);
    }

    public void CloseAll()
    {
        savesPanel2D.SetActive(false);
        savesPanel3D.SetActive(false);
        roomPanel.SetActive(false);
    }

    public void Credits()
    {
        credits.SetActive(!credits.activeSelf);
        CloseAll();
    }

    public void Help()
    {
        help.SetActive(!help.activeSelf);
        CloseAll();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private string[] ListDirectories(string path)
    {
        if (Directory.Exists(path))
        {
            string[] directoryPaths = Directory.GetDirectories(path);

            string[] directoryNames = new string[directoryPaths.Length];

            for (int i = 0; i < directoryPaths.Length; i++)
            {
                directoryNames[i] = Path.GetFileName(directoryPaths[i]);
            }

            return directoryNames;
        }
        else
        {
            Debug.LogError("Directory does not exist: " + path);
            return Array.Empty<string>();
        }
    }

    private string ConvertToDate(string timestamp)
    {
        long.TryParse(timestamp, out long timestampLong);

        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestampLong).DateTime;
        DateTime localDateTime = dateTime.ToLocalTime();
        string formattedDate = localDateTime.ToString("dd-MM-yyyy HH:mm:ss");

        return formattedDate;
    }

    private Sprite GetImage(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                return sprite;
            }
            else
            {
                Debug.LogError("Failed to load image");
            }
        }
        else
        {
            Debug.LogError($"File does not exist at {imagePath}");
        }
        return null;
    }
}
