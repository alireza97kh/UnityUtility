using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dobeil
{
    public enum TouchType
    {
        TOUCH = 0,
        DOUBLETOUCH = 1,
        DRAG = 2
    }
    public enum TouchMode
    {
        FINGER = 0,
        MOUSE = 1,
        BOTH = 2
    }
    public enum TargetMode
    {
        ME = 0,
        OTHER = 1,
    }
    public enum EndDTouchState
    {
        NONE = 0,
        BACK_TO_START = 1,
        BACK_TO_LAST_HIT = 2,
        STAY_IN_POSITION = 3,
        MOVE_TO_CUSTOM_POSITION = 4,
        MOVE_TO_CUSTOM_TRANSFORM = 5,
        DESTROY = 6,
        DEACTIVE = 7,
        UN_ENABLE = 8
    }
    public enum HitState
    {
        NONE = 0,
        COLLISION = 1,
        TRIGGER = 2,
    }
    public enum TouchingStates
    {
        NONE = 0,
        CUSTOM_TAG = 1,
        CUSTOM_LAYER = 2,
        CUSTOM_NAME = 3
    }

    public enum TouchingCodndions
    {
        NONE = 0,
        MAX_COUNT = 1,
        MAX_HIT = 2
    }

    [Serializable]
    public class StartTouchStateClass
    {
        [EnumToggleButtons]
        public TouchingStates state = TouchingStates.NONE;

        [Space(2)]
        [ShowIf("state", TouchingStates.CUSTOM_TAG)]
        public string customTag = "";

        [ShowIf("state", TouchingStates.CUSTOM_LAYER)]
        public int customLayer = 0;

        [ShowIf("state", TouchingStates.CUSTOM_NAME)]
        public string customName = "";
        public List<UnityEvent> events;
        //public virtual void 
    }
    [Serializable]
    public class TouchingStateClass
    {


        [EnumToggleButtons]
        public TouchingStates state = TouchingStates.NONE;
        [Space(10)]
        public EndDTouchState touchState = EndDTouchState.NONE;
        [Space(2)]
        [ShowIf("state", TouchingStates.CUSTOM_TAG)]
        public string customTag = "";
        [ShowIf("state", TouchingStates.CUSTOM_LAYER)]
        public int customLayer = 0;
        [ShowIf("state", TouchingStates.CUSTOM_NAME)]
        public string customName = "";
        [ShowIf("CheckCustomPos")]
        public Vector3 customPosition = Vector3.zero;
        [ShowIf("CheckCustomTransform")]
        [Required("Transform cannot be null", InfoMessageType.Error)]
        public Transform customTransform = null;
        public List<UnityEvent> events;
        bool CheckCustomPos()
        {
            return touchState == EndDTouchState.MOVE_TO_CUSTOM_POSITION;
        }
        bool CheckCustomTransform()
        {
            return touchState == EndDTouchState.MOVE_TO_CUSTOM_TRANSFORM;
        }
        //public virtual void 
    }
    [Serializable]
    public class EndTouchStateClass
    {

        [EnumToggleButtons]
        //[LabelText("state", true)]
        public TouchingStates state = TouchingStates.NONE;
        [Space(10)]
        public EndDTouchState touchStateWithHit = EndDTouchState.NONE;
        [ShowIf("state", TouchingStates.NONE)]
        [Space(2)]
        public EndDTouchState touchStateWithOutHit = EndDTouchState.NONE;
        [Space(2)]
        [ShowIf("state", TouchingStates.CUSTOM_TAG)]
        public string customTag = "";
        [ShowIf("state", TouchingStates.CUSTOM_LAYER)]
        public int customLayer = 0;
        [ShowIf("state", TouchingStates.CUSTOM_NAME)]
        public string customName = "";
        [ShowIf("CheckCustomPos")]
        public Vector3 customPosition = Vector3.zero;
        [ShowIf("CheckCustomTransform")]
        [Required("Transform cannot be null", InfoMessageType.Error)]
        public Transform customTransform = null;
        public List<UnityEvent> events;
        bool CheckCustomPos()
        {
            return touchStateWithHit == EndDTouchState.MOVE_TO_CUSTOM_POSITION || (touchStateWithOutHit == EndDTouchState.MOVE_TO_CUSTOM_POSITION && state == TouchingStates.NONE);
        }
        bool CheckCustomTransform()
        {
            return touchStateWithHit == EndDTouchState.MOVE_TO_CUSTOM_TRANSFORM || (touchStateWithOutHit == EndDTouchState.MOVE_TO_CUSTOM_TRANSFORM && state == TouchingStates.NONE);
        }
    }
}