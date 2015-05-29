using UnityEngine;
using System.Collections;

public class ButtonNextLevel : MonoBehaviour {

	public void LoadLevel(string levelName){
		Application.LoadLevel (levelName);
	}
}
