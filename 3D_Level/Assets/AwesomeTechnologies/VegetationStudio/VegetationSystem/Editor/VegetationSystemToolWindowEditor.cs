using System.Collections;
using System.Collections.Generic;
using AwesomeTechnologies;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using AwesomeTechnologies.VegetationStudio;
using UnityEditor;
using UnityEngine;

public class VegetationSystemToolWindowEditor : EditorWindow
{
    private GameObject _vegetationSystemPrefab;
    private PersistentVegetationStoragePackage _persistentVegetationStoragePackage;
    private VegetationPackage _vegetationPackage;
    private GameObject _rootObject;
    private bool _usePrefab;


    [MenuItem("Window/Awesome Technologies/Vegetation System tool window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(VegetationSystemToolWindowEditor));
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        EditorGUILayout.HelpBox("This tool will help you set up vegetation system components on all terrains in the scene.", MessageType.Info);

        _usePrefab = EditorGUILayout.Toggle("Use source prefab", _usePrefab);
        if (_usePrefab)
        {
            _vegetationSystemPrefab =
                (GameObject) EditorGUILayout.ObjectField("", _vegetationSystemPrefab, typeof(GameObject), true);
        }
        else
        {
            _vegetationPackage =
                (VegetationPackage)EditorGUILayout.ObjectField("", _vegetationPackage, typeof(VegetationPackage), true);
        }

        EditorGUILayout.HelpBox("Select use source prefab if you have a pre configured vegetation system object prefab you want to use. Uncheck to select a Vegetation Package and create a new Vegetation System Object.", MessageType.Info);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        _rootObject =
            (GameObject)EditorGUILayout.ObjectField("", _rootObject, typeof(GameObject), true);
        EditorGUILayout.HelpBox("Choose a root object for selecting Terrains. If this is left empty all terrains in the scene will be selected.", MessageType.Info);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Setup Vegetation Systems"))
        {
            if (_usePrefab)
            {
                SetupFromPrefab();
            }
            else
            {
                SetupFromVegetationPackage();
            }
        }
        GUILayout.EndVertical();
        // The actual window code goes here
    }

    void SetupFromVegetationPackage()
    {
        if (_vegetationPackage == null)
        {
            Debug.LogWarning("You need to assign a vegetation package");
            return;
        }
    
        var terrains = _rootObject ? _rootObject.GetComponentsInChildren<Terrain>() : GameObject.FindObjectsOfType<Terrain>();

        foreach (Terrain terrain in terrains)
        {
            VegetationStudioManager.AddVegetationSystemToTerrain(terrain, _vegetationPackage);
        }
    }

    void SetupFromPrefab()
    {
        if (_vegetationSystemPrefab == null)
        {
            Debug.LogWarning("You need to assign a source prefab");
            return;
        }

        var terrains = _rootObject ? _rootObject.GetComponentsInChildren<Terrain>() : GameObject.FindObjectsOfType<Terrain>();

        foreach (Terrain terrain in terrains)
        {
            GameObject vegetationSystemObject = Instantiate(_vegetationSystemPrefab, terrain.gameObject.transform);
            VegetationSystem vegetationSystem = vegetationSystemObject.GetComponent<VegetationSystem>();
            if (vegetationSystem)
            {
                vegetationSystem.AutoselectTerrain = false;
                vegetationSystem.SetTerrain(terrain);
            }
        }
    }
}
