using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//handle logic for holding an item in hand (AKA dragging/moving it to a different spot)
public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Inventory inventory;
    private PlacedObject placedObject;

    private void Awake() {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<PlacedObject>();
    }

    public void Setup(Inventory inventory) {
        this.inventory = inventory;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;

        ItemSO.CreateVisualGrid(transform.GetChild(0), placedObject.GetPlacedObjectTypeSO() as ItemSO, inventory.GetGrid().GetCellSize());
        DragDropSystem.Instance.StartedDragging(inventory, placedObject);
    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("OnDrag");
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        //Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        DragDropSystem.Instance.StoppedDragging(inventory, placedObject);
    }

    public void OnPointerDown(PointerEventData eventData) {
        //Debug.Log("OnPointerDown");
    }

}
