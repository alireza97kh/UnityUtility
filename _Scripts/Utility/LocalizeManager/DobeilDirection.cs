using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DobeilDirection : MonoBehaviour
{
    private bool _isFirstPlay = true;
    private List<RectTransform> _childRectTransforms = new List<RectTransform>();
    private List<Vector2> _firstAnchoredPosition = new List<Vector2>();
    private List<Vector2> _firstAnchorMin = new List<Vector2>();
    private List<Vector2> _firstAnchorMax = new List<Vector2>();
    private List<Vector2> _firstPivot = new List<Vector2>();
    private List<Text> _childTexts = new List<Text>();
    private List<TextAnchor> _firstTextsAlignment = new List<TextAnchor>();
    private void Awake()
    {
        DobeilLocalizeManager.RegisterDirection(this);
    }

    void OnEnable()
    {
        ChangeDirection();
    }

    public void ChangeDirection()
    {
        bool RTL = DobeilLocalizeManager.Instance.isRtl;
        if (_isFirstPlay)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform rectTransform = transform.GetChild(i).GetComponent<RectTransform>();
                if (rectTransform)
                {
                    _childRectTransforms.Add(rectTransform);
                    _firstAnchoredPosition.Add(rectTransform.anchoredPosition);
                    _firstAnchorMin.Add(rectTransform.anchorMin);
                    _firstAnchorMax.Add(rectTransform.anchorMax);
                    _firstPivot.Add(rectTransform.pivot);
                }

                Text text = transform.GetChild(i).GetComponent<Text>();
                if (text)
                {
                    _childTexts.Add(text);
                    _firstTextsAlignment.Add(text.alignment);
                }
            }

            _isFirstPlay = false;
        }

        for (var i = 0; i < _childRectTransforms.Count; i++)
        {
            _childRectTransforms[i].pivot = new Vector2(RTL ? 1 - _firstPivot[i].x : _firstPivot[i].x, _firstPivot[i].y);

            if (_firstAnchorMin[i].x != 0 || _firstAnchorMax[i].x != 1)
            {
                _childRectTransforms[i].anchorMin = new Vector2(RTL ? 1 - _firstAnchorMax[i].x : _firstAnchorMin[i].x,
                    _firstAnchorMin[i].y);

                _childRectTransforms[i].anchorMax = new Vector2(RTL ? 1 - _firstAnchorMin[i].x : _firstAnchorMax[i].x,
                    _firstAnchorMax[i].y);
            }

            _childRectTransforms[i].anchoredPosition =
                new Vector2(RTL ? -_firstAnchoredPosition[i].x : _firstAnchoredPosition[i].x,
                    _firstAnchoredPosition[i].y);


        }

        for (var i = 0; i < _childTexts.Count; i++)
        {
            if (RTL)
            {
                switch (_firstTextsAlignment[i])
                {
                    case TextAnchor.UpperLeft:
                        _childTexts[i].alignment = TextAnchor.UpperRight;
                        break;
                    case TextAnchor.UpperRight:
                        _childTexts[i].alignment = TextAnchor.UpperLeft;
                        break;
                    case TextAnchor.MiddleLeft:
                        _childTexts[i].alignment = TextAnchor.MiddleRight;
                        break;
                    case TextAnchor.MiddleRight:
                        _childTexts[i].alignment = TextAnchor.MiddleLeft;
                        break;
                    case TextAnchor.LowerLeft:
                        _childTexts[i].alignment = TextAnchor.LowerRight;
                        break;
                    case TextAnchor.LowerRight:
                        _childTexts[i].alignment = TextAnchor.LowerLeft;
                        break;
                }
            }
            else
            {
                _childTexts[i].alignment = _firstTextsAlignment[i];
            }
        }
    }
}
