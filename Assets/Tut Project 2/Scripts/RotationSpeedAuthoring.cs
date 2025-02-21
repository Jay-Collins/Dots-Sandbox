using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// This struct represents our runtime ECS component
// We use a struct instead of a class for better performance and memory layout
// IComponentData marks this as an ECS component that can be processed by systems
public  struct RotationSpeed : IComponentData
{
    // Store rotation speed in radians per second rather than degrees
    // This is more efficient for calculations since we won't need to convert
    // during runtime processing
    public float radiansPerSecond;
}

// This MonoBehaviour serves as the design-time component
// It appears in the Unity Inspector and allows designers to set values
// The "Authoring" suffix is a common naming convention in Unity ECS
public class RotationSpeedAuthoring : MonoBehaviour
{
    // This value appears in the Inspector
    // We use degrees here because they're more intuitive for humans
    // Default value of 360 means one full rotation per second

    public float degreesPerSecond = 360.0f;
}

// The Baker class converts our authoring MonoBehaviour into ECS components
// It runs during the conversion process from GameObjects to Entities
class RotationSpeedBaker : Baker<RotationSpeedAuthoring>
{
    // This method is called for each RotationSpeedAuthoring component
    // during the conversion process
    public override void Bake(RotationSpeedAuthoring authoring)
    {
        // Get or create an entity for this GameObject
        // TransformUsageFlags.Dynamic tells Unity this entity's transform
        // will change during runtime, helping with optimization
        var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        // Create our runtime component
        // Convert the designer-friendly degrees to radians
        // math.radians is an optimized conversion function from Unity.Mathematics
        var rotationSpeed = new RotationSpeed
        {
            radiansPerSecond = math.radians(authoring.degreesPerSecond)
        };

        // Add the runtime component to our entity
        // This makes the rotation speed data available to ECS systems
        AddComponent(entity, rotationSpeed);
    }
}
