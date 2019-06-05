using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

#if !UNITY_EDITOR
     using Windows.Storage;
     using Windows.Storage.Streams;
     using Windows.Storage.Pickers;
#endif

public class NanoCarbons : MonoBehaviour
{
    public Transform bondPrefab;
    public Transform carbonAtom;
    public Transform hydrogenAtom;
    public Transform oxygenAtom;
    public Transform nitrogenAtom;
    public Transform SulphurAtom;
    public Transform MoAtom;
    private byte[] byteArray;
    public GameObject tranferGameObject;

    public string filePath;
    public double[] newCenter = new double[3];
    private Utility utility = new Utility();

    public int frameCounter;
    public bool calledUtility = false;



    private float time = 0.0f;
    private int currentFrame = 0;
    public float interpolationPeriod = 0.03f;
    bool loadEnd = false;

#if !UNITY_EDITOR
         private FileOpenPicker openPicker;
#endif

    public void RenderAtoms()
    {
        for (int atomIndex = 0; atomIndex < utility.numAtoms; atomIndex++)
        {
            //Debug.Log(atomIndex + " , " +
            //    utility.atoms[atomIndex].iatom + " , " +
            //    utility.atoms[atomIndex].rr0[0] + " , " +
            //    utility.atoms[atomIndex].rr0[1] + " , " +
            //    utility.atoms[atomIndex].rr0[2]);

            float[] scaleFactorH = new float[3];
            for (int index = 0; index < 3; index++)
            {
                if (utility.scaleFactor[index] >= 1)
                    scaleFactorH[index] = 2;
                else
                    scaleFactorH[index] = (float)0.5;
            }

            Transform atom = null;
            switch (utility.atoms[atomIndex].iatom)
            {
                case "H": //white
                    atom = Instantiate(hydrogenAtom);
                    break;
                case "C": //dark cyan
                    atom = Instantiate(carbonAtom);
                    break;
                case "O": //red
                    atom = Instantiate(oxygenAtom);
                    break;
                case "N": //blue
                    atom = Instantiate(nitrogenAtom);
                    break;
                case "S":
                    atom = Instantiate(SulphurAtom);
                    break;
                case "Mo":
                    atom = Instantiate(MoAtom);
                    break;
                default:
                    atom = Instantiate(hydrogenAtom);
                    break;
            }
            utility.atoms[atomIndex].atomInstance = atom;
            utility.atoms[atomIndex].atomInstance.transform.parent = this.gameObject.transform;
            utility.atoms[atomIndex].atomInstance.transform.position = new Vector3(
                (float)utility.atoms[atomIndex].rr0[0],
                (float)utility.atoms[atomIndex].rr0[1],
                (float)utility.atoms[atomIndex].rr0[2]);
            switch (utility.atoms[atomIndex].iatom)
            {
                case "H":
                    utility.atoms[atomIndex].atomInstance.transform.localScale = new Vector3(
                        scaleFactorH[0] * atom.transform.localScale.x * (float)utility.scaleFactor[0],
                        scaleFactorH[1] * atom.transform.localScale.y * (float)utility.scaleFactor[0],
                        scaleFactorH[2] * atom.transform.localScale.z * (float)utility.scaleFactor[0]);
                    break;
                default:
                    /*utility.atoms[atomIndex].atomInstance.transform.localScale = new Vector3(
                        atom.transform.localScale.x * (float)utility.scaleFactor[0],
                        atom.transform.localScale.y * (float)utility.scaleFactor[0],
                        atom.transform.localScale.z * (float)utility.scaleFactor[0]);*/
                    utility.atoms[atomIndex].atomInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    break;
            }
        }
    }

    public void RenderBonds()
    {
        for (int bondIndex = 0; bondIndex < utility.numAtoms; bondIndex++)
        {
            
            if (utility.atomBonds[bondIndex, 0] == 0)
                continue;
            for (int neighbourIndex = 1; neighbourIndex <= utility.atomBonds[bondIndex, 0]; neighbourIndex++)
            {
                Transform bond = Instantiate(bondPrefab);
                //bond.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
                bond.transform.parent = this.gameObject.transform;

                Vector3 StartPosition = new Vector3(
                    (float)utility.atoms[bondIndex].rr0[0],
                    (float)utility.atoms[bondIndex].rr0[1],
                    (float)utility.atoms[bondIndex].rr0[2]);
                Vector3 EndPosition = new Vector3(
                    (float)utility.atoms[utility.atomBonds[bondIndex, neighbourIndex]].rr0[0],
                    (float)utility.atoms[utility.atomBonds[bondIndex, neighbourIndex]].rr0[1],
                    (float)utility.atoms[utility.atomBonds[bondIndex, neighbourIndex]].rr0[2]);

                Vector3 position = Vector3.Lerp(StartPosition, EndPosition, (float)0.5);
                bond.transform.position = position;
                bond.transform.up = EndPosition - StartPosition;

                float[] scale = new float[2] { (float)utility.scaleFactor[0], (float)utility.scaleFactor[2] };
                Vector3 newScale = bond.transform.localScale;
                newScale.x = (float)0.2 * scale[0];
                newScale.z = (float)0.2 * scale[0];
                newScale.y = Vector3.Distance(StartPosition, EndPosition) / 2;
                bond.transform.localScale = newScale;
            }
        }
    }

    // Use this for initialization
    private bool isAsyncDone = false;
    private bool isCancelled = false;

    void Start()
    {
     
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("file_path")))
        {
            filePath = PlayerPrefs.GetString("file_path");
            PlayerPrefs.DeleteKey("file_path");

            #if !UNITY_EDITOR
					 UnityEngine.WSA.Application.InvokeOnUIThread(()=> OpenFileAsync(), true);
            #endif

            while (!isAsyncDone && !isCancelled) { Debug.Log(isAsyncDone); }
            if (isCancelled)
            {
                StartCoroutine(loadScene("testing/Scenes/Scene1"));
            }
            calledUtility = true;
            Debug.Log("after end of async, before render Atoms ");

            RenderAtoms();
            if (utility.numAtoms <= 6000)
                RenderBonds();
        }
        else
        {
            LoadDefaultFile();
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (NetworkServer.connections.Count == 2 && !loadEnd)
        {
            for (int atomIndex = 0; atomIndex < utility.numAtoms; atomIndex++)
            {
                Debug.Log(utility.atoms[atomIndex].atomInstance.gameObject);
                NetworkServer.Spawn(utility.atoms[atomIndex].atomInstance.gameObject);
            }
            loadEnd = true;

        }
        
    }

    public void LoadDefaultFile()
    {
        Debug.Log(filePath);
        utility.FilePath = filePath;
        utility.NewCenter = newCenter;
        utility.GetStructureData();
        calledUtility = true;

        Debug.Log("after Util, before render Atoms ");

        RenderAtoms();
        if (utility.numAtoms <= 6000)
            RenderBonds();

    }


#if !UNITY_EDITOR
     private async void OpenFileAsync()
     {
         FileOpenPicker openPicker = new FileOpenPicker();
         openPicker.ViewMode = PickerViewMode.Thumbnail;
         openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
         openPicker.FileTypeFilter.Add(".txt");

         StorageFile file = await openPicker.PickSingleFileAsync();
         if (file != null)
         {
             // Application now has read/write access to the file
			var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
			using (var inputStream = stream.GetInputStreamAt(0))
			{
			
				var reader = new DataReader(inputStream);
				byteArray = new byte[stream.Size];
				await reader.LoadAsync((uint)stream.Size);
				reader.ReadBytes(byteArray);
				utility.ReadFile(new StreamReader(new MemoryStream(byteArray)));

				utility.NewCenter = newCenter;
                Debug.Log("In Async before util.SyncFunction called");
				utility.syncFunctions();
				Debug.Log("In Async after util.SyncFunction called");
				isAsyncDone = true;
                Debug.Log(isAsyncDone);
				
			}
         }
         else
         {
            Debug.Log("cancelled. clicked");
			isCancelled = true;
         }
     }
#endif

    private AsyncOperation sceneAsync;
    IEnumerator loadScene(string index)
    {
        Scene m_Scene = SceneManager.GetActiveScene();
        if (m_Scene.name.Equals("Scene1"))
        {
            //GameObject.Find("MixedRealityCameraParent");
        }
        AsyncOperation scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        scene.allowSceneActivation = false;
        sceneAsync = scene;

        //Wait until we are done loading the scene
        while (scene.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + scene.progress);
            yield return null;
        }
        OnFinishedLoadingAllScene(index);
    }

    void enableScene(string index)
    {
        //Activate the Scene
        sceneAsync.allowSceneActivation = true;


        Scene sceneToLoad = SceneManager.GetSceneByName(index);
        if (sceneToLoad.IsValid())
        {
            Debug.Log("Scene is Valid");
            SceneManager.MoveGameObjectToScene(tranferGameObject, sceneToLoad);
            SceneManager.SetActiveScene(sceneToLoad);
        }
    }

    void OnFinishedLoadingAllScene(string index)
    {
        Debug.Log("Done Loading Scene");
        enableScene(index);
        Debug.Log("Scene Activated!");
    }
}
