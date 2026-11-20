using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 movementLimits = new Vector2(10f, 5f);

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        desiredPosition.x = Mathf.Clamp(desiredPosition.x,
            originalPosition.x - movementLimits.x,
            originalPosition.x + movementLimits.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y,
            originalPosition.y - movementLimits.y,
            originalPosition.y + movementLimits.y);

        float interpolation = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, interpolation);
    }
}
