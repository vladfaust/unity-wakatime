using UnityEngine;
using UnityEditor;

namespace WakaTime {
  public class Window : EditorWindow {
    private string _apiKey = "";
    private bool _enabled = true;

    const string DASHBOARD_URL = "https://wakatime.com/dashboard/";

    [MenuItem("Window/WakaTime")]
    static void Init() {
      Window window = (Window)EditorWindow.GetWindow(typeof(Window), false, "WakaTime");
      window.Show();
    }

    void OnGUI() {
      _apiKey = EditorGUILayout.TextField("API key", _apiKey);
      _enabled = EditorGUILayout.Toggle("Enabled", _enabled);

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Save Preferences")) {
        EditorPrefs.SetString(Plugin.API_KEY_PREF, _apiKey);
        EditorPrefs.SetBool(Plugin.ENABLED_PREF, _enabled);
        Plugin.Initialize();
      }

      if (GUILayout.Button("Open Dashboard"))
        Application.OpenURL(DASHBOARD_URL);

      EditorGUILayout.EndHorizontal();
    }

    void OnFocus() {
      if (EditorPrefs.HasKey(Plugin.API_KEY_PREF))
        _apiKey = EditorPrefs.GetString(Plugin.API_KEY_PREF);
      if (EditorPrefs.HasKey(Plugin.ENABLED_PREF))
        _enabled = EditorPrefs.GetBool(Plugin.ENABLED_PREF);
    }
  }
}
