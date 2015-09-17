using UnityEngine;
using System.Collections;

public class ShakingObject : MonoBehaviour, Triggerable {

	//Shake stuff
	Vector3 actualPos;
	//Vector3 shakeAmount;
	int shakeTimer = 0;
	public int startShakeTimer = 0;
	public float shakeAmount = 0;

	// Use this for initialization
	void Start () {
		actualPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (shakeTimer > 0) {
			Vector3 diff = Vector3.Normalize(new Vector3(Random.value, Random.value, Random.value)) * Mathf.Lerp (0f, shakeAmount, (shakeTimer*1f)/startShakeTimer);
			this.transform.position = actualPos + diff;
			shakeTimer--;
		}
	}

	void Triggerable.Trigger(){
		shakeTimer = startShakeTimer;
	}

	void Triggerable.UnTrigger(){

	}
}
