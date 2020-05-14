namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    public interface IVegetationImporter
    {
        string ImporterName { get;}
        PersistentVegetationStoragePackage PersistentVegetationStoragePackage { get; set; }
        VegetationPackage VegetationPackage { get; set; }
        PersistentVegetationStorage PersistentVegetationStorage { get; set; }
        void OnGUI();
    }
}
