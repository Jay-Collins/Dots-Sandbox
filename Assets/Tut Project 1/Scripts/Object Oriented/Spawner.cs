using UnityEngine;

public class Spawner : MonoBehaviour
{
    // The set of targets and seekers are fixed, so rather than 
    // retrieve them every frame, we'll cache 
    // their transforms in these field.
    public static Transform[] TargetTransforms;
    public static Transform[] SeekerTransforms;

    public GameObject SeekerPrefab;
    public GameObject TargetPrefab;
    public int NumSeekers;
    public int NumTargets;
    public Vector2 Bounds;

    public void Start()
    {
        Random.InitState(123);

        SeekerTransforms = new Transform[NumSeekers];
        for (int i = 0; i < NumSeekers; i++)
        {
            GameObject go = GameObject.Instantiate(SeekerPrefab);
            Seeker seeker = go.GetComponent<Seeker>();
            Vector2 dir = Random.insideUnitCircle;
            seeker.Direction = new Vector3(dir.x, 0, dir.y);
            SeekerTransforms[i] = go.transform;
            go.transform.localPosition = new Vector3(
                Random.Range(0, Bounds.x), 0, Random.Range(0, Bounds.y));
        }

        TargetTransforms = new Transform[NumTargets];
        for (int i = 0; i < NumTargets; i++)
        {
            GameObject go = GameObject.Instantiate(TargetPrefab);
            Target target = go.GetComponent<Target>();
            Vector2 dir = Random.insideUnitCircle;
            target.Direction = new Vector3(dir.x, 0, dir.y);
            TargetTransforms[i] = go.transform;
            go.transform.localPosition = new Vector3(
                Random.Range(0, Bounds.x), 0, Random.Range(0, Bounds.y));
        }
    }
}