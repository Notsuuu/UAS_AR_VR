using UnityEngine;

public class ARInteract : MonoBehaviour
{
    [Header("Kecepatan Interaksi")]
    public float rotationSpeed = 0.2f;
    public float moveSpeed = 0.0005f;
    public float zoomSpeed = 0.002f;

    [Header("Batasan Zoom")]
    public float minScale = 0.02f;
    public float maxScale = 1.0f;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.deltaPosition;
                
                transform.Rotate(Vector3.up, -touchDelta.x * rotationSpeed, Space.World);
                
                transform.Rotate(Camera.main.transform.right, touchDelta.y * rotationSpeed, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
            {
                Vector2 avgDelta = (t0.deltaPosition + t1.deltaPosition) / 2;
                
                Vector3 moveDir = new Vector3(avgDelta.x, avgDelta.y, 0);
                moveDir = Camera.main.transform.TransformDirection(moveDir);
                
                transform.position += moveDir * moveSpeed;
            }

            Vector2 t0PrevPos = t0.position - t0.deltaPosition;
            Vector2 t1PrevPos = t1.position - t1.deltaPosition;

            float prevTouchDeltaMag = (t0PrevPos - t1PrevPos).magnitude;
            float touchDeltaMag = (t0.position - t1.position).magnitude;
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            if (Mathf.Abs(deltaMagnitudeDiff) > 0.01f) 
            {
                float newScale = transform.localScale.x + (deltaMagnitudeDiff * zoomSpeed);
                
                newScale = Mathf.Clamp(newScale, minScale, maxScale);

                transform.localScale = Vector3.one * newScale;
            }
        }
    }
}
            
        
    
