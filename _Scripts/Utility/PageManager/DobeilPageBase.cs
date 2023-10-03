using Dobeil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DobeilPageBase : MonoBehaviour
{
    public string pageName = "";
    public float delayToActive = (float)0;
    public PageType pageType = PageType.FullPage;
    public bool isActivated = false;

    private void Awake()
    {
        StartCoroutine(SetProperty());
    }
	IEnumerator SetProperty()
    {
        yield return new WaitForSeconds(0.5f);
        if (!DobeilPageManager.Instance.activePage)
        {
            DobeilPageManager.Instance.activePage = this;
            isActivated = true;
            ChangePageState(true);
        }

        SetPageEvents();
        SetPageProperty();
    }

    public void ChangePageState(bool active, object data = null)
    {
        StartCoroutine(SetPageActive(active, data));
    }

    IEnumerator SetPageActive(bool active, object data = null)
    {
        yield return new WaitForSeconds(delayToActive);
        isActivated = active;
        if (active)
        {
            ShowPage(data);
            //MyLogger.Log("Show Page : " + pageName);
        }
        else
        {
            HidePage(data);
            //MyLogger.Log("Hide Page : " + pageName);
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(active);
    }

    protected abstract void ShowPage(object data = null);
    protected abstract void HidePage(object data = null);
    protected abstract void SetPageEvents();
    protected abstract void SetPageProperty();
}
