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
			Pause();
		}
	}

	void Pause ()
	{
		PauseMenu.SetActive ( true );
		isPlaying = false;
		Time.timeScale = 0.0f;
	}

	public void Skip ( )
	{
		// In the future this will access the "NextLevel" which will be set somewhere
		Time.timeScale = 1.0f;
		Application.LoadLevel ( 1 );
	}

	public void Unpause ( )
	{
		PauseMenu.SetActive ( false );
		isPlaying = true;
		Time.timeScale = 1.0f;
	}
}
