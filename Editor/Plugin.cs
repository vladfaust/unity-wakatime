#if (UNITY_EDITOR)

using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;

// Heavily inspired by https://github.com/bengsfort/WakaTime-Unity

namespace WakaTime {
  [InitializeOnLoad]
  public class Plugin {
    public const string API_KEY_PREF = "WakaTime/APIKey";
    public const string ENABLED_PREF = "WakaTime/Enabled";
    public const string DEBUG_PREF = "WakaTime/Debug";
    public const string WAKATIME_PROJECT_FILE = ".wakatime-project";

    public static string ProjectName { get; private set; }

    private static string _apiKey = "";
    private static bool _enabled = true;
    private static bool _debug = true;

    private const string URL_PREFIX = "https://api.wakatime.com/api/v1/";
    private const int HEARTBEAT_COOLDOWN = 120;

    private static HeartbeatResponse _lastHeartbeat;

    static Plugin() {
      Initialize();
    }

    public static void Initialize() {
      if (EditorPrefs.HasKey(ENABLED_PREF))
        _enabled = EditorPrefs.GetBool(ENABLED_PREF);

      if (EditorPrefs.HasKey(DEBUG_PREF))
        _debug = EditorPrefs.GetBool(DEBUG_PREF);

      if (!_enabled) {
        if (_debug) Debug.Log("<WakaTime> Explicitly disabled, skipping initialization...");
        return;
      }

      if (EditorPrefs.HasKey(API_KEY_PREF)) {
        _apiKey = EditorPrefs.GetString(API_KEY_PREF);
      }

      if (_apiKey == string.Empty) {
        Debug.LogWarning("<WakaTime> API key is not set, skipping initialization...");
        return;
      }

      ProjectName = GetProjectName();

      if (_debug) Debug.Log("<WakaTime> Initializing...");

      SendHeartbeat();
      LinkCallbacks();
    }

    /// <summary>
    /// Reads .wakatime-project file
    /// <seealso cref="https://wakatime.com/faq#rename-projects"/>
    /// </summary>
    /// <returns>Lines of .wakatime-project or null if file not found</returns>
    public static string[] GetProjectFile() =>
      !File.Exists(WAKATIME_PROJECT_FILE) ? null : File.ReadAllLines(WAKATIME_PROJECT_FILE);

    /// <summary>
    /// Rewrites o creates new .wakatime-project file with given lines
    /// <seealso cref="https://wakatime.com/faq#rename-projects"/>
    /// </summary>
    /// <example>
    /// <code>
    /// project-override-name
    /// branch-override-name
    /// </code>
    /// </example>
    /// <param name="content"></param>
    public static void SetProjectFile(string[] content) {
      File.WriteAllLines(WAKATIME_PROJECT_FILE, content);
    }

    [Serializable]
    struct Response<T> {
      public string error;
      public T data;
    }

    [Serializable]
    struct HeartbeatResponse {
      public string id;
      public string entity;
      public string type;
      public float time;
    }

    struct Heartbeat {
      public string entity;
      public string type;
      public float time;
      public string project;
      public string branch;
      public string plugin;
      public string language;
      public bool is_write;
      public bool is_debugging;

      public Heartbeat(string file, bool save = false) {
        entity = file == string.Empty ? "Unsaved Scene" : file;
        type = "file";
        time = (float) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        project = ProjectName;
        plugin = "unity-wakatime";
        branch = "master";
        language = "unity";
        is_write = save;
        is_debugging = _debug;
      }
    }

    static void SendHeartbeat(bool fromSave = false) {
      if (_debug) Debug.Log("<WakaTime> Sending heartbeat...");

      var currentScene = EditorSceneManager.GetActiveScene().path;
      var file = currentScene != string.Empty
        ? Application.dataPath + "/" + currentScene.Substring("Assets/".Length)
        : string.Empty;

      var heartbeat = new Heartbeat(file, fromSave);
      if ((heartbeat.time - _lastHeartbeat.time < HEARTBEAT_COOLDOWN) && !fromSave &&
        (heartbeat.entity == _lastHeartbeat.entity)) {
        if (_debug) Debug.Log("<WakaTime> Skip this heartbeat");
        return;
      }

      var heartbeatJSON = JsonUtility.ToJson(heartbeat);

      var request = UnityWebRequest.Post(URL_PREFIX + "users/current/heartbeats?api_key=" + _apiKey, string.Empty);
      request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(heartbeatJSON));
      request.SetRequestHeader("Content-Type", "application/json");

      request.SendWebRequest().completed +=
        operation => {
          if (request.downloadHandler.text == string.Empty) {
            Debug.LogWarning(
              "<WakaTime> Network is unreachable. Consider disabling completely if you're working offline");
            return;
          }

          if (_debug)
            Debug.Log("<WakaTime> Got response\n" + request.downloadHandler.text);
          var response =
            JsonUtility.FromJson<Response<HeartbeatResponse>>(
              request.downloadHandler.text);

          if (response.error != null) {
            if (response.error == "Duplicate") {
              if (_debug) Debug.LogWarning("<WakaTime> Duplicate heartbeat");
            }
            else {
              Debug.LogError(
                "<WakaTime> Failed to send heartbeat to WakaTime!\n" +
                response.error);
            }
          }
          else {
            if (_debug) Debug.Log("<WakaTime> Sent heartbeat!");
            _lastHeartbeat = response.data;
          }
        };
    }

    [DidReloadScripts]
    static void OnScriptReload() {
      Initialize();
    }

    static void OnPlaymodeStateChanged(PlayModeStateChange change) {
      SendHeartbeat();
    }

    static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property) {
      SendHeartbeat();
    }

    static void OnHierarchyWindowChanged() {
      SendHeartbeat();
    }

    static void OnSceneSaved(Scene scene) {
      SendHeartbeat(true);
    }

    static void OnSceneOpened(Scene scene, OpenSceneMode mode) {
      SendHeartbeat();
    }

    static void OnSceneClosing(Scene scene, bool removingScene) {
      SendHeartbeat();
    }

    static void OnSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode) {
      SendHeartbeat();
    }

    static void LinkCallbacks(bool clean = false) {
      if (clean) {
        EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
        EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
        #if UNITY_2018_1_OR_NEWER
          EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;
        #else
          EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
        #endif
        EditorSceneManager.sceneSaved -= OnSceneSaved;
        EditorSceneManager.sceneOpened -= OnSceneOpened;
        EditorSceneManager.sceneClosing -= OnSceneClosing;
        EditorSceneManager.newSceneCreated -= OnSceneCreated;
      }

      EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
      EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
      #if UNITY_2018_1_OR_NEWER
        EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
      #else
        EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
      #endif
      EditorSceneManager.sceneSaved += OnSceneSaved;
      EditorSceneManager.sceneOpened += OnSceneOpened;
      EditorSceneManager.sceneClosing += OnSceneClosing;
      EditorSceneManager.newSceneCreated += OnSceneCreated;
    }

    /// <summary>
    /// Project name for sending <see cref="Heartbeat"/>
    /// </summary>
    /// <returns><see cref="Application.productName"/> or first line of .wakatime-project</returns>
    private static string GetProjectName() =>
      File.Exists(WAKATIME_PROJECT_FILE)
        ? File.ReadAllLines(WAKATIME_PROJECT_FILE)[0]
        : Application.productName;
  }
}

#endif
