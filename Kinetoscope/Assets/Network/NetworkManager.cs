﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject observatorEyes;
	public Transform networkedObservatorEyes;

	private string ipAddress = "127.0.0.1";
	private int port = 5555;
	private bool isConnected = false;
	private bool isObservatorInstantiated = false;
	private readonly int NB_ATTEMPS = 30;
	private int currentAttempt = 0;

	// Use this for initialization
	void Start () {
		LoadConfigurations.Configurations configs = GameObject.Find("ConfigurationsManager").GetComponent<LoadConfigurations>().LoadedConfigs;
		if (null != configs) {
			if(configs.IsNetworkEnabled)
			{
				ipAddress = configs.IpAddress;
				port = configs.Port;
				Debug.Log (ipAddress + " - " + configs.Port);
				StartCoroutine (ConnectionRoutine ());
			}
		}
	}

	private IEnumerator ConnectionRoutine()
	{
		while (!isConnected) {
			Network.Connect(ipAddress,port);
			Debug.Log("Trying to connect");
			yield return new WaitForSeconds(2.0f);
			currentAttempt++;
			if(currentAttempt > NB_ATTEMPS) break;
		}
	}

	private IEnumerator CreateObservatorEyesOnOtherClient()
	{
		yield return new WaitForSeconds (15);
		networkedObservatorEyes = Network.Instantiate (observatorEyes,new Vector3(0f,0f,0f),Quaternion.identity,0) as Transform;
		networkedObservatorEyes.position += Vector3.up;
		Debug.Log ("instantiated");
		isObservatorInstantiated = true;
	}

	private void OnConnectedToServer()
	{
		//a client just joigned the server
		isConnected = true;
		if (!isObservatorInstantiated) {
			StartCoroutine (CreateObservatorEyesOnOtherClient ());
			Debug.Log ("CONNECTED MODAFUCKAH");
		}
	}

	private void OnDisconnectedToServer()
	{
		//The connection has been lost or closed
		isConnected = false;
		Destroy (observatorEyes);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
