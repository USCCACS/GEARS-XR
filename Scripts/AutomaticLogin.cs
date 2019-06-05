using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class AutomaticLogin : MonoBehaviour {
    private NetworkManager m_networkManager;
	// Use this for initialization
	void Start () {
        m_networkManager = gameObject.GetComponent<NetworkManager>();
        m_networkManager.networkAddress = "192.168.86.28";
        m_networkManager.StartClient();
  }
}
