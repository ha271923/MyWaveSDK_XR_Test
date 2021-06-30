using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_AutoSelect : MonoBehaviour
{
    [SerializeField] private Selectable SelectableUI;

    private void OnEnable()
    {
        //SelectableUI.Select();

        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(SelectableUI.gameObject, pointer, ExecuteEvents.pointerClickHandler);


        var autoNavigation = GetComponent<UI_AutoNavigation>();
        if (autoNavigation) autoNavigation.StartAutoNavigation();
    }
}
