using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.External.MapMagicInterface
{
    [CustomEditor(typeof(MapMagicInfiniteTerrain))]
    public class MapMagicInfiniteTerrainEditor : VegetationStudioBaseEditor
    {
        private MapMagicInfiniteTerrain _mapMagicInfiniteTerrain;
        public override void OnInspectorGUI()
        {
            HelpTopic = "map-magic-infinite-terrain";
            _mapMagicInfiniteTerrain = (MapMagicInfiniteTerrain)target;
            base.OnInspectorGUI();
#if MAPMAGIC
            EditorGUILayout.HelpBox("Map Magic installed", MessageType.Info);
#else
            EditorGUILayout.HelpBox("Map Magic not detected", MessageType.Error);
#endif
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Settings", LabelStyle);

            _mapMagicInfiniteTerrain.VegetationSystem = EditorGUILayout.ObjectField("Source vegetation system.", _mapMagicInfiniteTerrain.VegetationSystem, typeof(VegetationSystem), true) as VegetationSystem;
            if (_mapMagicInfiniteTerrain.VegetationSystem == null)
            {
                EditorGUILayout.HelpBox("You need to select the source vegetation system.", MessageType.Error);
            }
            EditorGUILayout.HelpBox("Select the source Vegetation system. An instance of this will be spawned for each new terrain created by map magic.", MessageType.Info);

            _mapMagicInfiniteTerrain.AdvancedMode = EditorGUILayout.Toggle("Advanced Mode", _mapMagicInfiniteTerrain);
            if (_mapMagicInfiniteTerrain.AdvancedMode)
            {
                EditorGUILayout.HelpBox("When a new terrain is added we search for a child component that has the names below and use the vegetation system assigned as source. If none is found the default is used.", MessageType.Info);

                GUILayout.BeginVertical("box");
                _mapMagicInfiniteTerrain.VegetationSystemName1 = EditorGUILayout.ObjectField("Source vegetation system.", _mapMagicInfiniteTerrain.VegetationSystemName1, typeof(VegetationSystem), true) as VegetationSystem;
                _mapMagicInfiniteTerrain.SearcString1 =EditorGUILayout.TextField("Search name", _mapMagicInfiniteTerrain.SearcString1);                    
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                _mapMagicInfiniteTerrain.VegetationSystemName2 = EditorGUILayout.ObjectField("Source vegetation system.", _mapMagicInfiniteTerrain.VegetationSystemName2, typeof(VegetationSystem), true) as VegetationSystem;
                _mapMagicInfiniteTerrain.SearcString2 = EditorGUILayout.TextField("Search name", _mapMagicInfiniteTerrain.SearcString2);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                _mapMagicInfiniteTerrain.VegetationSystemName3 = EditorGUILayout.ObjectField("Source vegetation system.", _mapMagicInfiniteTerrain.VegetationSystemName3, typeof(VegetationSystem), true) as VegetationSystem;
                _mapMagicInfiniteTerrain.SearcString3 = EditorGUILayout.TextField("Search name", _mapMagicInfiniteTerrain.SearcString3);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                _mapMagicInfiniteTerrain.VegetationSystemName4 = EditorGUILayout.ObjectField("Source vegetation system.", _mapMagicInfiniteTerrain.VegetationSystemName4, typeof(VegetationSystem), true) as VegetationSystem;
                _mapMagicInfiniteTerrain.SearcString4 = EditorGUILayout.TextField("Search name", _mapMagicInfiniteTerrain.SearcString4);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                _mapMagicInfiniteTerrain.VegetationSystemName5 = EditorGUILayout.ObjectField("Source vegetation system.", _mapMagicInfiniteTerrain.VegetationSystemName5, typeof(VegetationSystem), true) as VegetationSystem;
                _mapMagicInfiniteTerrain.SearcString5 = EditorGUILayout.TextField("Search name", _mapMagicInfiniteTerrain.SearcString5);
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }
}