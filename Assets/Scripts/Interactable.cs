using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected MeshRenderer mesh;
    protected Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;

    private void Start()
    {
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = mesh.material;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interaction()
    {
        Debug.Log("Ineraction");
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;

        playerInteraction.GetInteractable().Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    public void HighlightActive(bool active)
    {
        if (active)
            mesh.material = highlightMaterial;
        else
            mesh.material = defaultMaterial;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;

        playerInteraction.GetInteractable().Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}
