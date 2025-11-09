using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public List<Interactable> interactables = new List<Interactable>();
    private Interactable closestInteractable;

    private void Start()
    {
        Player player = GetComponent<Player>();

        player.controls.Character.Ineraction.performed += context => InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        closestInteractable?.Interaction();
       
    }

    public void UpdateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);

        closestInteractable = null;
        float closestDistance = float.MaxValue;

        // Lọc ra các interactable còn tồn tại và active
        interactables.RemoveAll(i => i == null || !i.gameObject.activeInHierarchy);

        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (closestDistance > distance)
            {
                closestInteractable = interactable;
                closestDistance = distance;
            }
        }

        closestInteractable?.HighlightActive(true);
    }

    public List<Interactable> GetInteractable() => interactables;

    public void RemoveClosestInteractable()
    {
        interactables.Remove(closestInteractable); //remove because if i interact with weapon and no remove, but i hide gameobj, List still have closestInteractable

        UpdateClosestInteractable();
    } 
}
