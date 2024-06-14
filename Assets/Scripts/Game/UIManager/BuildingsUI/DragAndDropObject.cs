using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropObject : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    [SerializeField] private GameObject infoMenuPrefab;
    [SerializeField] private Image image;
    public BuildingsUI buildingsUI;

    public UnityEvent onBeginDrag;
    public UnityEvent onDrop;
    public Func<bool> isAvailable;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        Color newColor = image.color;

        if (isAvailable.Invoke() && !buildingsUI.isDragging)
        {
            newColor.a = 1f;
        }
        else
        {
            newColor.a = 0.5f;
        }

        image.color = newColor;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!isAvailable.Invoke() || buildingsUI.isDragging)
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
        if (!buildingsUI.isDragging && buildingsUI.infoMenuGO == null)
        {
            buildingsUI.infoMenuGO = Instantiate(infoMenuPrefab, 
                transform.position + new Vector3(0f, 150f, 0f), Quaternion.identity, transform.parent);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buildingsUI.infoMenuGO != null)
        {
            Destroy(buildingsUI.infoMenuGO);
            buildingsUI.infoMenuGO = null;
        }
    }
}
