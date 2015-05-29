using UnityEngine;
using System.Collections;

public class ClickDetector : MonoBehaviour {

	private Node node;

	// Use this for initialization
	void Start () {
		this.node = this.transform.parent.GetComponent<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		Node.FindPath (this.node);
	}
}
