using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGhost : MonoBehaviour {

    private RectTransform rectTransform;
    private Transform visual;
    private PlacedItem placedItem;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        RefreshVisual();

        ManualPlacement.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Vector2 targetPosition = ManualPlacement.Instance.GetCanvasSnappedPosition();

        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, ManualPlacement.Instance.GetPlacedItemRotation(), Time.deltaTime * 15f);
    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedItem placedItem = ManualPlacement.Instance.GetPlacedItem();

        if (placedItem != null) {
            visual = Instantiate(placedItem.GetItem().visual, transform);
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
        }
    }

}
