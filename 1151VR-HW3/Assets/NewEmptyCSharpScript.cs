using UnityEngine;
using UnityEngine.InputSystem;

public class RouletteController : MonoBehaviour
{
    public float maxSpeed = 500f;       // Maximum rotation speed
    public float acceleration = 250f;   // Acceleration rate
    public float deceleration = 150f;   // Deceleration rate
    public float spinDuration = 2.5f;   // How long it spins before it starts slowing down
    
    private float currentSpeed = 0f;
    private float spinTimer = 0f;
    private bool isSpinning = false;
    private bool isStopping = false;

    private AudioSource audioSource;
    private Camera mainCamera;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main; 
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Left-click: Start the spin
        if (Mouse.current.leftButton.wasPressedThisFrame && !isSpinning)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                isSpinning = true;
                isStopping = false;
                spinTimer = 0f; // Reset the timer on a new spin
                
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }

        // Rotation and timing logic
        if (isSpinning)
        {
            if (!isStopping)
            {
                // Accelerate to max speed
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
                
                // Run the timer
                spinTimer += Time.deltaTime;
                
                // Trigger deceleration automatically when the time is up
                if (spinTimer >= spinDuration)
                {
                    isStopping = true;
                }
            }
            else
            {
                // Decelerate gradually to 0
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
                
                if (currentSpeed <= 0)
                {
                    isSpinning = false;
                    isStopping = false;
                    if (audioSource != null)
                    {
                        audioSource.Stop(); 
                    }
                }
            }

            transform.Rotate(0, 0, -currentSpeed * Time.deltaTime);
        }
    }
}
