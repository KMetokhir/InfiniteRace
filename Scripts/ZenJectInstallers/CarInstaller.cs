using UnityEngine;
using Zenject;

public class CarInstaller : MonoInstaller
{
    [SerializeField] private Car _carPrefab;
    [SerializeField] private Transform _spawnPosition;   

    public override void InstallBindings()
    {
       var carDynamicChasis = Container.InstantiatePrefabForComponent<DynamicChassis>(_carPrefab, _spawnPosition.position + Vector3.up, Quaternion.identity, null);
        
       Container.BindInstance(carDynamicChasis). AsSingle().NonLazy();       
    }
}