using UnityEngine;

namespace AwesomeTechnologies.External.MapMagicInterface
{
    public class MapMagicInfiniteTerrain : MonoBehaviour
    {
        public VegetationSystem VegetationSystem;

        public VegetationSystem VegetationSystemName1;
        public VegetationSystem VegetationSystemName2;
        public VegetationSystem VegetationSystemName3;
        public VegetationSystem VegetationSystemName4;
        public VegetationSystem VegetationSystemName5;

        public string SearcString1;
        public string SearcString2;
        public string SearcString3;
        public string SearcString4;
        public string SearcString5;

        public bool AdvancedMode = false;

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
#if MAPMAGIC
            //MapMagic.MapMagic.OnGenerateCompleted += OnGenerateCompleted;
            MapMagic.MapMagic.OnApplyCompleted += OnGenerateCompleted;
#endif
        }

        // ReSharper disable once UnusedMember.Local
        void OnGenerateCompleted(Terrain terrain)
        {
            if (AdvancedMode)
            {
                if (SearcString1 != "" && VegetationSystemName1 != null)
                {
                    GameObject go = RecursiveFindChild(terrain.transform, SearcString1);
                    Debug.Log(go);
                    if (go) InstantiateVegetationSystem(VegetationSystemName1, terrain);
                    return;
                }

                if (SearcString2 != "" && VegetationSystemName2 != null)
                {
                    GameObject go = RecursiveFindChild(terrain.transform, SearcString1);
                    if (go) InstantiateVegetationSystem(VegetationSystemName1, terrain);
                    return;
                }

                if (SearcString3 != "" && VegetationSystemName3 != null)
                {
                    GameObject go = RecursiveFindChild(terrain.transform, SearcString1);
                    if (go) InstantiateVegetationSystem(VegetationSystemName1, terrain);
                    return;
                }

                if (SearcString4 != "" && VegetationSystemName4 != null)
                {
                    GameObject go = RecursiveFindChild(terrain.transform, SearcString1);
                    if (go) InstantiateVegetationSystem(VegetationSystemName1, terrain);
                    return;
                }

                if (SearcString5 != "" && VegetationSystemName5 != null)
                {
                    GameObject go = RecursiveFindChild(terrain.transform, SearcString1);
                    if (go) InstantiateVegetationSystem(VegetationSystemName1, terrain);
                    return;
                }

                if (VegetationSystem)
                {
                    if (terrain.gameObject.GetComponent<VegetationSystem>() == null)
                    {
                        GameObject newVegetationSystemObject = Instantiate(VegetationSystem.gameObject);
                        newVegetationSystemObject.transform.SetParent(terrain.transform);

                        VegetationSystem tempVegetationSystem = newVegetationSystemObject.GetComponent<VegetationSystem>();
                        tempVegetationSystem.AutoselectTerrain = false;
                        tempVegetationSystem.currentTerrain = terrain;
                    }
                }
            }
            else
            {
                if (VegetationSystem)
                {
                    if (terrain.gameObject.GetComponent<VegetationSystem>() == null)
                    {
                        GameObject newVegetationSystemObject = Instantiate(VegetationSystem.gameObject);
                        newVegetationSystemObject.transform.SetParent(terrain.transform);

                        VegetationSystem tempVegetationSystem = newVegetationSystemObject.GetComponent<VegetationSystem>();
                        tempVegetationSystem.AutoselectTerrain = false;
                        tempVegetationSystem.currentTerrain = terrain;
                    }
                }
            }
        }

        GameObject RecursiveFindChild(Transform parent, string childName)
        {
            GameObject findedObject = null;
            foreach (Transform child in parent)
            {
                if (child.name.ToLower().Contains(childName.ToLower()))
                {
                    return child.gameObject;
                }
                else
                {
                    findedObject = RecursiveFindChild(child, childName);
                }
            }
            return findedObject;
        }

        void InstantiateVegetationSystem(VegetationSystem vegetationSystem, Terrain terrain)
        {
            GameObject newVegetationSystemObject = Instantiate(vegetationSystem.gameObject);
            newVegetationSystemObject.transform.SetParent(terrain.transform);

            VegetationSystem tempVegetationSystem = newVegetationSystemObject.GetComponent<VegetationSystem>();
            tempVegetationSystem.AutoselectTerrain = false;
            tempVegetationSystem.currentTerrain = terrain;
        }
    }
}
