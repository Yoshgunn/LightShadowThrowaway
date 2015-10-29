using UnityEngine;
using System.Collections;

public class ButtonNextLevel : MonoBehaviour {

	public void LoadLevel(int levelNum){
		//Take care of ending the current scene
		GameController.EndLevel (levelNum);

		//Application.LoadLevel (levelName);
	}
}
