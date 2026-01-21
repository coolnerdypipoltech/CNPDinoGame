using System;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;

    public float jumpForce = 12f;
    public float gravity = 9.81f * 2f;

    public bool DebugMode = false;

    // Microphone variables
    private AudioSource audioSource;
    private string microphoneDevice;
    private AudioClip microphoneClip;



    [Header("Jump Physics")]
    public float gravityMultiplierUp = 1.5f; // Gravedad más fuerte al subir (1.5x más rápido)
    public float gravityMultiplierDown = 2.5f; // Gravedad más fuerte al caer (2.5x más rápido)
    public float gravityMultiplierPeak = 0.3f; // Gravedad reducida en la cúspide (flotar)
    public float peakThreshold = 1f; // Velocidad vertical considerada como "cúspide"

    private bool jumpTrigger = false;

    private void Awake()
    {
        character = GetComponent<CharacterController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 0f;
    }


    private void OnEnable()
    {
        direction = Vector3.zero;
    }

    public void TriggerJump()
    {
        if (character.isGrounded)
        {
            direction = Vector3.up * jumpForce;
            jumpForce = 12f;
            Debug.Log("Jump triggered!");
        }
    }

    public void SetJumpTriggerForce(float force)
    {
        jumpTrigger = true;
        jumpForce = Mathf.Clamp(force / 4, 12f, 20f);
    }

    public void SetJumpTrigger()
    {
        Debug.Log("Jump trigger set!");
        jumpTrigger = true;
    }

    void Update()
    {
        if (DebugMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Jump key pressed!");
                SetJumpTrigger();
            }
        }
    }

    private void FixedUpdate()
    {
        // Determinar qué multiplicador de gravedad usar
        float currentGravityMultiplier;
        
        if (Mathf.Abs(direction.y) < peakThreshold && direction.y > 0)
        {
            // En la cúspide del salto (velocidad vertical baja y positiva)
            currentGravityMultiplier = gravityMultiplierPeak;
        }
        else if (direction.y > 0)
        {
            // Subiendo (velocidad vertical positiva)
            currentGravityMultiplier = gravityMultiplierUp;
        }
        else
        {
            // Cayendo (velocidad vertical negativa)
            currentGravityMultiplier = gravityMultiplierDown;
        }

        // Aplicar gravedad con el multiplicador correspondiente
        direction += gravity * currentGravityMultiplier * Time.deltaTime * Vector3.down;
        

        if (jumpTrigger)
        {
            jumpTrigger = false;
            TriggerJump();
        }
        else
        {
            if (character.isGrounded)
            {
                direction = Vector3.down;
            }
        }

        character.Move(direction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }
}