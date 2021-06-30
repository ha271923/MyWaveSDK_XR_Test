using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_AutoNavigation : MonoBehaviour
{
    [SerializeField] protected Selectable CurrentUI;
    [SerializeField] protected Selectable NextUI;
    [SerializeField] protected Selectable PreviousUI;

    [SerializeField] protected Selectable DummyForReselect;

    protected bool isStartAutoNavigation = false;

    protected bool navToNext = false;

    protected void OnDisable()
    {
        //Debug.Log("Stop Auto Navigation.");
        isStartAutoNavigation = false;
        navToNext = false;
    }

    public virtual void StartAutoNavigation()
    {
        isStartAutoNavigation = true;
        navToNext = false;
    }

    private void LateUpdate()
    {
        if (navToNext)
        {
            NavigateToNext();
            navToNext = false;
        }
        if (isStartAutoNavigation == true)
        {
            if (CheckAcceptable() == true)
            {
                isStartAutoNavigation = false;
                //NavigateToNext();
                navToNext = true;
            }
        }
    }

    protected virtual bool CheckAcceptable()
    {
        return false;
    }

    protected virtual void NavigateToNext()
    {
        if (NextUI != null)
        {
            //NextUI.Select();
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(NextUI.gameObject, pointer, ExecuteEvents.pointerClickHandler);

            var nextUI_AutoNavigation = NextUI.GetComponent<UI_AutoNavigation>();
            if (nextUI_AutoNavigation != null)
            {
                nextUI_AutoNavigation.PreviousUI = CurrentUI;
                nextUI_AutoNavigation.StartAutoNavigation();
            }
        }
        else
        {
            isStartAutoNavigation = true;
        }
    }

}
