using AwesomeTechnologies.Common;
using UnityEditor;
using UnityEngine;

#if GAIA_PRESENT && UNITY_EDITOR
namespace Gaia.GX.AwesomeTechnologies
{
    public class GaiaExtention : MonoBehaviour
    {
        public static string GetPublisherName()
        {
            return "Awesome Technologies";
        }

        public static string GetPackageName()
        {
            return "Vegetation Studio";
        }

        public static void GX_About()
        {
            EditorUtility.DisplayDialog("About Vegetation Studio", "Vegetation Studio is a procedural vegetation placement and rendering system, replacing unity terrain vegetation handling.","OK");
        }

        public static void GX_ManualSetup_AddVegetationStudioToScene()
        {
            VegetationStudioManagerEditor.AddVegetationStudioManager();
        }

        public static void GX_AutomaticSetup_SetupAndImportTerrainTrees()
        {
            VegetationStudioManagerEditor.AddVegetationStudioManager();
        }

        

       
    }
}
#endif
