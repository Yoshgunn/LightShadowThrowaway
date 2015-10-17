using UnityEngine;
using System.Collections;

public class ShakingObject : MonoBehaviour, Triggerable {

	//Shake stuff
	Vector3 actualPos;
	//Vector3 shakeAmount;
	float shakeTimer = 0f;
	public float startShakeTimer = 0.5f;
	public float shakeAmount = 0.1f;

	// Use this for initialization
	void Start () {
		actualPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (shakeTimer > 0) {
			Vector3 diff = Vector3.Normalize(new Vector3(Random.value-0.5f, Random.value-0.5f, Random.value-0.5f)) * Mathf.Lerp (0f, shakeAmount, shakeTimer/startShakeTimer);
			this.transform.position = actualPos + diff;
			shakeTimer-=Time.deltaTime;
			if (shakeTimer <= 0){
				Node[] nodes = transform.GetComponentsInChildren<Node>();
				foreach (Node n in nodes){
					n.RecalculateEdges();
				}
			}
		}
	}

	void Triggerable.Trigger(){
		shakeTimer = startShakeTimer;
	}

	void Triggerable.UnTrigger(){

	}
}
