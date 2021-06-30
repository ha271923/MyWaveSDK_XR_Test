using HTC.Triton.LensUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColliderDuringDragControl : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject[] collidersNeedToBeDisabled;
    public void OnBeginDrag(PointerEventData eventData)
    {
        foreach (var collider in collidersNeedToBeDisabled)
        {
            collider.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        foreach (var collider in collidersNeedToBeDisabled)
        {
            collider.SetActive(true);
        }
    }

    
}
