using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Editor.AutoLoader
{
    [InitializeOnLoad]
    internal static class BoostrapAutoloader
    {
        private const string BOOSTRAP_SCENE_PATH = ""; // Path sample: "Assets/_Project/Scenes/BoostrapScene.unity";
        private const string PREF_KEY_PREV_SCENE = "PREV_SCENE";

        static BoostrapAutoloader() {
            EditorApplication.playModeStateChanged += OnPlaymodeStateChangedHandler;
        }

        private static void OnPlaymodeStateChangedHandler(PlayModeStateChange change)
        {
            Scene startScene = EditorSceneManager.GetSceneByBuildIndex(0);
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (string.IsNullOrWhiteSpace(BOOSTRAP_SCENE_PATH))
                {
                    Debug.Log($"Boostrap scene path is null");
                    EditorApplication.isPlaying = false;
                }

                if (SceneManager.GetActiveScene() == startScene) return;
                 
                EditorPrefs.SetString(PREF_KEY_PREV_SCENE, SceneManager.GetActiveScene().path); 
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    try
                    {
                        EditorSceneManager.OpenScene(BOOSTRAP_SCENE_PATH);
                    }
                    catch (Exception e) { 
                        Debug.LogError($"Cannot load scene: {startScene} with {e}");
                        EditorApplication.isPlaying = false;
                    }
                }
                else
                {
                    Debug.Log("Loading boostrap scene is breaked. Safe all changes.");
                    EditorApplication.isPlaying = false;
                }
            }

            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                try
                { 
                    EditorSceneManager.OpenScene(EditorPrefs.GetString(PREF_KEY_PREV_SCENE));
                }
                catch (Exception e)
                {
                    Debug.LogError($"Cannot load scene: {startScene} with {e}");
                }
            }
        }
    }
}
