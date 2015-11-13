using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamTestGameController : GameController {

	bool playerStanding = false;

	// Use this for initialization
	public void Start () {
		base.Start ();
	}

	public void EndInstance (int levelNum){
		base.EndInstance (levelNum);
	}
	
	// Update is called once per frame
	public void Update () {

		if (!playerStanding) {
			PathfindingPlayer.PLAYER.StandUp ();
			playerStanding=true;
		}

		base.Update ();
	}

	/*public static void EndLevel(string levelName){
		if (ENV) {
			ENV.EndInstance (levelName);
		}
	}*/
}
