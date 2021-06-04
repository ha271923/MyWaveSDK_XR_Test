using HTC.UnityPlugin.LiteCoroutineSystem;
using HTC.UnityPlugin.Utility.LiteTweener;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PageController : MonoBehaviour
{
    public RectTransform LeftTransform;
    public RectTransform MiddleTransform;
    public RectTransform RightTransform;
    public GameObject PanelBlocker;
    public GameObject PageBlocker;
    [Space]
    public UnityEvent Init = new UnityEvent();

    private GameObject currentPage;
    private GameObject currentPopUpWindow;

    protected virtual void OnEnable()
    {
        Init.Invoke();
        LiteCoroutine.StartCoroutine(TriggerDefaultPage(), false);
    }

    private IEnumerator TriggerDefaultPage()
    {
        yield return null;

        OpenStartPage();

        yield break;
    }

    protected virtual void OpenStartPage()
    {

    }

    public void ChangeToNextPage(GameObject nextPage)
    {
        if (nextPage == currentPage) return;

        PanelBlocker.SetActive(true);

        if (currentPage != null)
        {
            currentPage.SetActive(false);
        }

        nextPage.GetComponent<LiteRectTransformTweener>().SnapToTransform(RightTransform);
        nextPage.SetActive(true);
        nextPage.GetComponent<Animation>().Play();

        nextPage.GetComponent<LiteRectTransformTweener>().OnAnyTargetReached += PageController_OnAnyTargetReached;
        nextPage.GetComponent<LiteRectTransformTweener>().TweenToTransform(MiddleTransform);

        currentPage = nextPage;
    }

    protected void PageController_OnAnyTargetReached()
    {
        PanelBlocker.SetActive(false);
        currentPage.GetComponent<Animation>().Stop();
        currentPage.GetComponent<CanvasGroup>().alpha = 1.0f;
        currentPage.GetComponent<LiteRectTransformTweener>().OnAnyTargetReached -= PageController_OnAnyTargetReached;
    }

    public void ChangeToBackPage(GameObject backPage)
    {
        if (backPage == currentPage) return;

        PanelBlocker.SetActive(true);

        if (currentPage != null)
        {
            currentPage.SetActive(false);
        }

        backPage.GetComponent<LiteRectTransformTweener>().SnapToTransform(LeftTransform);
        backPage.SetActive(true);
        backPage.GetComponent<Animation>().Play();

        backPage.GetComponent<LiteRectTransformTweener>().OnAnyTargetReached += PageController_OnAnyTargetReached;
        backPage.GetComponent<LiteRectTransformTweener>().TweenToTransform(MiddleTransform);

        currentPage = backPage;
    }

    public void SnapChangeToPage(GameObject page)
    {
        if (page == currentPage) return;

        if (currentPage != null)
        {
            currentPage.SetActive(false);
        }

        page.GetComponent<LiteRectTransformTweener>().SnapToTransform(MiddleTransform);
        page.SetActive(true);
        currentPage = page;
    }

    public void SnapChangeToPageWithoutChangingSize(GameObject page)
    {
        if (page == currentPage) return;

        if (currentPage != null)
        {
            currentPage.SetActive(false);
        }

        page.SetActive(true);
        currentPage = page;
    }

    public void ShowPopUpWindow(GameObject popUpWindow)
    {
        if(currentPopUpWindow) currentPopUpWindow.SetActive(false);

        PageBlocker.SetActive(true);

        popUpWindow.SetActive(true);

        currentPopUpWindow = popUpWindow;
    }

    public void HidePopUpWindow()
    {
        if (currentPopUpWindow == null)
        {
            return;
        }

        currentPopUpWindow.SetActive(false);
        PageBlocker.SetActive(false);
        currentPopUpWindow = null;
    }
}
