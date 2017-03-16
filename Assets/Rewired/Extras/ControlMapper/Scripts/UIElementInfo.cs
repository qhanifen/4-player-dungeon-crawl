// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using Rewired;

    [AddComponentMenu("")]
    public abstract class UIElementInfo : MonoBehaviour, ISelectHandler {

        public string identifier;
        public int intData;
        public Text text;

        public event System.Action<GameObject> OnSelectedEvent;

        #region ISelectHandler Implementation

        public void OnSelect(BaseEventData eventData) {
            if(OnSelectedEvent != null) OnSelectedEvent(this.gameObject);
        }

        #endregion
    }
}