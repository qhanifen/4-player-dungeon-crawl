using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonGeneration
{
    [CustomEditor(typeof(Room))]
    public class RoomEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Create Junction"))
            { 

            }
        }
    }
}