// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Data {

    using UnityEngine;
    using System.Collections.Generic;
    using Rewired;

    /// <summary>
    /// Class for saving data to PlayerPrefs. Add this as a component to your Rewired Input Manager to save and load data automatically to PlayerPrefs.
    /// Copy this class and customize it to your needs to create a new custom data storage system.
    /// </summary>
    public class UserDataStore_PlayerPrefs : UserDataStore {

        [SerializeField]
        private bool isEnabled = true;

        [SerializeField]
        private bool loadDataOnStart = true;

        [SerializeField]
        private string playerPrefsKeyPrefix = "RewiredSaveData";

        #region UserDataStore Implementation

        // Public Methods

        /// <summary>
        /// Save all data now.
        /// </summary>
        public override void Save() {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
                return;
            }
            SaveAll();
        }

        /// <summary>
        /// Save all data for a specific controller for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void SaveControllerData(int playerId, ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
                return;
            }
            SaveControllerDataNow(playerId, controllerType, controllerId);
        }

        /// <summary>
        /// Save all data for a specific controller. Does not save Player data.
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void SaveControllerData(ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
                return;
            }
            SaveControllerDataNow(controllerType, controllerId);
        }

        /// <summary>
        /// Save all data for a specific Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        public override void SavePlayerData(int playerId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
                return;
            }
            SavePlayerDataNow(playerId);
        }

        /// <summary>
        /// Save all data for a specific InputBehavior for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="behaviorId">Input Behavior id</param>
        public override void SaveInputBehavior(int playerId, int behaviorId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
                return;
            }
            SaveInputBehaviorNow(playerId, behaviorId);
        }

        /// <summary>
        /// Load all data now.
        /// </summary>
        public override void Load() {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
                return;
            }
            LoadAll();
        }

        /// <summary>
        /// Load all data for a specific controller for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void LoadControllerData(int playerId, ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
                return;
            }
            LoadControllerDataNow(playerId, controllerType, controllerId);
        }

        /// <summary>
        /// Load all data for a specific controller. Does not load Player data.
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void LoadControllerData(ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
                return;
            }
            LoadControllerDataNow(controllerType, controllerId);
        }

        /// <summary>
        /// Load all data for a specific Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        public override void LoadPlayerData(int playerId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
                return;
            }
            LoadPlayerDataNow(playerId);
        }

        /// <summary>
        /// Load all data for a specific InputBehavior for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="behaviorId">Input Behavior id</param>
        public override void LoadInputBehavior(int playerId, int behaviorId) {
            if(!isEnabled) {
                Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
                return;
            }
            LoadInputBehaviorNow(playerId, behaviorId);
        }

        // Event Handlers

        /// <summary>
        /// Called when SaveDataStore is initialized.
        /// </summary>
        protected override void OnInitialize() {
            if(loadDataOnStart) Load();
        }

        /// <summary>
        /// Called when a controller is connected.
        /// </summary>
        /// <param name="args">ControllerStatusChangedEventArgs</param>
        protected override void OnControllerConnected(ControllerStatusChangedEventArgs args) {
            if(!isEnabled) return;

            // Load data when joystick is connected
            if(args.controllerType == ControllerType.Joystick) LoadJoystickData(args.controllerId);
        }

        /// <summary>
        /// Calls after a controller has been disconnected.
        /// </summary>
        /// <param name="args">ControllerStatusChangedEventArgs</param>
        protected override void OnControllerPreDiscconnect(ControllerStatusChangedEventArgs args) {
            if(!isEnabled) return;

            // Save data before joystick is disconnected
            if(args.controllerType == ControllerType.Joystick) SaveJoystickData(args.controllerId);
        }

        /// <summary>
        /// Called when a controller is about to be disconnected.
        /// </summary>
        /// <param name="args">ControllerStatusChangedEventArgs</param>
        protected override void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {
            if(!isEnabled) return;

            // Nothing to do
        }

        #endregion

        #region Load

        private void LoadAll() {
            
            // Load all data for all players
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) {
                LoadPlayerDataNow(allPlayers[i]);
            }

            // Load all joystick calibration maps
            LoadAllJoystickCalibrationData();
        }

        private void LoadPlayerDataNow(int playerId) {
            LoadPlayerDataNow(ReInput.players.GetPlayer(playerId));
        }
        private void LoadPlayerDataNow(Player player) {
            if(player == null) return;

            // Load Input Behaviors
            LoadInputBehaviors(player.id);

            // Load Keyboard Maps
            LoadControllerMaps(player.id, ControllerType.Keyboard, 0);

            // Load Mouse Maps
            LoadControllerMaps(player.id, ControllerType.Mouse, 0);

            // Load Joystick Maps for each joystick
            foreach(Joystick joystick in player.controllers.Joysticks) {
                LoadControllerMaps(player.id, ControllerType.Joystick, joystick.id);
            }
        }

        private void LoadAllJoystickCalibrationData() {
            // Load all calibration maps from all joysticks
            IList<Joystick> joysticks = ReInput.controllers.Joysticks;
            for(int i = 0; i < joysticks.Count; i++) {
                LoadJoystickCalibrationData(joysticks[i]);
            }
        }

        private void LoadJoystickCalibrationData(Joystick joystick) {
            if(joystick == null) return;
            joystick.ImportCalibrationMapFromXmlString(GetJoystickCalibrationMapXml(joystick)); // load joystick calibration map
        }
        private void LoadJoystickCalibrationData(int joystickId) {
            LoadJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
        }

        private void LoadJoystickData(int joystickId) {
            // Load joystick maps in all Players for this joystick id
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) { // this controller may be owned by more than one player, so check all
                Player player = allPlayers[i];
                if(!player.controllers.ContainsController(ControllerType.Joystick, joystickId)) continue; // player does not have the joystick
                LoadControllerMaps(player.id, ControllerType.Joystick, joystickId); // load the maps
            }

            // Load calibration maps for joystick
            LoadJoystickCalibrationData(joystickId);
        }

        private void LoadControllerDataNow(int playerId, ControllerType controllerType, int controllerId) {

            // Load map data
            LoadControllerMaps(playerId, controllerType, controllerId);

            // Loat other controller data
            LoadControllerDataNow(controllerType, controllerId);
        }
        private void LoadControllerDataNow(ControllerType controllerType, int controllerId) {

            // Load calibration data for joysticks
            if(controllerType == ControllerType.Joystick) {
                LoadJoystickCalibrationData(controllerId);
            }
        }

        private void LoadControllerMaps(int playerId, ControllerType controllerType, int controllerId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return;

            Controller controller = ReInput.controllers.GetController(controllerType, controllerId);
            if(controller == null) return;

            // Load the controller maps first and make sure we have them to load
            List<string> xmlMaps = GetAllControllerMapsXml(player, true, controllerType, controller);
            if(xmlMaps.Count == 0) return;

            // Load Joystick Maps
            player.controllers.maps.AddMapsFromXml(controllerType, controllerId, xmlMaps); // load controller maps
        }

        private void LoadInputBehaviors(int playerId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return;

            // All players have an instance of each input behavior so it can be modified
            IList<InputBehavior> behaviors = ReInput.mapping.GetInputBehaviors(player.id); // get all behaviors from player
            for(int i = 0; i < behaviors.Count; i++) {
                LoadInputBehaviorNow(player, behaviors[i]);
            }
        }

        private void LoadInputBehaviorNow(int playerId, int behaviorId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return;

            InputBehavior behavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
            if(behavior == null) return;

            LoadInputBehaviorNow(player, behavior);
        }
        private void LoadInputBehaviorNow(Player player, InputBehavior inputBehavior) {
            if(player == null || inputBehavior == null) return;

            string xml = GetInputBehaviorXml(player, inputBehavior.id); // try to the behavior for this id
            if(xml == null || xml == string.Empty) return; // no data found for this behavior
            inputBehavior.ImportXmlString(xml); // import the data into the behavior
        }

        #endregion

        #region Save

        private void SaveAll() {

            // Save all data in all Players including System Player
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) {
                SavePlayerDataNow(allPlayers[i]);
            }

            // Save joystick calibration maps
            SaveAllJoystickCalibrationData();

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }

        private void SavePlayerDataNow(int playerId) {
            SavePlayerDataNow(ReInput.players.GetPlayer(playerId));
        }
        private void SavePlayerDataNow(Player player) {
            if(player == null) return;

            // Get all savable data from player
            PlayerSaveData playerData = player.GetSaveData(true);

            // Save Input Behaviors
            SaveInputBehaviors(player, playerData);

            // Save controller maps
            SaveControllerMaps(player, playerData);
        }

        private void SaveAllJoystickCalibrationData() {
            // Save all calibration maps from all joysticks
            IList<Joystick> joysticks = ReInput.controllers.Joysticks;
            for(int i = 0; i < joysticks.Count; i++) {
                SaveJoystickCalibrationData(joysticks[i]);
            }
        }

        private void SaveJoystickCalibrationData(int joystickId) {
            SaveJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
        }
        private void SaveJoystickCalibrationData(Joystick joystick) {
            if(joystick == null) return;
            JoystickCalibrationMapSaveData saveData = joystick.GetCalibrationMapSaveData();
            string key = GetJoystickCalibrationMapPlayerPrefsKey(saveData);
            PlayerPrefs.SetString(key, saveData.map.ToXmlString()); // save the map to player prefs in XML format
        }

        private void SaveJoystickData(int joystickId) {
            // Save joystick maps in all Players for this joystick id
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) { // this controller may be owned by more than one player, so check all
                Player player = allPlayers[i];
                if(!player.controllers.ContainsController(ControllerType.Joystick, joystickId)) continue; // player does not have the joystick

                // Save controller maps
                SaveControllerMaps(player.id, ControllerType.Joystick, joystickId);
            }

            // Save calibration data
            SaveJoystickCalibrationData(joystickId);
        }

        private void SaveControllerDataNow(int playerId, ControllerType controllerType, int controllerId) {

            // Save map data
            SaveControllerMaps(playerId, controllerType, controllerId);

            // Save other controller data
            SaveControllerDataNow(controllerType, controllerId);
        }
        private void SaveControllerDataNow(ControllerType controllerType, int controllerId) {

            // Save calibration data for joysticks
            if(controllerType == ControllerType.Joystick) {
                SaveJoystickCalibrationData(controllerId);
            }
        }

        private void SaveControllerMaps(Player player, PlayerSaveData playerSaveData) {
            foreach(ControllerMapSaveData saveData in playerSaveData.AllControllerMapSaveData) {
                string key = GetControllerMapPlayerPrefsKey(player, saveData);
                PlayerPrefs.SetString(key, saveData.map.ToXmlString()); // save the map to player prefs in XML format
            }
        }
        private void SaveControllerMaps(int playerId, ControllerType controllerType, int controllerId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return;

            // Save controller maps in this player for this controller id
            string key;
            if(!player.controllers.ContainsController(controllerType, controllerId)) return; // player does not have the controller

            // Save controller maps
            ControllerMapSaveData[] saveData = player.controllers.maps.GetMapSaveData(controllerType, controllerId, true);
            if(saveData == null) return;

            for(int i = 0; i < saveData.Length; i++) {
                key = GetControllerMapPlayerPrefsKey(player, saveData[i]);
                PlayerPrefs.SetString(key, saveData[i].map.ToXmlString()); // save the map to player prefs in XML format
            }
        }

        private void SaveInputBehaviors(Player player, PlayerSaveData playerSaveData) {
            if(player == null) return;
            InputBehavior[] inputBehaviors = playerSaveData.inputBehaviors;
            for(int i = 0; i < inputBehaviors.Length; i++) {
                SaveInputBehaviorNow(player, inputBehaviors[i]);
            }
        }

        private void SaveInputBehaviorNow(int playerId, int behaviorId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return;

            InputBehavior behavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
            if(behavior == null) return;

            SaveInputBehaviorNow(player, behavior);
        }
        private void SaveInputBehaviorNow(Player player, InputBehavior inputBehavior) {
            if(player == null || inputBehavior == null) return;

            string key = GetInputBehaviorPlayerPrefsKey(player, inputBehavior);
            PlayerPrefs.SetString(key, inputBehavior.ToXmlString()); // save the behavior to player prefs in XML format
        }

        #endregion

        #region PlayerPrefs Methods

        /* NOTE ON PLAYER PREFS:
         * PlayerPrefs on Windows Standalone is saved in the registry. There is a bug in Regedit that makes any entry with a name equal to or greater than 255 characters
         * (243 + 12 unity appends) invisible in Regedit. Unity will still load the data fine, but if you are debugging and wondering why your data is not showing up in
         * Regedit, this is why. If you need to delete the values, either call PlayerPrefs.Clear or delete the key folder in Regedit -- Warning: both methods will
         * delete all player prefs including any ones you've created yourself or other plugins have created.
         */

        // WARNING: Do not use & symbol in keys. Linux cannot load them after the current session ends.

        private string GetBasePlayerPrefsKey(Player player) {
            string key = playerPrefsKeyPrefix;
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
            string key = playerPrefsKeyPrefix;
            key += "|dataType=CalibrationMap";
            key += "|controllerType=" + saveData.controllerType.ToString();
            key += "|hardwareIdentifier=" + saveData.hardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
            key += "|hardwareGuid=" + saveData.joystickHardwareTypeGuid.ToString();
            return key;
        }

        private string GetJoystickCalibrationMapXml(Joystick joystick) {
            string key = playerPrefsKeyPrefix;
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
    }
}