using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float laneMoveSpeed = 5f;
    [SerializeField] private float yLaneOffset = 0.25f;
    [SerializeField] private float laneZoomOffset = 0.5f;
    [SerializeField] private Vector2 movementLimits = new Vector2(10f, 5f);

    private Vector3 originalPosition;
    private float originalOrthographicSize;
    private Camera cameraComponent;

    private void Start()
    {
        originalPosition = transform.position;
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent != null)
        {
            originalOrthographicSize = cameraComponent.orthographicSize;
        }

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

        Vector3 desiredPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);

        desiredPosition.x = Mathf.Clamp(desiredPosition.x,
            originalPosition.x - movementLimits.x,
            originalPosition.x + movementLimits.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y,
            originalPosition.y - movementLimits.y,
            originalPosition.y + movementLimits.y);

        float interpolation = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, interpolation);

        bool isFrontOrMiddleLane = GameState.CurrentLane == GameState.FrontLane ||
                                   GameState.CurrentLane == GameState.MiddleLane;

        if (isFrontOrMiddleLane)
        {
            float laneOffset = GameState.CurrentLane - GameState.MiddleLane;
            float targetY = originalPosition.y + (laneOffset * yLaneOffset * 2);
            float targetSize = originalOrthographicSize - (laneOffset * laneZoomOffset);

            if (cameraComponent != null)
            {
                cameraComponent.orthographicSize = Mathf.MoveTowards(
                    cameraComponent.orthographicSize,
                    targetSize,
                    laneMoveSpeed * Time.deltaTime);
            }

            transform.position = new Vector3(
                transform.position.x,
                Mathf.MoveTowards(transform.position.y, targetY, laneMoveSpeed * Time.deltaTime),
                transform.position.z);
        }
    }
}
