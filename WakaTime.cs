using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using System.Net;

// Heavily inspired by https://github.com/bengsfort/WakaTime-Unity
// Do not forget to set apiKey and disable isDebug

namespace WakaTime {
  [InitializeOnLoad]
  public static class Wakatime {
    static string apiKey = "<ENTER YOUR API KEY HERE>"; // This
    static bool isDebug = true; // Set to false to disabled debugging

    const string URL_PREFIX = "https://wakatime.com/api/v1/";
    const int HEARTBEAT_COOLDOWN = 120;

    static HeartbeatResponse lastHeartbeat;

    static Wakatime() {
      if (isDebug) Debug.Log("<WakaTime> Initializing...");
      SendHeartbeat();
      LinkCallbacks();
    }

    struct Response<T> {
      public string error;
      public T data;
    }

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
        entity = (file == string.Empty ? "Unsaved Scene" : file);
        type = "file";
        time = (float)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        project = Application.productName;
        plugin = "unity-wakatime";
        branch = "master";
        language = "unity";
        is_write = save;
        is_debugging = isDebug;
      }
    }

    static void SendHeartbeat(bool fromSave = false) {
      if (isDebug) Debug.Log("<WakaTime> Sending heartbeat...");

      var currentScene = EditorSceneManager.GetActiveScene().path;
      var file = currentScene != string.Empty ? Path.Combine(Application.dataPath, currentScene.Substring("Assets/".Length)) : string.Empty;

      var heartbeat = new Heartbeat(file, fromSave);
      if ((heartbeat.time - lastHeartbeat.time < HEARTBEAT_COOLDOWN) && !fromSave && (heartbeat.entity == lastHeartbeat.entity)) {
        if (isDebug) Debug.Log("<WakaTime> Skip this heartbeat");
        return;
      }

      var heartbeatJSON = JsonUtility.ToJson(heartbeat);

      var request = UnityWebRequest.Post(URL_PREFIX + "users/current/heartbeats?api_key=" + apiKey, string.Empty);
      request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(heartbeatJSON));
      request.chunkedTransfer = false;
      request.SetRequestHeader("Content-Type", "application/json");

      request.SendWebRequest().completed += (operation) => {
        if (isDebug) Debug.Log("<WakaTime> Got response\n" + request.downloadHandler.text);
        var response = JsonUtility.FromJson<Response<HeartbeatResponse>>(request.downloadHandler.text);

        if (response.error != null) {
          if (response.error == "Duplicate") {
            if (isDebug) Debug.LogWarning("<WakaTime> Duplicate heartbeat");
          } else {
            Debug.LogError("<WakaTime> Failed to send heartbeat to WakaTime!\n" + response.error);
          }
        } else {
          if (isDebug) Debug.Log("<WakaTime> Sent heartbeat!");
          lastHeartbeat = response.data;
        }
      };
    }

    [DidReloadScripts()]
    static void OnScriptReload() {
      SendHeartbeat();
      LinkCallbacks(true);
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
        EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
        EditorSceneManager.sceneSaved -= OnSceneSaved;
        EditorSceneManager.sceneOpened -= OnSceneOpened;
        EditorSceneManager.sceneClosing -= OnSceneClosing;
        EditorSceneManager.newSceneCreated -= OnSceneCreated;
      }

      EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
      EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
      EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
      EditorSceneManager.sceneSaved += OnSceneSaved;
      EditorSceneManager.sceneOpened += OnSceneOpened;
      EditorSceneManager.sceneClosing += OnSceneClosing;
      EditorSceneManager.newSceneCreated += OnSceneCreated;
    }
  }
}
