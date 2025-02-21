using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CubeSpawner>(); 
    }

    // The SpawnSystem is a system that you only want to update once, so in the OnUpdate,
    // the Enabled property of the SystemState is set to false, which prevents subsequent
    // updates of the system. 
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        // Because you can be sure that only one entity will have the Spawner component,
        // you can access the component conveniently by calling SystemAPI.GetSingleton<Spawner>().
        var prefab = SystemAPI.GetSingleton<CubeSpawner>().cubePrefab;
        
        //The Instantiate call creates 10 new instances of the prefab entity and returns a NativeArray
        //of the new Entity IDs. In this case, the array is only needed for the duration of the OnUpdate call,
        //so Allocator.Temp is used.
        var instances = state.EntityManager.Instantiate(prefab, 10, Allocator.Temp);

        // Here a random number generator from the Unity.Mathematics package is used with a fixed,
        // arbitrarily chosen seed of 123. In the loop, SystemAPI.GetComponentRW returns a read-write
        // reference to each entity’s LocalTransform component, and via this reference, the Transform
        // of the entity is set to a random position (in the range from 0 to 10 along each axis).
        var random = new Random(123);
        foreach (var entity in instances)
        {
            var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            transform.ValueRW.Position = random.NextFloat3(new float3(10, 10, 10));
        }
    }


}
