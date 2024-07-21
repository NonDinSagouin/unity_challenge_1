using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

// credit: https://www.youtube.com/watch?v=fThb5M2OBJ8

public class PlayerController : MonoBehaviour
{
    [Header("Plane Stats")]
    [Tooltip("À quel point l'accélérateur peux monter ou descendre")]
    public float throttleIncrement = 0.1f;
    [Tooltip("Poussée maximale du moteur à 100 % des gaz")]
    public float maxThrust = 200f;
    [Tooltip("Dans quelle mesure l'avion est-il réactif lors du roulis, du tangage et du lacet")]
    public float responsiveness = 10f;

    public float lift = 135f;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    private Rigidbody rb;
    private AudioSource engineSound;
    [SerializeField] TextMeshProUGUI hud;
    [SerializeField] Transform propella;

    private float responseModifier{
        get { 
            return (rb.mass / 10f) * responsiveness;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
    }

    private void HandleInputs()
    {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if(Input.GetKey(KeyCode.Space)) { throttle += throttleIncrement; }
        else if(Input.GetKey(KeyCode.LeftControl)) { throttle -= throttleIncrement; }

        throttle = Mathf.Clamp(throttle, 0, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
        UpdateHUD();

        propella.Rotate(Vector3.right * throttle);
        engineSound.volume = throttle * 0.01f;
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        rb.AddForce(transform.right * maxThrust * throttle);
        rb.AddTorque(transform.up * yaw * responseModifier);
        rb.AddTorque(transform.right * pitch * responseModifier);
        rb.AddTorque(-transform.forward * roll * responseModifier);

        rb.AddForce(Vector3.up * rb.velocity.magnitude * lift);

    }

    private void UpdateHUD()
    {
        string str_throttle = throttle.ToString("F0");
        string str_airSpeed = (rb.velocity.magnitude * 3.6f).ToString("F0");
        string str_altitude  = transform.position.y.ToString("F0");

        hud.text = $"Throttle {str_throttle} %\n";
        hud.text += $"Airspeed {str_airSpeed} km/h %\n";
        hud.text += $"Altitude {str_altitude} m";
    }

}
