using UnityEngine;
using UnityEditor;

namespace WakaTime {
  public class Window : EditorWindow {
    string apiKey = "";

    const string DASHBOARD_URL = "https://wakatime.com/dashboard/";

    [MenuItem("Window/WakaTime")]
    static void Init() {
      Window window = (Window)EditorWindow.GetWindow(typeof(Window), false, "WakaTime");
      window.Show();
    }

    void OnGUI() {
      apiKey = EditorGUILayout.TextField("API key", apiKey);

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Save API Key")) {
        EditorPrefs.SetString(Plugin.PREFS_PATH, apiKey);
        Plugin.Initialize();
      }

      if (GUILayout.Button("Open Dashboard"))
        Application.OpenURL(DASHBOARD_URL);

      EditorGUILayout.EndHorizontal();
    }

    void OnFocus() {
      if (EditorPrefs.HasKey(Plugin.PREFS_PATH))
        apiKey = EditorPrefs.GetString(Plugin.PREFS_PATH);
    }
  }
}
