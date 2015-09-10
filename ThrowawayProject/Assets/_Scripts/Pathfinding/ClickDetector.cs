using UnityEngine;
using System.Collections;

public class ClickDetector : MonoBehaviour {

	private const float COLOR_FADE_RATE = 0.03f;
	private const float COLOR_FADE_RATIO = 0.95f;

	private Node node;
	private Renderer rend;
	private Color color = new Color(0,0,0,0);

	// Use this for initialization
	void Start () {
		this.node = this.transform.parent.GetComponent<Node> ();
		rend = this.transform.GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (rend.enabled) {
			if (color.a > 0.01f) {
				//color.a -= COLOR_FADE_RATE;
				color.a *= COLOR_FADE_RATIO;
				rend.material.color = color;
				Debug.Log ("Color: " + color);
			} else {
				color.a = 0;
				rend.material.color = color;
				rend.enabled = false;
				Debug.Log ("Turn off renderer");
			}
		}
	}

	void OnMouseDown(){
		Activate ();
	}

	public void Activate(){
		Debug.Log ("ACTIVATING!");
		if (MyCamera.CAM) {
			//MyCamera.CAM.ShakeCamera (1, 40);
		}

		//Do the light fade effect
		if (rend) {
			rend.enabled = true;
			color = rend.material.color;
			color.a = 1;
			rend.material.color = color;
		}

		Node.FindPath (this.node);
	}
}
