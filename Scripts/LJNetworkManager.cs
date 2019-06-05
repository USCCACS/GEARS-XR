using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LJNetworkManager : NetworkManager
{
    [SerializeField]
    MolecularDynamics md;
    private bool isSpawned = false;


    void Start()
    {
        Debug.Log(playerPrefab.ToString());
        //md = new MolecularDynamics();
        md.BaseParticle = spawnPrefabs[0];
        Scene m_Scene = SceneManager.GetActiveScene();
        if (m_Scene.name.Equals("Demo4(LJ)"))
        {
            NetworkManager m_networkManager = gameObject.GetComponent<NetworkManager>();
            m_networkManager.StartHost();
            
        }
    }

    public override void OnStartServer()
    {
        LogFilter.currentLogLevel = 1;
        Debug.Log("RookiesManager: OnServerStart:  ");
        //md = new MolecularDynamics(playerPrefab);
        md.Init();
        //while(!NetworkServer.active)
        //{

        //}
        //md.Spawn();
    }

    private void Update()
    {
        if (NetworkServer.active && isSpawned == false)
        {
            md.Spawn();
            isSpawned = true;
        }
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        md.Destroy();
    }
}
