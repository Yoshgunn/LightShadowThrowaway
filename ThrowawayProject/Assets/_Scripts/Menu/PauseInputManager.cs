using UnityEngine;
using System.Collections;

public class PauseInputManager : MonoBehaviour 
{
	public GameObject PauseMenu;

	public void PauseMenuToggle ()
	{
		PauseMenu.SetActive ( !PauseMenu.activeSelf );
	}
}
