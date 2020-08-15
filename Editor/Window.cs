using UnityEngine;
using UnityEditor;

namespace WakaTime {
  public class Window : EditorWindow {
    private string _apiKey = "";
    private string _projectName = "";
    private bool _enabled = true;
    private bool _debug = true;

    private bool _needToReload;

    const string DASHBOARD_URL = "https://wakatime.com/dashboard/";

    [MenuItem("Window/WakaTime")]
    static void Init() {
      Window window = (Window) GetWindow(typeof(Window), false, "WakaTime");
      window.Show();
    }

    void OnGUI() {
      _enabled = EditorGUILayout.Toggle("Enable WakaTime", _enabled);
      _apiKey = EditorGUILayout.TextField("API key", _apiKey);
      EditorGUILayout.LabelField("Project name", _projectName);

      if (GUILayout.Button("Change project name")) {
        ProjectEditWindow.Display();
        _needToReload = true;
      }

      _debug = EditorGUILayout.Toggle("Debug", _debug);

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Save Preferences")) {
        EditorPrefs.SetString(Plugin.API_KEY_PREF, _apiKey);
        EditorPrefs.SetBool(Plugin.ENABLED_PREF, _enabled);
        EditorPrefs.SetBool(Plugin.DEBUG_PREF, _debug);
        Plugin.Initialize();
      }

      if (GUILayout.Button("Open Dashboard"))
        Application.OpenURL(DASHBOARD_URL);

      EditorGUILayout.EndHorizontal();
    }

    void OnFocus() {
      if (_needToReload) {
        Plugin.Initialize();
        _needToReload = false;
      }

      if (EditorPrefs.HasKey(Plugin.API_KEY_PREF))
        _apiKey = EditorPrefs.GetString(Plugin.API_KEY_PREF);
      if (EditorPrefs.HasKey(Plugin.ENABLED_PREF))
        _enabled = EditorPrefs.GetBool(Plugin.ENABLED_PREF);
      if (EditorPrefs.HasKey(Plugin.DEBUG_PREF))
        _debug = EditorPrefs.GetBool(Plugin.DEBUG_PREF);

      _projectName = Plugin.ProjectName;
    }
  }
}
