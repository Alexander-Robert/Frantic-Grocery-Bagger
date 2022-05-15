using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Inventory inventory;
    private PlacedItem placedItem;

    private void Awake() {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        placedItem = GetComponent<PlacedItem>();
    }

    public void Setup(Inventory inventory) {
        this.inventory = inventory;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;

        Item.CreateVisualGrid(transform.GetChild(0), placedItem.GetItem() as Item, inventory.GetGrid().TileSize);
        DragDropSystem.Instance.StartedDragging(inventory, placedItem);
    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("OnDrag");
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        //Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        DragDropSystem.Instance.StoppedDragging(inventory, placedItem);
    }

    public void OnPointerDown(PointerEventData eventData) {
        //Debug.Log("OnPointerDown");
    }

}
