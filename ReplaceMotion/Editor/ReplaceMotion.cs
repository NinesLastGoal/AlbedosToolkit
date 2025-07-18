using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using System.Linq;

namespace DreadScripts.ReplaceMotion
{
    public class ReplaceMotion : EditorWindow
    {
        static List<Motion> originalMotion = new List<Motion>();
        private static bool hasEmptyState;
        private static bool replacingEmptyState;
        static bool[] replaceFields;
        static Motion[] targetMotion;
        static Motion emptyTargetMotion;
        static readonly Dictionary<Motion, Motion> replaceValues = new Dictionary<Motion, Motion>();

        private static VRCAvatarDescriptor mainAvatar;
        private static AnimatorController mainController;

        private static Vector2 scroll;

        [MenuItem("DreadTools/Utility/Replace Motion")]
        private static void showWindow()
        {
            GetWindow<ReplaceMotion>(false, "Replace Motion", true);
        }

        private void OnEnable()
        {
            if (!mainAvatar && !mainController)
            {
                mainAvatar = FindObjectOfType<VRCAvatarDescriptor>();
                if (mainAvatar)
                    GetMotions(mainAvatar);
            }
        }

        private void OnGUI()
        {
            GUIStyle labelButton = new GUIStyle(GUI.skin.button) { padding = new RectOffset(1, 1, 1, 1), margin = new RectOffset(), alignment = TextAnchor.MiddleCenter };
            scroll = EditorGUILayout.BeginScrollView(scroll);
            using (new GUILayout.HorizontalScope("box"))
            {
                EditorGUI.BeginChangeCheck();
                Object dummy = CustomObjectField(new GUIContent("Target"), (Object)mainAvatar ?? mainController ?? null, true, true, out int resultType, typeof(VRCAvatarDescriptor), typeof(AnimatorController));
                if (EditorGUI.EndChangeCheck())
                {
                    switch (resultType)
                    {
                        case -1:
                            mainAvatar = null;
                            mainController = null;
                            break;
                        case 0:
                            mainAvatar = (VRCAvatarDescriptor)dummy;
                            mainController = null;
                            break;
                        case 1:
                            mainAvatar = null;
                            mainController = (AnimatorController)dummy;
                            break;
                    }
                    if (mainAvatar)
                        GetMotions(mainAvatar);
                    else if (mainController)
                        GetMotions(mainController);
                }
            }
            EditorGUI.BeginDisabledGroup(!(mainAvatar || mainController));
            if (GUILayout.Button("Replace"))
            {
                PopulateDictionary();
                if (mainAvatar)
                    SetMotions(mainAvatar);
                else
                    SetMotions(mainController);
                AssetDatabase.SaveAssets();
                Debug.Log("<color=green>[Replace Motion] </color>Done!");

                if (mainAvatar)
                    GetMotions(mainAvatar);
                else
                    GetMotions(mainController);
            }
            EditorGUI.EndDisabledGroup();
            

            if (originalMotion.Count > 0 || hasEmptyState)
            {
                DrawSeperator();
                if (hasEmptyState)
                {
                    using (new GUILayout.HorizontalScope("box"))
                    {
                        EditorGUILayout.ObjectField(null, typeof(Motion), false);
                        GUILayout.Space(20);
                        GUILayout.Label("->", "boldlabel", GUILayout.Width(20));
                        GUILayout.Space(20);

                        if (!replacingEmptyState)
                            replacingEmptyState = GUILayout.Toggle(replacingEmptyState, new GUIContent("Replace", "Replace all instances of the original motion"), "button");

                        else
                        {
                            if (GUILayout.Button("X", labelButton, GUILayout.Width(20)))
                                replacingEmptyState = false;

                            emptyTargetMotion = (Motion)EditorGUILayout.ObjectField(emptyTargetMotion, typeof(Motion), true);
                        }

                    }
                }
                for (int i = 0; i < originalMotion.Count; i++)
                {
                    using (new GUILayout.HorizontalScope("box"))
                    {
                        EditorGUILayout.ObjectField(originalMotion[i], typeof(Motion), false);
                        GUILayout.Space(20);
                        GUILayout.Label("->", "boldlabel", GUILayout.Width(20));
                        GUILayout.Space(20);
                        if (!replaceFields[i]) 
                            replaceFields[i] = GUILayout.Toggle(replaceFields[i],new GUIContent("Replace","Replace all instances of the original motion"), "button");
                        else
                        {
                            if (GUILayout.Button("X", labelButton, GUILayout.Width(20)))
                                replaceFields[i] = false;
                            targetMotion[i] = (Motion)EditorGUILayout.ObjectField(targetMotion[i], typeof(Motion), true);
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private static Object CustomObjectField(GUIContent label, Object displayObject, bool allowScene, bool checkComponents, out int resultTypeIndex, params System.Type[] validTypes)
        {
            //Special Cases
            bool supportsController = false;
            int controllerTypeIndex = -1;
            if (checkComponents)
            {
                for (int i = 0; i < validTypes.Length; i++)
                {
                    if (validTypes[i] == typeof(AnimatorController))
                    {
                        supportsController = true;
                        controllerTypeIndex = i;
                        break;
                    }
                }
            }
            ///////////////


            EditorGUI.BeginChangeCheck();
            Object dummy = EditorGUILayout.ObjectField(label, displayObject, typeof(Object), allowScene);

            if (EditorGUI.EndChangeCheck())
            {
                if (!dummy)
                {
                    resultTypeIndex = -1;
                    return null;
                }

                for (int i = 0; i < validTypes.Length; i++)
                {
                    if (dummy.GetType() == validTypes[i])
                    {
                        resultTypeIndex = i;
                        return dummy;
                    }
                }

                if (checkComponents && dummy is GameObject go)
                {
                    Component[] components = go.GetComponents<Component>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        for (int j = 0; j < validTypes.Length; j++)
                        {
                            if (components[i].GetType() == validTypes[j])
                            {
                                resultTypeIndex = j;
                                return components[i];
                            }

                            //Special Cases
                            if (supportsController && (components[i] is Animator ani) && ani.runtimeAnimatorController)
                            {
                                resultTypeIndex = controllerTypeIndex;
                                return AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(ani.runtimeAnimatorController));
                            }
                            ///////////////
                        }
                    }
                }
                string validTypesMessage = string.Join(", ", validTypes.Select(t => t.Name));
                Debug.LogWarning("Field must be of Type: " + validTypesMessage);
            }
            resultTypeIndex = -2;
            return dummy;
        }
        private void SetMotions(VRCAvatarDescriptor avatar)
        {
            IterateStates(avatar, SetMotions);
        }
        
        private void GetMotions(VRCAvatarDescriptor avatar)
        {
            GetStart();
            IterateStates(avatar, s => GetMotions(s.motion));
            GetFinal();
        }
        private void SetMotions(AnimatorController controller)
        {
            IterateStates(controller, SetMotions);
        }
        private void GetMotions(AnimatorController controller)
        {
            GetStart();
            IterateStates(controller, s => GetMotions(s.motion));
            GetFinal();
        }

        private void GetStart()
        {
            hasEmptyState = false;
            emptyTargetMotion = null;
            originalMotion.Clear();
        }
        private void GetFinal()
        {
            originalMotion = originalMotion.Distinct().ToList();
            targetMotion = new Motion[originalMotion.Count];
            replaceFields = new bool[originalMotion.Count];
        }

        private void GetMotions(BlendTree tree)
        {
            originalMotion.Add(tree);
            for (int i = 0; i < tree.children.Length; i++)
            {
                GetMotions(tree.children[i].motion);
            }
        }

        private void GetMotions(Motion motion)
        {
            if (!motion)
                hasEmptyState = true;
            else
            {
                if (motion is AnimationClip)
                    originalMotion.Add(motion);
                else
                {
                    if (motion is BlendTree tree)
                    {
                        GetMotions(tree);
                    }
                }
            }
        }

        private void SetMotions(AnimatorState state)
        {
            if (!state.motion)
            {
                if (replacingEmptyState)
                    state.motion = emptyTargetMotion;
            }
            else
            {
                if (replaceValues.ContainsKey(state.motion))
                {
                    state.motion = replaceValues[state.motion];
                    EditorUtility.SetDirty(state);
                }
                else
                {
                    if (state.motion is BlendTree tree)
                    {
                        SetMotions(tree);
                    }
                }
            }
        }
        private void SetMotions(BlendTree tree)
        {
            ChildMotion[] newMotions = tree.children;
            for (int i = 0; i < newMotions.Length; i++)
            {
                if (replaceValues.ContainsKey(newMotions[i].motion))
                    newMotions[i].motion = replaceValues[newMotions[i].motion];
                else
                    if (newMotions[i].motion is BlendTree subTree)
                    SetMotions(subTree);
            }
            tree.children = newMotions;
            EditorUtility.SetDirty(tree);
        }

        private void PopulateDictionary()
        {
            replaceValues.Clear();
            for (int i = 0; i < originalMotion.Count; i++)
            {
                if (!replaceFields[i])
                    replaceValues.Add(originalMotion[i], originalMotion[i]);
                else
                    replaceValues.Add(originalMotion[i], targetMotion[i]);
            }
        }

        public static void IterateStates(VRCAvatarDescriptor avatar, System.Action<AnimatorState> action)
        {
            HashSet<AnimatorController> visitedControllers = new HashSet<AnimatorController>();
            foreach (var layer in avatar.baseAnimationLayers.Concat(avatar.specialAnimationLayers))
            {
                if (layer.animatorController)
                {
                    AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(layer.animatorController));
                    if (controller && !visitedControllers.Contains(controller))
                    {
                        IterateStates(controller, action);
                        visitedControllers.Add(controller);
                    }
                }
            }
            foreach (var runtimeController in avatar.GetComponentsInChildren<Animator>().Select(a => a.runtimeAnimatorController))
            {
                if (runtimeController)
                {
                    AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(runtimeController));
                    if (controller && !visitedControllers.Contains(controller))
                    {
                        IterateStates(controller, action);
                        visitedControllers.Add(controller);
                    }
                }
            }
        }

        public static void IterateStates(AnimatorController controller, System.Action<AnimatorState> action)
        {
            foreach (var layer in controller.layers)
            {
                IterateStates(layer.stateMachine, action, true);
            }
        }

        public static void IterateStates(AnimatorStateMachine machine, System.Action<AnimatorState> action, bool deep = true)
        {
            if (deep)
                foreach (var subMachine in machine.stateMachines.Select(c => c.stateMachine))
                {
                    IterateStates(subMachine, action);
                }

            foreach (var state in machine.states.Select(s => s.state))
            {
                action(state);
            }
        }

        private static void DrawSeperator()
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(1 + 2));
            r.height = 1;
            r.y += 1;
            r.x -= 2;
            r.width += 6;
            ColorUtility.TryParseHtmlString(EditorGUIUtility.isProSkin ? "#595959" : "#858585", out Color lineColor);
            EditorGUI.DrawRect(r, lineColor);
        }
        
    }
    
}