using UnityEngine;

namespace AwesomeTechnologies.Vegetation
{
    public class VegetationTypeDetector
    {
        public static void SetDefaultVegetationInfoSettings(GameObject prefab, VegetationType vegetationType,
            VegetationItemInfo vegetationItemInfo, int seed)
        {           
            Random.InitState(Mathf.RoundToInt(Time.realtimeSinceStartup)+seed);

            vegetationItemInfo.VegetationType = vegetationType;
            vegetationItemInfo.VegetationPrefab = prefab;

            if (vegetationItemInfo.PrefabType == VegetationPrefabType.Mesh)
            {
                vegetationItemInfo.Name = prefab.name;
            }

            vegetationItemInfo.VegetationHeightType = VegetationHeightType.Simple;
            vegetationItemInfo.UseHeightLevel = true;
            vegetationItemInfo.MinimumHeight = 0f;
            vegetationItemInfo.MaximumHeight = 1000f;
            vegetationItemInfo.LodIndex = 2;
            vegetationItemInfo.VegetationRenderType = VegetationRenderType.Instanced;
            vegetationItemInfo.ColliderType = ColliderType.Disabled;
            vegetationItemInfo.RandomizePosition = true;

            vegetationItemInfo.UseAngle = true;
            vegetationItemInfo.VegetationSteepnessType = VegetationSteepnessType.Simple;
            vegetationItemInfo.MinimumSteepness = 0;

            vegetationItemInfo.MaxScale = 0.8f;
            vegetationItemInfo.MaxScale = 1.2f;

            vegetationItemInfo.UsePerlinMask = true;
            vegetationItemInfo.PerlinCutoff = Random.Range(0.4f, 0.6f);
            vegetationItemInfo.InversePerlinMask = RandomBoolean();
            vegetationItemInfo.Seed = Random.Range(0, 100);
            vegetationItemInfo.ShaderType = GetVegetationShaderType(prefab);
            vegetationItemInfo.PerlinScale = Random.Range(3, 10);

            switch (vegetationItemInfo.ShaderType)
            {
                case VegetationShaderType.VegetationStudioGrass:
                    vegetationItemInfo.VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    vegetationItemInfo.LodIndex = 2;

                    Material material = GetVegetationItemMaterial(prefab);
                    if (material)
                    {
                        vegetationItemInfo.ColorTint1 = material.GetColor("_Color");
                        vegetationItemInfo.ColorTint2 = material.GetColor("_ColorB");
                        vegetationItemInfo.TextureCutoff = material.GetFloat("_Cutoff");

                        vegetationItemInfo.RandomDarkening = material.GetFloat("_RandomDarkening");
                        vegetationItemInfo.RootAmbient = material.GetFloat("_RootAmbient");
                        vegetationItemInfo.ColorAreaScale = material.GetVector("_AG_ColorNoiseArea").y;
                    }
                    break;
            }

            switch (vegetationType)
            {
                case VegetationType.Grass:
                    vegetationItemInfo.Rotation = VegetationRotationType.FollowTerrainScale;
                    vegetationItemInfo.MaximumSteepness = 35f;
                    vegetationItemInfo.SampleDistance = Random.Range(0.8f, 1.2f);
                    break;

                case VegetationType.Plant:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.MaximumSteepness = 35f;
                    vegetationItemInfo.SampleDistance = Random.Range(1.8f, 2.2f);
                    vegetationItemInfo.MinScale = 1.6f;
                    vegetationItemInfo.MaxScale = 2.2f;
                    break;

                case VegetationType.Tree:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.SampleDistance = Random.Range(5f, 20f);
                    vegetationItemInfo.MaximumSteepness = 25f;
                    vegetationItemInfo.UseBillboards = true;
                    vegetationItemInfo.BillboardQuality = BillboardQuality.High;
                    break;

                case VegetationType.Objects:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.SampleDistance = Random.Range(5f, 7f);
                    vegetationItemInfo.MaximumSteepness = 25f;
                    break;
                case VegetationType.LargeObjects:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.SampleDistance = Random.Range(8f, 10f);
                    vegetationItemInfo.MaximumSteepness = 25f;
                    break;
            }
        }

        public static Material GetVegetationItemMaterial(GameObject prefab)
        {
            GameObject selectedVegetationModel = MeshUtils.SelectMeshObject(prefab, MeshType.Normal, 0);
            MeshRenderer meshrenderer = selectedVegetationModel.GetComponent<MeshRenderer>();
            if (meshrenderer)
                return meshrenderer.sharedMaterial;

            return null;
        }

        public static VegetationShaderType GetVegetationShaderType(GameObject prefab)
        {
            GameObject selectedVegetationModel = MeshUtils.SelectMeshObject(prefab, MeshType.Normal, 0);
            if (selectedVegetationModel)
            {
                MeshRenderer meshrenderer = selectedVegetationModel.GetComponentInChildren<MeshRenderer>();
                if (meshrenderer && meshrenderer.sharedMaterial)
                {
                    Shader shader = meshrenderer.sharedMaterial.shader;

                    return GetVegetationShaderTypeFromName(shader.name);
                }
            }           
            return VegetationShaderType.Other;
        }

        public static VegetationShaderType GetVegetationShaderTypeFromName(string name)
        {
            switch (name)
            {
                case "AwesomeTechnologies/Grass/Grass":
                    return VegetationShaderType.VegetationStudioGrass;
                case "AwesomeTechnologies/Grass/Grass-wind-r":
                    return VegetationShaderType.VegetationStudioGrass;
                case "Nature/SpeedTree":
                    return VegetationShaderType.Speedtree;
                case "AwesomeTechnologies/Custom/VS_SpeedTreeIndirect":
                    return VegetationShaderType.Speedtree;
                case "Alloy/Nature/SpeedTree":
                    return VegetationShaderType.Speedtree;
                default:
                    return VegetationShaderType.Other;
            }
        }

        public static void SetDefaultTexture3DSettings(Texture2D texture, VegetationType vegetationType,
            VegetationItemInfo vegetationItemInfo)
        {
        }

        public static bool RandomBoolean()
        {
            if (Random.value >= 0.5)
                return true;
            return false;
        }
    }
}


