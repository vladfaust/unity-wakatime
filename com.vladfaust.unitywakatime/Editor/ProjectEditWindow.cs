using UnityEditor;
using UnityEngine;

namespace WakaTime
{
    public class ProjectEditWindow : EditorWindow
    {
        private static ProjectEditWindow window;
        
        private static readonly Vector2 size = new Vector2(400, 100);
        private static readonly string[] projectSettings = {"", ""};
    
        public static void Display()
        {
            if (window)
            {
                FocusWindowIfItsOpen<ProjectEditWindow>();
            }
            else
            {
                window = CreateInstance<ProjectEditWindow>();
                window.ShowUtility();
            }
            
            var pos = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) - size;
            window.position = new Rect(pos/2, size);
            window.titleContent = new GUIContent("Change project name");
            Plugin.GetProjectFile().CopyTo(projectSettings, 0);
        }

        void OnGUI()
        {
            projectSettings[0] = EditorGUILayout.TextField("Project name",projectSettings[0]);
            projectSettings[1] = EditorGUILayout.TextField("Current branch",projectSettings[1]);
            
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save"))
                {
                    Plugin.SetProjectFile(projectSettings);
                    Plugin.Initialize();
                    CloseAndNull();
                    FocusWindowIfItsOpen<Window>();
                }

                if (GUILayout.Button("Cancel"))
                {
                    CloseAndNull();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnLostFocus()
        {
            CloseAndNull();
        }

        private void CloseAndNull()
        {
            Close();
            window = null;
        }
    }
}