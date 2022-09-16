using UnityEngine;

public class ObjectLauncher : MonoBehaviour
{
    public Transform objectToSpawnPrefab;
    public Transform spawnAtTransform;

    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Vector3 lastPinchPos;

    public float launchVelocityMultiplier = 80;

    private void OnEnable()
    {
        startPos = spawnAtTransform.position;
    }

    private void OnDisable()
    {
        lastPinchPos = Vector3.zero;
        startPos = Vector3.zero;
    }

    private void Update()
    {
        lastPinchPos = spawnAtTransform.position;
    }

    public void ThrowObject()
    {
        if (startPos == Vector3.zero || lastPinchPos == Vector3.zero)
        {
            return;
        }

        Vector3 dir = startPos - lastPinchPos;

        if (dir.magnitude > 0.01f)
        {
            var rbody = Instantiate(objectToSpawnPrefab, spawnAtTransform.position, spawnAtTransform.rotation).GetComponent<Rigidbody>();
            rbody.velocity = dir * launchVelocityMultiplier;
        }
    }
}