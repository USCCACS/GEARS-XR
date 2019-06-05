using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.IO;

#if !UNITY_EDITOR
     using Windows.Storage;
     using Windows.Storage.Streams;
     using Windows.Storage.Pickers;
     using Windows.Storage.Search;
#endif


public class Graphene : MonoBehaviour {
    public Transform bondPrefab;
    public Transform carbonAtom;
    public Transform hydrogenAtom;
    public Transform oxygenAtom;
    public Transform nitrogenAtom;
    public Transform SulphurAtom;
    public Transform MoAtom;

    public string filePath;
    public double[] newCenter = new double[3];
	public double[] rotation = new double[3];
	private GrapheneUtility utility = new GrapheneUtility();

    public int frameCounter;
    public bool calledUtility = false;

	private float time = 0.0f;
	private int currentFrame = 0;
	public float interpolationPeriod = 0.03f;
    bool loadEnd = false;

    public void RenderAtoms()
    {
        for (int atomIndex = 0; atomIndex < utility.numAtoms; atomIndex++)
        {
			//Debug.Log(atomIndex + " , " +
			//    utility.atoms[atomIndex].iatom + " , " +
			//    utility.atoms[atomIndex].rr0[0] + " , " +
			//    utility.atoms[atomIndex].rr0[1] + " , " +
			//    utility.atoms[atomIndex].rr0[2]);

			//Debug.Log(atomIndex + " : " + utility.atoms[atomIndex].rr0[0] + " , " + utility.atoms[atomIndex].rr0[1] + " , " + utility.atoms[atomIndex].rr0[2]);
			float[] scaleFactorH = new float[3];
            for (int index = 0; index < 3; index++)
            {
                if (utility.scaleFactor[index] >= 1)
                    scaleFactorH[index] = 2;
                else
                    scaleFactorH[index] = (float)0.5;
            }


            switch (utility.atoms[atomIndex].iatom)
            {
                case "H": //white
					utility.atoms[atomIndex].atomInstance = Instantiate(hydrogenAtom);
                    break;
                case "N": //dark cyan
					utility.atoms[atomIndex].atomInstance = Instantiate(nitrogenAtom);
                    break;
                case "C": //red
					utility.atoms[atomIndex].atomInstance = Instantiate(carbonAtom);
                    break;
                default:
					utility.atoms[atomIndex].atomInstance = Instantiate(hydrogenAtom);
                    break;
            }
			utility.atoms[atomIndex].atomInstance.transform.parent = this.gameObject.transform;
			utility.atoms[atomIndex].atomInstance.transform.position = new Vector3(
                (float)utility.atoms[atomIndex].rr0[0],
                (float)utility.atoms[atomIndex].rr0[1],
                (float)utility.atoms[atomIndex].rr0[2]);
            switch (utility.atoms[atomIndex].iatom)
            {
                case "H":
					utility.atoms[atomIndex].atomInstance.transform.localScale = new Vector3(
                        scaleFactorH[0] * utility.atoms[atomIndex].atomInstance.transform.localScale.x * (float)utility.scaleFactor[0],
                        scaleFactorH[1] * utility.atoms[atomIndex].atomInstance.transform.localScale.y * (float)utility.scaleFactor[1],
                        scaleFactorH[2] * utility.atoms[atomIndex].atomInstance.transform.localScale.z * (float)utility.scaleFactor[2]);
                    break;
                default:
					/* atom.transform.localScale = new Vector3(
						 atom.transform.localScale.x * (float)utility.scaleFactor[0];
						 atom.transform.localScale.y * (float)utility.scaleFactor[0];
						 atom.transform.localScale.z * (float)utility.scaleFactor[0];
					 */
                    
					utility.atoms[atomIndex].atomInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					float stress = (float)utility.atoms[atomIndex].stress;
					//utility.atoms[atomIndex].atomInstance.GetComponent<Renderer>().material.color = new Color(1 - stress, 0, stress);
					break;
            }
        }
    }

    public void RenderBonds()
    {
        for(int bondIndex = 0; bondIndex < utility.numAtoms; bondIndex++)
        {
            Debug.Log(utility.l[0] + "," + utility.l[1] + "," + utility.l[2] + ", "+ bondIndex + ","+utility.atomBonds[bondIndex, 0]);
            if (utility.atomBonds[bondIndex, 0] == 0)
                continue;
            for(int neighbourIndex = 1; neighbourIndex <= utility.atomBonds[bondIndex,0]; neighbourIndex++)
            {
                Debug.Log("in");

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
                newScale.x = (float)0.1 * scale[0];
                newScale.z = (float)0.1 * scale[1];
                newScale.y = Vector3.Distance(StartPosition, EndPosition) / 2;
                bond.transform.localScale = newScale;
            }
        }
    }

    // Use this for initialization
    private bool isAsyncDone = false;
    private bool isCancelled = false;


    void Start ()
    {
        PlayerPrefs.SetInt("play_pause_flag", 1);


        utility.datadir = filePath;
		utility.numFrames = frameCounter;
        utility.NewCenter = newCenter;
		utility.rotation = rotation;
        
        //RenderBonds();

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("file_path")))
        {
            filePath = PlayerPrefs.GetString("file_path");
            PlayerPrefs.DeleteKey("file_path");
            Debug.Log("before Async ");

#if !UNITY_EDITOR
	               UnityEngine.WSA.Application.InvokeOnUIThread(()=> OpenFileAsync(), true);
#endif
            int count = 0;
            while (!isAsyncDone && !isCancelled) { Debug.Log(isAsyncDone); }
            if (isCancelled)
            {
                StartCoroutine(loadScene("testing/Scenes/Scene1"));
            }
            calledUtility = true;
            Debug.Log("after end of async, before render Atoms ");

            RenderAtoms();
            //if (utility.numAtoms <= 6000)
            //    RenderBonds();
        }
        else
        {
            LoadDefaultFile();
        }
    }


    public void LoadDefaultFile()
    {
        utility.datadir = filePath;
        utility.numFrames = frameCounter;
        utility.NewCenter = newCenter;
        utility.rotation = rotation;
        utility.ReadFrames();
        utility.GetStructureData();
        calledUtility = true;
        RenderAtoms();

    }
    // Update is called once per frame
    void Update () {
        /*if (NetworkServer.connections.Count == 2 && !loadEnd)
        {
            for (int atomIndex = 0; atomIndex < utility.numAtoms; atomIndex++)
            {
                NetworkServer.Spawn(utility.atoms[atomIndex].atomInstance.gameObject);
            }
            loadEnd = true;
        }
        */
        time += Time.deltaTime;

        Debug.Log("pause play val:"+ PlayerPrefs.GetInt("play_pause_flag"));
        if(PlayerPrefs.GetInt("play_pause_flag") == 1) { 
		    if (time >= interpolationPeriod)
		    {
			    time = 0.0f;
			    if (currentFrame + 1 > frameCounter)
				    currentFrame = 0;
			    else
			    {
				    for(int atomIndex = 0; atomIndex < utility.numAtoms; atomIndex++)
				    {
					    utility.atoms[atomIndex].atomInstance.transform.position = utility.atoms[atomIndex].framePos[currentFrame];
                        //float stress = (float)utility.atoms[atomIndex].frameStress[currentFrame];
                        //utility.atoms[atomIndex].atomInstance.GetComponent<Renderer>().material.color = new Color(1 - stress, 0, stress, 1);

                    }
				    currentFrame++;
			    }
			
		    }
        }
    }

    private byte[] byteArray;

#if !UNITY_EDITOR
         private async void OpenFileAsync()
         {
             Debug.Log("In Async ");
             var folderPicker = new Windows.Storage.Pickers.FolderPicker();
             folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
             folderPicker.FileTypeFilter.Add("*");

             Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                IReadOnlyList<StorageFile> sortedItems = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
                utility.numFrames = sortedItems.Count-1;
                frameCounter = sortedItems.Count-1;
                List<StreamReader> stream_reader_files = new List<StreamReader>();
                Debug.Log("total number of files: "+sortedItems.Count);
                foreach (StorageFile file in sortedItems){

                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
			        using (var inputStream = stream.GetInputStreamAt(0))
			        {			
				        var reader = new DataReader(inputStream);
				        byteArray = new byte[stream.Size];
				        await reader.LoadAsync((uint)stream.Size);
				        reader.ReadBytes(byteArray);
                        stream_reader_files.Add(new StreamReader(new MemoryStream(byteArray)));
                    }
                }
                Debug.Log("the stream_reader_files length is:"+stream_reader_files.Count);
                utility.ReadFrames(stream_reader_files);
                utility.GetStructureData();
                isAsyncDone = true;
                Debug.Log("GetStructureData in async in working.");
                
            }
            else
            {
                Debug.Log("cancelled. clicked");
			    isCancelled = true;
            }
         }
#endif

    private AsyncOperation sceneAsync;
    
    public GameObject tranferGameObject;

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
