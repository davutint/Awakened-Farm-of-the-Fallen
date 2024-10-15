/* ================================================================
   ----------------------------------------------------------------
   Project   :   AI Tree
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Zinnur Davleev
   ----------------------------------------------------------------
   Copyright 2022-2023 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RenownedGames.AITreeEditor
{
    public class AllSelector : Manipulator
    {
#if UNITY_2022_3_OR_NEWER
        private EditorWindow window;
#endif

        /// <summary>
        /// Called to register event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
#if UNITY_2022_3_OR_NEWER
            AITreeSettings settings = AITreeSettings.instance;
            EditorApplication.update -= HotKeyListener;
            if (settings.HotKeyListener())
            {
                EditorApplication.update += HotKeyListener;
            }
            else
            {
                target.RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
            }
#else
            target.RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
#endif
        }

        /// <summary>
        /// Called to unregister event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
#if UNITY_2022_3_OR_NEWER
            EditorApplication.update -= HotKeyListener;
#endif
            target.UnregisterCallback<KeyDownEvent>(OnKeyDownEvent);
        }

        /// <summary>
        /// Select all graph elements.
        /// </summary>
        private void SelectAll()
        {
            BehaviourTreeGraph graph = target as BehaviourTreeGraph;
            if (graph == null)
            {
                return;
            }

            graph.ClearSelection();
            foreach (GraphElement element in graph.graphElements)
            {
                if (element is ISelectable selectable && element is not Edge)
                {
                    graph.AddToSelection(selectable);
                }
            }
        }

        /// <summary>
        /// Built-in Unity key down callback.
        /// </summary>
        /// <param name="evt">Event reference.</param>
        private void OnKeyDownEvent(KeyDownEvent evt)
        {
            if (target == null)
            {
                return;
            }

            if (evt.modifiers == EventModifiers.Control && evt.keyCode == KeyCode.A)
            {
                SelectAll();
            }
        }

#if UNITY_2022_3_OR_NEWER
        /// <summary>
        /// Hot key callback listener.
        /// </summary>
        private void HotKeyListener()
        {
            if (window == null)
            {
                BehaviourTreeGraph graph = target as BehaviourTreeGraph;
                window = graph.GetWindow();
            }

            if (HotKeyUtility.TryGetEvent(window, out Event evt)
                && evt.type == EventType.Used
                && evt.keyCode == KeyCode.A
                && evt.control)
            {
                SelectAll();
            }
        }
#endif
    }
}