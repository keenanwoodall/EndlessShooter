// Copyright (c) 2016 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
// Based on Unity StandaloneInputModule.cs, version 5.3
// https://bitbucket.org/Unity-Technologies/ui/src/b5f9aae6ff7c2c63a521a1cb8b3e3da6939b191b/UnityEngine.UI/EventSystem/InputModules?at=5.3

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Rewired.Integration.UnityUI {

    [AddComponentMenu("Event/Rewired Standalone Input Module")]
    public class RewiredStandaloneInputModule : PointerInputModule {
        
        #region Rewired Variables and Properties

        private const string DEFAULT_ACTION_MOVE_HORIZONTAL = "UIHorizontal";
        private const string DEFAULT_ACTION_MOVE_VERTICAL = "UIVertical";
        private const string DEFAULT_ACTION_SUBMIT = "UISubmit";
        private const string DEFAULT_ACTION_CANCEL = "UICancel";

        private int[] playerIds;
        private bool recompiling;
        private bool isTouchSupported;

        /// <summary>
        /// Allow all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.
        /// </summary>
        [SerializeField]
        [Tooltip("Use all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.")]
        private bool useAllRewiredGamePlayers = false;

        /// <summary>
        /// Allow the Rewired System Player to control the UI.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow the Rewired System Player to control the UI.")]
        private bool useRewiredSystemPlayer = false;

        /// <summary>
        /// A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.
        /// </summary>
        [SerializeField]
        [Tooltip("A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.")]
        private int[] rewiredPlayerIds = new int[1] { 0 };
        
        /// <summary>
        /// Allow only Players with Player.isPlaying = true to control the UI.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow only Players with Player.isPlaying = true to control the UI.")]
        private bool usePlayingPlayersOnly = false;

        /// <summary>
        /// Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.
        /// </summary>
        [SerializeField]
        [Tooltip("Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.")]
        private bool moveOneElementPerAxisPress;

        /// <summary>
        /// Allow all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.
        /// </summary>
        public bool UseAllRewiredGamePlayers {
            get { return useAllRewiredGamePlayers; }
            set {
                bool changed = value != useAllRewiredGamePlayers;
                useAllRewiredGamePlayers = value;
                if(changed) SetupRewiredVars();
            }
        }

        /// <summary>
        /// Allow the Rewired System Player to control the UI.
        /// </summary>
        public bool UseRewiredSystemPlayer {
            get { return useRewiredSystemPlayer; }
            set {
                bool changed = value != useRewiredSystemPlayer;
                useRewiredSystemPlayer = value;
                if(changed) SetupRewiredVars();
            }
        }
        /// <summary>
        /// A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.
        /// Returns a clone of the array.
        /// </summary>
        public int[] RewiredPlayerIds {
            get { return (int[])rewiredPlayerIds.Clone(); }
            set {
                rewiredPlayerIds = (value != null ? (int[])value.Clone() : new int[0]);
                SetupRewiredVars();
            }
        }
        
        /// <summary>
        /// Allow only Players with Player.isPlaying = true to control the UI.
        /// </summary>
        public bool UsePlayingPlayersOnly {
            get { return usePlayingPlayersOnly; }
            set { usePlayingPlayersOnly = value; }
        }

        /// <summary>
        /// Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.
        /// </summary>
        public bool MoveOneElementPerAxisPress {
            get { return moveOneElementPerAxisPress; }
            set { moveOneElementPerAxisPress = value; }
        }

        /// <summary>
        /// Allows the mouse to be used to select elements.
        /// </summary>
        public bool allowMouseInput {
            get { return m_allowMouseInput; }
            set { m_allowMouseInput = value; }
        }

        /// <summary>
        /// Allows the mouse to be used to select elements if the device also supports touch control.
        /// </summary>
        public bool allowMouseInputIfTouchSupported {
            get { return m_allowMouseInputIfTouchSupported; }
            set { m_allowMouseInputIfTouchSupported = value; }
        }

        private bool isMouseSupported {
            get {
                if(!Input.mousePresent) return false;
                if(!m_allowMouseInput) return false;
                return isTouchSupported ? m_allowMouseInputIfTouchSupported : true;
            }
        }

        #endregion

        private float m_PrevActionTime;
        Vector2 m_LastMoveVector;
        int m_ConsecutiveMoveCount = 0;

        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;
        private bool m_HasFocus = true;

        [SerializeField]
        private string m_HorizontalAxis = DEFAULT_ACTION_MOVE_HORIZONTAL;

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the vertical axis for movement (if axis events are used).")]
        private string m_VerticalAxis = DEFAULT_ACTION_MOVE_VERTICAL;

        /// <summary>
        /// Name of the action used to submit.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the action used to submit.")]
        private string m_SubmitButton = DEFAULT_ACTION_SUBMIT;

        /// <summary>
        /// Name of the action used to cancel.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the action used to cancel.")]
        private string m_CancelButton = DEFAULT_ACTION_CANCEL;

        /// <summary>
        /// Number of selection changes allowed per second when a movement button/axis is held in a direction.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of selection changes allowed per second when a movement button/axis is held in a direction.")]
        private float m_InputActionsPerSecond = 10;

        /// <summary>
        /// Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.
        /// </summary>
        [SerializeField]
        [Tooltip("Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.")]
        private float m_RepeatDelay = 0.0f;

        /// <summary>
        /// Allows the mouse to be used to select elements.
        /// </summary>
        [SerializeField]
        [Tooltip("Allows the mouse to be used to select elements.")]
        private bool m_allowMouseInput = true;

        /// <summary>
        /// Allows the mouse to be used to select elements if the device also supports touch control.
        /// </summary>
        [SerializeField]
        [Tooltip("Allows the mouse to be used to select elements if the device also supports touch control.")]
        private bool m_allowMouseInputIfTouchSupported = true;

        /// <summary>
        /// Forces the module to always be active.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
        [Tooltip("Forces the module to always be active.")]
        private bool m_ForceModuleActive;

        /// <summary>
        /// Allows the module to control UI input on mobile devices..
        /// </summary>
        [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead")]
        public bool allowActivationOnMobileDevice {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        /// <summary>
        /// Forces the module to always be active.
        /// </summary>
        public bool forceModuleActive {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        // <summary>
        /// Number of selection changes allowed per second when a movement button/axis is held in a direction.
        /// </summary>
        public float inputActionsPerSecond {
            get { return m_InputActionsPerSecond; }
            set { m_InputActionsPerSecond = value; }
        }

        /// <summary>
        /// Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.
        /// </summary>
        public float repeatDelay {
            get { return m_RepeatDelay; }
            set { m_RepeatDelay = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public string horizontalAxis {
            get { return m_HorizontalAxis; }
            set { m_HorizontalAxis = value; }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public string verticalAxis {
            get { return m_VerticalAxis; }
            set { m_VerticalAxis = value; }
        }

        /// <summary>
        /// Name of the action used to submit.
        /// </summary>
        public string submitButton {
            get { return m_SubmitButton; }
            set { m_SubmitButton = value; }
        }

        /// <summary>
        /// Name of the action used to cancel.
        /// </summary>
        public string cancelButton {
            get { return m_CancelButton; }
            set { m_CancelButton = value; }
        }

        // Constructor

        protected RewiredStandaloneInputModule() { }

        // Methods

        protected override void Awake() {
            base.Awake();

            // Determine if touch is supported
            isTouchSupported = Input.touchSupported;

            // Deactivate the TouchInputModule because it has been deprecated in 5.3. Functionality was moved into here on all versions.
            TouchInputModule tim = GetComponent<TouchInputModule>();
            if(tim != null) {
                tim.enabled = false;
#if UNITY_EDITOR
                Debug.LogWarning("The TouchInputModule is no longer used as the functionality has been moved into the RewiredStandaloneInputModule. Please remove the TouchInputModule component.");
#endif
            }

            // Initialize Rewired
            InitializeRewired();
        }

        public override void UpdateModule() {
            CheckEditorRecompile();
            if(recompiling) return;
            if(!ReInput.isReady) return;

            if(!m_HasFocus && ShouldIgnoreEventsOnNoFocus()) return;

            if(isMouseSupported) {
                m_LastMousePosition = m_MousePosition;
                m_MousePosition = UnityEngine.Input.mousePosition;
            }
        }

        public override bool IsModuleSupported() {
            return true; // there is never any reason this module should not be supported now that TouchInputModule is deprecated, so always return true.
        }

        public override bool ShouldActivateModule() {
            if(!base.ShouldActivateModule()) return false;
            if(recompiling) return false;
            if(!ReInput.isReady) return false;

            bool shouldActivate = m_ForceModuleActive;

            // Combine input for all players
            for(int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
                if(player == null) continue;
                if(usePlayingPlayersOnly && !player.isPlaying) continue;

                shouldActivate |= player.GetButtonDown(m_SubmitButton);
                shouldActivate |= player.GetButtonDown(m_CancelButton);
                if(moveOneElementPerAxisPress) { // axis press moves only to the next UI element with each press
                    shouldActivate |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
                    shouldActivate |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
                } else { // default behavior - axis press scrolls quickly through UI elements
                    shouldActivate |= !Mathf.Approximately(player.GetAxisRaw(m_HorizontalAxis), 0.0f);
                    shouldActivate |= !Mathf.Approximately(player.GetAxisRaw(m_VerticalAxis), 0.0f);
                }
            }

            // Mouse input
            if(isMouseSupported) {
                shouldActivate |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0f;
                shouldActivate |= UnityEngine.Input.GetMouseButtonDown(0);
            }

            // Touch input
            if(isTouchSupported) {
                for(int i = 0; i < Input.touchCount; ++i) {
                    Touch input = Input.GetTouch(i);
                    shouldActivate |= input.phase == TouchPhase.Began
                        || input.phase == TouchPhase.Moved
                        || input.phase == TouchPhase.Stationary;
                }
            }
            
            return shouldActivate;
        }

        public override void ActivateModule() {
            if(!m_HasFocus && ShouldIgnoreEventsOnNoFocus()) return;

            base.ActivateModule();

            if(isMouseSupported) {
                Vector2 mousePosition = UnityEngine.Input.mousePosition;
                m_MousePosition = mousePosition;
                m_LastMousePosition = mousePosition;
            }

            var toSelect = eventSystem.currentSelectedGameObject;
            if(toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

        public override void Process() {
            if(!ReInput.isReady) return;
            if(!m_HasFocus && ShouldIgnoreEventsOnNoFocus()) return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            if(eventSystem.sendNavigationEvents) {
                if(!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if(!usedEvent)
                    SendSubmitEventToSelectedObject();
            }

            // touch needs to take precedence because of the mouse emulation layer
            if(!ProcessTouchEvents()) {
                if(isMouseSupported) ProcessMouseEvent();
            }
        }

        private bool ProcessTouchEvents() {
            if(!isTouchSupported) return false;

            for(int i = 0; i < Input.touchCount; ++i) {
                Touch input = Input.GetTouch(i);

#if UNITY_5_3_OR_NEWER
                if(input.type == TouchType.Indirect)
                    continue;
#endif

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(input, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if(!released) {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                } else
                    RemovePointerData(pointer);
            }
            return Input.touchCount > 0;
        }

        private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released) {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if(pressed) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                if(pointerEvent.pointerEnter != currentOverGo) {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if(newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if(newPressed == pointerEvent.lastPress) {
                    var diffTime = time - pointerEvent.clickTime;
                    if(diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                } else {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if(pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if(released) {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if(pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                } else if(pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if(pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                if(pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        /// <summary>
        /// Process submit keys.
        /// </summary>
        protected bool SendSubmitEventToSelectedObject() {
            if(eventSystem.currentSelectedGameObject == null)
                return false;
            if(recompiling) return false;

            var data = GetBaseEventData();
            for(int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
                if(player == null) continue;
                if(usePlayingPlayersOnly && !player.isPlaying) continue;

                if(player.GetButtonDown(m_SubmitButton)) {
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                    break;
                }

                if(player.GetButtonDown(m_CancelButton)) {
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                    break;
                }
            }
            return data.used;
        }

        private Vector2 GetRawMoveVector() {
            if(recompiling) return Vector2.zero;

            Vector2 move = Vector2.zero;
            bool horizButton = false;
            bool vertButton = false;

            // Combine inputs of all Players
            for(int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
                if(player == null) continue;
                if(usePlayingPlayersOnly && !player.isPlaying) continue;

                if(moveOneElementPerAxisPress) { // axis press moves only to the next UI element with each press
                    float x = 0.0f;
                    if(player.GetButtonDown(m_HorizontalAxis)) x = 1.0f;
                    else if(player.GetNegativeButtonDown(m_HorizontalAxis)) x = -1.0f;

                    float y = 0.0f;
                    if(player.GetButtonDown(m_VerticalAxis)) y = 1.0f;
                    else if(player.GetNegativeButtonDown(m_VerticalAxis)) y = -1.0f;
                    
                    move.x += x;
                    move.y += y;

                } else { // default behavior - axis press scrolls quickly through UI elements
                    move.x += player.GetAxisRaw(m_HorizontalAxis);
                    move.y += player.GetAxisRaw(m_VerticalAxis);
                }
                
                horizButton |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
                vertButton |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
            }

            if(horizButton) {
                if(move.x < 0)
                    move.x = -1f;
                if(move.x > 0)
                    move.x = 1f;
            }
            if(vertButton) {
                if(move.y < 0)
                    move.y = -1f;
                if(move.y > 0)
                    move.y = 1f;
            }
            return move;
        }

        /// <summary>
        /// Process keyboard events.
        /// </summary>
        protected bool SendMoveEventToSelectedObject() {
            if(recompiling) return false; // never allow movement while recompiling

            float time = Time.unscaledTime; // get the current time

            // Check for zero movement and clear
            Vector2 movement = GetRawMoveVector();
            if(Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f)) {
                m_ConsecutiveMoveCount = 0;
                return false;
            }
            
            // Check if movement is in the same direction as previously
            bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);

            // Check if a button/key/axis was just pressed this frame
            bool buttonJustPressed = CheckButtonOrKeyMovement(time);

            // If user just pressed button/key/axis, always allow movement
            bool allow = buttonJustPressed;
            if(!allow) {

                // Apply repeat delay and input actions per second limits

                if(m_RepeatDelay > 0.0f) { // apply repeat delay
                    // Otherwise, user held down key or axis.
                    // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
                    if(similarDir && m_ConsecutiveMoveCount == 1) { // this is the 2nd tick after the initial that activated the movement in this direction
                        allow = (time > m_PrevActionTime + m_RepeatDelay);
                        // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                    } else {
                        allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // apply input actions per second limit
                    }
                
                } else { // not using a repeat delay
                    allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // apply input actions per second limit
                }
            }
            if(!allow) return false; // movement not allowed, done

            // Get the axis move event
            var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
            if(axisEventData.moveDir == MoveDirection.None) return false; // input vector was not enough to move this cycle, done

            // Execute the move
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            
            // Update records and counters
            if(!similarDir) m_ConsecutiveMoveCount = 0;
            m_ConsecutiveMoveCount++;
            m_PrevActionTime = time;
            m_LastMoveVector = movement;

            return axisEventData.used;
        }

        private bool CheckButtonOrKeyMovement(float time) {
            bool allow = false;
            
            for(int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
                if(player == null) continue;
                if(usePlayingPlayersOnly && !player.isPlaying) continue;

                allow |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
                allow |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
            }

            return allow;
        }

        protected void ProcessMouseEvent() {
            ProcessMouseEvent(0);
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        protected void ProcessMouseEvent(int id) {

            // Breaking change to UnityEngine.EventSystems.PointerInputModule.GetMousePointerEventData() in Unity 5.1.2p1. This code cannot compile in these patch releases because no defines exist for patch releases
#if !UNITY_5 || (UNITY_5 && (UNITY_5_0 || UNITY_5_1_0 || UNITY_5_1_1 || UNITY_5_1_2))
            var mouseData = GetMousePointerEventData();
#else
            var mouseData = GetMousePointerEventData(id);
#endif
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            if(!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f)) {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        protected bool SendUpdateEventToSelectedObject() {
            if(eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        protected void ProcessMousePress(MouseButtonEventData data) {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if(data.PressedThisFrame()) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if(newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if(newPressed == pointerEvent.lastPress) {
                    var diffTime = time - pointerEvent.clickTime;
                    if(diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                } else {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if(pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if(data.ReleasedThisFrame()) {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if(pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                } else if(pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if(pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if(currentOverGo != pointerEvent.pointerEnter) {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

        protected virtual void OnApplicationFocus(bool hasFocus) {
            m_HasFocus = hasFocus;
        }

        private bool ShouldIgnoreEventsOnNoFocus() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WSA || UNITY_WINRT
#if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isRemoteConnected) return false;
#endif
            if(!ReInput.isReady) return true;
#else
            if(!ReInput.isReady) return false;
#endif
            return ReInput.configuration.ignoreInputWhenAppNotInFocus;
        }

        #region Rewired Methods

        private void InitializeRewired() {
            if(!Rewired.ReInput.isReady) {
                Debug.LogError("Rewired is not initialized! Are you missing a Rewired Input Manager in your scene?");
                return;
            }
            Rewired.ReInput.EditorRecompileEvent += OnEditorRecompile;
            SetupRewiredVars();
        }

        private void SetupRewiredVars() {
            // Set up Rewired vars

            // Set up Rewired Players
            if(useAllRewiredGamePlayers) {
                IList<Rewired.Player> rwPlayers = useRewiredSystemPlayer ? Rewired.ReInput.players.AllPlayers : Rewired.ReInput.players.Players;
                playerIds = new int[rwPlayers.Count];
                for(int i = 0; i < rwPlayers.Count; i++) {
                    playerIds[i] = rwPlayers[i].id;
                }
            } else {
                int rewiredPlayerCount = rewiredPlayerIds.Length + (useRewiredSystemPlayer ? 1 : 0);
                playerIds = new int[rewiredPlayerCount];
                for(int i = 0; i < rewiredPlayerIds.Length; i++) {
                    playerIds[i] = Rewired.ReInput.players.GetPlayer(rewiredPlayerIds[i]).id;
                }
                if(useRewiredSystemPlayer) playerIds[rewiredPlayerCount - 1] = Rewired.ReInput.players.GetSystemPlayer().id;
            }
        }

        private void CheckEditorRecompile() {
            if(!recompiling) return;
            if(!Rewired.ReInput.isReady) return;
            recompiling = false;
            InitializeRewired();
        }

        private void OnEditorRecompile() {
            recompiling = true;
            ClearRewiredVars();
        }

        private void ClearRewiredVars() {
            Array.Clear(playerIds, 0, playerIds.Length);
        }

        #endregion
    }
}