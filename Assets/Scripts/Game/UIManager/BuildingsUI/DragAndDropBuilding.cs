using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragAndDropBuilding : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    [SerializeField] private GameObject infoMenuPrefab;
    public BuildingsUI buildingsUI;

    private GameObject infoMenuGO;
    public UnityEvent onBeginDrag;
    public UnityEvent onDrop;
    public Func<bool> isAvaliable;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!isAvaliable.Invoke() || buildingsUI.isDragging)
        {
            return;
        }

        buildingsUI.isDragging = true;
        onBeginDrag.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!buildingsUI.isDragging)
        {
            return;
        }

        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!buildingsUI.isDragging)
        {
            return;
        }

        onDrop.Invoke();
        buildingsUI.isDragging = false;
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!buildingsUI.isDragging && infoMenuGO == null)
        {
            infoMenuGO = Instantiate(infoMenuPrefab, transform.position + new Vector3(0f, 120f, 0f), Quaternion.identity, transform.parent);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoMenuGO != null)
        {
            Destroy(infoMenuGO);
            infoMenuGO = null;
        }
    }
}
