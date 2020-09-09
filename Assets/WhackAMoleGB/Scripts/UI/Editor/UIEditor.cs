using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI))]
public class UIEditor : Editor {
	public override void OnInspectorGUI() {
		UI t = target as UI;
		base.OnInspectorGUI();
		GUILayout.BeginHorizontal(EditorStyles.helpBox);
			if(GUILayout.Button("Show Startscreen")) { t.startScreen.Show(); }
			if(GUILayout.Button("Hide Startscreen")) t.startScreen.Hide();
		GUILayout.EndHorizontal();
	}
}