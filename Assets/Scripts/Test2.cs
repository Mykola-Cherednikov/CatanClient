using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test2 : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    [SerializeField] private GameObject infoMenuPrefab;
    public BuildingsUI buildingsUI;

    private GameObject infoMenuGO;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!buildingsUI.isDragging)
        {
            //buildingsUI.CreateNewGameObject();
            buildingsUI.isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (buildingsUI.isDragging)
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (buildingsUI.isDragging)
        {
            Destroy(gameObject);
            buildingsUI.isDragging = false;
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!buildingsUI.isDragging && infoMenuGO == null)
        {
            infoMenuGO = Instantiate(infoMenuPrefab, transform.position + new Vector3(0f, 120f, 0f), Quaternion.identity, transform.parent);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(infoMenuGO != null)
        {
            Destroy(infoMenuGO);
            infoMenuGO = null;
        }
    }
}
