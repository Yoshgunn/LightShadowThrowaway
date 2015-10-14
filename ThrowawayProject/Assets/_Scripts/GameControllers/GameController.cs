using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public static GameController ENV;
	public static int FPS = 24;

	public GameObject finalNodeGO;

	private int torchLightTimer = 30;
	private int fadeOutTimer = 0;
	private string levelToLoad;
	private Node finalNode;
	private bool levelOver = false;

	// Use this for initialization
	public virtual void Start () {
		ENV = this;

		if (finalNodeGO) {
			finalNode = finalNodeGO.GetComponentInChildren<Node> ();
		}
	}

	public virtual void EndInstance (string levelName){
		Node.EndScene ();
		Boundary.EndScene ();
		Torch.EndScene ();
		ENV = null;

		levelToLoad = levelName;
		fadeOutTimer = Torch.DEFAULT_FLICKER_OFF_TIME + 30;
		Application.LoadLevel (levelName);
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
		//Wait a bit, then start the lights up
		if (torchLightTimer >= 0) {
			torchLightTimer--;
			if (torchLightTimer==0){
				//First, turn on aesthetic player light
				//TODO: we might need to make this more robust if there are a lot of 'aesthetic' lights
				if (PathfindingPlayer.PLAYER)
					PathfindingPlayer.PLAYER.transform.GetChild (0).transform.GetComponent<Light>().enabled = true;

				foreach (Torch t in Torch.allTorches) {
					if (t.startsOn){
						t.GetComponent<MyLight>().FlickerOn ();
					}
				}
			}
		}

		//Wait a bit before loading the next level
		if (fadeOutTimer > 0) {
			fadeOutTimer--;
			if (fadeOutTimer==0){
				Application.LoadLevel(levelToLoad);
			}else if (fadeOutTimer==30){
				if (PathfindingPlayer.PLAYER)
					PathfindingPlayer.PLAYER.transform.GetChild (0).transform.GetComponent<Light>().enabled = false;
			}
		}

		//If the player is on the final node, end the level
		if (PathfindingPlayer.PLAYER.GetCurrentNode () == finalNode && PathfindingPlayer.PLAYER.GetTargetNode () == null && !levelOver) {
			//levelOver = true;
			//EndLevel("Menu");
			WinLevel ();

		}
	}

	public static void EndLevel(string levelName){
		if (ENV) {
			ENV.EndInstance (levelName);
		}
	}

	public void WinLevel(){
		//Winning the level:
		//	First, fade all of the lights out
		//	Then, get the name of the next level
		//	Then, load that level

		levelOver = true;
		EndInstance(GetNameOfNextLevel ());
	}

	private static string GetNameOfNextLevel(){
		string thisLevel = Application.loadedLevelName;
		Debug.Log (thisLevel);
		int i = thisLevel.IndexOf ('.');
		string levelPrefix = thisLevel.Substring(0, i+1);
		string levelSuffix = thisLevel.Substring(i+1);
		int levelNum = Int32.Parse (levelSuffix);
		levelNum++;
		thisLevel = levelPrefix + levelNum;
		return thisLevel;
	}
}
