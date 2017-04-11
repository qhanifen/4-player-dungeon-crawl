using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(Hero)), CanEditMultipleObjects]
public class HeroEditor : Editor 
{
	public SerializedProperty 
		heroName_Prop,
		heroType_Prop,
		health_Prop,
		maxHealth_Prop,
		target_Prop,
		meleeSpeed_Prop,
		rangedSpeed_Prop,
		rangedFirePoint_Prop,
		rangedBulletPrefab_Prop;

	// Use this for initialization
	void OnEnable()
	{
		heroName_Prop = serializedObject.FindProperty("heroName");
		heroType_Prop = serializedObject.FindProperty("heroType");
		health_Prop = serializedObject.FindProperty("health");
		maxHealth_Prop = serializedObject.FindProperty("maxHealth");
		target_Prop = serializedObject.FindProperty("target");
		meleeSpeed_Prop = serializedObject.FindProperty("meleeSpeed");
		rangedSpeed_Prop = serializedObject.FindProperty("rangedFireRate");
		rangedFirePoint_Prop = serializedObject.FindProperty("firePoint");
		rangedBulletPrefab_Prop = serializedObject.FindProperty("defaultShot");
	}

    /*
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		#region Generic Base Stats
		EditorGUILayout.PropertyField(heroName_Prop, new GUIContent(heroName_Prop.displayName));
		EditorGUILayout.PropertyField(heroType_Prop, new GUIContent(heroType_Prop.displayName));


		EditorGUILayout.PropertyField(health_Prop, new GUIContent(health_Prop.displayName));
		EditorGUILayout.PropertyField(maxHealth_Prop, new GUIContent(maxHealth_Prop.displayName));

		EditorGUILayout.PropertyField(target_Prop, new GUIContent(target_Prop.displayName));

		#endregion

		Hero.HeroType ht = (Hero.HeroType)heroType_Prop.intValue;

		switch(ht)
		{
		case Hero.HeroType.Melee:
			EditorGUILayout.PropertyField(meleeSpeed_Prop, new GUIContent(meleeSpeed_Prop.displayName));
			break;
		case Hero.HeroType.Ranged:
			EditorGUILayout.PropertyField(rangedSpeed_Prop, new GUIContent(rangedSpeed_Prop.displayName));
			EditorGUILayout.PropertyField(rangedFirePoint_Prop, new GUIContent(rangedFirePoint_Prop.displayName));
			EditorGUILayout.PropertyField(rangedBulletPrefab_Prop, new GUIContent(rangedBulletPrefab_Prop.displayName));
			break;
		default:
			break;
		}

		serializedObject.ApplyModifiedProperties();
	}
    */
}
