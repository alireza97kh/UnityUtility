using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Dobeil;
using TouchType = Dobeil.TouchType;
public abstract class DobeilTouchManager : MonoBehaviour
{
    [PreviewField(70, ObjectFieldAlignment.Center)]
    public Texture2D SingleObject;
    [EnumToggleButtons]
    [HideLabel]
    [FoldoutGroup("Touch Manager")]
    public TouchType touchType = TouchType.DRAG;

    [EnumToggleButtons]
    [Title("Target Mode", null, TitleAlignments.Centered)]
    [HideLabel]
    [FoldoutGroup("Touch Manager")]
    public TargetMode targetMode = TargetMode.ME;

    [ShowIf("targetMode", TargetMode.OTHER)]
    [Required("Target cannot be null", InfoMessageType.Warning)]
    [Space(2)]
    [FoldoutGroup("Touch Manager")]
    public Transform targetObject;

    [EnumToggleButtons]
    [Title("Touch Mode", null, TitleAlignments.Centered)]
    [HideLabel]
    [FoldoutGroup("Touch Manager")]
    public TouchMode touchMode = TouchMode.MOUSE;

    [Space(20)]
    [FoldoutGroup("Touch Manager")]
    public List<UnityEvent> onInitActions;

    [Space(4)]
    [InfoBox("For hit Target should have Collider and RigidBody", InfoMessageType.Warning)]
    [FoldoutGroup("Touch Manager")]
    [EnumPaging]
    public HitState hitState = HitState.TRIGGER;

    [Title("Start Touch", null, TitleAlignments.Centered)]
    [FoldoutGroup("Touch Manager")]
    public StartTouchStateClass startTouchState;

    [Title("Touching", null, TitleAlignments.Centered)]
    [FoldoutGroup("Touch Manager")]
    public TouchingStateClass touchingState;

    [Title("End Touch", null, TitleAlignments.Centered)]
    [FoldoutGroup("Touch Manager")]
    public EndTouchStateClass endTouchState;

    [Space(10)]
    [ShowIf("touchType", TouchType.DRAG)]
    [InfoBox("Just use X of Touch")]
    [FoldoutGroup("Touch Manager")]
    public bool justDragInX = false;

    [ShowIf("touchType", TouchType.DRAG)]
    [InfoBox("Just use Y of Touch")]
    [FoldoutGroup("Touch Manager")]
    public bool justDragInY = false;

    [ShowIf("touchType", TouchType.DOUBLETOUCH)]
    [InfoBox("Delay between two touches")]
    [FoldoutGroup("Touch Manager")]
    public float maxWaitForDoubleClick = 0.4f;


    bool isDragging = false;
    bool isClicking = false;
    int isDoubleClicking = -1; // -1 => Not Clicked, 0 => one time Clicked, 1 => double Clicked
    Vector3 screenPosition = Vector3.zero;
    Vector3 offset;
    float timer = 0;
    Transform lastTransformSended = null;
    Transform currentTransformSended = null;
    protected Vector3 startPosition = Vector3.zero;
    Transform lastAvaiableTransformTouching;
    Transform lastAvaiableTransformEndTouch;
    bool hasHit = false;
    bool isInHit = false;


    void Start()
    {
        if (targetMode == TargetMode.ME)
        {
            targetObject = transform;
        }
        lastTransformSended = targetObject;
        for (int i = 0; i < onInitActions.Count; i++)
        {
            onInitActions[i]?.Invoke();
        }
        startPosition = targetObject.position;
    }
    void Update()
    {
        if (touchMode == TouchMode.MOUSE)
        {
            MouseManager();
        }
        else if (touchMode == TouchMode.FINGER)
        {
            FingerManager();
        }
        else if (touchMode == TouchMode.BOTH)
        {
            if (Input.touchCount > 0)
                FingerManager();
            else
                MouseManager();
        }
    }
    #region Abstract Functios 
    public abstract void TouchStart(Transform other);
    public abstract void TouchEnd(Transform other);
    public abstract void IsInTouch(Transform other);
    public virtual void BackToStartPosition()
    {
        targetObject.position = startPosition;
    }
    #endregion

    GameObject ReturnClickedObject(Vector3 position, out RaycastHit hit)
    {
        GameObject targetObject = null;
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            targetObject = hit.collider.gameObject;
        }
        return targetObject;
    }
    void ResetToStartValues()
    {
        lastTransformSended = targetObject;
        lastAvaiableTransformEndTouch = null;
        currentTransformSended = null;
        hasHit = false;
    }
    #region Mouse Managers
    void MouseManager()
    {
        if (touchType == TouchType.DRAG)
        {
            MouseDragManager();
        }
        else if (touchType == TouchType.TOUCH)
        {
            MouseClickManager();
        }
        else if (touchType == TouchType.DOUBLETOUCH)
        {
            MouseDoubleClickManager();
        }
    }
    void EndTouchStateManagerForClickAndDouble(EndDTouchState state, Vector3 customPosition, Transform customTransform)
    {
        switch (state)
        {
            case EndDTouchState.MOVE_TO_CUSTOM_POSITION:
                targetObject.position = customPosition;
                break;
            case EndDTouchState.MOVE_TO_CUSTOM_TRANSFORM:
                targetObject.position = customTransform.position;
                break;
            case EndDTouchState.DESTROY:
                Destroy(targetObject.gameObject);
                break;
            case EndDTouchState.DEACTIVE:
                targetObject.gameObject.SetActive(false);
                break;
            case EndDTouchState.UN_ENABLE:
                this.enabled = false;
                break;
            default:
                break;
        }
    }

    #region Drag
    void MouseDragManager()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckStartDragManager(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndDragManager();
        }
        if (isDragging)
        {
            DragginManager(Input.mousePosition);
        }
    }

    void CheckStartDragManager(Vector3 position)
    {
        RaycastHit hitInfo;
        GameObject target = ReturnClickedObject(position, out hitInfo);
        if (target != null && target.transform == targetObject)
        {
            switch (startTouchState.state)
            {
                case TouchingStates.CUSTOM_TAG:
                    if (target.tag == startTouchState.customTag)
                        StartTouch(target, position);
                    break;
                case TouchingStates.CUSTOM_LAYER:
                    if (target.layer == startTouchState.customLayer)
                        StartTouch(target, position);
                    break;
                case TouchingStates.CUSTOM_NAME:
                    if (target.name == startTouchState.customName)
                        StartTouch(target, position);
                    break;
                case TouchingStates.NONE:
                    StartTouch(target, position);
                    break;
                default:
                    break;
            }
        }
    }
    void StartTouch(GameObject target, Vector3 position)
    {
        ResetToStartValues();
        lastAvaiableTransformTouching = targetObject;
        lastAvaiableTransformEndTouch = targetObject;
        startPosition = targetObject.position;
        isDragging = true;
        //Here we Convert world position to screen position.
        screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
        offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, screenPosition.z));
        for (int i = 0; i < startTouchState.events.Count; i++)
        {
            startTouchState.events[i]?.Invoke();
        }
        TouchStart(targetObject);

    }
    void DragginManager(Vector3 position)
    {
        if (lastTransformSended != currentTransformSended)
        {
            lastTransformSended = currentTransformSended;
            for (int i = 0; i < touchingState.events.Count; i++)
            {
                touchingState.events[i]?.Invoke();
            }
            IsInTouch(currentTransformSended);
        }
        Vector3 currentScreenSpace = new Vector3(position.x, position.y, screenPosition.z);

        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;

        if (justDragInX && !justDragInY)
            currentPosition = new Vector3(currentPosition.x, startPosition.y, startPosition.z);
        else if (justDragInY && !justDragInX)
            currentPosition = new Vector3(startPosition.x, currentPosition.y, startPosition.z);

        targetObject.transform.position = currentPosition;
    }
    void EndDragManager()
    {
        if (isDragging)
        {
            CheckEndTouch();
            ResetToStartValues();
            isDragging = false;
        }
    }
    void CheckEndTouch()
    {
        switch (endTouchState.state)
        {
            case TouchingStates.NONE:
                if (hasHit)
                    EndStateWithHit();
                else
                    EndDragStateManager(endTouchState.touchStateWithOutHit);
                break;
            case TouchingStates.CUSTOM_TAG:
                if (lastAvaiableTransformEndTouch != null && lastAvaiableTransformEndTouch.tag == endTouchState.customTag)
                    EndStateWithHit();
                else
                    EndDragStateManager(endTouchState.touchStateWithOutHit);
                break;
            case TouchingStates.CUSTOM_LAYER:
                if (lastAvaiableTransformEndTouch != null && lastAvaiableTransformEndTouch.gameObject.layer == endTouchState.customLayer)
                    EndStateWithHit();
                else
                    EndDragStateManager(endTouchState.touchStateWithOutHit);
                break;
            case TouchingStates.CUSTOM_NAME:
                if (lastAvaiableTransformEndTouch != null && lastAvaiableTransformEndTouch.name == endTouchState.customName)
                    EndStateWithHit();
                else
                    EndDragStateManager(endTouchState.touchStateWithOutHit);
                break;
            default:
                break;
        }
    }

    void EndStateWithHit()
    {
        EndDragStateManager(endTouchState.touchStateWithHit);
        if (lastTransformSended != currentTransformSended)
        {
            for (int i = 0; i < endTouchState.events.Count; i++)
            {
                endTouchState.events[i]?.Invoke();
            }
            lastTransformSended = currentTransformSended;
            TouchEnd(currentTransformSended);
        }
    }

    void EndDragStateManager(EndDTouchState state)
    {
        switch (state)
        {
            case EndDTouchState.BACK_TO_START:
                targetObject.position = startPosition;
                break;
            case EndDTouchState.BACK_TO_LAST_HIT:
                targetObject.position = lastAvaiableTransformEndTouch.position;
                break;
            case EndDTouchState.MOVE_TO_CUSTOM_POSITION:
                targetObject.position = endTouchState.customPosition;
                break;
            case EndDTouchState.MOVE_TO_CUSTOM_TRANSFORM:
                targetObject.position = endTouchState.customTransform.position;
                break;
            case EndDTouchState.DESTROY:
                Destroy(gameObject);
                break;
            case EndDTouchState.DEACTIVE:
                gameObject.SetActive(false);
                break;
            case EndDTouchState.UN_ENABLE:
                this.enabled = false;
                break;
            default:
                break;
        }
    }
    #endregion
    #region Click
    void MouseClickManager()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartClick(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && isClicking)
        {
            EndClick(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && isClicking)
        {
            InClicking();
        }
    }

    void StartClick(Vector3 position)
    {
        lastTransformSended = null;
        RaycastHit hitInfo;
        GameObject target = ReturnClickedObject(position, out hitInfo);
        if (target != null && target.transform == targetObject)
        {
            switch (startTouchState.state)
            {
                case TouchingStates.CUSTOM_TAG:
                    if (target.tag == startTouchState.customTag)
                        StartClickDone();
                    break;
                case TouchingStates.CUSTOM_LAYER:
                    if (target.layer == startTouchState.customLayer)
                        StartClickDone();
                    break;
                case TouchingStates.CUSTOM_NAME:
                    if (target.name == startTouchState.customName)
                        StartClickDone();
                    break;
                case TouchingStates.NONE:
                    StartClickDone();
                    break;
                default:
                    break;
            }
        }
    }
    void StartClickDone()
    {
        currentTransformSended = targetObject;
        isClicking = true;
        for (int i = 0; i < startTouchState.events.Count; i++)
        {
            startTouchState.events[i]?.Invoke();
        }
        TouchStart(targetObject);
    }

    void EndClick(Vector3 position)
    {
        RaycastHit hitInfo;
        GameObject target = ReturnClickedObject(position, out hitInfo);
        if (target != null && target.transform == targetObject)
        {
            switch (endTouchState.state)
            {
                case TouchingStates.CUSTOM_TAG:
                    if (target.tag == endTouchState.customTag)
                        EndClickDone();
                    break;
                case TouchingStates.CUSTOM_LAYER:
                    if (target.layer == endTouchState.customLayer)
                        EndClickDone();
                    break;
                case TouchingStates.CUSTOM_NAME:
                    if (target.name == endTouchState.customName)
                        EndClickDone();
                    break;
                case TouchingStates.NONE:
                    EndClickDone();
                    break;
                default:
                    break;
            }
        }
    }
    void EndClickDone()
    {
        isClicking = false;
        EndTouchStateManagerForClickAndDouble(endTouchState.touchStateWithHit, endTouchState.customPosition, endTouchState.customTransform);
        for (int i = 0; i < endTouchState.events.Count; i++)
        {
            endTouchState.events[i]?.Invoke();
        }
        TouchEnd(currentTransformSended);
    }

    void InClicking()
    {
        if (lastTransformSended != currentTransformSended)
        {
            switch (touchingState.state)
            {
                case TouchingStates.NONE:
                    InClickingDone();
                    break;
                case TouchingStates.CUSTOM_TAG:
                    if (currentTransformSended.tag == touchingState.customTag)
                        InClickingDone();
                    break;
                case TouchingStates.CUSTOM_LAYER:
                    if (currentTransformSended.gameObject.layer == touchingState.customLayer)
                        InClickingDone();
                    break;
                case TouchingStates.CUSTOM_NAME:
                    if (currentTransformSended.name == touchingState.customName)
                        InClickingDone();
                    break;
                default:
                    break;
            }
        }
    }
    void InClickingDone()
    {
        lastTransformSended = currentTransformSended;
        EndTouchStateManagerForClickAndDouble(touchingState.touchState, touchingState.customPosition, touchingState.customTransform);
        for (int i = 0; i < touchingState.events.Count; i++)
        {
            touchingState.events[i]?.Invoke();
        }
        IsInTouch(currentTransformSended);
    }

    #endregion
    #region Double Click
    void MouseDoubleClickManager()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DoubleClick_StartClick(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && isDoubleClicking >= 1)
        {
            isDoubleClicking = -1;
            timer = 0;
            DoubleClick_EndClick(Input.mousePosition);
        }
        if (isDoubleClicking == 0)
        {
            DoubleClick_IsInClick();
        }
        DoubleClick_TimerController();
    }

    void DoubleClick_StartClick(Vector3 position)
    {
        RaycastHit hitInfo;
        GameObject target = ReturnClickedObject(position, out hitInfo);
        lastTransformSended = null;
        if (target != null && target.transform == targetObject)
        {
            switch (startTouchState.state)
            {
                case TouchingStates.NONE:
                    DoubleClick_StartClickDone();
                    break;
                case TouchingStates.CUSTOM_TAG:
                    if (target.tag == startTouchState.customTag)
                        DoubleClick_StartClickDone();
                    break;
                case TouchingStates.CUSTOM_LAYER:
                    if (target.gameObject.layer == startTouchState.customLayer)
                        DoubleClick_StartClickDone();
                    break;
                case TouchingStates.CUSTOM_NAME:
                    if (target.name == startTouchState.customName)
                        DoubleClick_StartClickDone();
                    break;
                default:
                    break;
            }
        }
    }
    void DoubleClick_StartClickDone()
    {
        if (isDoubleClicking == -1)
        {
            for (int i = 0; i < startTouchState.events.Count; i++)
            {
                startTouchState.events[i]?.Invoke();
            }
            currentTransformSended = targetObject;
            TouchStart(targetObject);
        }
        isDoubleClicking += 1;
    }

    void DoubleClick_EndClick(Vector3 position)
    {
        RaycastHit hitInfo;
        GameObject target = ReturnClickedObject(position, out hitInfo);
        if (target != null && target.transform == targetObject)
        {
            switch (endTouchState.state)
            {
                case TouchingStates.NONE:
                    DoubleClick_EndClickDone();
                    break;
                case TouchingStates.CUSTOM_TAG:
                    if (target.tag == endTouchState.customTag)
                        DoubleClick_EndClickDone();
                    break;
                case TouchingStates.CUSTOM_LAYER:
                    if (target.gameObject.layer == endTouchState.customLayer)
                        DoubleClick_EndClickDone();
                    break;
                case TouchingStates.CUSTOM_NAME:
                    if (target.name == endTouchState.customName)
                        DoubleClick_EndClickDone();
                    break;
                default:
                    break;
            }
        }
    }
    void DoubleClick_EndClickDone()
    {
        currentTransformSended = targetObject;
        EndTouchStateManagerForClickAndDouble(endTouchState.touchStateWithHit, endTouchState.customPosition, endTouchState.customTransform);
        for (int i = 0; i < endTouchState.events.Count; i++)
        {
            endTouchState.events[i]?.Invoke();
        }
        TouchEnd(currentTransformSended);
    }

    void DoubleClick_IsInClick()
    {
        switch (touchingState.state)
        {
            case TouchingStates.NONE:
                if (lastTransformSended != currentTransformSended)
                    DoubleClick_IsInClickDone();
                break;
            case TouchingStates.CUSTOM_TAG:
                if (lastTransformSended != currentTransformSended)
                    if (currentTransformSended.tag == touchingState.customTag)
                        DoubleClick_IsInClickDone();
                break;
            case TouchingStates.CUSTOM_LAYER:
                if (lastTransformSended != currentTransformSended)
                    if (currentTransformSended.gameObject.layer == touchingState.customLayer)
                        DoubleClick_IsInClickDone();
                break;
            case TouchingStates.CUSTOM_NAME:
                if (lastTransformSended != currentTransformSended)
                    if (currentTransformSended.name == touchingState.customName)
                        DoubleClick_IsInClickDone();
                break;
            default:
                break;
        }
    }
    void DoubleClick_IsInClickDone()
    {
        lastTransformSended = currentTransformSended;
        EndTouchStateManagerForClickAndDouble(touchingState.touchState, touchingState.customPosition, touchingState.customTransform);
        for (int i = 0; i < touchingState.events.Count; i++)
        {
            touchingState.events[i]?.Invoke();
        }
        IsInTouch(currentTransformSended);
    }

    void DoubleClick_TimerController()
    {
        if (isDoubleClicking == 0)
        {
            timer += Time.deltaTime;
        }
        if (timer >= maxWaitForDoubleClick)
        {
            isDoubleClicking = -1;
            timer = 0;
        }
    }

    #endregion
    #endregion
    #region Finger Manager
    void FingerManager()
    {
        if (touchType == TouchType.DRAG)
        {
            TouchDragManager();
        }
        else if (touchType == TouchType.TOUCH)
        {
            TouchClickManager();
        }
        else if (touchType == TouchType.DOUBLETOUCH)
        {
            TouchDoubleClickManager();
        }
    }

    #region Drag
    void TouchDragManager()
    {
        Touch[] touches = Input.touches;
        if (touches.Length > 0)
        {
            if (touches[0].phase == TouchPhase.Began)
            {
                CheckStartDragManager(touches[0].position);
            }
            else if (touches[0].phase == TouchPhase.Ended)
            {
                EndDragManager();
            }
            else if (isDragging)
            {
                DragginManager(touches[0].position);
            }
        }
    }
    #endregion
    #region Click
    void TouchClickManager()
    {
        Touch[] touches = Input.touches;
        if (touches.Length > 0)
        {
            if (touches[0].phase == TouchPhase.Began)
            {
                StartClick(touches[0].position);
            }
            else if (touches[0].phase == TouchPhase.Ended && isClicking)
            {
                EndClick(touches[0].position);
            }
            else if ((touches[0].phase == TouchPhase.Moved || touches[0].phase == TouchPhase.Stationary) && isClicking)
            {
                InClicking();
            }
        }
    }
    #endregion
    #region Double Click
    void TouchDoubleClickManager()
    {
        Touch[] touches = Input.touches;
        if (touches.Length > 0)
        {
            if (touches[0].phase == TouchPhase.Began)
            {
                DoubleClick_StartClick(touches[0].position);
            }
            else if (touches[0].phase == TouchPhase.Ended && isDoubleClicking >= 1)
            {
                isDoubleClicking = -1;
                timer = 0;
                DoubleClick_EndClick(touches[0].position);
            }
            if (isDoubleClicking == 0)
            {
                DoubleClick_IsInClick();
            }
            DoubleClick_TimerController();
        }
    }
    #endregion
    #endregion
    #region Hit
    private void OnTriggerEnter(Collider other)
    {
        if (hitState == HitState.TRIGGER)
            ManageTouchingStateInHit(other.transform);
    }
    public void OnTriggerStay(Collider other)
    {
        if (hitState == HitState.TRIGGER)
            CheckHitStay(other.transform, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (hitState == HitState.TRIGGER)
            CheckHitStay(null, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hitState == HitState.COLLISION)
            ManageTouchingStateInHit(collision.transform);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (hitState == HitState.COLLISION)
            CheckHitStay(collision.transform, true);
    }
    public void OnCollisionExit(Collision collision)
    {
        if (hitState == HitState.COLLISION)
            CheckHitStay(null, false);
    }

    void ManageTouchingStateInHit(Transform other, Action onDone = null)
    {
        switch (touchingState.state)
        {
            case TouchingStates.NONE:
                CorrectStateOfHit(other);
                break;
            case TouchingStates.CUSTOM_TAG:
                if (other.tag == touchingState.customTag)
                    CorrectStateOfHit(other);
                break;
            case TouchingStates.CUSTOM_LAYER:
                if (other.gameObject.layer == touchingState.customLayer)
                    CorrectStateOfHit(other);
                break;
            case TouchingStates.CUSTOM_NAME:
                if (other.name == touchingState.customName)
                    CorrectStateOfHit(other);
                break;
            default:
                break;
        }

        switch (endTouchState.state)
        {
            case TouchingStates.NONE:
                lastAvaiableTransformEndTouch = other;
                break;
            case TouchingStates.CUSTOM_TAG:
                if (other.tag == touchingState.customTag)
                    lastAvaiableTransformEndTouch = other;
                break;
            case TouchingStates.CUSTOM_LAYER:
                if (other.gameObject.layer == touchingState.customLayer)
                    lastAvaiableTransformEndTouch = other;
                break;
            case TouchingStates.CUSTOM_NAME:
                if (other.name == touchingState.customName)
                    lastAvaiableTransformEndTouch = other;
                break;
            default:
                break;
        }

    }

    void CorrectStateOfHit(Transform other)
    {
        for (int i = 0; i < touchingState.events.Count; i++)
        {
            touchingState.events[i]?.Invoke();
        }
        currentTransformSended = other.transform;
        lastAvaiableTransformTouching = other.transform;
        hasHit = true;
        CheckAfterHit();
    }

    void CheckAfterHit()
    {
        switch (touchingState.touchState)
        {
            case EndDTouchState.NONE:
                break;
            case EndDTouchState.BACK_TO_START:
                targetObject.position = startPosition;
                isDragging = false;
                break;
            case EndDTouchState.BACK_TO_LAST_HIT:
                targetObject.position = lastAvaiableTransformTouching.position;
                isDragging = false;
                break;
            case EndDTouchState.STAY_IN_POSITION:
                isDragging = false;
                break;
            case EndDTouchState.MOVE_TO_CUSTOM_POSITION:
                targetObject.position = touchingState.customPosition;
                isDragging = false;
                break;
            case EndDTouchState.MOVE_TO_CUSTOM_TRANSFORM:
                targetObject.position = touchingState.customTransform.position;
                isDragging = false;
                break;
            case EndDTouchState.DESTROY:
                Destroy(targetObject.gameObject);
                break;
            case EndDTouchState.DEACTIVE:
                targetObject.gameObject.SetActive(false);
                break;
            case EndDTouchState.UN_ENABLE:
                this.enabled = false;
                break;
            default:
                break;
        }
    }

    void CheckHitStay(Transform other, bool _isInHit)
    {
        switch (touchingState.state)
        {
            case TouchingStates.NONE:
                currentTransformSended = other;
                isInHit = _isInHit;
                break;
            case TouchingStates.CUSTOM_TAG:
                if ((other == null && !_isInHit) || (other != null && _isInHit && other.tag == touchingState.customTag))
                {
                    currentTransformSended = other;
                    isInHit = _isInHit;
                }
                break;
            case TouchingStates.CUSTOM_LAYER:
                if ((other == null && !_isInHit) || (other != null && _isInHit && other.gameObject.layer == touchingState.customLayer))
                {
                    currentTransformSended = other;
                    isInHit = _isInHit;
                }
                break;
            case TouchingStates.CUSTOM_NAME:
                if ((other == null && !_isInHit) || (other != null && _isInHit && other.name == touchingState.customName))
                {
                    currentTransformSended = other;
                    isInHit = _isInHit;
                }
                break;
            default:
                break;
        }
        if (other != null)
        {
            lastAvaiableTransformEndTouch = other;
        }
    }
    #endregion
}