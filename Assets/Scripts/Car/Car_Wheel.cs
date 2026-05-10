using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxeType
{
    Front,
    Back
}

[RequireComponent(typeof(WheelCollider))]
public class Car_Wheel : MonoBehaviour
{
    public AxeType axeType;
    public WheelCollider cd { get; private set; }
    public TrailRenderer trail { get; private set; }
    public GameObject model;

    public float defaultSideStiffness;

    private void Awake()
    {
        cd = GetComponent<WheelCollider>();
        trail = GetComponentInChildren<TrailRenderer>();

        trail.emitting = false;

        if (model == null)
            model = GetComponentInChildren<MeshRenderer>().gameObject;

    }

    public void SetDefaultStiffness(float newValue)
    {
        defaultSideStiffness = newValue;
        RestoreDefaultSideStiffness();
    }

    public void RestoreDefaultSideStiffness()
    {
        WheelFrictionCurve frictionCurve = cd.sidewaysFriction;
        frictionCurve.stiffness = defaultSideStiffness;
        cd.sidewaysFriction = frictionCurve;
    }
}
