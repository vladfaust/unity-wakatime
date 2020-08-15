using System.IO;
using UnityEditor;
using UnityEngine;

namespace WakaTime {
  /// <summary>
  /// Popup window for editing .wakatime-project file
  /// <seealso cref="https://wakatime.com/faq#rename-projects"/>
  /// </summary>
  public class ProjectEditWindow : EditorWindow {
    private static ProjectEditWindow _window;

    private static Vector2 _size = new Vector2(400, 138);
    private static string[] _projectSettings;
    private static string _branch;

    private static readonly GUIStyle RichHelpBoxStyle
      = new GUIStyle(EditorStyles.helpBox) {richText = true};

    private static bool _isProjectFileMissed;

    public static void Display() {
      if (_window) {
        FocusWindowIfItsOpen<ProjectEditWindow>();
      }
      else {
        _window = CreateInstance<ProjectEditWindow>();
        _window.ShowPopup();
      }

      // We need only first 2 lines from .wakatime-project
      _projectSettings = new[] {"", ""};
      var projectFile = Plugin.GetProjectFile();

      if (projectFile == null)
        _isProjectFileMissed = true;
      else {
        _isProjectFileMissed = false;
        projectFile.CopyTo(_projectSettings, 0);
      }

      _branch = string.IsNullOrEmpty(_projectSettings[1])
        ? "" // TODO: Read current git branch
        : _projectSettings[1];

      // If we need to display "project file missing" line, we are making different height
      // TODO: calculate height dynamically, if path is too long buttons may not fit
      _size.y = _isProjectFileMissed ? 170 : 138;

      // Display at screen center
      var pos = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) - _size;
      _window.position = new Rect(pos / 2, _size);
      _window.titleContent = new GUIContent("Change project name");
    }

    void OnGUI() {
      EditorGUILayout.BeginHorizontal(); {
        EditorGUILayout.PrefixLabel("Project name");
        _projectSettings[0] = EditorGUILayout.TextField(_projectSettings[0]);
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.LabelField(
        "A project name to send to WakaTime (Product Name from Player Settings by default)", RichHelpBoxStyle);

      EditorGUILayout.BeginHorizontal(); {
        EditorGUILayout.PrefixLabel("Branch");
        EditorGUILayout.SelectableLabel(_branch, RichHelpBoxStyle,
          GUILayout.Height(EditorGUIUtility.singleLineHeight));
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.LabelField(
        "A branch name to send to WakaTime (current git branch by default) <i>(not implemented in this plugin yet)</i>",
        RichHelpBoxStyle);

      if (_isProjectFileMissed)
        GUILayout.Label($"<b>{Path.GetFullPath(".wakatime_project")}</b> will be created on save",
          RichHelpBoxStyle);
      EditorGUILayout.BeginHorizontal(); {
        if (GUILayout.Button("Save")) {
          Plugin.SetProjectFile(_projectSettings);
          Plugin.Initialize();
          CloseAndNull();
          FocusWindowIfItsOpen<Window>(); // Updates main window for information redraw
        }

        if (GUILayout.Button("Cancel")) {
          CloseAndNull();
        }
      }
      EditorGUILayout.EndHorizontal();
    }

    private void OnLostFocus() {
      CloseAndNull();
    }

    /// <summary>
    /// Closes this window and erases instance
    /// </summary>
    private void CloseAndNull() {
      Close();
      _window = null;
    }
  }
}
