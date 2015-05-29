using UnityEngine;
using System.Collections;

public class Connector : MonoBehaviour {

	private GameObject node;
	private GameObject connectedTo;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetNode (GameObject go){
		this.node = go;
	}
	
	public GameObject GetNode (){
		return this.node;
	}

	public void SetConnectedTo(GameObject go){
		this.connectedTo = go;
	}

	public GameObject GetConnectedTo(){
		return this.connectedTo;
	}
}
