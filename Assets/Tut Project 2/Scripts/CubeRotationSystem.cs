using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct CubeRotationSystem : ISystem
{
    // Unlike a MonoBehaviour, a system is not explicitly added into a scene. Instead,
    // by default, each system is automatically instantiated when you enter Play mode,
    // and so the OnUpdate of the CubeRotationSystem will be invoked once every frame.
    // The BurstCompile attribute marks the OnUpdate to be Burst compiled, which is
    // valid as long as the OnUpdate does not access any managed objects 
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get the time elapsed since the last frame
        // SystemAPI provides a centralized way to access common ECS functionality
        var deltaTime = SystemAPI.Time.DeltaTime;
        
        // SystemAPI.Query is the modern way to iterate over entities with specific components
        // RefRW<LocalTransform> means we need read-write access to the transform
        // RefRO<RotationSpeed> means we only need read-only access to the rotation speed
        // This access pattern helps Unity optimize memory access and prevent race conditions
        foreach (var (transform, rotationSpeed) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
        {
            // ValueRW gives us write access to the transform
            // RotateY is a helper method that creates a new transform rotated around the Y axis
            // We use the Y axis for this example, but you could rotate around any axis
            // The rotation is applied in radians, which is why we converted from degrees earlier
            var radians = rotationSpeed.ValueRO.radiansPerSecond * deltaTime;
            transform.ValueRW = transform.ValueRW.RotateY(radians);
        }
    }
}
