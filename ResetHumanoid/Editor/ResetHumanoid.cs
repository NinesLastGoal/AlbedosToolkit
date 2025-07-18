using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DreadScripts.ResetHumanoid
{
	public class ResetHumanoid : EditorWindow
	{
		private static Animator ani;
		private static bool full;
		private static bool pos = true, rot = true, scale = true;

		[MenuItem("DreadTools/Utility/Reset Humanoid")]
		private static void showWindow()
		{
			GetWindow<ResetHumanoid>(false, "Reset Humanoid", true);
			if (ani == null) ani = FindObjectOfType<Animator>();
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 60;
			ani = (Animator) EditorGUILayout.ObjectField("Avatar", ani, typeof(Animator), true);
			EditorGUILayout.BeginHorizontal();
			pos = EditorGUILayout.Toggle("Position", pos);
			rot = EditorGUILayout.Toggle("Rotation", rot);
			scale = EditorGUILayout.Toggle("Scale", scale);
			EditorGUIUtility.labelWidth = 0;
			EditorGUILayout.EndHorizontal();
			full = EditorGUILayout.Toggle(new GUIContent("Full Reset", "Reset includes the objects that the Model was imported with"), full);
			if (GUILayout.Button("Reset"))
				ResetPose(ani, full, pos, rot, scale);

		}

		public static void ResetPose(Animator ani, bool fullReset = false, bool position = true, bool rotation = true, bool scale = true)
		{
			if (!ani.avatar)
			{
				Debug.LogWarning("Avatar is required to reset pose!");
				return;
			}

			// Humans IDs if not full reset, otherwise All Ids
			// ID > Path
			// ID > Element > Transform Data
			Undo.RegisterFullObjectHierarchyUndo(ani.gameObject, "HierarchyReset");
			SerializedObject sAvi = new SerializedObject(ani.avatar);
			SerializedProperty humanIds = sAvi.FindProperty("m_Avatar.m_Human.data.m_Skeleton.data.m_ID");
			SerializedProperty allIds = sAvi.FindProperty("m_Avatar.m_AvatarSkeleton.data.m_ID");
			SerializedProperty defaultPose = sAvi.FindProperty("m_Avatar.m_DefaultPose.data.m_X");
			SerializedProperty tos = sAvi.FindProperty("m_TOS");

			Dictionary<long, int> idToElem = new Dictionary<long, int>();
			Dictionary<int, TransformData> elemToTransform = new Dictionary<int, TransformData>();
			Dictionary<long, string> IdToPath = new Dictionary<long, string>();

			for (int i = 0; i < allIds.arraySize; i++)
				idToElem.Add(allIds.GetArrayElementAtIndex(i).longValue, i);

			for (int i = 0; i < defaultPose.arraySize; i++)
				elemToTransform.Add(i, new TransformData(defaultPose.GetArrayElementAtIndex(i)));

			for (int i = 0; i < tos.arraySize; i++)
			{
				SerializedProperty currProp = tos.GetArrayElementAtIndex(i);
				IdToPath.Add(currProp.FindPropertyRelative("first").longValue, currProp.FindPropertyRelative("second").stringValue);
			}

			Action<Transform, TransformData> applyTransform = (transform, data) =>
			{
				if (transform)
				{
					if (position)
						transform.localPosition = data.pos;
					if (rotation)
						transform.localRotation = data.rot;
					if (scale)
						transform.localScale = data.scale;
				}
			};

			if (!fullReset)
			{
				for (int i = 0; i < humanIds.arraySize; i++)
				{
					Transform myBone = ani.transform.Find(IdToPath[humanIds.GetArrayElementAtIndex(i).longValue]);
					TransformData data = elemToTransform[idToElem[humanIds.GetArrayElementAtIndex(i).longValue]];
					applyTransform(myBone, data);
				}
			}
			else
			{
				for (int i = 0; i < allIds.arraySize; i++)
				{
					Transform myBone = ani.transform.Find(IdToPath[allIds.GetArrayElementAtIndex(i).longValue]);
					TransformData data = elemToTransform[idToElem[allIds.GetArrayElementAtIndex(i).longValue]];
					applyTransform(myBone, data);
				}
			}
		}

		struct TransformData
		{
			public Vector3 pos;
			public Quaternion rot;
			public Vector3 scale;

			public TransformData(SerializedProperty t)
			{
				SerializedProperty tProp = t.FindPropertyRelative("t");
				SerializedProperty qProp = t.FindPropertyRelative("q");
				SerializedProperty sProp = t.FindPropertyRelative("s");
				pos = new Vector3(tProp.FindPropertyRelative("x").floatValue, tProp.FindPropertyRelative("y").floatValue, tProp.FindPropertyRelative("z").floatValue);
				rot = new Quaternion(qProp.FindPropertyRelative("x").floatValue, qProp.FindPropertyRelative("y").floatValue, qProp.FindPropertyRelative("z").floatValue, qProp.FindPropertyRelative("w").floatValue);
				scale = new Vector3(sProp.FindPropertyRelative("x").floatValue, sProp.FindPropertyRelative("y").floatValue, sProp.FindPropertyRelative("z").floatValue);
			}
		}
	}
}