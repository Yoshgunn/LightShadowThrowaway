using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StressTestGameController : GameController {

	public GameObject upperLevel;
	public GameObject cam;

	// Use this for initialization
	public void Start () {
		base.Start ();

		cam.GetComponent<MyCamera> ().SetMode (4, new Vector3 (10,10,10));
	}

	public void EndInstance (int levelNum){
		base.EndInstance (levelNum);
	}
	
	// Update is called once per frame
	public void Update () {

		//Take care of the upper level appearing
		if (PathfindingPlayer.PLAYER.transform.position.y >= 3 && !upperLevel.gameObject.activeSelf) {
			upperLevel.gameObject.SetActive(true);
		}

		base.Update ();
	}

	/*public static void EndLevel(string levelName){
		if (ENV) {
			ENV.EndInstance (levelName);
		}
	}*/
}
