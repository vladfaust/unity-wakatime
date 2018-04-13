using UnityEngine;
using UnityEditor;

namespace WakaTime {
  public class Window : EditorWindow {
    string apiKey = "";
    private bool _enabled = true;

    const string DASHBOARD_URL = "https://wakatime.com/dashboard/";

    [MenuItem("Window/WakaTime")]
    static void Init() {
      Window window = (Window)EditorWindow.GetWindow(typeof(Window), false, "WakaTime");
      window.Show();
    }

    void OnGUI() {
      apiKey = EditorGUILayout.TextField("API key", apiKey);
      _enabled = EditorGUILayout.Toggle("Enabled", _enabled);

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Save Preferences")) {
        EditorPrefs.SetString(Plugin.PREFS_PATH, apiKey);
        EditorPrefs.SetBool(Plugin.ENABLED_PREF, _enabled);
        Plugin.Initialize();
      }

      if (GUILayout.Button("Open Dashboard"))
        Application.OpenURL(DASHBOARD_URL);

      EditorGUILayout.EndHorizontal();
    }

    void OnFocus() {
      if (EditorPrefs.HasKey(Plugin.PREFS_PATH))
        apiKey = EditorPrefs.GetString(Plugin.PREFS_PATH);
      if (EditorPrefs.HasKey(Plugin.ENABLED_PREF))
        _enabled = EditorPrefs.GetBool(Plugin.ENABLED_PREF);
    }
  }
}
