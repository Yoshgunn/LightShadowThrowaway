using UnityEngine;
using System.Collections;

public class ButtonNextLevel : MonoBehaviour {

	public void LoadLevel(string levelName){
		//Take care of ending the current scene
		GameController.EndLevel (levelName);

		//Application.LoadLevel (levelName);
	}
}
