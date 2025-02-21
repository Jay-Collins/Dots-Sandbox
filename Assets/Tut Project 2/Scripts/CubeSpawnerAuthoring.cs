using Unity.Entities;
using UnityEngine;

// This is our authoring component that exists in the Unity Editor
// It serves as the bridge between the traditional GameObject system and ECS
// The class inherits from MonoBehaviour so it can be attached to GameObjects
// and appear in the Unity Inspector
public class CubeSpawnerAuthoring : MonoBehaviour
{
    public GameObject cubePrefab;

    // The Baker class is responsible for converting our MonoBehaviour data
    // into ECS components during the baking process
    // It's nested within CubeSpawnerAuthoring to keep related code together
    class Baker : Baker<CubeSpawnerAuthoring>
    {
        // This method is called during the conversion process
        // It takes our authoring component and creates the corresponding
        // ECS components and entities
        public override void Bake(CubeSpawnerAuthoring authoring)
        {
            // Create an entity for the spawner itself
            // We use TransformUsageFlags.None because the spawner doesn't need
            // a transform - it's just a logical entity that manages spawning
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            
            // Create our runtime component
            // Here we convert the GameObject prefab reference into an Entity reference
            // We use TransformUsageFlags.Dynamic because spawned cubes will move
            var cubeSpawner = new CubeSpawner
            {
                cubePrefab = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic)
            };
            
            // Add the runtime component to our spawner entity
            // This makes the prefab reference available to ECS systems
            AddComponent(entity, cubeSpawner);
        }
    }
}

// This is our runtime ECS component
// It's a struct rather than a class for better performance
// IComponentData marks this as an ECS component that can be processed by systems
struct CubeSpawner : IComponentData
{
    // Store a reference to the cube prefab as an Entity
    // In ECS, everything is an Entity, including prefabs
    // This reference will be used when instantiating new cubes
    public Entity cubePrefab;
}
