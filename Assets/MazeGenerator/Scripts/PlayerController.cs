using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 8f; // Increased for better jump height
    public float groundCheckDistance = 0.5f; // Increased further for better detection
    public LayerMask groundLayer;
    
    [Header("Debug")]
    public bool showGroundDetection = true;
    public bool debugJump = true;
    
    private Rigidbody rb;
    private Transform cam;
    private bool isGrounded;
    private Vector3 moveDir;
    private CapsuleCollider playerCollider; // Reference to the player's collider

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;    // cache your orbit camera
        playerCollider = GetComponent<CapsuleCollider>();
        
        // Set player's starting position to the bottom-right corner of the maze
        transform.position = FindObjectOfType<MazeSpawner>().GetPlayerStartPosition();
        
        // Make sure we have a proper ground layer set
        if (groundLayer.value == 0)
        {
            // Use everything as ground
            groundLayer = Physics.AllLayers;
            Debug.LogWarning("Ground layer not set, using all layers for ground detection");
        }
        
        // Force isGrounded to true at start
        isGrounded = true;
    }

    void Update()
    {
        // Check for jump input in Update to ensure we don't miss quick button presses
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
                if (debugJump)
                {
                    Debug.Log("JUMPED with force: " + jumpForce);
                }
            }
            else if (debugJump)
            {
                Debug.Log("Jump failed - player not grounded");
            }
        }
        
        // Check if player is touching ground
        CheckGrounded();
    }

    void FixedUpdate()
    {
        // read input
        float h = Input.GetAxis("Horizontal"); // A/D or ←/→
        float v = Input.GetAxis("Vertical");   // W/S or ↑/↓

        // build directions relative to camera
        Vector3 forward = cam.forward;
        forward.y = 0;           // ignore vertical tilt
        forward.Normalize();

        Vector3 right = cam.right;
        right.y = 0;
        right.Normalize();

        // combine for movement vector
        moveDir = forward * v + right * h;

        // apply to Rigidbody - preserve vertical velocity during movement
        Vector3 velocity = moveDir * speed;
        velocity.y = rb.linearVelocity.y; 
        rb.linearVelocity = velocity;
    }
    
    private void Jump()
    {
        // Add upward force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        // Set grounded to false immediately after jumping
        isGrounded = false;
    }
    
    private void CheckGrounded()
    {
        // Use multiple methods to check if grounded
        bool wasGrounded = isGrounded;
        isGrounded = false;
        
        // Method 1: Simple direction raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f, groundLayer))
        {
            float distance = hit.distance;
            // If we're very close to the ground, count as grounded
            if (distance < 1.0f) 
            {
                isGrounded = true;
                if (debugJump && !wasGrounded)
                {
                    Debug.Log("Grounded: Simple raycast hit " + hit.collider.gameObject.name);
                }
            }
        }
        
        // Method 2: Check from the center with a wider radius
        if (!isGrounded)
        {
            // Use a spherecast for better detection
            Vector3 origin = transform.position;
            origin.y += 0.1f; // Start a bit above to avoid hitting our own collider
            
            if (Physics.SphereCast(origin, 0.4f, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                isGrounded = true;
                if (debugJump && !wasGrounded)
                {
                    Debug.Log("Grounded: Sphere cast hit " + hit.collider.gameObject.name);
                }
            }
        }
        
        // Method 3: Check all 4 corners of the player
        if (!isGrounded && playerCollider != null)
        {
            float radius = playerCollider.radius;
            float height = playerCollider.height;
            
            // Check at the 4 bottom corners of the player
            Vector3[] checkPoints = new Vector3[5];
            checkPoints[0] = transform.position + new Vector3(0, -height/2 + radius, 0); // Center bottom
            checkPoints[1] = checkPoints[0] + new Vector3(radius*0.8f, 0, radius*0.8f);  // Front-Right
            checkPoints[2] = checkPoints[0] + new Vector3(-radius*0.8f, 0, radius*0.8f); // Front-Left
            checkPoints[3] = checkPoints[0] + new Vector3(radius*0.8f, 0, -radius*0.8f); // Back-Right
            checkPoints[4] = checkPoints[0] + new Vector3(-radius*0.8f, 0, -radius*0.8f); // Back-Left
            
            for (int i = 0; i < checkPoints.Length; i++)
            {
                if (Physics.Raycast(checkPoints[i], Vector3.down, out hit, groundCheckDistance, groundLayer))
                {
                    isGrounded = true;
                    if (debugJump && !wasGrounded)
                    {
                        Debug.Log("Grounded: Corner " + i + " hit " + hit.collider.gameObject.name);
                    }
                    break;
                }
                
                if (showGroundDetection)
                {
                    // Draw debug rays
                    Debug.DrawRay(checkPoints[i], Vector3.down * groundCheckDistance, 
                        isGrounded ? Color.green : Color.red);
                }
            }
        }
        
        // Method 4: Even simpler - check if falling
        if (!isGrounded && rb.linearVelocity.y <= 0.05f)
        {
            // Check if player is almost stationary in y-direction
            bool rayCastHit = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance * 2, groundLayer);
            if (rayCastHit)
            {
                isGrounded = true;
                if (debugJump && !wasGrounded)
                {
                    Debug.Log("Grounded: Not moving up and raycast hit");
                }
            }
        }
        
        // Override grounding for debugging if needed
        if (Input.GetKeyDown(KeyCode.G))
        {
            isGrounded = true;
            Debug.Log("Force grounded with G key");
        }
        
        // Visual feedback
        if (showGroundDetection)
        {
            // Always show ground check
            Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, 
                isGrounded ? Color.green : Color.red);
            
            // Show text in scene view
            if (isGrounded != wasGrounded)
            {
                Debug.Log(isGrounded ? "GROUNDED" : "NOT GROUNDED");
            }
        }
    }

    public void ResetPosition()
    {
        // Reset player's position to the bottom-right corner of the maze
        transform.position = FindObjectOfType<MazeSpawner>().GetPlayerStartPosition();
        
        // Reset velocity when position is reset
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        
        // Force grounded state after reset
        isGrounded = true;
    }
}
