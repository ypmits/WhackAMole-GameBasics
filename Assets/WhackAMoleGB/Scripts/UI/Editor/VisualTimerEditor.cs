using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisualTimer))]
public class VisualTimerEditor : Editor {
	private float _duration = 10f;
	public override void OnInspectorGUI() {
		VisualTimer t = target as VisualTimer;
		base.OnInspectorGUI();
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal(EditorStyles.helpBox);
				_duration = EditorGUILayout.FloatField(new GUIContent("Duration"), _duration);
				if(GUILayout.Button("Setup")) t.Setup(_duration);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				if(GUILayout.Button("Start")) t.StartTimer();
				if(GUILayout.Button("Pause")) t.Pause();
				if(GUILayout.Button("Stop")) t.Reset();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				if(GUILayout.Button("Restart")) { t.Reset(); t.StartTimer(); }
				if(GUILayout.Button("Add 1 second")) t.AddTime(1f);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				if(GUILayout.Button("Show")) { t.Show(); }
				if(GUILayout.Button("Hide")) t.Hide();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}