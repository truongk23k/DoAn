using UnityEngine;

public class Enemy_Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollparent;
    private Collider[] ragdollColiders;
    private Rigidbody[] ragdollRigidbodies;

    private void Awake()
    {
        ragdollColiders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        RagdollActive(false);
    }

    public void RagdollActive(bool active)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }
    }

    public void CollidersActive(bool active)
    {
        foreach (Collider cd in ragdollColiders)
        {
            cd.enabled = active;
        }
    }
}
