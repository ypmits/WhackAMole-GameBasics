using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LifeCounterObjects))]
public class MistakeObjectsEditor : Editor {
	public override void OnInspectorGUI() {
		LifeCounterObjects t = target as LifeCounterObjects;
		base.OnInspectorGUI();
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal(EditorStyles.helpBox);
			if(GUILayout.Button("Show")) { t.Show(); }
			if(GUILayout.Button("Hide")) t.Hide();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(EditorStyles.helpBox);
			if(GUILayout.Button("Next")) { t.TakeLife(); }
			if(GUILayout.Button("Reset")) t.Reset();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}