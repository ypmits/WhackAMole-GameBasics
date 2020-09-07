using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CountDown))]
public class CountDownEditor : Editor {
	public override void OnInspectorGUI() {
		CountDown t = target as CountDown;
		base.OnInspectorGUI();
		GUILayout.BeginHorizontal();
			if(GUILayout.Button("Start")) {
				t.StartCountDown();
			}
		GUILayout.EndHorizontal();
	}
}