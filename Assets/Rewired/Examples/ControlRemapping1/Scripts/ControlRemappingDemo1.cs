// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649 // disable warnings about unused variables

namespace Rewired.Demos {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using Rewired;

    [AddComponentMenu("")]
    public class ControlRemappingDemo1 : MonoBehaviour {

        private const string playerPrefsBaseKey = "UserRemappingDemo";
        private const float defaultModalWidth = 250.0f;
        private const float defaultModalHeight = 200.0f;
        private const float assignmentTimeout = 5.0f;

        // Helper objects
        private DialogHelper dialog;

        // GUI state management
        private bool guiState;
        private bool busy;
        private bool pageGUIState;

        // Selections
        private Player selectedPlayer;
        private int selectedMapCategoryId;
        private ControllerSelection selectedController;
        ControllerMap selectedMap;

        // Other flags
        private bool showMenu;

        // Scroll view positions
        private Vector2 actionScrollPos;
        private Vector2 calibrateScrollPos;

        // Queues
        private Queue<QueueEntry> actionQueue;

        // Setup vars
        private bool setupFinished;

        // Editor state management
        [System.NonSerialized]
        private bool initialized;
        private bool isCompiling;

        // Styles
        GUIStyle style_wordWrap;
        GUIStyle style_centeredBox;

        #region Initialization

        private void Awake() {
            Initialize();
        }

        private void Initialize() {
            dialog = new DialogHelper();
            actionQueue = new Queue<QueueEntry>();
            selectedController = new ControllerSelection();
            ReInput.ControllerConnectedEvent += JoystickConnected;
            ReInput.ControllerPreDisconnectEvent += JoystickPreDisconnect; // runs before joystick is completely disconnected so we can save maps
            ReInput.ControllerDisconnectedEvent += JoystickDisconnected; // final disconnect that runs after joystick has been fully removed
            Reset();
            initialized = true;
            LoadAllMaps(); // load saved user maps on start if there are any to load

            if(ReInput.unityJoystickIdentificationRequired) {
                IdentifyAllJoysticks();
            }
        }

        private void Setup() {
            if(setupFinished) return;

            // Create styles
            style_wordWrap = new GUIStyle(GUI.skin.label);
            style_wordWrap.wordWrap = true;
            style_centeredBox = new GUIStyle(GUI.skin.box);
            style_centeredBox.alignment = TextAnchor.MiddleCenter;

            setupFinished = true;
        }

        #endregion

        #region Main Update

        public void OnGUI() {
            #if UNITY_EDITOR
            // Check for script recompile in the editor
            CheckRecompile();
            #endif

            if(!initialized) return;

            Setup();

            HandleMenuControl();

            if(!showMenu) {
                DrawInitialScreen();
                return;
            }

            SetGUIStateStart();

            // Process queue
            ProcessQueue();

            // Draw contents
            DrawPage();
            ShowDialog();

            SetGUIStateEnd();

            // Clear momentary vars
            busy = false;
        }

        #endregion

        #region Menu Control

        private void HandleMenuControl() {
            if(dialog.enabled) return; // don't allow closing the menu while dialog is open so there won't be issues remapping the Menu button

            if(ReInput.players.GetSystemPlayer().GetButtonDown("Menu")) {
                if(showMenu) { // menu is open and will be closed
                    SaveAllMaps(); // save all maps when menu is closed
                    Close();
                } else {
                    Open();
                }
            }
        }

        private void Close() {

            ClearWorkingVars();
            showMenu = false;
        }

        private void Open() {
            showMenu = true;
        }

        #endregion

        #region Draw

        private void DrawInitialScreen() {
            GUIContent content;
            ActionElementMap map = ReInput.players.GetSystemPlayer().controllers.maps.GetFirstElementMapWithAction("Menu", true);
            
            if(map != null) {
                content = new GUIContent("Press " + map.elementIdentifierName + " to open the menu.");
            } else {
                content = new GUIContent("There is no element assigned to open the menu!");
            }

            // Draw the box
            GUILayout.BeginArea(GetScreenCenteredRect(300, 50));
            GUILayout.Box(content, style_centeredBox, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            GUILayout.EndArea();
        }

        private void DrawPage() {
            if(GUI.enabled != pageGUIState) GUI.enabled = pageGUIState;

            Rect screenRect = new Rect((Screen.width - (Screen.width * 0.9f)) * 0.5f, (Screen.height - (Screen.height * 0.9f)) * 0.5f, Screen.width * 0.9f, Screen.height * 0.9f);
            GUILayout.BeginArea(screenRect);

            // Player Selector
            DrawPlayerSelector();

            // Joystick Selector
            DrawJoystickSelector();

            // Mouse
            DrawMouseAssignment();

            // Controllers
            DrawControllerSelector();

            // Controller Calibration
            DrawCalibrateButton();

            // Categories
            DrawMapCategories();

            // Create scroll view
            actionScrollPos = GUILayout.BeginScrollView(actionScrollPos);

            // Actions
            DrawCategoryActions();

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private void DrawPlayerSelector() {
            if(ReInput.players.allPlayerCount == 0) {
                GUILayout.Label("There are no players.");
                return;
            }

            GUILayout.Space(15);
            GUILayout.Label("Players:");
            GUILayout.BeginHorizontal();

            foreach(Player player in ReInput.players.GetPlayers(true)) {
                if(selectedPlayer == null) selectedPlayer = player;  // if no player selected, select first

                bool prevValue = player == selectedPlayer ? true : false;
                bool value = GUILayout.Toggle(prevValue, player.descriptiveName != string.Empty ? player.descriptiveName : player.name, "Button", GUILayout.ExpandWidth(false));
                if(value != prevValue) { // value changed
                    if(value) { // selected
                        selectedPlayer = player;
                        selectedController.Clear(); // reset the device selection
                        selectedMapCategoryId = -1; // clear category selection
                    } // do not allow deselection
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawMouseAssignment() {
            bool origGuiEnabled = GUI.enabled; // save GUI state
            if(selectedPlayer == null) GUI.enabled = false;

            GUILayout.Space(15);
            GUILayout.Label("Assign Mouse:");
            GUILayout.BeginHorizontal();

            bool prevValue = selectedPlayer != null && selectedPlayer.controllers.hasMouse ? true : false;
            bool value = GUILayout.Toggle(prevValue, "Assign Mouse", "Button", GUILayout.ExpandWidth(false));
            if(value != prevValue) { // user clicked
                if(value) {
                    selectedPlayer.controllers.hasMouse = true;
                    foreach(Player player in ReInput.players.Players) { // de-assign mouse from all players except System
                        if(player == selectedPlayer) continue; // skip self
                        player.controllers.hasMouse = false;
                    }
                } else {
                    selectedPlayer.controllers.hasMouse = false;
                }
            }

            GUILayout.EndHorizontal();
            if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled; // restore GUI state
        }

        private void DrawJoystickSelector() {
            bool origGuiEnabled = GUI.enabled; // save GUI state
            if(selectedPlayer == null) GUI.enabled = false;

            GUILayout.Space(15);
            GUILayout.Label("Assign Joysticks:");
            GUILayout.BeginHorizontal();

            bool prevValue = selectedPlayer == null || selectedPlayer.controllers.joystickCount == 0 ? true : false;
            bool value = GUILayout.Toggle(prevValue, "None", "Button", GUILayout.ExpandWidth(false));
            if(value != prevValue) { // user clicked
                selectedPlayer.controllers.ClearControllersOfType(ControllerType.Joystick);
                ControllerSelectionChanged();
                // do not allow deselection
            }

            if(selectedPlayer != null) {
                foreach(Joystick joystick in ReInput.controllers.Joysticks) {
                    prevValue = selectedPlayer.controllers.ContainsController(joystick);
                    value = GUILayout.Toggle(prevValue, joystick.name, "Button", GUILayout.ExpandWidth(false));
                    if(value != prevValue) { // user clicked
                        EnqueueAction(new JoystickAssignmentChange(selectedPlayer.id, joystick.id, value));
                    }
                }
            }

            GUILayout.EndHorizontal();
            if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled; // restore GUI state
        }

        private void DrawControllerSelector() {
            if(selectedPlayer == null) return;
            bool origGuiEnabled = GUI.enabled; // save GUI state

            GUILayout.Space(15);
            GUILayout.Label("Controller to Map:");
            GUILayout.BeginHorizontal();

            bool value, prevValue;

            if(!selectedController.hasSelection) {
                selectedController.Set(0, ControllerType.Keyboard); // select keyboard if nothing selected
                ControllerSelectionChanged();
            }

            // Keyboard
            prevValue = selectedController.type == ControllerType.Keyboard;
            value = GUILayout.Toggle(prevValue, "Keyboard", "Button", GUILayout.ExpandWidth(false));
            if(value != prevValue) {
                selectedController.Set(0, ControllerType.Keyboard); // set current selected device to this
                ControllerSelectionChanged();
            }

            // Mouse
            if(!selectedPlayer.controllers.hasMouse) GUI.enabled = false; // disable mouse if player doesn't have mouse assigned
            prevValue = selectedController.type == ControllerType.Mouse;
            value = GUILayout.Toggle(prevValue, "Mouse", "Button", GUILayout.ExpandWidth(false));
            if(value != prevValue) {
                selectedController.Set(0, ControllerType.Mouse); // set current selected device to this
                ControllerSelectionChanged();
            }
            if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled; // re-enable gui

            // Joystick List
            foreach(Joystick j in selectedPlayer.controllers.Joysticks) {
                prevValue = selectedController.type == ControllerType.Joystick && selectedController.id == j.id;
                value = GUILayout.Toggle(prevValue, j.name, "Button", GUILayout.ExpandWidth(false));
                if(value != prevValue) {
                    selectedController.Set(j.id, ControllerType.Joystick); // set current selected device to this
                    ControllerSelectionChanged();
                }
            }

            GUILayout.EndHorizontal();
            if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled; // restore GUI state
        }

        private void DrawCalibrateButton() {
            if(selectedPlayer == null) return;
            bool origGuiEnabled = GUI.enabled; // save GUI state

            GUILayout.Space(10);

            Controller controller = selectedController.hasSelection ? selectedPlayer.controllers.GetController(selectedController.type, selectedController.id) : null;

            if(controller == null || selectedController.type != ControllerType.Joystick) {
                GUI.enabled = false;
                GUILayout.Button("Select a controller to calibrate", GUILayout.ExpandWidth(false));
                if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled;
            } else { // Calibrate joystick
                if(GUILayout.Button("Calibrate " + controller.name, GUILayout.ExpandWidth(false))) {
                    Joystick joystick = controller as Joystick;
                    if(joystick != null) {
                        CalibrationMap calibrationMap = joystick.calibrationMap;
                        if(calibrationMap != null) {
                            EnqueueAction(new Calibration(selectedPlayer, joystick, calibrationMap));
                        }
                    }
                }
            }

            if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled; // restore GUI state
        }

        private void DrawMapCategories() {
            if(selectedPlayer == null) return;
            if(!selectedController.hasSelection) return;
            bool origGuiEnabled = GUI.enabled; // save GUI state

            GUILayout.Space(15);
            GUILayout.Label("Categories:");
            GUILayout.BeginHorizontal();

            foreach(InputMapCategory category in ReInput.mapping.UserAssignableMapCategories) {
                if(!selectedPlayer.controllers.maps.ContainsMapInCategory(selectedController.type, category.id)) { // if player has no maps in this category for controller don't allow them to select it
                    GUI.enabled = false;
                } else {
                    // Select first available category if none selected
                    if(selectedMapCategoryId < 0) {
                        selectedMapCategoryId = category.id; // if no category selected, select first
                        selectedMap = selectedPlayer.controllers.maps.GetFirstMapInCategory(selectedController.type, selectedController.id, category.id);
                    }
                }

                bool prevValue = category.id == selectedMapCategoryId ? true : false;
                bool value = GUILayout.Toggle(prevValue, category.descriptiveName != string.Empty ? category.descriptiveName : category.name, "Button", GUILayout.ExpandWidth(false));
                if(value != prevValue) { // category changed
                    selectedMapCategoryId = category.id;
                    selectedMap = selectedPlayer.controllers.maps.GetFirstMapInCategory(selectedController.type, selectedController.id, category.id);
                }
                if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled;
            }

            GUILayout.EndHorizontal();
            if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled; // restore GUI state
        }

        private void DrawCategoryActions() {
            if(selectedPlayer == null) return;
            if(selectedMapCategoryId < 0) return;
            bool origGuiEnabled = GUI.enabled; // save GUI state
            if(selectedMap == null) return; // controller map does not exist for this category in this controller

            GUILayout.Space(15);
            GUILayout.Label("Actions:");

            InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(selectedMapCategoryId); // get the selected map category
            if(mapCategory == null) return;
            InputCategory actionCategory = ReInput.mapping.GetActionCategory(mapCategory.name); // get the action category with the same name
            if(actionCategory == null) return; // no action category exists with the same name

            float labelWidth = 150.0f;

            // Draw the list of actions for the selected action category
            foreach(InputAction action in ReInput.mapping.ActionsInCategory(actionCategory.id)) {
                string name = action.descriptiveName != string.Empty ? action.descriptiveName : action.name;

                if(action.type == InputActionType.Button) {

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(name, GUILayout.Width(labelWidth));
                    DrawAddActionMapButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, true); // Add assignment button

                    // Write out assigned elements
                    foreach(ActionElementMap elementMap in selectedMap.AllMaps) {
                        if(elementMap.actionId != action.id) continue;
                        DrawActionAssignmentButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, true, elementMap);
                    }
                    GUILayout.EndHorizontal();

                } else if(action.type == InputActionType.Axis) { // Axis

                    // Draw main axis label and actions assigned to the full axis
                    if(selectedController.type != ControllerType.Keyboard) { // don't draw this for keyboards since keys can only be assigned to the +/- axes anyway

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(name, GUILayout.Width(labelWidth));
                        DrawAddActionMapButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, true); // Add assignment button

                        // Write out assigned elements
                        foreach(ActionElementMap elementMap in selectedMap.AllMaps) {
                            if(elementMap.actionId != action.id) continue;
                            if(elementMap.elementType == ControllerElementType.Button) continue; // skip buttons, will handle below
                            if(elementMap.axisType == AxisType.Split) continue; // skip split axes, will handle below
                            DrawActionAssignmentButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, true, elementMap);
                            DrawInvertButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, elementMap);
                        }
                        GUILayout.EndHorizontal();

                    }

                    // Positive action
                    string positiveName = action.positiveDescriptiveName != string.Empty ? action.positiveDescriptiveName : action.descriptiveName + " +";
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(positiveName, GUILayout.Width(labelWidth));
                    DrawAddActionMapButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, false); // Add assignment button

                    // Write out assigned elements
                    foreach(ActionElementMap elementMap in selectedMap.AllMaps) {
                        if(elementMap.actionId != action.id) continue;
                        if(elementMap.axisContribution != Pole.Positive) continue; // axis contribution is incorrect, skip
                        if(elementMap.axisType == AxisType.Normal) continue; // normal axes handled above
                        DrawActionAssignmentButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, false, elementMap);
                    }
                    GUILayout.EndHorizontal();

                    // Negative action
                    string negativeName = action.negativeDescriptiveName != string.Empty ? action.negativeDescriptiveName : action.descriptiveName + " -";
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(negativeName, GUILayout.Width(labelWidth));
                    DrawAddActionMapButton(selectedPlayer.id, action, Pole.Negative, selectedController, selectedMap, false); // Add assignment button

                    // Write out assigned elements
                    foreach(ActionElementMap elementMap in selectedMap.AllMaps) {
                        if(elementMap.actionId != action.id) continue;
                        if(elementMap.axisContribution != Pole.Negative) continue; // axis contribution is incorrect, skip
                        if(elementMap.axisType == AxisType.Normal) continue; // normal axes handled above
                        DrawActionAssignmentButton(selectedPlayer.id, action, Pole.Negative, selectedController, selectedMap, false, elementMap);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled; // restore GUI state
        }

        #endregion

        #region Buttons

        private void DrawActionAssignmentButton(int playerId, InputAction action, Pole actionAxisContribution, ControllerSelection controller, ControllerMap controllerMap,
            bool assignFullAxis, ActionElementMap elementMap) {

            if(GUILayout.Button(elementMap.elementIdentifierName, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30.0f))) {
                EnqueueAction(new ElementAssignmentChange(playerId, controller.id, controller.type, controllerMap,
                    ElementAssignmentChangeType.ReassignOrRemove, elementMap.id, action.id, actionAxisContribution, action.type, assignFullAxis, elementMap.invert));
            }
            GUILayout.Space(4);
        }

        private void DrawInvertButton(int playerId, InputAction action, Pole actionAxisContribution, ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap) {
            bool value = elementMap.invert;
            bool newValue = GUILayout.Toggle(value, "Invert", GUILayout.ExpandWidth(false));
            if(newValue != value) {
                elementMap.invert = newValue;
            }
            GUILayout.Space(10);
        }

        private void DrawAddActionMapButton(int playerId, InputAction action, Pole actionAxisContribution, ControllerSelection controller, ControllerMap controllerMap,
            bool assignFullAxis) {
            if(GUILayout.Button("Add...", GUILayout.ExpandWidth(false))) {
                EnqueueAction(new ElementAssignmentChange(playerId, controller.id, controller.type, controllerMap,
                    ElementAssignmentChangeType.Add, -1, action.id, actionAxisContribution, action.type, assignFullAxis, false));
            }
            GUILayout.Space(10);
        }

        #endregion

        #region Dialog Boxes

        private void ShowDialog() {
            dialog.Update();
        }

        #region Draw Window Functions

        private void DrawModalWindow(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            // Buttons
            dialog.DrawConfirmButton("Okay");

            GUILayout.FlexibleSpace();
            dialog.DrawCancelButton();
            
            GUILayout.EndHorizontal();
        }

        private void DrawModalWindow_OkayOnly(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            // Buttons
            dialog.DrawConfirmButton("Okay");

            GUILayout.EndHorizontal();
        }

        private void DrawElementAssignmentWindow(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);

            GUILayout.FlexibleSpace();

            ElementAssignmentChange entry = actionQueue.Peek() as ElementAssignmentChange; // get item from queue
            if(entry == null) {
                dialog.Cancel();
                return;
            }

            // Poll the controller for input assignment
            PollControllerForAssignment(entry);

            // Show the cancel timer
            GUILayout.Label("Assignment will be canceled in " + ((int)Mathf.Ceil(dialog.closeTimer)).ToString() + "...", style_wordWrap);
        }

        private void DrawElementAssignmentProtectedConflictWindow(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);

            GUILayout.FlexibleSpace();

            ElementAssignmentChange entry = actionQueue.Peek() as ElementAssignmentChange; // get item from queue
            if(entry == null) {
                dialog.Cancel();
                return;
            }

            // Draw Buttons
            GUILayout.BeginHorizontal();

            dialog.DrawConfirmButton(UserResponse.Custom1, "Add");
            GUILayout.FlexibleSpace();
            dialog.DrawCancelButton();

            GUILayout.EndHorizontal();
        }

        private void DrawElementAssignmentNormalConflictWindow(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);

            GUILayout.FlexibleSpace();

            ElementAssignmentChange entry = actionQueue.Peek() as ElementAssignmentChange; // get item from queue
            if(entry == null) {
                dialog.Cancel();
                return;
            }

            // Draw Buttons
            GUILayout.BeginHorizontal();
            
            dialog.DrawConfirmButton(UserResponse.Confirm, "Replace");
            GUILayout.FlexibleSpace();
            dialog.DrawConfirmButton(UserResponse.Custom1, "Add");
            GUILayout.FlexibleSpace();
            dialog.DrawCancelButton();

            GUILayout.EndHorizontal();
        }

        private void DrawReassignOrRemoveElementAssignmentWindow(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            // Buttons
            dialog.DrawConfirmButton("Reassign");

            GUILayout.FlexibleSpace();

            dialog.DrawCancelButton("Remove");

            GUILayout.EndHorizontal();
        }

        private void DrawFallbackJoystickIdentificationWindow(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            FallbackJoystickIdentification entry = actionQueue.Peek() as FallbackJoystickIdentification;
            if(entry == null) {
                dialog.Cancel();
                return;
            }

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);
            
            GUILayout.Label("Press any button or axis on \"" + entry.joystickName + "\" now.", style_wordWrap);

            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Skip")) {
                dialog.Cancel();
                return;
            }

            // Do not allow assignment until dialog is ready
            if(dialog.busy) return;

            // Remap the joystick input source
            bool success = ReInput.controllers.SetUnityJoystickIdFromAnyButtonOrAxisPress(entry.joystickId, 0.8f, false);
            if(!success) return;

            // Finish
            dialog.Confirm();
        }

        private void DrawCalibrationWindow(string title, string message) {
            if(!dialog.enabled) return; // prevent this from running after dialog is closed

            Calibration entry = actionQueue.Peek() as Calibration;
            if(entry == null) {
                dialog.Cancel();
                return;
            }

            GUILayout.Space(5);

            // Message
            GUILayout.Label(message, style_wordWrap);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            bool origGUIEnabled = GUI.enabled;

            // Controller element selection
            GUILayout.BeginVertical(GUILayout.Width(200));

			// Create a scroll view for the axis list in case using the default controller map which has a lot of axes
			calibrateScrollPos = GUILayout.BeginScrollView(calibrateScrollPos);

            if(entry.recording) GUI.enabled = false; // don't allow switching while recording min/max
            IList<ControllerElementIdentifier> axisIdentifiers = entry.joystick.AxisElementIdentifiers;
            for(int i = 0; i < axisIdentifiers.Count; i++) {
                ControllerElementIdentifier identifier = axisIdentifiers[i];
                bool isSelected = entry.selectedElementIdentifierId == identifier.id;
                bool newValue = GUILayout.Toggle(isSelected, identifier.name, "Button", GUILayout.ExpandWidth(false));
                if(isSelected != newValue) {
                    entry.selectedElementIdentifierId = identifier.id; // store the selection index
                }
            }
            if(GUI.enabled != origGUIEnabled) GUI.enabled = origGUIEnabled; // restore gui

			GUILayout.EndScrollView();

            GUILayout.EndVertical();

            // Selected object information and controls
            GUILayout.BeginVertical(GUILayout.Width(200));

            if(entry.selectedElementIdentifierId >= 0) {

                float axisValue = entry.joystick.GetAxisRawById(entry.selectedElementIdentifierId);

                GUILayout.Label("Raw Value: " + axisValue.ToString());
                
				// Get the axis index from the element identifier id
                int axisIndex = entry.joystick.GetAxisIndexById(entry.selectedElementIdentifierId);
                AxisCalibration axis = entry.calibrationMap.GetAxis(axisIndex); // get the axis calibration

                // Show current axis information
				GUILayout.Label("Calibrated Value: " + entry.joystick.GetAxisById(entry.selectedElementIdentifierId));
                GUILayout.Label("Zero: " + axis.calibratedZero);
                GUILayout.Label("Min: " + axis.calibratedMin);
                GUILayout.Label("Max: " + axis.calibratedMax);
                GUILayout.Label("Dead Zone: " + axis.deadZone);

                GUILayout.Space(15);

                // Enabled -- allows user to disable an axis entirely if its giving them problems
                bool newEnabled = GUILayout.Toggle(axis.enabled, "Enabled", "Button", GUILayout.ExpandWidth(false));
                if(axis.enabled != newEnabled) {
                    axis.enabled = newEnabled;
                }

                GUILayout.Space(10);

                // Records Min/Max
                bool newRecording = GUILayout.Toggle(entry.recording, "Record Min/Max", "Button", GUILayout.ExpandWidth(false));
                if(newRecording != entry.recording) {
                    if(newRecording) { // just started recording
                        // Clear previous calibration so we can record min max from this session only
                        axis.calibratedMax = 0.0f;
                        axis.calibratedMin = 0.0f;
                    }
                    entry.recording = newRecording;
                }

                if(entry.recording) {
                    axis.calibratedMin = Mathf.Min(axis.calibratedMin, axisValue, axis.calibratedMin);
                    axis.calibratedMax = Mathf.Max(axis.calibratedMax, axisValue, axis.calibratedMax);
                    GUI.enabled = false;
                }

                // Set Zero state
                if(GUILayout.Button("Set Zero", GUILayout.ExpandWidth(false))) {
                    axis.calibratedZero = axisValue;
                }

                // Set Dead Zone
                if(GUILayout.Button("Set Dead Zone", GUILayout.ExpandWidth(false))) {
                    axis.deadZone = axisValue;
                }

                // Invert
                bool newInvert = GUILayout.Toggle(axis.invert, "Invert", "Button", GUILayout.ExpandWidth(false));
                if(axis.invert != newInvert) {
                    axis.invert = newInvert;
                }

                GUILayout.Space(10);
                if(GUILayout.Button("Reset", GUILayout.ExpandWidth(false))) {
                    axis.Reset();
                }

                if(GUI.enabled != origGUIEnabled) GUI.enabled = origGUIEnabled;

            } else {
                GUILayout.Label("Select an axis to begin.");
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            // Close Button
            GUILayout.FlexibleSpace();
            if(entry.recording) GUI.enabled = false;
            if(GUILayout.Button("Close")) {
                calibrateScrollPos = new Vector2(); // clear the scroll view position
                dialog.Confirm();
            }
            if(GUI.enabled != origGUIEnabled) GUI.enabled = origGUIEnabled;
        }

        #endregion

        #region Result Callbacks

        private void DialogResultCallback(int queueActionId, UserResponse response) {
            foreach(QueueEntry entry in actionQueue) { // find the right one and cancel or confirm it
                if(entry.id != queueActionId) continue;
                if(response != UserResponse.Cancel) entry.Confirm(response); // mark the entry as confirmed and record user response
                else entry.Cancel(); // mark the entry as canceled
                break;
            }
        }

        #endregion

        #region Polling

        private void PollControllerForAssignment(ElementAssignmentChange entry) {
            if(dialog.busy) return; // do not allow assignment until dialog is ready

            switch (entry.controllerType) {
                case ControllerType.Keyboard:
                    PollKeyboardForAssignment(entry); // poll keyboard for key presses
                    break;
                case ControllerType.Joystick:
                    PollJoystickForAssignment(entry); // poll joystick for element activations
                    break;
                case ControllerType.Mouse:
                    PollMouseForAssignment(entry); // poll mouse for element activations
                    break;
            }
        }

        private void PollKeyboardForAssignment(ElementAssignmentChange entry) {
            int modifierPressedCount = 0; // the number of modifier keys being pressed this cycle
            ControllerPollingInfo nonModifierKeyInfo = new ControllerPollingInfo();
            ControllerPollingInfo firstModifierKeyInfo = new ControllerPollingInfo();
            ModifierKeyFlags curModifiers = ModifierKeyFlags.None;

            // Check all keys being pressed at present so we can handle modifier keys
            foreach(ControllerPollingInfo info in ReInput.controllers.Keyboard.PollForAllKeys()) {
                KeyCode key = info.keyboardKey;
                if(key == KeyCode.AltGr) continue; // skip AltGr key because it gets fired when alt and control are held on some keyboards

                // determine if a modifier key is being pressed
                if(Keyboard.IsModifierKey(info.keyboardKey)) { // a modifier key is pressed
                    if(modifierPressedCount == 0) firstModifierKeyInfo = info; // store the polling info for the first modifier key pressed in case its the only key pressed

                    curModifiers |= Keyboard.KeyCodeToModifierKeyFlags(key); // add the key to the current modifier flags
                    modifierPressedCount += 1; // count how many modifier keys are pressed

                } else { // this is not a modifier key

                    if(nonModifierKeyInfo.keyboardKey != KeyCode.None) continue; // skip after the first one detected, we only need one non-modifier key press
                    nonModifierKeyInfo = info; // store the polling info
                }

            }

            // Commit immediately if a non-modifier key was pressed
            if(nonModifierKeyInfo.keyboardKey != KeyCode.None) { // a regular key was pressed

                if(modifierPressedCount == 0) { // only the regular key was pressed

                    entry.pollingInfo = nonModifierKeyInfo; // copy polling info into entry
                    dialog.Confirm(); // finish
                    return;

                } else { // one more more modifier keys was pressed too

                    entry.pollingInfo = nonModifierKeyInfo; // copy polling info into entry
                    entry.modifierKeyFlags = curModifiers; // set the modifier key flags in the entry
                    dialog.Confirm(); // finish
                    return;

                }

            } else if(modifierPressedCount > 0) { // one or more modifier keys were pressed, but no regular keys
                dialog.StartCloseTimer(assignmentTimeout); // reset close timer if a modifier key is pressed

                if(modifierPressedCount == 1) { // only one modifier is pressed, allow assigning just the modifier key

                    // Assign the modifier key as the main key if the user holds it for 1 second
                    if(ReInput.controllers.Keyboard.GetKeyTimePressed(firstModifierKeyInfo.keyboardKey) > 1.0f) { // key was pressed for one second
                        entry.pollingInfo = firstModifierKeyInfo; // copy polling info into entry
                        dialog.Confirm(); // finish
                        return;
                    }

                    // Show the key that is being pressed
                    GUILayout.Label(Keyboard.GetKeyName(firstModifierKeyInfo.keyboardKey));

                } else { // more than one modifier key is pressed
                    // do nothing because we don't want to assign modified modifier key presses such as Control + Alt, but you could if you wanted to.

                    // Show the modifier keys being held
                    GUILayout.Label(Keyboard.ModifierKeyFlagsToString(curModifiers));

                }

            }
        }

        private void PollJoystickForAssignment(ElementAssignmentChange entry) {
            // Poll the controller via the owning player
            Player player = ReInput.players.GetPlayer(entry.playerId);
            if(player == null) {
                dialog.Cancel();
                return;
            }

            // Just poll the controller for the first axis or button press
            entry.pollingInfo = player.controllers.polling.PollControllerForFirstElementDown(entry.controllerType, entry.controllerId);
            if(entry.pollingInfo.success) { // polling returned a result
                dialog.Confirm(); // finish
            }
        }

        private void PollMouseForAssignment(ElementAssignmentChange entry) {
            // Poll the controller via the owning player
            Player player = ReInput.players.GetPlayer(entry.playerId);
            if(player == null) {
                dialog.Cancel();
                return;
            }

            // Just poll the controller for the first axis or button press
            entry.pollingInfo = player.controllers.polling.PollControllerForFirstElementDown(entry.controllerType, entry.controllerId); // PollMouseForFirstElement_ExcludeXYAxis(entry.controllerId); // poll the mouse for all input except the primary X/Y axes -- we'll handle these separately
            if(entry.pollingInfo.success) { // polling returned a result
                dialog.Confirm(); // finish
            }
        }

        #endregion

        #region Misc

        private Rect GetScreenCenteredRect(float width, float height) {
            return new Rect(
                (float)(Screen.width * 0.5f - width * 0.5f),
                (float)(Screen.height * 0.5 - height * 0.5f),
                width,
                height
            );
        }

        #endregion

        #endregion

        #region Action Queue

        private void EnqueueAction(QueueEntry entry) {
            if(entry == null) return;
            busy = true;
            GUI.enabled = false; // disable control on everything until next cycle
            actionQueue.Enqueue(entry);
        }

        private void ProcessQueue() {
            if(dialog.enabled) return; // dialog is open, do not process queue
            if(busy || actionQueue.Count == 0) return;

            while(actionQueue.Count > 0) {
                QueueEntry queueEntry = actionQueue.Peek(); // get next item from queue

                bool goNext = false;

                // Process different types of actions
                switch(queueEntry.queueActionType) {
                    case QueueActionType.JoystickAssignment:
                        goNext = ProcessJoystickAssignmentChange((JoystickAssignmentChange)queueEntry);
                        break;
                    case QueueActionType.ElementAssignment:
                        goNext = ProcessElementAssignmentChange((ElementAssignmentChange)queueEntry);
                        break;
                    case QueueActionType.FallbackJoystickIdentification:
                        goNext = ProcessFallbackJoystickIdentification((FallbackJoystickIdentification)queueEntry);
                        break;
                    case QueueActionType.Calibrate:
                        goNext = ProcessCalibration((Calibration)queueEntry);
                        break;
                }

                // Quit processing the queue if we opened a modal
                if(!goNext) break;

                // Remove item from queue since we're done with it
                actionQueue.Dequeue();
            }
        }

        private bool ProcessJoystickAssignmentChange(JoystickAssignmentChange entry) {

            // Handle user cancelation
            if(entry.state == QueueEntry.State.Canceled) { // action was canceled
                return true;
            }

            Player player = ReInput.players.GetPlayer(entry.playerId);
            if(player == null) return true;

            if(!entry.assign) { // deassign joystick
                player.controllers.RemoveController(ControllerType.Joystick, entry.joystickId);
                ControllerSelectionChanged();
                return true;
            }

            // Assign joystick
            if(player.controllers.ContainsController(ControllerType.Joystick, entry.joystickId)) return true; // same player, nothing to change
            bool alreadyAssigned = ReInput.controllers.IsJoystickAssigned(entry.joystickId);

            if(!alreadyAssigned || entry.state == QueueEntry.State.Confirmed) { // not assigned or user confirmed the action already, do it
                player.controllers.AddController(ControllerType.Joystick, entry.joystickId, true);
                ControllerSelectionChanged();
                return true;
            }

            // Create dialog and start waiting for user confirmation
            dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties {
                title = "Joystick Reassignment",
                message = "This joystick is already assigned to another player. Do you want to reassign this joystick to " + player.descriptiveName + "?",
                rect = GetScreenCenteredRect(defaultModalWidth, defaultModalHeight),
                windowDrawDelegate = DrawModalWindow
            },
            DialogResultCallback);
            return false; // don't process anything more in this queue
        }

        private bool ProcessElementAssignmentChange(ElementAssignmentChange entry) {

            switch(entry.changeType) {
                case ElementAssignmentChangeType.ReassignOrRemove:
                    return ProcessRemoveOrReassignElementAssignment(entry);
                case ElementAssignmentChangeType.Remove:
                    return ProcessRemoveElementAssignment(entry);
                case ElementAssignmentChangeType.Add:
                case ElementAssignmentChangeType.Replace:
                    return ProcessAddOrReplaceElementAssignment(entry);
                case ElementAssignmentChangeType.ConflictCheck:
                    return ProcessElementAssignmentConflictCheck(entry);
                default:
                    throw new System.NotImplementedException();
            }
        }

        private bool ProcessRemoveOrReassignElementAssignment(ElementAssignmentChange entry) {
            if(entry.controllerMap == null) return true;
            
            if(entry.state == QueueEntry.State.Canceled) { // delete entry
                // Enqueue a new action to delete the entry
                ElementAssignmentChange newEntry = new ElementAssignmentChange(entry); // copy the entry
                newEntry.changeType = ElementAssignmentChangeType.Remove; // change the type to Remove
                actionQueue.Enqueue(newEntry); // enqueue the new entry
                return true;
            }

            // Check for user confirmation
            if(entry.state == QueueEntry.State.Confirmed) { // reassign entry
                // Enqueue a new action to reassign the entry
                ElementAssignmentChange newEntry = new ElementAssignmentChange(entry); // copy the entry
                newEntry.changeType = ElementAssignmentChangeType.Replace; // change the type to Replace
                actionQueue.Enqueue(newEntry); // enqueue the new entry
                return true;
            }

            // Create dialog and start waiting for user assignment
            dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties {
                title = "Reassign or Remove",
                message = "Do you want to reassign or remove this assignment?",
                rect = GetScreenCenteredRect(defaultModalWidth, defaultModalHeight),
                windowDrawDelegate = DrawReassignOrRemoveElementAssignmentWindow
            },
            DialogResultCallback);

            return false;
        }

        private bool ProcessRemoveElementAssignment(ElementAssignmentChange entry) {
            if(entry.controllerMap == null) return true;
            if(entry.state == QueueEntry.State.Canceled) return true; // user canceled

            // Delete element
            if(entry.state == QueueEntry.State.Confirmed) { // user confirmed, delete it
                entry.controllerMap.DeleteElementMap(entry.actionElementMapId);
                return true;
            }
            
            // Create dialog and start waiting for user confirmation
            dialog.StartModal(entry.id, DialogHelper.DialogType.DeleteAssignmentConfirmation, new WindowProperties {
                title = "Remove Assignment",
                message = "Are you sure you want to remove this assignment?",
                rect = GetScreenCenteredRect(defaultModalWidth, defaultModalHeight),
                windowDrawDelegate = DrawModalWindow
            },
            DialogResultCallback);
            return false; // don't process anything more in this queue
        }

        private bool ProcessAddOrReplaceElementAssignment(ElementAssignmentChange entry) {
            Player player = ReInput.players.GetPlayer(entry.playerId);
            if(player == null) return true;
            if(entry.controllerMap == null) return true;
            if(entry.state == QueueEntry.State.Canceled) return true; // user canceled

            // Check for user confirmation
            if(entry.state == QueueEntry.State.Confirmed) { // the action assignment has been confirmed
                if(Event.current.type != EventType.Layout) return false; // only make changes in layout to avoid GUI errors when new controls appear

                // Do a check for element assignment conflicts
                if(!ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(entry.ToElementAssignmentConflictCheck())) {  // no conflicts
                    entry.ReplaceOrCreateActionElementMap(); // make the assignment, done

                } else { // we had conflicts
                    
                    // Enqueue a conflict check
                    ElementAssignmentChange newEntry = new ElementAssignmentChange(entry); // clone the entry
                    newEntry.changeType = ElementAssignmentChangeType.ConflictCheck; // set the new type to check for conflicts
                    actionQueue.Enqueue(newEntry); // enqueue the new entry
                }
                
                return true; // finished
            }

            // Customize the message for different controller types and different platforms
            string message;
            if(entry.controllerType == ControllerType.Keyboard) {

                if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer) {
                    message = "Press any key to assign it to this action. You may also use the modifier keys Command, Control, Alt, and Shift. If you wish to assign a modifier key ifselt this action, press and hold the key for 1 second.";
                } else {
                    message = "Press any key to assign it to this action. You may also use the modifier keys Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second.";
                }

                // Editor modifier key disclaimer
                if(Application.isEditor) {
                    message += "\n\nNOTE: Some modifier key combinations will not work in the Unity Editor, but they will work in a game build.";
                }

            } else if(entry.controllerType == ControllerType.Mouse) {
                message = "Press any mouse button or axis to assign it to this action.\n\nTo assign mouse movement axes, move the mouse quickly in the direction you want mapped to the action. Slow movements will be ignored.";
            } else {
                message = "Press any button or axis to assign it to this action.";
            }

            // Create dialog and start waiting for user assignment
            dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties {
                title = "Assign",
                message = message,
                rect = GetScreenCenteredRect(defaultModalWidth, defaultModalHeight),
                windowDrawDelegate = DrawElementAssignmentWindow
            },
            DialogResultCallback,
            assignmentTimeout);

            return false;
        }

        private bool ProcessElementAssignmentConflictCheck(ElementAssignmentChange entry) {
            Player player = ReInput.players.GetPlayer(entry.playerId);
            if(player == null) return true;
            if(entry.controllerMap == null) return true;
            if(entry.state == QueueEntry.State.Canceled) return true; // user canceled

            // Check for user confirmation
            if(entry.state == QueueEntry.State.Confirmed) {
                
                entry.changeType = ElementAssignmentChangeType.Add; // set the change type back to add before we add the assignment
                
                if(entry.response == UserResponse.Confirm) { // remove and add

                    ReInput.controllers.conflictChecking.RemoveElementAssignmentConflicts(entry.ToElementAssignmentConflictCheck()); // remove conflicts
                    entry.ReplaceOrCreateActionElementMap(); // create or replace the element

                } else if(entry.response == UserResponse.Custom1) { // add without removing

                    entry.ReplaceOrCreateActionElementMap(); // add the element

                } else throw new System.NotImplementedException();


                return true; // finished
            }

            // Do a detailed conflict check
            bool protectedConflictFound = false;
            foreach(ElementAssignmentConflictInfo info in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(entry.ToElementAssignmentConflictCheck())) {
                // Check if this conflict is with a protected assignment
                if(!info.isUserAssignable) {
                    protectedConflictFound = true;
                    break;
                }
            }

            // Open a different dialog depending on if a protected conflict was found
            if(protectedConflictFound) {
                string message = entry.elementName + " is already in use and is protected from reassignment. You cannot remove the protected assignment, but you can still assign the action to this element. If you do so, the element will trigger multiple actions when activated.";

                // Create dialog and start waiting for user assignment
                dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties {
                    title = "Assignment Conflict",
                    message = message,
                    rect = GetScreenCenteredRect(defaultModalWidth, defaultModalHeight),
                    windowDrawDelegate = DrawElementAssignmentProtectedConflictWindow
                },
                DialogResultCallback);

            } else {
                string message = entry.elementName + " is already in use. You may replace the other conflicting assignments, add this assignment anyway which will leave multiple actions assigned to this element, or cancel this assignment.";

                // Create dialog and start waiting for user assignment
                dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties {
                    title = "Assignment Conflict",
                    message = message,
                    rect = GetScreenCenteredRect(defaultModalWidth, defaultModalHeight),
                    windowDrawDelegate = DrawElementAssignmentNormalConflictWindow
                },
                DialogResultCallback);

            }

            return false;
        }

        private bool ProcessFallbackJoystickIdentification(FallbackJoystickIdentification entry) {
            // Handle user cancelation
            if(entry.state == QueueEntry.State.Canceled) { // action was canceled
                return true;
            }

            // Identify joystick
            if(entry.state == QueueEntry.State.Confirmed) {
                // nothing to do, done
                return true;
            }

            // Create dialog and start waiting for user confirmation
            dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties {
                title = "Joystick Identification Required",
                message = "A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:",
                rect = GetScreenCenteredRect(defaultModalWidth, defaultModalHeight),
                windowDrawDelegate = DrawFallbackJoystickIdentificationWindow
            },
            DialogResultCallback,
            0.0f,
            1.0f); // add a longer delay after the dialog opens to prevent one joystick press from being used for subsequent joysticks if held for a short time
            return false; // don't process anything more in this queue
        }

        private bool ProcessCalibration(Calibration entry) {
            // Handle user cancelation
            if(entry.state == QueueEntry.State.Canceled) { // action was canceled
                return true;
            }

            if(entry.state == QueueEntry.State.Confirmed) {
                return true;
            }

            // Create dialog and start waiting for user confirmation
            dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties {
                title = "Calibrate Controller",
                message = "Select an axis to calibrate on the " + entry.joystick.name + ".",
                rect = GetScreenCenteredRect(450, 480),
                windowDrawDelegate = DrawCalibrationWindow
            },
            DialogResultCallback);
            return false; // don't process anything more in this queue
        }

        #endregion

        #region Selection Chaging

        private void PlayerSelectionChanged() {
            ClearControllerSelection();
        }

        private void ControllerSelectionChanged() {
            ClearMapSelection();
        }

        private void ClearControllerSelection() {
            selectedController.Clear(); // reset the device selection because joystick list will have changed
            ClearMapSelection();
        }

        #endregion

        #region Clear

        private void ClearMapSelection() {
            selectedMapCategoryId = -1; // clear map cat selection
            selectedMap = null;
        }

        private void Reset() {
            ClearWorkingVars();
            initialized = false;
            showMenu = false;
        }

        private void ClearWorkingVars() {
            selectedPlayer = null;
            ClearMapSelection();
            selectedController.Clear();
            actionScrollPos = new Vector2();
            dialog.FullReset();
            actionQueue.Clear();
            busy = false;
        }

        #endregion

        #region GUI State

        private void SetGUIStateStart() {
            guiState = true;
            if(busy) guiState = false;
            pageGUIState = guiState && !busy && !dialog.enabled && !dialog.busy; // enable page gui only if not busy and not in dialog mode
            if(GUI.enabled != guiState) GUI.enabled = guiState;
        }

        private void SetGUIStateEnd() {
            // always enable GUI again before exiting
            guiState = true;
            if(!GUI.enabled) GUI.enabled = guiState;
        }

        #endregion

        #region Joystick Connection Callbacks

        private void JoystickConnected(ControllerStatusChangedEventArgs args) {
            // Reload maps if a joystick is connected
            LoadJoystickMaps(args.controllerId);

            // Always force reidentification of all joysticks when a joystick is added or removed when using Unity input on a platform that requires manual identification
            if(ReInput.unityJoystickIdentificationRequired) IdentifyAllJoysticks();
        }

        private void JoystickPreDisconnect(ControllerStatusChangedEventArgs args) {
            // Check if the current editing controller was just disconnected and deselect it
            if(selectedController.hasSelection && args.controllerType == selectedController.type && args.controllerId == selectedController.id) {
                ClearControllerSelection(); // joystick was disconnected
            }

            // Save the user maps before the joystick is disconnected if in the menu since user may have changed something
            if(showMenu) SaveJoystickMaps(args.controllerId);
        }

        private void JoystickDisconnected(ControllerStatusChangedEventArgs args) {
            // Close dialogs and clear queue if a joystick is disconnected
            if(showMenu) ClearWorkingVars();

            // Always force reidentification of all joysticks when a joystick is added or removed when using Unity input
            if(ReInput.unityJoystickIdentificationRequired) IdentifyAllJoysticks();
        }

        #endregion

        #region Load/Save

        private void LoadAllMaps() {
            // This example uses PlayerPrefs because its convenient, though not efficient, but you could use any data storage method you like.

            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) {
                Player player = allPlayers[i];

                // Load Input Behaviors - all players have an instance of each input behavior so it can be modified
                IList<InputBehavior> behaviors = ReInput.mapping.GetInputBehaviors(player.id); // get all behaviors from player
                for(int j = 0; j < behaviors.Count; j++) {
                    string xml = GetInputBehaviorXml(player, behaviors[j].id); // try to the behavior for this id
                    if(xml == null || xml == string.Empty) continue; // no data found for this behavior
                    behaviors[j].ImportXmlString(xml); // import the data into the behavior
                }

                // Load the maps first and make sure we have them to load before clearing

                // Load Keyboard Maps
				List<string> keyboardMaps = GetAllControllerMapsXml(player, true, ControllerType.Keyboard, ReInput.controllers.Keyboard);

                // Load Mouse Maps
                List<string> mouseMaps = GetAllControllerMapsXml(player, true, ControllerType.Mouse, ReInput.controllers.Mouse); // load mouse controller maps
                
                // Load Joystick Maps
                bool foundJoystickMaps = false;
                List<List<string>> joystickMaps = new List<List<string>>();
                foreach(Joystick joystick in player.controllers.Joysticks) {
                    List<string> maps = GetAllControllerMapsXml(player, true, ControllerType.Joystick, joystick);
                    joystickMaps.Add(maps);
                    if(maps.Count > 0) foundJoystickMaps = true;
                }
                
                // Now add the maps to the controller

                // Keyboard maps
                if(keyboardMaps.Count > 0) player.controllers.maps.ClearMaps(ControllerType.Keyboard, true); // clear only user-assignable maps, but only if we found something to load. Don't _really_ have to clear the maps as adding ones in the same cat/layout will just replace, but let's clear anyway.
                player.controllers.maps.AddMapsFromXml(ControllerType.Keyboard, 0, keyboardMaps); // add the maps to the player

                // Joystick maps
                if(foundJoystickMaps) player.controllers.maps.ClearMaps(ControllerType.Joystick, true); // clear only user-assignable maps, but only if we found something to load. Don't _really_ have to clear the maps as adding ones in the same cat/layout will just replace, but let's clear anyway.
                int count = 0;
                foreach(Joystick joystick in player.controllers.Joysticks) {
                    player.controllers.maps.AddMapsFromXml(ControllerType.Joystick, joystick.id, joystickMaps[count]); // add joystick controller maps to player
                    count++;
                }

                // Mouse Maps
                if(mouseMaps.Count > 0) player.controllers.maps.ClearMaps(ControllerType.Mouse, true); // clear only user-assignable maps, but only if we found something to load. Don't _really_ have to clear the maps as adding ones in the same cat/layout will just replace, but let's clear anyway.
                player.controllers.maps.AddMapsFromXml(ControllerType.Mouse, 0, mouseMaps); // add the maps to the player
            }

            // Load joystick calibration maps
            foreach(Joystick joystick in ReInput.controllers.Joysticks) {
                joystick.ImportCalibrationMapFromXmlString(GetJoystickCalibrationMapXml(joystick)); // load joystick calibration map if any
            }
        }

        private void SaveAllMaps() {
            // This example uses PlayerPrefs because its convenient, though not efficient, but you could use any data storage method you like.

            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) {
                Player player = allPlayers[i];

                // Get all savable data from player
                PlayerSaveData playerData = player.GetSaveData(true);

                // Save Input Behaviors
                foreach(InputBehavior behavior in playerData.inputBehaviors) {
                    string key = GetInputBehaviorPlayerPrefsKey(player, behavior);
                    PlayerPrefs.SetString(key, behavior.ToXmlString()); // save the behavior to player prefs in XML format
                }

                // Save controller maps
                foreach(ControllerMapSaveData saveData in playerData.AllControllerMapSaveData) {
                    string key = GetControllerMapPlayerPrefsKey(player, saveData);
                    PlayerPrefs.SetString(key, saveData.map.ToXmlString()); // save the map to player prefs in XML format
                }
            }

            // Save joystick calibration maps
            foreach(Joystick joystick in ReInput.controllers.Joysticks) {
                JoystickCalibrationMapSaveData saveData = joystick.GetCalibrationMapSaveData();
                string key = GetJoystickCalibrationMapPlayerPrefsKey(saveData);
                PlayerPrefs.SetString(key, saveData.map.ToXmlString()); // save the map to player prefs in XML format
            }

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }

        private void LoadJoystickMaps(int joystickId) {
            // This example uses PlayerPrefs because its convenient, though not efficient, but you could use any data storage method you like.

            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) { // this controller may be owned by more than one player, so check all
                Player player = allPlayers[i];
                if(!player.controllers.ContainsController(ControllerType.Joystick, joystickId)) continue; // player does not have the joystick

                Joystick joystick = player.controllers.GetController<Joystick>(joystickId);
                if(joystick == null) continue;

                // Load the joystick maps first and make sure we have them to load before clearing
                List<string> xmlMaps = GetAllControllerMapsXml(player, true, ControllerType.Joystick, joystick);
                if(xmlMaps.Count == 0) continue;

                // Clear joystick maps first
                player.controllers.maps.ClearMaps(ControllerType.Joystick, true); // clear only user-assignable maps (technically you don't have to clear -- adding a map in same catId/layoutId will overwrite)

                // Load Joystick Maps
                player.controllers.maps.AddMapsFromXml(ControllerType.Joystick, joystickId, xmlMaps); // load joystick controller maps
                
                // Load joystick calibration map
                joystick.ImportCalibrationMapFromXmlString(GetJoystickCalibrationMapXml(joystick)); // load joystick calibration map
            }
        }

        private void SaveJoystickMaps(int joystickId) {
            // This example uses PlayerPrefs because its convenient, though not efficient, but you could use any data storage method you like.

            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) { // this controller may be owned by more than one player, so check all
                string key;
                Player player = allPlayers[i];
                if(!player.controllers.ContainsController(ControllerType.Joystick, joystickId)) continue; // player does not have the joystick

                // Save controller maps
                JoystickMapSaveData[] saveData = player.controllers.maps.GetMapSaveData<JoystickMapSaveData>(joystickId, true);
                if(saveData != null) {
                    for(int j = 0; j < saveData.Length; j++) {
                        key = GetControllerMapPlayerPrefsKey(player, saveData[j]);
                        PlayerPrefs.SetString(key, saveData[j].map.ToXmlString()); // save the map to player prefs in XML format
                    }
                }

                // Save joystick calibration map
                Joystick joystick = player.controllers.GetController<Joystick>(joystickId);
                JoystickCalibrationMapSaveData calibrationSaveData = joystick.GetCalibrationMapSaveData();
                key = GetJoystickCalibrationMapPlayerPrefsKey(calibrationSaveData);
                PlayerPrefs.SetString(key, calibrationSaveData.map.ToXmlString()); // save the map to player prefs in XML format
            }

            // Save calibration maps
            IList<Joystick> joysticks = ReInput.controllers.Joysticks;
            for(int i = 0; i < joysticks.Count; i++) {
                JoystickCalibrationMapSaveData saveData = joysticks[i].GetCalibrationMapSaveData();
                string key = GetJoystickCalibrationMapPlayerPrefsKey(saveData);
                PlayerPrefs.SetString(key, saveData.map.ToXmlString()); // save the map to player prefs in XML format
            }
        }

        #region PlayerPrefs Methods

        /* NOTE ON PLAYER PREFS:
         * PlayerPrefs on Windows Standalone is saved in the registry. There is a bug in Regedit that makes any entry with a name equal to or greater than 255 characters
         * (243 + 12 unity appends) invisible in Regedit. Unity will still load the data fine, but if you are debugging and wondering why your data is not showing up in
         * Regedit, this is why. If you need to delete the values, either call PlayerPrefs.Clear or delete the key folder in Regedit -- Warning: both methods will
         * delete all player prefs including any ones you've created yourself or other plugins have created.
         */
         
         // WARNING: Do not use & symbol in keys. Linux cannot load them after the current session ends.
       
        private string GetBasePlayerPrefsKey(Player player) {
            string key = playerPrefsBaseKey;
            key += "|playerName=" + player.name; // make a key for this specific player, could use id, descriptive name, or a custom profile identifier of your choice
            return key;
        }

        private string GetControllerMapPlayerPrefsKey(Player player, ControllerMapSaveData saveData) {
            // Create a player prefs key like a web querystring so we can search for player prefs key when loading maps
            string key = GetBasePlayerPrefsKey(player);
            key += "|dataType=ControllerMap";
            key += "|controllerMapType=" + saveData.mapTypeString;
            key += "|categoryId=" + saveData.map.categoryId + "|" + "layoutId=" + saveData.map.layoutId;
			key += "|hardwareIdentifier=" + saveData.controllerHardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
            if(saveData.mapType == typeof(JoystickMap)) { // store special info for joystick maps
                key += "|hardwareGuid=" + ((JoystickMapSaveData)saveData).joystickHardwareTypeGuid.ToString(); // the identifying GUID that determines which known joystick this is
            }
            return key;
        }

        private string GetControllerMapXml(Player player, ControllerType controllerType, int categoryId, int layoutId, Controller controller) {
            string key = GetBasePlayerPrefsKey(player);
            key += "|dataType=ControllerMap";
            key += "|controllerMapType=" + controller.mapTypeString;
            key += "|categoryId=" + categoryId + "|" + "layoutId=" + layoutId;
			key += "|hardwareIdentifier=" + controller.hardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
            if(controllerType == ControllerType.Joystick) {
                Joystick joystick = (Joystick)controller;
                key += "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString(); // the identifying GUID that determines which known joystick this is
            }

            if(!PlayerPrefs.HasKey(key)) return string.Empty; // key does not exist
            return PlayerPrefs.GetString(key); // return the data
        }

        private List<string> GetAllControllerMapsXml(Player player, bool userAssignableMapsOnly, ControllerType controllerType, Controller controller) {
            // Because player prefs does not allow us to search for partial keys, we have to check all possible category ids and layout ids to find the maps to load

            List<string> mapsXml = new List<string>();

            IList<InputMapCategory> categories = ReInput.mapping.MapCategories;
            for(int i = 0; i < categories.Count; i++) {
                InputMapCategory cat = categories[i];
                if(userAssignableMapsOnly && !cat.userAssignable) continue; // skip map because not user-assignable

                IList<InputLayout> layouts = ReInput.mapping.MapLayouts(controllerType);
                for(int j = 0; j < layouts.Count; j++) {
                    InputLayout layout = layouts[j];
					string xml = GetControllerMapXml(player, controllerType, cat.id, layout.id, controller);
                    if(xml == string.Empty) continue;
                    mapsXml.Add(xml);
                }
            }

            return mapsXml;
        }

        private string GetJoystickCalibrationMapPlayerPrefsKey(JoystickCalibrationMapSaveData saveData) {
            // Create a player prefs key like a web querystring so we can search for player prefs key when loading maps
            string key = playerPrefsBaseKey;
            key += "|dataType=CalibrationMap";
            key += "|controllerType=" + saveData.controllerType.ToString();
            key += "|hardwareIdentifier=" + saveData.hardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
            key += "|hardwareGuid=" + saveData.joystickHardwareTypeGuid.ToString();
            return key;
        }

        private string GetJoystickCalibrationMapXml(Joystick joystick) {
            string key = playerPrefsBaseKey;
            key += "|dataType=CalibrationMap";
            key += "|controllerType=" + joystick.type.ToString();
			key += "|hardwareIdentifier=" + joystick.hardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
            key += "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();

            if(!PlayerPrefs.HasKey(key)) return string.Empty; // key does not exist
            return PlayerPrefs.GetString(key); // return the data
        }

        private string GetInputBehaviorPlayerPrefsKey(Player player, InputBehavior saveData) {
            // Create a player prefs key like a web querystring so we can search for player prefs key when loading maps
            string key = GetBasePlayerPrefsKey(player);
            key += "|dataType=InputBehavior";
            key += "|id=" + saveData.id;
            return key;
        }

        private string GetInputBehaviorXml(Player player, int id) {
            string key = GetBasePlayerPrefsKey(player);
            key += "|dataType=InputBehavior";
            key += "|id=" + id;

            if(!PlayerPrefs.HasKey(key)) return string.Empty; // key does not exist
            return PlayerPrefs.GetString(key); // return the data
        }

        #endregion

        #endregion

        #region Fallback Methods

        public void IdentifyAllJoysticks() {
            // Check if there are any joysticks
            if(ReInput.controllers.joystickCount == 0) return; // no joysticks, nothing to do

            // Clear all vars first which will clear dialogs and queues
            ClearWorkingVars();

            // Open the menu if its closed
            Open();

            // Enqueue the joysticks up for identification
            foreach(Joystick joystick in ReInput.controllers.Joysticks) {
                actionQueue.Enqueue(new FallbackJoystickIdentification(joystick.id, joystick.name)); // enqueue each joystick for identification
            }
        }

        #endregion

        #region Editor Methods


        protected void CheckRecompile() {
#if UNITY_EDITOR
            // Destroy system if recompiling
            if(UnityEditor.EditorApplication.isCompiling) { // editor is recompiling
                if(!isCompiling) { // this is the first cycle of recompile
                    Reset();
                    isCompiling = true;
                }
                GUILayout.Window(0, GetScreenCenteredRect(300, 100), RecompileWindow, new GUIContent("Scripts are Compiling"));
                return;
            }

            // Check for end of compile
            if(isCompiling) { // compiling is done
                isCompiling = false;
                Initialize();
            }
#endif
        }

        private void RecompileWindow(int windowId) {
#if UNITY_EDITOR
            GUILayout.FlexibleSpace();
            GUILayout.Label("Please wait while script compilation finishes...");
            GUILayout.FlexibleSpace();
#endif
        }

        #endregion

        #region Classes

        private class ControllerSelection {
            private int _id;
            private int _idPrev;
            private ControllerType _type;
            private ControllerType _typePrev;

            public ControllerSelection() {
                Clear();
            }

            public int id {
                get {
                    return _id;
                }
                set {
                    _idPrev = _id;
                    _id = value;
                }
            }

            public ControllerType type {
                get {
                    return _type;
                }
                set {
                    _typePrev = _type;
                    _type = value;
                }
            }

            public int idPrev { get { return _idPrev; } }
            public ControllerType typePrev { get { return _typePrev; } }
            public bool hasSelection { get { return _id >= 0; } }

            public void Set(int id, ControllerType type) {
                this.id = id;
                this.type = type;
            }

            public void Clear() {
                _id = -1;
                _idPrev = -1;
                _type = ControllerType.Joystick;
                _typePrev = ControllerType.Joystick;
            }
        }

        private class DialogHelper {
            private const float openBusyDelay = 0.25f; // a small delay after opening the window that prevents assignment input for a short time after window opens
            private const float closeBusyDelay = 0.1f; // a small after closing the window that the GUI will still be busy to prevent button clickthrough

            private DialogType _type;
            private bool _enabled;
            private float _closeTime;
            private bool _closeTimerRunning;
            private float _busyTime;
            private bool _busyTimerRunning;
            private float busyTimer { get { if(!_busyTimerRunning) return 0.0f; return _busyTime - Time.realtimeSinceStartup; } }

            public bool enabled {
                get {
                    return _enabled;
                }
                set {
                    if(value) {
                        if(_type == DialogType.None) return; // cannot enable, no type set
                        StateChanged(openBusyDelay);
                    } else {
                        _enabled = value;
                        _type = DialogType.None;
                        StateChanged(closeBusyDelay);
                    }
                    
                }
            }
            public DialogType type {
                get {
                    if(!_enabled) return DialogType.None;
                    return _type;
                }
                set {
                    if(value == DialogType.None) {
                        _enabled = false;
                        StateChanged(closeBusyDelay);
                    } else {
                        _enabled = true;
                        StateChanged(openBusyDelay);
                    }
                    _type = value;
                }
            }
            public float closeTimer { get { if(!_closeTimerRunning) return 0.0f; return _closeTime - Time.realtimeSinceStartup; } }
            public bool busy { get { return _busyTimerRunning; } }

            private Action<int> drawWindowDelegate;
            private GUI.WindowFunction drawWindowFunction;
            private WindowProperties windowProperties;

            private int currentActionId;
            private Action<int, UserResponse> resultCallback;

            public DialogHelper() {
                drawWindowDelegate = DrawWindow;
                drawWindowFunction = new GUI.WindowFunction(drawWindowDelegate);
            }

            public void StartModal(int queueActionId, DialogType type, WindowProperties windowProperties, Action<int, UserResponse> resultCallback) {
                StartModal(queueActionId, type, windowProperties, resultCallback, 0.0f, -1.0f);
            }
            public void StartModal(int queueActionId, DialogType type, WindowProperties windowProperties, Action<int, UserResponse> resultCallback, float closeTimer) {
                StartModal(queueActionId, type, windowProperties, resultCallback, closeTimer, -1.0f);
            }
            public void StartModal(int queueActionId, DialogType type, WindowProperties windowProperties, Action<int, UserResponse> resultCallback, float closeTimer, float openBusyDelay) {
                currentActionId = queueActionId;
                this.windowProperties = windowProperties;
                this.type = type;
                this.resultCallback = resultCallback;
                if(closeTimer > 0.0f) StartCloseTimer(closeTimer);
                if(openBusyDelay >= 0.0f) StateChanged(openBusyDelay); // override with user defined open busy delay
            }

            public void Update() {
                Draw();
                UpdateTimers();
            }

            public void Draw() {
                if(!_enabled) return;
                bool origGuiEnabled = GUI.enabled;
                GUI.enabled = true;
                GUILayout.Window(windowProperties.windowId, windowProperties.rect, drawWindowFunction, windowProperties.title);
                GUI.FocusWindow(windowProperties.windowId);
                if(GUI.enabled != origGuiEnabled) GUI.enabled = origGuiEnabled;
            }

            public void DrawConfirmButton() {
                DrawConfirmButton("Confirm");
            }
            public void DrawConfirmButton(string title) {
                bool origGUIEnabled = GUI.enabled; // store original gui state
                if(busy) GUI.enabled = false; // disable GUI if dialog is busy to prevent click though
                if(GUILayout.Button(title)) {
                    Confirm(UserResponse.Confirm);
                }
                if(GUI.enabled != origGUIEnabled) GUI.enabled = origGUIEnabled; // restore GUI
            }

            public void DrawConfirmButton(UserResponse response) {
                DrawConfirmButton(response, "Confirm");
            }
            public void DrawConfirmButton(UserResponse response, string title) {
                bool origGUIEnabled = GUI.enabled; // store original gui state
                if(busy) GUI.enabled = false; // disable GUI if dialog is busy to prevent click though
                if(GUILayout.Button(title)) {
                    Confirm(response);
                }
                if(GUI.enabled != origGUIEnabled) GUI.enabled = origGUIEnabled; // restore GUI
            }

            public void DrawCancelButton() {
                DrawCancelButton("Cancel");
            }
            public void DrawCancelButton(string title) {
                bool origGUIEnabled = GUI.enabled; // store original gui state
                if(busy) GUI.enabled = false; // disable GUI if dialog is busy to prevent click though
                if(GUILayout.Button(title)) {
                    Cancel();
                }
                if(GUI.enabled != origGUIEnabled) GUI.enabled = origGUIEnabled; // restore GUI
            }

            public void Confirm() {
                Confirm(UserResponse.Confirm);
            }
            public void Confirm(UserResponse response) {
                resultCallback(currentActionId, response);
                Close();
            }

            public void Cancel() {
                resultCallback(currentActionId, UserResponse.Cancel);
                Close();
            }

            private void DrawWindow(int windowId) {
                windowProperties.windowDrawDelegate(windowProperties.title, windowProperties.message);
            }

            private void UpdateTimers() {
                if(_closeTimerRunning) {
                    if(closeTimer <= 0.0f) Cancel();
                }
                if(_busyTimerRunning) {
                    if(busyTimer <= 0.0f) _busyTimerRunning = false;
                }
            }

            public void StartCloseTimer(float time) {
                _closeTime = time + Time.realtimeSinceStartup;
                _closeTimerRunning = true;
            }

            private void StartBusyTimer(float time) {
                _busyTime = time + Time.realtimeSinceStartup;
                _busyTimerRunning = true;
            }

            private void Close() {
                Reset();
                StateChanged(closeBusyDelay);
            }

            private void StateChanged(float delay) {
                StartBusyTimer(delay);
            }

            private void Reset() {
                _enabled = false;
                _type = DialogType.None;
                currentActionId = -1;
                resultCallback = null;
                _closeTimerRunning = false;
                _closeTime = 0.0f;
            }

            private void ResetTimers() {
                _busyTimerRunning = false;
                _closeTimerRunning = false;
            }

            public void FullReset() {
                Reset();
                ResetTimers();
            }

            // Enums

            public enum DialogType {
                None = 0,
                JoystickConflict = 1,
                ElementConflict = 2,
                KeyConflict = 3,
                DeleteAssignmentConfirmation = 10,
                AssignElement = 11
            }
        }

        private abstract class QueueEntry {
            public int id { get; protected set; }
            public QueueActionType queueActionType { get; protected set; }
            public State state { get; protected set; }
            public UserResponse response { get; protected set; }

            private static int uidCounter;
            protected static int nextId {
                get {
                    int id = uidCounter;
                    uidCounter += 1;
                    return id;
                }
            }

            public QueueEntry(QueueActionType queueActionType) {
                id = nextId;
                this.queueActionType = queueActionType;
            }

            public void Confirm(UserResponse response) {
                state = State.Confirmed;
                this.response = response;
            }

            public void Cancel() {
                state = State.Canceled;
            }

            public enum State {
                Waiting = 0,
                Confirmed = 1,
                Canceled = 2
            }
        }

        private class JoystickAssignmentChange : QueueEntry {
            public int playerId { get; private set; }
            public int joystickId { get; private set; }
            public bool assign { get; private set; }

            public JoystickAssignmentChange(
                int newPlayerId,
                int joystickId,
                bool assign
            ) : base(QueueActionType.JoystickAssignment) {
                this.playerId = newPlayerId;
                this.joystickId = joystickId;
                this.assign = assign;
            }
        }

        private class ElementAssignmentChange : QueueEntry {
            public int playerId { get; private set; }
            public int controllerId { get; private set; }
            public ControllerType controllerType { get; private set; }
            public ControllerMap controllerMap { get; private set; }
            public int actionElementMapId { get; private set; }
            public int actionId { get; private set; }
            public Pole actionAxisContribution { get; private set; }
            public InputActionType actionType { get; private set; }
            public bool assignFullAxis { get; private set; }
            public bool invert { get; private set; }

            // Assignable outside constructor
            public ElementAssignmentChangeType changeType { get; set; }
            public ControllerPollingInfo pollingInfo { get; set; }
            public ModifierKeyFlags modifierKeyFlags { get; set; }

            public AxisRange AssignedAxisRange {
                get {
                    if(!pollingInfo.success) return AxisRange.Positive;

                    // Get info from the polling data
                    ControllerElementType elementType = pollingInfo.elementType;
                    Pole axisPole = pollingInfo.axisPole;

                    // Determine which part of the axis is being assigned -- full or just one pole
                    AxisRange axisRange = AxisRange.Positive;
                    if(elementType == ControllerElementType.Axis) { // axis is being assigned
                        if(actionType == InputActionType.Axis) { // assigned to an axis
                            if(assignFullAxis) axisRange = AxisRange.Full;
                            else axisRange = axisPole == Pole.Positive ? AxisRange.Positive : AxisRange.Negative;
                        } else { // assigned to a button
                            axisRange = axisPole == Pole.Positive ? AxisRange.Positive : AxisRange.Negative;
                        }
                    }
                    return axisRange;
                }
            }
            public string elementName {
                get {
                    if(controllerType == ControllerType.Keyboard) {
                        if(modifierKeyFlags != ModifierKeyFlags.None) {
                            return string.Format("{0} + {1}", Keyboard.ModifierKeyFlagsToString(modifierKeyFlags), pollingInfo.elementIdentifierName);
                        }
                    }
                    return pollingInfo.elementIdentifierName;
                }
            }

            public ElementAssignmentChange(
                int playerId,
                int controllerId,
                ControllerType controllerType,
                ControllerMap controllerMap,
                ElementAssignmentChangeType changeType,
                int actionElementMapId,
                int actionId,
                Pole actionAxisContribution,
                InputActionType actionType,
                bool assignFullAxis,
                bool invert
            ) : base(QueueActionType.ElementAssignment) {
                this.playerId = playerId;
                this.controllerId = controllerId;
                this.controllerType = controllerType;
                this.controllerMap = controllerMap;
                this.changeType = changeType;
                this.actionElementMapId = actionElementMapId;
                this.actionId = actionId;
                this.actionAxisContribution = actionAxisContribution;
                this.actionType = actionType;
                this.assignFullAxis = assignFullAxis;
                this.invert = invert;
            }

            public ElementAssignmentChange(ElementAssignmentChange source) : base(QueueActionType.ElementAssignment) {
                // Clones but with new id
                this.playerId = source.playerId;
                this.controllerId = source.controllerId;
                this.controllerType = source.controllerType;
                this.controllerMap = source.controllerMap;
                this.changeType = source.changeType;
                this.actionElementMapId = source.actionElementMapId;
                this.actionId = source.actionId;
                this.actionAxisContribution = source.actionAxisContribution;
                this.actionType = source.actionType;
                this.assignFullAxis = source.assignFullAxis;
                this.invert = source.invert;
                this.pollingInfo = source.pollingInfo;
                this.modifierKeyFlags = source.modifierKeyFlags;
            }

            public void ReplaceOrCreateActionElementMap() {
                controllerMap.ReplaceOrCreateElementMap(this.ToElementAssignment()); // create new element map or replace existing
            }

            public ElementAssignmentConflictCheck ToElementAssignmentConflictCheck() {
                // Create an ElementAssignmentConflictCheck struct for use in conflict checking
                return new ElementAssignmentConflictCheck(
                    playerId, controllerType, controllerId, controllerMap.id,
                    pollingInfo.elementType, pollingInfo.elementIdentifierId, AssignedAxisRange,
                    pollingInfo.keyboardKey, modifierKeyFlags, actionId,
                    actionAxisContribution, invert, actionElementMapId
                );
            }

            public ElementAssignment ToElementAssignment() {
                // Create an ElementAssignment struct for use in assignment
                return new ElementAssignment(
                    controllerType, pollingInfo.elementType, pollingInfo.elementIdentifierId, AssignedAxisRange, pollingInfo.keyboardKey,
                    modifierKeyFlags, actionId, actionAxisContribution, invert, actionElementMapId
                );
            }
        }

        private class FallbackJoystickIdentification : QueueEntry {
            public int joystickId { get; private set; }
            public string joystickName { get; private set; }

            public FallbackJoystickIdentification(
                int joystickId,
                string joystickName
            )
                : base(QueueActionType.FallbackJoystickIdentification) {
                this.joystickId = joystickId;
                this.joystickName = joystickName;
            }
        }

        private class Calibration : QueueEntry {
            public Player player { get; private set; }
            public ControllerType  controllerType { get; private set; }
            public Joystick joystick { get; private set; }
            public CalibrationMap calibrationMap { get; private set; }
            
            public int selectedElementIdentifierId;
            public bool recording;

            public Calibration(
                Player player,
                Joystick joystick,
                CalibrationMap calibrationMap
            )
                : base(QueueActionType.Calibrate) {
                    this.player = player;
                    this.joystick = joystick;
                    this.calibrationMap = calibrationMap;
                    selectedElementIdentifierId = -1;
            }
        }

        private struct WindowProperties {
            public int windowId;
            public Rect rect;
            public Action<string, string> windowDrawDelegate;
            public string title;
            public string message;
        }

        #endregion

        #region Enums

        private enum QueueActionType {
            None = 0,
            JoystickAssignment = 1,
            ElementAssignment = 2,
            FallbackJoystickIdentification = 3,
            Calibrate = 4
        }

        private enum ElementAssignmentChangeType {
            Add = 0,
            Replace = 1,
            Remove = 2,
            ReassignOrRemove = 3,
            ConflictCheck = 4
        }


        public enum UserResponse {
            Confirm = 0,
            Cancel = 1,
            Custom1 = 2,
            Custom2 = 3
        }
        #endregion
    }
}
