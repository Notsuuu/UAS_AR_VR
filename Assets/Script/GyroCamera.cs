using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    private GameObject camContainer;

    void Start()
    {
        camContainer = new GameObject("Camera Container");
        camContainer.transform.position = transform.position;
        transform.SetParent(camContainer.transform);

        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            camContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
        }
    }

    void Update()
    {
        if (SystemInfo.supportsGyroscope)
        {
            transform.localRotation = Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
        }
    }
}