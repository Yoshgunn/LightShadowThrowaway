using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuInputManager : MonoBehaviour 
{
	public GameObject mainMenu;
	public GameObject pauseMenu;
	public GameObject pauseButton;
	public Color brightWhite;
	public Color fadedWhite;

	public void Play ( )
	{
		Debug.Log ( "Hide that menu!" );

		mainMenu.SetActive ( false );
	}

	public void TogglePauseMenu ( )
	{
		pauseMenu.SetActive ( !pauseMenu.activeSelf );

		if ( pauseMenu.activeSelf == true)
			pauseButton.GetComponent<Image>().color = brightWhite;
		else {
			pauseButton.GetComponent<Image>().color = fadedWhite;
		}
	}
}
