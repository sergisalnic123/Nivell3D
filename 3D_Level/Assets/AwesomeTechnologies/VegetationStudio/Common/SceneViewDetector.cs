﻿using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;


namespace AwesomeTechnologies.Utility
{
    [InitializeOnLoad]
    public class SceneViewDetector : MonoBehaviour
    {
        private static EditorWindow _currentEditorWindow;
        private static SceneView _currentSceneView;

        public delegate void MultiVegetationCellRefreshDelegate(Camera sceneviewCamera);
        public static MultiVegetationCellRefreshDelegate OnChangedSceneViewCameraDelegate;

        static SceneViewDetector()
        {
            EditorApplication.update += UpdateEditorCallback;
        }

        private static void UpdateEditorCallback()
        {
            if (_currentEditorWindow == EditorWindow.focusedWindow) return;

            _currentEditorWindow = EditorWindow.focusedWindow;
            var view = _currentEditorWindow as SceneView;
            if (view != null)
            {
                if (_currentSceneView != view)
                {
                    _currentSceneView = view;
                    if (OnChangedSceneViewCameraDelegate != null)
                        OnChangedSceneViewCameraDelegate(_currentSceneView.camera);
                }
            }
        }

        public static Camera GetCurrentSceneViewCamera()
        {
            if (_currentSceneView != null)
            {
                //Debug.Log("returning current sceneview camera");

                return _currentSceneView.camera;
            }

            Camera[] sceneviewCameras = SceneView.GetAllSceneCameras();
            return sceneviewCameras.Length > 0 ? sceneviewCameras[0] : null;
        }


        void DisableEditorApi()
        {
            EditorApplication.update -= UpdateEditorCallback;
        }
    }
}
#endif
