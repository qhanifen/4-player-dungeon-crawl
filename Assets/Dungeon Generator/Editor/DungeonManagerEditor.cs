using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonGeneration
{
    [CustomEditor(typeof(DungeonManager))]
    public class DungeonManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Dungeon"))
            {
                DungeonManager generator = (DungeonManager)target;

                generator.GenerateDungeon();
            }
        }
    }
}