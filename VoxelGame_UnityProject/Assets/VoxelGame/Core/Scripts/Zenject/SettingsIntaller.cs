using UnityEngine;
using Zenject;

using VoxelGame.Voxel;

namespace VoxelGame.Core
{
    public class SettingsIntaller : MonoInstaller
    {
        [SerializeField] private VoxelSettingsSO voxelSettings;

        public override void InstallBindings()
        {
            BindSettings();
        }

        private void BindSettings()
        {
            Container.Bind<VoxelSettingsSO>().FromInstance(voxelSettings).AsSingle();
            Container.Bind<ISaveLoadChunk>().To<SaveLoadChunkBinary>().AsSingle();
            voxelSettings.Init();
        }
    }
}