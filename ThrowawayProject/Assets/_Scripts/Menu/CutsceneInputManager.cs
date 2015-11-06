using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CutsceneInputManager : MonoBehaviour 
{
	public GameObject PauseMenu;
	public bool isPlaying;
	public float nextClick;
	public float CLICK_COOLDOWN;
	public EventSystem ourEventSystem;

	void Start () 
	{
		Debug.Log ("Test");
	}

	void Update ()
	{
		if ( Input.GetMouseButton ( 0 ) && isPlaying )
		{
			PauseMenu.SetActive ( true );
			isPlaying = false;
		}
	}

	public void Skip ( )
	{
		// In the future this will access the "NextLevel" which will be set somewhere

		Application.LoadLevel ( 1 );
	}

	public void Unpause ( )
	{
		Debug.Log ( "Unpause!!" );
		PauseMenu.SetActive ( false );
		isPlaying = true;
	}
}
