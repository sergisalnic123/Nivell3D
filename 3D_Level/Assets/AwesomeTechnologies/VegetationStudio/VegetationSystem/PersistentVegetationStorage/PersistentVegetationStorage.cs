using System;
using System.Collections.Generic;
using System.Threading;
using AwesomeTechnologies.Billboards;
using AwesomeTechnologies.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    [Serializable]
    public enum PrecisionPaintingMode
    {
        Terrain,
        TerrainAndColliders,
        TerrainAndMeshes
    }

    [HelpURL("http://www.awesometech.no/index.php/persistent-vegetation-storage")]
    public class PersistentVegetationStorage : MonoBehaviour
    {
        public PersistentVegetationStoragePackage PersistentVegetationStoragePackage;
        public VegetationSystem VegetationSystem;

        [NonSerialized]
        public int CurrentTabIndex;
        public int SelectedBrushIndex;
        public float BrushSize = 5;
        public float SampleDistance = 1f;
        public bool RandomizePosition = true;
        public bool PaintOnColliders;
        public bool UseSteepnessRules;
        public bool DisablePersistentStorage = false;

        public string SelectedEditVegetationID;
        public string SelectedPaintVegetationID;
        public string SelectedBakeVegetationID;
        public string SelectedStorageVegetationID;
        public string SelectedPrecisionPaintingVegetationID;
        public PrecisionPaintingMode PrecisionPaintingMode = PrecisionPaintingMode.TerrainAndMeshes;

        public bool AutoInitPersistentVegetationStoragePackage = false;

        public List<IVegetationImporter> VegetationImporterList = new List<IVegetationImporter>();
        public int SelectedImporterIndex;

        /// <summary>
        /// Tests if the persistent storage is initialized for the current terrain. 
        /// </summary>
        /// <param name="cellCount"></param>
        /// <returns></returns>
        public bool HasValidPersistentStorage(int cellCount)
        {
            //TUDO add terrainID and size to the test
            if (PersistentVegetationStoragePackage == null) return false;
            if (PersistentVegetationStoragePackage.PersistentVegetationCellList.Count != cellCount) return false;

            return true;
        }

        /// <summary>
        /// Sets a new persistentVegetationStoragePackage. Will refresh the VegetationSystem component.
        /// </summary>
        /// <param name="persistentVegetationStoragePackage"></param>
        public void SetPersistentVegetationStoragePackage(
            PersistentVegetationStoragePackage persistentVegetationStoragePackage)
        {
            PersistentVegetationStoragePackage = persistentVegetationStoragePackage;
            if (VegetationSystem)
            {
                VegetationSystem.RefreshVegetationPackage();
            }
        }

        /// <summary>
        /// InitializePersistentStorage will clean the storage and set it up for the current VegetationSystem.
        /// </summary>
        public void InitializePersistentStorage()
        {            
            if (PersistentVegetationStoragePackage != null)
            {               
                PersistentVegetationStoragePackage.ClearPersistentVegetationCells();
                for (int i = 0; i <= VegetationSystem.VegetationCellList.Count - 1; i++)
                {
                    PersistentVegetationStoragePackage.AddVegetationCell();
                }
            }
        }

        public void InitializePersistentStorage(int cellCount)
        {
            if (PersistentVegetationStoragePackage != null)
            {
                PersistentVegetationStoragePackage.ClearPersistentVegetationCells();
                for (int i = 0; i <= cellCount - 1; i++)
                {
                    PersistentVegetationStoragePackage.AddVegetationCell();
                }
            }
        }

        /// <summary>
        /// AddVegetationItem will add a new instance of a Vegetation Item to the persistent storage. Position, scale and rotation is in worldspace. The Optional clearCellCache will refresh the area where the item is added. 
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="worldPosition"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="applyMeshRotation"></param>
        /// <param name="vegetationSourceID"></param>
        /// <param name="clearCellCache"></param>
        public void AddVegetationItemInstance(string vegetationItemID, Vector3 worldPosition, Vector3 scale, Quaternion rotation, bool applyMeshRotation, byte vegetationSourceID,bool clearCellCache = false)
        {
            if (!VegetationSystem || !PersistentVegetationStoragePackage) return;

#if UNITY_EDITOR
            if (ThreadUtility.MainThread == Thread.CurrentThread)
            {
                if (VegetationSystem.GetSleepMode() && !Application.isPlaying)
                {
                    Debug.LogWarning("You need to Start Vegetation Studio in order to use the Persistent storage API");
                    return;
                }
            }
#endif
            Rect positionRect = new Rect(new UnityEngine.Vector2(worldPosition.x, worldPosition.z), UnityEngine.Vector2.zero);

            VegetationItemInfo vegetationItemInfo =
                VegetationSystem.CurrentVegetationPackage.GetVegetationInfo(vegetationItemID);

            if (applyMeshRotation)
            {
                rotation *= Quaternion.Euler(vegetationItemInfo.RotationOffset);
            }

            List<VegetationCell> overlapCellList = VegetationSystem.VegetationCellQuadTree.Query(positionRect);


            Vector3 terrainPosition = VegetationSystem.UnityTerrainData.terrainPosition;
      
      
            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].CellIndex;
                if (clearCellCache)
                {
                    overlapCellList[i].ClearCache();
                    VegetationSystem.RefreshVegetationBillboards(overlapCellList[i]);
                    VegetationSystem.SetDirty();
                }

                PersistentVegetationStoragePackage.AddVegetationItemInstance(cellIndex, vegetationItemID, worldPosition - terrainPosition, scale, rotation,vegetationSourceID);
            }           
        }



        public void AddVegetationItemInstanceEx(string vegetationItemID, Vector3 worldPosition, Vector3 scale, Quaternion rotation, byte vegetationSourceID,float minimumDistance, bool clearCellCache = false)
        {
            if (!VegetationSystem || !PersistentVegetationStoragePackage || VegetationSystem.VegetationCellQuadTree == null || VegetationSystem.UnityTerrainData == null) return;

            Rect positionRect = new Rect(new UnityEngine.Vector2(worldPosition.x, worldPosition.z), UnityEngine.Vector2.zero);

            List<VegetationCell> overlapCellList = VegetationSystem.VegetationCellQuadTree.Query(positionRect);

            Vector3 terrainPosition = VegetationSystem.UnityTerrainData.terrainPosition;
            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].CellIndex;
                if (clearCellCache)
                {
                    overlapCellList[i].ClearCache();
                    VegetationSystem.RefreshVegetationBillboards(overlapCellList[i]);
                    VegetationSystem.SetDirty();
                }

                PersistentVegetationStoragePackage.AddVegetationItemInstanceEx(cellIndex, vegetationItemID, worldPosition - terrainPosition, scale, rotation, vegetationSourceID, minimumDistance);
            }
        }

        public void RemoveVegetationItemInstance(string vegetationItemID, Vector3 worldPosition, float minimumDistance, bool clearCellCache = false)
        {
            if (!VegetationSystem || !PersistentVegetationStoragePackage) return;
            Rect positionRect = new Rect(new UnityEngine.Vector2(worldPosition.x, worldPosition.z), UnityEngine.Vector2.zero);

            List<VegetationCell> overlapCellList = VegetationSystem.VegetationCellQuadTree.Query(positionRect);

            Vector3 terrainPosition = VegetationSystem.UnityTerrainData.terrainPosition;
            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].CellIndex;
                if (clearCellCache)
                {
                    overlapCellList[i].ClearCache();
                    VegetationSystem.RefreshVegetationBillboards(overlapCellList[i]);
                    VegetationSystem.SetDirty();
                }

                PersistentVegetationStoragePackage.RemoveVegetationItemInstance(cellIndex, vegetationItemID, worldPosition - terrainPosition,  minimumDistance);
            }
        }


        /// <summary>
        /// RepositionCellItems is used to check all instances of a VegetationItem in a cell and confirm that they are located in the correct cell. 
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="id"></param>
        public void RepositionCellItems(int cellIndex, string id)
        {
            PersistentVegetationInfo persistentVegetationInfo = PersistentVegetationStoragePackage
                .PersistentVegetationCellList[cellIndex]
                .GetPersistentVegetationInfo(id);
            if (persistentVegetationInfo == null) return;

            List<PersistentVegetationItem> origialItemList = new List<PersistentVegetationItem>();
            origialItemList.AddRange(persistentVegetationInfo.VegetationItemList);
            persistentVegetationInfo.ClearCell();

            for (int i = 0; i <= origialItemList.Count - 1; i++)
            {
                AddVegetationItemInstance(id, origialItemList[i].Position + VegetationSystem.UnityTerrainData.terrainPosition, origialItemList[i].Scale,
                    origialItemList[i].Rotation, false, origialItemList[i].VegetationSourceID,true);
            }

            VegetationSystem.VegetationCellList[cellIndex].ClearCache();
            VegetationSystem.RefreshVegetationBillboards(cellIndex);
            VegetationSystem.SetDirty();
        }

        /// <summary>
        /// Returns the numbers of cells in the persistent vegetation storage.
        /// </summary>
        /// <returns></returns>
        public int GetPersistentVegetationCellCount()
        {
            if (PersistentVegetationStoragePackage && PersistentVegetationStoragePackage.PersistentVegetationCellList != null)
            {
                return PersistentVegetationStoragePackage.PersistentVegetationCellList.Count;
            }

            return 0;
        }

       
        // ReSharper disable once UnusedMember.Local
        void Reset()
        {
            VegetationSystem = gameObject.GetComponent<VegetationSystem>();
        }

        /// <summary>
        /// ClearVegetationItem will remove any instanced of vegetation in the storage with the provided VegetationItemID and VegetationSourceID
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="vegetationSourceID"></param>
        public void RemoveVegetationItemInstances(string vegetationItemID, byte vegetationSourceID)
        {
            if (PersistentVegetationStoragePackage == null) return;
            PersistentVegetationStoragePackage.RemoveVegetationItemInstances(vegetationItemID, vegetationSourceID);
        }

        /// <summary>
        /// ClearVegetationItem will remove any instances of a VegetationItem from the storage. Items from all sourceIDs will be removed.
        /// </summary>
        /// <param name="vegetationItemID"></param>
        public void RemoveVegetationItemInstances(string vegetationItemID)
        {
            if (PersistentVegetationStoragePackage == null) return;
            PersistentVegetationStoragePackage.RemoveVegetationItemInstances(vegetationItemID);
        }

        /// <summary>
        /// BakeVegetationItem will bake all instances of a VegetationItem from the rules to the Persisitent Vegetation Storage. The original rule will set "Include in Terrain" to false.
        /// </summary>
        /// <param name="vegetationItemID"></param>
        public void BakeVegetationItem(string vegetationItemID)
        {


            if (!VegetationSystem) return;
            if (!VegetationSystem.CurrentVegetationPackage) return;

            int vegetationIndex =
                VegetationSystem.CurrentVegetationPackage.GetVegetationItemIndexFromID(vegetationItemID);
           

            if (vegetationItemID == "")
            {
                Debug.Log("vegetationItemID empty");
                return;
            }

            VegetationSystem.CurrentVegetationPackage.VegetationInfoList[vegetationIndex].EnableRuntimeSpawn = true;

#if UNITY_EDITOR
            VegetationItemInfo vegetationItemInfo =
                VegetationSystem.CurrentVegetationPackage.GetVegetationInfo(vegetationItemID);
            if (!Application.isPlaying) EditorUtility.DisplayProgressBar("Bake vegetation item: " + vegetationItemInfo.Name, "Spawn all cells", 0);
#endif
            for (int i = 0; i <= VegetationSystem.VegetationCellList.Count - 1; i++)
            {
#if UNITY_EDITOR
                if (i % 100 == 0)
                {

                    if (!Application.isPlaying) EditorUtility.DisplayProgressBar("Bake vegetation item: " + vegetationItemInfo.Name, "Spawn cell " + i + "/" + (VegetationSystem.VegetationCellList.Count - 1), i/((float)VegetationSystem.VegetationCellList.Count - 1));
                }
#endif
                List<Matrix4x4> vegetationItemList = VegetationSystem.VegetationCellList[i]
                    .DirectSpawnVegetation(vegetationItemID,false);

                for (int j = 0; j <= vegetationItemList.Count - 1; j++)
                {
                    Matrix4x4 vegetationItemMatrix = vegetationItemList[j];
                    AddVegetationItemInstance(vegetationItemID, MatrixTools.ExtractTranslationFromMatrix(vegetationItemMatrix),
                        MatrixTools.ExtractScaleFromMatrix(vegetationItemMatrix),
                        MatrixTools.ExtractRotationFromMatrix(vegetationItemMatrix), false,0);
                }
            }
            VegetationSystem.CurrentVegetationPackage.VegetationInfoList[vegetationIndex].EnableRuntimeSpawn = false;
            VegetationSystem.ClearVegetationCellCache();
#if UNITY_EDITOR
            if (!Application.isPlaying)  EditorUtility.ClearProgressBar();
#endif

        }
    }
}
