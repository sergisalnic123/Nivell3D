﻿using AwesomeTechnologies.Billboards;
using AwesomeTechnologies.Colliders;
#if TOUCH_REACT
using AwesomeTechnologies.TouchReact;
#endif 
using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine;
using UnityEditor;
using AwesomeTechnologies.VegetationStudio;

namespace AwesomeTechnologies.Common
{
    [CustomEditor(typeof(VegetationStudioManager))]
    public class VegetationStudioManagerEditor : VegetationStudioBaseEditor
    {
        [MenuItem("Window/Awesome Technologies/Add Vegetation Studio to scene")]
        // ReSharper disable once UnusedMember.Local
        public static void AddVegetationStudioManager()
        {
            VegetationStudioManager vegetationStudioManager = FindObjectOfType<VegetationStudioManager>();
            if (vegetationStudioManager)
            {
                EditorUtility.DisplayDialog("Vegetation Studio Component",
                    "There is already a Vegetation Studio Manager Component in the scene. There can be only one.", "OK");
            }
            else
            {
                GameObject go = new GameObject { name = "VegetationStudio" };
                go.AddComponent<VegetationStudioManager>();

                GameObject vegetationSystem = new GameObject { name = "VegetationSystem" };
                vegetationSystem.transform.SetParent(go.transform);
                vegetationSystem.AddComponent<VegetationSystem>();
                vegetationSystem.AddComponent<TerrainSystem>();
                vegetationSystem.AddComponent<BillboardSystem>();
                vegetationSystem.AddComponent<ColliderSystem>();
                vegetationSystem.AddComponent<PersistentVegetationStorage>();

                GameObject touchReactSystem = new GameObject { name = "TouchReactSystem" };
                touchReactSystem.transform.SetParent(go.transform);
#if TOUCH_REACT
                touchReactSystem.AddComponent<TouchReactSystem>();
#endif 
            } 
        }

        [MenuItem("Window/Awesome Technologies/Multi terrain/Add additional VegetationSystem")]
        // ReSharper disable once UnusedMember.Local
        public static VegetationSystem AddVegetationSystem()
        {
            GameObject vegetationSystem = new GameObject { name = "VegetationSystem" };
            var vs = vegetationSystem.AddComponent<VegetationSystem>();
            vegetationSystem.AddComponent<TerrainSystem>();
            vegetationSystem.AddComponent<BillboardSystem>();
            vegetationSystem.AddComponent<ColliderSystem>();
            vegetationSystem.AddComponent<PersistentVegetationStorage>();
            return vs;
        }

        [MenuItem("CONTEXT/Terrain/Add VegetationSystem")]
        static void AddVegetationSystemToTerrain(MenuCommand command)
        {
            Terrain terrain = (Terrain)command.context;
            var vs = AddVegetationSystem();
            vs.transform.SetParent(terrain.transform);
            vs.AutoselectTerrain = false;
            vs.currentTerrain = terrain;
            Selection.activeGameObject = vs.gameObject;
        }

        private VegetationStudioManager _vegetationStudioManager;

        public override void OnInspectorGUI()
        {
            //OverrideLogoTextureName = "SectionBanner_VegetationStudioManager";
            HelpTopic = "vegetation-studio-manager";
            LargeLogo = true;
            _vegetationStudioManager = (VegetationStudioManager) target;
            base.OnInspectorGUI();

            if (_vegetationStudioManager.HasSleepingVegetationSystems())
            {
                GUILayout.BeginVertical("box");
                var oldColor = GUI.backgroundColor;
                Color backgroundColor = new Color(110f / 255f, 235f / 255f, 110f / 255f);

                GUI.backgroundColor = backgroundColor;
                if (GUILayout.Button("Start all Vegetation Systems", GUILayout.Height(35)))
                {
                    _vegetationStudioManager.WakeUpVegetationSystems();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.HelpBox("Vegetation Studio starts in sleep mode when scene loads in editor. In playmode and builds it starts as normal.", MessageType.Info);

                GUILayout.EndVertical();
            }

            EditorGUILayout.HelpBox(
                "Vegetation System Manager will manage all Vegetation, Terrain, Collider, Masks and Billboard Systems in the scene. Only one instance per scene.",
                MessageType.Info);

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Vegetation systems", LabelStyle);
            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i <= _vegetationStudioManager.VegetationSystemList.Count - 1; i++)
                EditorGUILayout.ObjectField("Vegetation System", _vegetationStudioManager.VegetationSystemList[i],
                    typeof(VegetationSystem), true);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Terrain systems", LabelStyle);
            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i <= _vegetationStudioManager.TerrainSystemList.Count - 1; i++)
                EditorGUILayout.ObjectField("Terrain System", _vegetationStudioManager.TerrainSystemList[i],
                    typeof(TerrainSystem), true);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();           
        }
    }
}

