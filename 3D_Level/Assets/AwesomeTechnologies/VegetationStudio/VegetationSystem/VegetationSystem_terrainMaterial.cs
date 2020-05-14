#if UNITY_EDITOR
#endif
using System;
using UnityEngine;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
#if !UNITY_2019_2_OR_NEWER
        private Terrain.MaterialType _originalTerrainMaterialType = Terrain.MaterialType.BuiltInStandard;
#endif
        private Material _originalTerrainMaterial;
        private float _originalTerrainheightmapPixelError;
        private bool _originalRenderVegetation;
        public bool TerrainMaterialOverridden;
        [NonSerialized]
        public Material VegetationHeatMapMaterial;

        private void SetupHeatMap()
        {
            VegetationHeatMapMaterial = Resources.Load("TerrainHeatMap") as Material;
        }

        public void ShowHeatmap(int vegIndex)
        {
            UpdateHeatmapMaterial(vegIndex);
            OverrideTerrainMaterial(VegetationHeatMapMaterial);
        }

        public void OverrideTerrainMaterial(Material material)
        {
            if (!currentTerrain) return;

            if (!TerrainMaterialOverridden)
            {
#if !UNITY_2019_2_OR_NEWER
                _originalTerrainMaterialType = currentTerrain.materialType;
#endif            
                _originalTerrainMaterial = currentTerrain.materialTemplate;
                _originalTerrainheightmapPixelError = currentTerrain.heightmapPixelError;
                _originalRenderVegetation = RenderVegetation;
                TerrainMaterialOverridden = true;                    
            }

#if !UNITY_2019_2_OR_NEWER
            currentTerrain.materialType = Terrain.MaterialType.Custom;
#endif
            currentTerrain.materialTemplate = material;
            currentTerrain.heightmapPixelError = 1;
            RenderVegetation = false;
        }
        public void RestoreTerrainMaterial()
        {
            if (!currentTerrain || !TerrainMaterialOverridden) return;

#if !UNITY_2019_2_OR_NEWER
            currentTerrain.materialType = _originalTerrainMaterialType;
#endif
            currentTerrain.materialTemplate = _originalTerrainMaterial;
            currentTerrain.heightmapPixelError = _originalTerrainheightmapPixelError;
            TerrainMaterialOverridden = false;
            RenderVegetation = _originalRenderVegetation;
        }

        public void UpdateHeatmapMaterial(int vegIndex)
        {
            if (!InitDone) return;
            if (UnityTerrainData == null) return;
            if (!VegetationHeatMapMaterial) return;
           
            VegetationHeatMapMaterial.SetFloat("_TerrainMinHeight", GetWaterLevel());
            VegetationHeatMapMaterial.SetFloat("_TerrainMaxHeight", UnityTerrainData.MaxTerrainHeight);
            VegetationHeatMapMaterial.SetFloat("_MinHeight", 0);
            VegetationHeatMapMaterial.SetFloat("_MaxHeight", 0);
            VegetationHeatMapMaterial.SetFloat("_MinSteepness", 0);
            VegetationHeatMapMaterial.SetFloat("_MaxSteepness", 90);
            VegetationHeatMapMaterial.SetTexture("_CurveTexture", new Texture2D(1, 1));
            if (VegetationPackageList[VegetationPackageIndex].VegetationInfoList.Count > vegIndex)
            {
                if (VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].VegetationHeightType ==
                    VegetationHeightType.Simple)
                {
                    VegetationHeatMapMaterial.SetFloatArray("_HeightCurve", CreateSimpleHeightCurveArray(VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].MinimumHeight, VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].MaximumHeight,UnityTerrainData.MaxTerrainHeight));
                }
                else
                {
                    VegetationHeatMapMaterial.SetFloatArray("_HeightCurve", VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].HeightCurveArray);
                }

                if (VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].VegetationSteepnessType ==VegetationSteepnessType.Simple)
                {
                    VegetationHeatMapMaterial.SetFloatArray("_SteepnessCurve", CreateSimpleHeightCurveArray(VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].MinimumSteepness, VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].MaximumSteepness, 90));
                }
                else
                {
                    VegetationHeatMapMaterial.SetFloatArray("_SteepnessCurve", VegetationPackageList[VegetationPackageIndex].VegetationInfoList[vegIndex].SteepnessCurveArray);
                }
            }               
        }

        private float[] CreateSimpleHeightCurveArray(float min, float max, float maxCurveHeight)
        {
            float normalizedMin = min / maxCurveHeight;
            float normalizedMax = max / maxCurveHeight;

            float[] curveArray = new float[256];
            for (int i = 0; i <= curveArray.Length - 1; i++)
            {
                float normalizedIndex = (float) i / curveArray.Length;
                if (normalizedIndex >= normalizedMin && normalizedIndex <= normalizedMax) curveArray[i] = 1;
            }
            return curveArray;
        }
    }
}
