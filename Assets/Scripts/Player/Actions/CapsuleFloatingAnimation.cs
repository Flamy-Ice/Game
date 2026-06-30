using UnityEngine;

public class CapsuleFloatingAnimation : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatAmplitude = 0.5f;

    private Vector3 startLocalPosition;

    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float newY = startLocalPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startLocalPosition.x, newY, startLocalPosition.z);
    }
}