using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dobeil;
[RequireComponent(typeof(ScrollRect))]
public class DobeilScrollerUtilityComponent : MonoBehaviour
{
    public ScrollerTrype scrollerTrype;
    public ScrollTo scrollTo;

    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform targetObject;
    public bool scrollToHorizontal = false;
    public bool scrollToVertical = true;
    private void Start()
	{
        StartCoroutine(ScrollToTargetObject(false, true));
	}



    public IEnumerator ScrollToTargetObject(bool scrollToHorizontal, bool scrollToVertical)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        Vector2 targetNormalizedPosition = CalculateNormalizedPosition(scrollToHorizontal, scrollToVertical, false);

        content.anchoredPosition = targetNormalizedPosition;
    }

    private Vector2 CalculateNormalizedPosition(bool scrollToHorizontal, bool scrollToVertical, bool scrollToMiddle)
    {
        RectTransform content = scrollRect.content;
        Vector3 targetLocalPosition = content.InverseTransformPoint(targetObject.position);

        float x = scrollToHorizontal ? -targetLocalPosition.x - (targetObject.rect.width / 2) : content.anchoredPosition.x;

        float y;
        if (scrollToVertical)
        {
            if (scrollToMiddle)
            {
                y = -targetLocalPosition.y + (content.rect.height / 2) - (targetObject.rect.height / 2);
            }
            else
            {
                y = -targetLocalPosition.y - (targetObject.rect.height / 2);
            }
        }
        else
        {
            y = content.anchoredPosition.y;
        }

        return new Vector2(x, y);
    }




}
