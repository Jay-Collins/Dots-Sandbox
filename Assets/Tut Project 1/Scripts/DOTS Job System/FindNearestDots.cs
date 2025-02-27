using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FindNearestDots : MonoBehaviour
{
    // The size of our arrays does not need to vary, so rather than create
    // new arrays every field, we'll create the arrays in Awake() and store them
    // in these fields.
    NativeArray<float3> TargetPositions;
    NativeArray<float3> SeekerPositions;
    NativeArray<float3> NearestTargetPositions;

    public void Start()
    {
        Spawner spawner = Object.FindObjectOfType<Spawner>();
        // We use the Persistent allocator because these arrays must
        // exist for the run of the program.
        TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
        SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
    }

    // We are responsible for disposing of our allocations
    // when we no longer need them.
    public void OnDestroy()
    {
        TargetPositions.Dispose();
        SeekerPositions.Dispose();
        NearestTargetPositions.Dispose();
    }

    public void Update()
    {
        // Copy every target transform to a NativeArray.
        for (int i = 0; i < TargetPositions.Length; i++)
        {
            // Vector3 is implicitly converted to float3
            TargetPositions[i] = Spawner.TargetTransforms[i].localPosition;
        }

        // Copy every seeker transform to a NativeArray.
        for (int i = 0; i < SeekerPositions.Length; i++)
        {
            // Vector3 is implicitly converted to float3
            SeekerPositions[i] = Spawner.SeekerTransforms[i].localPosition;
        }

        /* single thread
         
        // To schedule a job, we first need to create an instance and populate its fields.
        FindNearestJob findJob = new FindNearestJob
        {
            TargetPositions = TargetPositions,
            SeekerPositions = SeekerPositions,
            NearestTargetPositions = NearestTargetPositions,
        };

        
        // Schedule() puts the job instance on the job queue.
        // JobHandle findHandle = findJob.Schedule();
        */
        
        // Parallel threads
        
        // To schedule a job, we first need to create an instance and populate its fields.
        FindNearestJobParallel findJob = new FindNearestJobParallel()
        {
            TargetPositions = TargetPositions,
            SeekerPositions = SeekerPositions,
            NearestTargetPositions = NearestTargetPositions,
        };
        
        // This job processes every seeker, so the
        // seeker array length is used as the index count.
        // A batch size of 100 is semi-arbitrarily chosen here 
        // simply because it's not too big but not too small.
        JobHandle findHandle = findJob.Schedule(SeekerPositions.Length, 100);

        // The Complete method will not return until the job represented by
        // the handle finishes execution. Effectively, the main thread waits
        // here until the job is done.
        findHandle.Complete();

        // Draw a debug line from each seeker to its nearest target.
        for (int i = 0; i < SeekerPositions.Length; i++)
        {
            // float3 is implicitly converted to Vector3
            Debug.DrawLine(SeekerPositions[i], NearestTargetPositions[i]);
        }
    }
}