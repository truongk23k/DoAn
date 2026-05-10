using System;
using UnityEngine;


public enum DriveType
{
    FrontWheelDrive,
    RearWheelDrive,
    AllWheelDrive
}

[RequireComponent(typeof(Rigidbody))]
public class Car_Controller : MonoBehaviour
{
    public Car_Sounds carSounds { get; private set; }

    public bool carActive { get; private set; }
    private PlayerControlls controls;
    public Rigidbody rb { get; private set; }
    private float moveInput;
    private float steerInput;

    public float speed;

    [SerializeField] private LayerMask whatIsGround;

    [Range(30f, 60f)]
    [SerializeField] private float turnSensitivity = 30f;
    [Header("Car Settings")]
    [SerializeField] private DriveType driveType;
    [SerializeField] private Transform centerOfMass;
    [Range(350f, 1000f)]
    [SerializeField] private float carMass = 400f;
    [Range(20, 80)]
    [SerializeField] private float wheelsMass = 30f;
    [Range(0.5f, 2f)]
    [SerializeField] private float frontWheelTraction = 1f;
    [Range(0.5f, 2f)]
    [SerializeField] private float backWheelTraction = 1f;

    [Header("Engine Settings")]
    [SerializeField] private float currentSpeed;
    [Range(7, 12)]
    [SerializeField] private float maxSpeed = 7;
    [Range(0.5f, 10f)]
    [SerializeField] private float accelerationSpeed = 2f;
    [Range(1500, 5000)]
    [SerializeField] private float motorForce = 1500f;

    [Header("Brakes Settings")]
    [Range(0f, 10f)]
    [SerializeField] private float frontBrakesSensitivity = 5f;
    [Range(0f, 10f)]
    [SerializeField] private float backBrakesSensitivity = 5f;
    [Range(4000f, 6000f)]
    [SerializeField] private float brakePower = 5000f;

    private bool isBraking;

    [Header("Drifr Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float frontDriftFactor = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float backDriftFactor = 0.5f;
    [SerializeField] private float driftDuration = 1f;
    private float driftTimer;
    private bool isDrifting;
    private bool canEmitTrails = true;

    private Car_Wheel[] wheels;
    private UI ui;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();
        carSounds = GetComponent<Car_Sounds>();
        ui = UI.instance;

        controls = ControlsManager.instance.controls;
        //ControlsManager.instance.SwitchToCarControls();


        ActivateCar(false);
        AssignInputEvents();

        SetupDefaultValues();
    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelsMass;

            if (wheel.axeType == AxeType.Front)
            {
                wheel.SetDefaultStiffness(frontWheelTraction);
            }
            else
            {
                wheel.SetDefaultStiffness(backWheelTraction);
            }
        }
    }

    private void Update()
    {
        if (!carActive)
            return;

        ui.inGameUI.UpdateCarSpeedText(Mathf.RoundToInt(speed * 15).ToString() + " km/h");

        speed = rb.velocity.magnitude;

        driftTimer -= Time.deltaTime;
        if (driftTimer < 0)
            isDrifting = false;
    }

    private void FixedUpdate()
    {
        ApplyTrailsOnGround();
        if (!carActive)
            return;


        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();
        ApplySpeedLimit();

        if (isDrifting)
            ApplyDrift();
        else
            StopDrift();
    }


    private void ApplyDrive()
    {
        currentSpeed = moveInput * accelerationSpeed * Time.deltaTime;

        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.axeType == AxeType.Front)
                {
                    wheel.cd.motorTorque = motorTorqueValue;
                }
            }
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.axeType == AxeType.Back)
                {
                    wheel.cd.motorTorque = motorTorqueValue;
                }
            }
            else
            {
                wheel.cd.motorTorque = motorTorqueValue;
            }
        }
    }

    private void ApplySpeedLimit()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void ApplySteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axeType == AxeType.Front)
            {
                float targetSteerAngle = steerInput * turnSensitivity;
                wheel.cd.steerAngle = Mathf.Lerp(wheel.cd.steerAngle, targetSteerAngle, 0.5f);
            }
        }
    }

    private void ApplyBrakes()
    {
        foreach (var wheel in wheels)
        {
            bool frontBrakes = wheel.axeType == AxeType.Front;
            float brakeSensitivity = frontBrakes ? frontBrakesSensitivity : backBrakesSensitivity;

            float newBrakeTorque = brakePower * brakeSensitivity * Time.deltaTime;
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0f;

            wheel.cd.brakeTorque = currentBrakeTorque;
        }
    }

    private void ApplyDrift()
    {
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axeType == AxeType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : backDriftFactor;

            WheelFrictionCurve sidewaysFriction = wheel.cd.sidewaysFriction;

            sidewaysFriction.stiffness *= (1 - driftFactor);
            wheel.cd.sidewaysFriction = sidewaysFriction;
        }
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultSideStiffness();
        }
    }

    private void ApplyAnimationToWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            wheel.cd.GetWorldPose(out position, out rotation);

            if (wheel.model != null)
            {
                wheel.model.transform.position = position;
                wheel.model.transform.rotation = rotation;
            }
        }
    }
    private void ApplyTrailsOnGround()
    {
        if (!canEmitTrails)
            return;

        foreach (var wheel in wheels)
        {
            WheelHit hit;
            if (wheel.cd.GetGroundHit(out hit))
            {
                if (whatIsGround == (whatIsGround | (1 << hit.collider.gameObject.layer)))
                {
                    if (wheel.trail != null)
                        wheel.trail.emitting = true;
                }
                else
                {
                    if (wheel.trail != null)
                        wheel.trail.emitting = false;
                }
            }
            else
            {
                if (wheel.trail != null)
                    wheel.trail.emitting = false;
            }
        }
    }


    public void ActivateCar(bool activate)
    {
        carActive = activate;

        if (carSounds != null)
            carSounds.ActivateCarSFX(activate);

        /*if (activate)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }*/

        /* if (!activate)
         {
             foreach (var wheel in wheels)
             {
                 wheel.RestoreDefaultSideStiffness();

                 if (wheel.trail != null)
                     wheel.trail.emitting = false;
             }
         }*/
    }

    public void BrakeTheCar()
    {
        canEmitTrails = false;

        foreach (var wheel in wheels)
        {
            if (wheel.trail != null)
                wheel.trail.emitting = false;
        }

        rb.drag = 1;
        motorForce = 0;
        isDrifting = true;
        frontDriftFactor = 0.9f;
        backDriftFactor = 0.9f;
    }

    private void AssignInputEvents()
    {
        controls.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            moveInput = input.y;
            steerInput = input.x;
        };

        controls.Car.Movement.canceled += ctx =>
        {
            moveInput = 0;
            steerInput = 0;
        };

        controls.Car.Brake.performed += ctx =>
        {
            isBraking = true;
            isDrifting = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += ctx => isBraking = false;

        controls.Car.CarExit.performed += ctx => GetComponent<Car_Interaction>().GetOutOfTheCar();
    }
}
