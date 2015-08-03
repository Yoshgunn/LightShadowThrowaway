using UnityEngine;
using System.Collections;

public class ButtonNextLevel : MonoBehaviour {

	public void LoadLevel(string levelName){
		//Take care of ending the current scene
		//TODO: This should probably be done in the gamecontroller rather than here.
		Node.EndScene ();
		Boundary.EndScene ();

		Application.LoadLevel (levelName);
	}
}
