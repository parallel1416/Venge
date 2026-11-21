using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float flipSpeed = 720f;
    [SerializeField] private Vector2 movementLimits = new Vector2(10f, 5f);

    private int facingDirection = 1;
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput -= 1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput += 1f;
        }

        if (horizontalInput != 0f)
        {
            int movementDirection = horizontalInput > 0f ? 1 : -1;

            if (movementDirection != facingDirection)
            {
                Flip();
            }

            transform.position += Vector3.right * horizontalInput * moveSpeed * Time.deltaTime;

            Vector3 constrainedPosition = transform.position;
            constrainedPosition.x = Mathf.Clamp(constrainedPosition.x,
                originalPosition.x - movementLimits.x,
                originalPosition.x + movementLimits.x);
            constrainedPosition.y = Mathf.Clamp(constrainedPosition.y,
                originalPosition.y - movementLimits.y,
                originalPosition.y + movementLimits.y);
            transform.position = constrainedPosition;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeLane(1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeLane(-1);
        }

        Quaternion targetRotation = Quaternion.Euler(0f, facingDirection > 0 ? 0f : 180f, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, flipSpeed * Time.deltaTime);
    }

    private void ChangeLane(int laneDirection)
    {
        int previousLane = GameState.CurrentLane;
        int newLane = Mathf.Clamp(previousLane + laneDirection, GameState.FrontLane, GameState.BackLane);

        if (newLane == previousLane)
        {
            return;
        }

        GameState.CurrentLane = newLane;
        transform.position += Vector3.forward * laneDirection;
        Debug.Log($"Current lane: {GameState.CurrentLane}");
    }

    private void Flip()
    {
        facingDirection *= -1;
    }
}
