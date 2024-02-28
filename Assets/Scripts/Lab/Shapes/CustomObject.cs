

using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using Dummiesman;
//using Application = UnityEngine.Application;
using UnityEditor;
using SFB;

public class CustomObject : MonoBehaviour
{
    //private static extern void OpenFileDialog();
    public void OpenFile()
    {
        var extensions = new[] {
            new ExtensionFilter("Obj Files", "obj"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Object File", "", extensions, false);


        // OpenFileDialog openFileDialog = new OpenFileDialog();
        // openFileDialog.Filter = "obj files (*.obj)|*.obj";
        // openFileDialog.RestoreDirectory = true;

        if (paths.Length != 0)
        {
            // Pobierz ścieżkę do wybranego pliku
            //string filePath = paths.FileName;
            bool is3d = FindObjectOfType<EditManager>().is3d;
            // Wczytaj obiekt .obj z wybranej ścieżki
            GameObject loadedObject = new OBJLoader().Load(paths[0]);
            loadedObject.name = "CO";
            loadedObject.AddComponent<BoxCollider2D>();
            loadedObject.AddComponent<MeshRenderer>();
            loadedObject.AddComponent<Glass>();
            loadedObject.tag = "reflect";

            // Zmień materiał obiektu na nowy materiał
            Material newMat;
            if(is3d)
                newMat = Resources.Load("glass3d", typeof(Material)) as Material;
            else
                newMat = Resources.Load("glass", typeof(Material)) as Material;

            loadedObject.GetComponent<Renderer>().material = newMat;

            foreach (Transform child in loadedObject.transform)
            {
                child.gameObject.AddComponent<MeshCollider>();
                // Sprawdź, czy podobiekt ma komponent Renderer
                if (child.GetComponent<Renderer>())
                {
                    // Zmień materiał podobiektu 
                    
                    child.GetComponent<Renderer>().material = newMat;

                }
            }
            Vector3 screenCenter;
            if(is3d){
                Vector3 cameraPosition = Camera.main.transform.position;

                // Pobierz kierunek, w którym kamera jest skierowana
                Vector3 cameraDirection = Camera.main.transform.forward;

                // Ustal miejsce, w którym chcesz stworzyć obiekt
                screenCenter = cameraPosition + cameraDirection * 10; // distance to be defined
            }else
                screenCenter = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

            loadedObject.transform.position = screenCenter;
            // Dodaj obiekt do sceny
            //GameObject co = Instantiate(loadedObject, screenCenter, Quaternion.identity);

            //Destroy(loadedObject);
        }

    }

}
