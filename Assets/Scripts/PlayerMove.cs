using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float pushSpeed = 2f;
    [SerializeField] private float flipSpeed = 720f;
    [SerializeField] private float yLaneOffset = 0.25f;
    [SerializeField] private float laneMoveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private Vector2 movementLimits = new Vector2(10f, 5f);
    [SerializeField] private Transform stage;

    private Animator animator;
    private int facingDirection = 1;
    private Vector3 originalPosition;
    private float verticalVelocity;
    private float groundY;
    private float originalStageX;
    private float originalStageY;

    private void Start()
    {
        originalPosition = transform.position;
        groundY = transform.position.y;
        animator = GetComponentInChildren<Animator>();

        if (stage != null)
        {
            originalStageX = stage.position.x;
            originalStageY = stage.position.y;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && animator != null)
        {
            animator.SetTrigger("attack");
        }

        bool isPushing = Input.GetKey(KeyCode.F);

        if (animator != null)
        {
            animator.SetBool("isPushing", isPushing);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Approximately(transform.position.y, groundY))
        {
            verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
        }

        if (!Mathf.Approximately(transform.position.y, groundY) || verticalVelocity > 0f)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            Vector3 jumpPosition = transform.position;
            jumpPosition.y += verticalVelocity * Time.deltaTime;

            if (jumpPosition.y <= groundY && verticalVelocity <= 0f)
            {
                jumpPosition.y = groundY;
                verticalVelocity = 0f;
            }

            transform.position = jumpPosition;
        }

        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput -= 1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput += 1f;
        }

        float activeMoveSpeed = isPushing ? pushSpeed : moveSpeed;

        if (horizontalInput != 0f)
        {
            int movementDirection = horizontalInput > 0f ? 1 : -1;

            if (movementDirection != facingDirection)
            {
                Flip();
            }

            transform.position += Vector3.right * horizontalInput * activeMoveSpeed * Time.deltaTime;

            Vector3 constrainedPosition = transform.position;
            constrainedPosition.x = Mathf.Clamp(constrainedPosition.x,
                originalPosition.x - movementLimits.x,
                originalPosition.x + movementLimits.x);
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

        if (stage != null)
        {
            int laneOffset = GameState.CurrentLane - GameState.MiddleLane;
            float targetStageY = originalStageY - (laneOffset * yLaneOffset);
            float newStageY = Mathf.MoveTowards(stage.position.y, targetStageY, laneMoveSpeed * Time.deltaTime);
            stage.position = new Vector3(originalStageX, newStageY, stage.position.z);
        }

        float currentSpeed = Mathf.Abs(horizontalInput * activeMoveSpeed);

        if (animator != null)
        {
            animator.SetFloat("speed", currentSpeed);

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
            string animationState = clipInfos.Length > 0 ? clipInfos[0].clip.name : stateInfo.shortNameHash.ToString();

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
