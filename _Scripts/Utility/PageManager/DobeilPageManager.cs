using Dobeil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobeilPageManager : MonoSingleton<DobeilPageManager>
{
    [HideInInspector] public DobeilPageBase activePage = null;
    [HideInInspector] public List<DobeilPageBase> pageList;
    [HideInInspector] public DobeilPageBase activePopUp = null;
    [HideInInspector] public Stack<DobeilPageBase> pageStack = new Stack<DobeilPageBase>();
    private bool adjustingPage = false;
    private string adjustingPageName = "";

    void Awake()
    {
        DobeilPageBase[] temp = GameObject.FindObjectsOfType<DobeilPageBase>();
        for (int i = 0; i < temp.Length; i++)
        {
            pageList.Add(temp[i]);
        }
        activePage = pageList.Find(page => page.isActivated == true);
        if (activePage != null)
            ChangePageState(activePage.pageName, true);
    }

    public void AddPage(DobeilPageBase newPage)
    {
        DobeilPageBase foundPage = pageList.Find(page => page.pageName == newPage.pageName);
        if (foundPage != null)
            pageList.Add(newPage);
    }

    public void ShowPageByName(string pageName, bool saveLast = false, object data = null)
    {
        if (adjustingPage == false)
        {
            adjustingPageName = pageName;
            adjustingPage = true;
            DobeilPageBase currentPage = pageList.Find(page => page.pageName == pageName);
            if (currentPage != null && currentPage.pageType == PageType.FullPage)
            {
                if (activePopUp != null)
                {
                    activePopUp.ChangePageState(false, data);
                }

                if (activePage != null)
                {
                    activePage.ChangePageState(false, data);
                }

                if (saveLast == true && activePopUp != null)
                {
                    if (activePage != null && (pageStack.Count == 0 || pageStack.Contains(activePage) == false))
                        pageStack.Push(activePage);
                    pageStack.Push(activePopUp);
                }
                else if (saveLast == true && activePage != null)
                {
                    pageStack.Push(activePage);
                }

                activePopUp = null;
                activePage = currentPage;
                activePage.ChangePageState(true, data);
            }
            else if (currentPage != null && currentPage.pageType == PageType.PopUp)
            {
                if (activePopUp != null)
                {
                    activePopUp.ChangePageState(false, data);
                }

                if (saveLast == true && activePopUp != null)
                {
                    if (activePage != null && (pageStack.Count == 0 || pageStack.Contains(activePage) == false))
                        pageStack.Push(activePage);
                    pageStack.Push(activePopUp);
                }
                else if (saveLast == true && activePage != null)
                {
                    pageStack.Push(activePage);
                }

                activePopUp = currentPage;
                activePopUp.ChangePageState(true, data);
            }
            StartCoroutine(AdjustingPageToFree(currentPage.delayToActive));
        }
        else
        {
            StartCoroutine(WaitToAdjustingLastPage(pageName, saveLast, data));
        }
    }

	IEnumerator AdjustingPageToFree(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        adjustingPage = false;
        adjustingPageName = "";
    }

    IEnumerator WaitToAdjustingLastPage(string pageName, bool saveLast = false, object data = null)
    {
        yield return new WaitForSeconds(activePage.delayToActive);
        adjustingPage = false;
        adjustingPageName = "";
        ShowPageByName(pageName, saveLast, data);
    }

    public void closePopup(object data = null)
    {
        if (activePopUp != null)
        {
            activePopUp.ChangePageState(false, data);
            if (pageStack.Count > 0)
            {
                DobeilPageBase lastPage = pageStack.Peek();
                if (lastPage.pageName == activePopUp.pageName)
                    pageStack.Pop();
            }

            activePopUp = null;
        }
    }

    public void BackToLastPage(object data = null)
    {
        if (activePopUp != null)
        {
            activePopUp.ChangePageState(false, data);
            activePopUp = null;
            if (pageStack.Count > 0)
            {
                DobeilPageBase lastPage = pageStack.Pop();
                if (lastPage.pageType == PageType.PopUp)
                {
                    activePopUp = lastPage;
                    activePopUp.ChangePageState(true, data);
                }
                else if (lastPage.pageType == PageType.FullPage)
                {
                    if (activePage != null && activePage.pageName != lastPage.pageName)
                    {
                        activePage.ChangePageState(false, data);
                        activePage = lastPage;
                        activePage.ChangePageState(true, data);
                    }
                    else
                    {
                        activePage = lastPage;
                    }
                }
            }
        }
        else if (activePage != null)
        {
            activePopUp = null;
            if (pageStack.Count > 0)
            {
                DobeilPageBase lastPage = pageStack.Pop();
                if (lastPage.pageType == PageType.PopUp)
                {
                    DobeilPageBase alternativesPage = null;
                    foreach (var page in pageStack)
                    {
                        if (page.pageType == PageType.FullPage)
                        {
                            alternativesPage = page;
                            break;
                        }
                    }

                    if (alternativesPage != null)
                    {
                        activePage.ChangePageState(false, data);
                        activePage = alternativesPage;
                        activePage.ChangePageState(true, data);
                    }

                    activePopUp = lastPage;
                    activePopUp.ChangePageState(true, data);
                }
                else if (lastPage.pageType == PageType.FullPage)
                {
                    if (activePage != null && activePage.pageName != lastPage.pageName)
                    {
                        activePage.ChangePageState(false, data);
                        activePage = lastPage;
                        activePage.ChangePageState(true, data);
                    }
                    else
                    {
                        activePage = lastPage;
                    }
                }
            }
        }
    }

    public DobeilPageBase GetActivePage()
    {
        DobeilPageBase foundPage = pageList.Find(page => page.isActivated == true);
        return foundPage;
    }

    public void ChangePageState(string pageName, bool state)
    {
        DobeilPageBase foundPage = pageList.Find(page => page.pageName == pageName);
        foundPage.ChangePageState(state);
    }

    public DobeilPageBase GetPageByName(string pageName)
    {
        DobeilPageBase foundPage = pageList.Find(page => page.pageName == pageName);
        return foundPage;
    }
}
