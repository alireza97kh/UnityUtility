using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobeilPageOneExample : DobeilPageBase
{
	protected override void HidePage(object data = null)
	{
	}

	protected override void SetPageEvents()
	{
	}

	protected override void SetPageProperty()
	{
	}

	protected override void ShowPage(object data = null)
	{
	}

	public void OnClickNextPage(string nextPageName)
	{
		DobeilPageManager.Instance.ShowPageByName(nextPageName);
	}
}
