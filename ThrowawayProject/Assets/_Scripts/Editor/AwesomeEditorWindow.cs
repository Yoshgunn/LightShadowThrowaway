using UnityEditor;

public class AwesomeEditorWindow : EditorWindow {

	[CanEditMultipleObjects]
	[MenuItem ("Awesome Stuff/Awesome Window")]
	public static void ShowAwesomeEditorWindow(){
		var window = GetWindow <AwesomeEditorWindow> ();
		window.title = "The Window";
	}

	public void OnGUI(){

	}

	public void DoCanvas(){

	}
}
