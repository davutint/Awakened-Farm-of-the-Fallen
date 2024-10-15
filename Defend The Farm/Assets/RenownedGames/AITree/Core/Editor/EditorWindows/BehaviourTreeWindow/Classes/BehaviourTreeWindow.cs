/* ================================================================
   ----------------------------------------------------------------
   Project   :   AI Tree
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Zinnur Davleev
   ----------------------------------------------------------------
   Copyright 2022-2023 Renowned Games All rights reserved.
   ================================================================ */

using RenownedGames.AITree;
using RenownedGames.ExLibEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RenownedGames.AITreeEditor
{
    [TrackerWindowTitle("Behaviour Tree", IconPath = "Images/Icons/Window/BehaviourTreeIcon.png")]
    public sealed class BehaviourTreeWindow : TrackerWindow
    {
        private bool autoLiveLink;
        private bool hasChanges;
        private int frameCount;
        private AITreeSettings settings;

        private Object rootTarget;
        private List<BehaviourTree> nestedTrees;

        private BehaviourTreeGraph graph;
        private Label treeName;
        private Label nodeDescription;
        private ToolbarMenu toolbarAssets;
        private ToolbarButton nestedtreesButton;
        private ToolbarToggle autoLiveLinkToggle;
        private ToolbarButton saveButton;
        private ToolbarToggle autoSaveToggle;
        private VisualElement simulatingBorder;

        /// <summary>
        /// This function is called when the window is loaded.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            nestedTrees = new List<BehaviourTree>();

            LoadVisualElements();

            if (!TryGetTarget(out Object target))
            {
                target = Selection.activeObject;
            }

            if(target != null)
            {
                NodeTypeCache.Loaded += _ => TrackEditor(target);
            }

            EditorApplication.playModeStateChanged -= OnPlayMoveStateChanged;
            EditorApplication.playModeStateChanged += OnPlayMoveStateChanged;
            AITreeSettings.Saved -= OnSettingsSaved;
            AITreeSettings.Saved += OnSettingsSaved;

            settings = AITreeSettings.instance;

#if UNITY_2022_3_OR_NEWER
            ShowHotKeyNotification();
#endif
        }

        /// <summary>
        /// Called when the window gets keyboard focus.
        /// </summary>
        private void OnFocus()
        {
            if (settings == null)
            {
                settings = AITreeSettings.instance;
            }

            if (HasUnloadedVisualElements())
            {
                LoadVisualElements();
            }

            UpdateTitleLabel();
        }

        /// <summary>
        /// Called for rendering and handling GUI events.
        /// </summary>
        private void OnGUI()
        {
#if UNITY_2022_3_OR_NEWER
            if (focusedWindow == this)
            {
                HotKeyUtility.SetEvent(Event.current);
            }
#endif
        }

        /// <summary>
        /// Called at 10 frames per second to give the inspector a chance to update.
        /// </summary>
        private void OnInspectorUpdate()
        {
            graph?.OnInspectorUpdate();
        }

        /// <summary>
        /// This function is called when the window is closed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            SaveChanges();
            EditorApplication.playModeStateChanged -= OnPlayMoveStateChanged;
            AITreeSettings.Saved -= OnSettingsSaved;
            graph.OnClose();

#if UNITY_2022_3_OR_NEWER
            HotKeyUtility.SetEvent(null);
#endif
        }

        /// <summary>
        /// Called every time the project changes.
        /// </summary>
        private void OnProjectChange()
        {
            if (TryGetBehaviourTree(out BehaviourTree behaviourTree) && graph.GetBehaviourTree() != null)
            {
                if (EditorUtility.IsDirty(behaviourTree))
                {
                    hasChanges = true;
                    saveButton.SetEnabled(true);
                    if (autoSaveToggle.value)
                    {
                        SaveChanges();
                    }
                }
            }
            else
            {
                TrackEditor(null);
            }
        }

        /// <summary>
        /// Called every time the selection changes.
        /// </summary>
        private void OnSelectionChange()
        {
            Object obj = Selection.activeObject;
            if (IsValidTarget(obj))
            {
                nestedTrees.Clear();
                nestedTrees.TrimExcess();
                nestedtreesButton.visible = false;
            }

            TrackEditor(obj);
        }

        /// <summary>
        /// Start tracking specified behaviour tree.
        /// </summary>
        /// <param name="target">Behaviour tree reference.</param>
        protected override void OnTrackEditor(Object target)
        {
            if (HasUnloadedVisualElements())
            {
                LoadVisualElements();
            }

            if (TryGetBehaviourTree(out BehaviourTree behaviourTree))
            {
                if (!Application.isPlaying && !AssetDatabase.IsNativeAsset(behaviourTree))
                {
                    behaviourTree.BecameNativeAsset -= OnBecameNativeAsset;
                    behaviourTree.BecameNativeAsset += OnBecameNativeAsset;
                    return;
                }

                Simulating(behaviourTree.IsRunning());
                graph.ClearSelection();
                graph.PopulateView(behaviourTree);
                treeName.text = ObjectNames.NicifyVariableName(behaviourTree.name);
                NotifyTrackEditor<NodeInspectorWindow>(behaviourTree);

                EditorApplication.update -= FrameAll;
                EditorApplication.update += FrameAll;
            }
            else
            {
                treeName.text = "BEHAVIOUR TREE";
                Simulating(false);
                graph.ClearSelection();
                graph.ClearGraph();
            }
        }

        /// <summary>
        /// Check if passed target references is valid.
        /// </summary>
        /// <param name="target">Target reference.</param>
        /// <returns>True if valid, otherwise false.</returns>
        protected override bool IsValidTarget(Object target)
        {
            return target is BehaviourTree || target is BehaviourRunner || (target is GameObject go && go.GetComponent<BehaviourRunner>());
        }

        /// <summary>
        /// Try get behaviour tree reference from target.
        /// </summary>
        /// <param name="behaviourTree">BehaviourTree reference.</param>
        /// <returns>True if behaviour Tree is exists. Otherwise false.</returns>
        public bool TryGetBehaviourTree(out BehaviourTree behaviourTree)
        {
            behaviourTree = null;
            if (TryGetTarget(out Object target))
            {
                if(target is BehaviourTree)
                {
                    behaviourTree = target as BehaviourTree;
                }
                else if(target is BehaviourRunner runner ||
                    target is GameObject go && go.TryGetComponent(out runner))
                {
                    behaviourTree = EditorApplication.isPlaying ? runner.GetBehaviourTree() : runner.GetSharedBehaviourTree();
                }
            }
            return behaviourTree != null;
        }

        /// <summary>
        /// Save behaviour tree changes.
        /// </summary>
        public override void SaveChanges()
        {
            base.SaveChanges();

            if (TryGetBehaviourTree(out BehaviourTree behaviourTree) && !behaviourTree.IsRunning())
            {
                AssetDatabase.SaveAssetIfDirty(behaviourTree);
                AssetDatabase.Refresh();
                MarkAsSaved();
            }
        }

        /// <summary>
        /// Discards behaviour tree changes.
        /// </summary>
        public override void DiscardChanges()
        {
            base.DiscardChanges();
        }

        /// <summary>
        /// Marks behaviour tree window as changed and need to save.
        /// </summary>
        public void MarkAsChanged()
        {
            MarkAsChanged(true);
        }

        /// <summary>
        /// Marks behaviour tree window as saved.
        /// </summary>
        public void MarkAsSaved()
        {
            MarkAsSaved(true);
        }

        /// <summary>
        /// Check if current window, is in nested mode.
        /// </summary>
        public bool IsNestedMode()
        {
            return nestedTrees.Count > 0 && rootTarget != null;
        }

        /// <summary>
        /// Reset nested mode and track root target.
        /// </summary>
        public void ResetNestedMode()
        {
            if (IsNestedMode())
            {
                TrackEditor(rootTarget);
            }

            nestedTrees.Clear();
            nestedTrees.TrimExcess();
            rootTarget = null;

            RefreshNestedModeElements();
        }

        /// <summary>
        /// Enter in subtree mode from root tree.
        /// </summary>
        /// <param name="subTree">Sub tree reference.</param>
        internal void SubTreeIn(BehaviourTree subTree)
        {
            if (!TryGetBehaviourTree(out BehaviourTree behaviourTree))
            {
                Debug.LogError("Sub tree mode available only after tracking the root target.");
                return;
            }

            if(nestedTrees.Count == 0)
            {
                rootTarget = GetTarget();
            }

            nestedTrees.Add(behaviourTree);
            TrackEditor(subTree);
            RefreshNestedModeElements();
        }

        /// <summary>
        /// Exit from subtree to parent tree.
        /// </summary>
        /// <param name="subTree">Parent sub tree reference.</param>
        internal void SubTreeOut(BehaviourTree subTree)
        {
            if (!TryGetBehaviourTree(out BehaviourTree behaviourTree))
            {
                Debug.LogError("Sub tree mode available only after tracking the root tree.");
                return;
            }

            nestedTrees.RemoveAll(t => t == null);
            for (int i = 0; i < nestedTrees.Count; i++)
            {
                BehaviourTree tree = nestedTrees[i];
                if (subTree == tree)
                {
                    TrackEditor(tree);
                    nestedTrees.RemoveRange(i, nestedTrees.Count - i);
                    break;
                }
            }
            RefreshNestedModeElements();
        }

        /// <summary>
        /// Exit from subtree to previous parent tree.
        /// </summary>
        internal void SubTreeOut()
        {
            nestedTrees.RemoveAll(t => t == null);
            if(nestedTrees.Count > 0)
            {
                int index = nestedTrees.Count - 1;
                BehaviourTree tree = nestedTrees[index];
                TrackEditor(tree);
                nestedTrees.RemoveAt(index);
            }
            RefreshNestedModeElements();
        }

        /// <summary>
        /// Marks behaviour tree window as changed and need to save.
        /// </summary>
        internal void MarkAsChanged(bool notifyAll)
        {
            if(TryGetBehaviourTree(out BehaviourTree tree))
            {
                EditorUtility.SetDirty(tree);
                hasChanges = true;
                saveButton.SetEnabled(true);

                if (autoSaveToggle.value)
                {
                    SaveChanges();
                }

                if (notifyAll)
                {
                    foreach (TrackerWindow tracker in ActiveTrackers)
                    {
                        if (tracker.GetInstanceID() != GetInstanceID()
                            && tracker is BehaviourTreeWindow window
                            && window.TryGetBehaviourTree(out BehaviourTree bt) && bt == tree)
                        {
                            window.MarkAsChanged(true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Marks behaviour tree window as saved.
        /// </summary>
        internal void MarkAsSaved(bool notifyAll)
        {
            hasChanges = false;
            saveButton.SetEnabled(false);

            if (notifyAll)
            {
                foreach (TrackerWindow tracker in ActiveTrackers)
                {
                    if (tracker.GetInstanceID() != GetInstanceID()
                        && tracker is BehaviourTreeWindow window
                        && window.TryGetBehaviourTree(out BehaviourTree lhs) == TryGetBehaviourTree(out BehaviourTree rhs)
                        && lhs == rhs)
                    {
                        window.MarkAsSaved(false);
                    }
                }
            }
        }

        /// <summary>
        /// Refresh nested mode UI elements.
        /// </summary>
        private void RefreshNestedModeElements()
        {
            nestedtreesButton.visible = nestedTrees.Count > 0;
        }

        /// <summary>
        /// Focus view all elements in the graph.
        /// </summary>
        private void FrameAll()
        {
            graph.FrameAll();
            Repaint();

            // A two-frame plug is necessary in order to make sure that the alignment has been performed.
            // GraphView.FrameAll() can skip framing.
            if (frameCount > 1)
            {
                EditorApplication.update -= FrameAll;
                frameCount = 0;
            }
            frameCount++;
        }

        /// <summary>
        /// Simulating mode is enabled.
        /// </summary>
        /// <returns></returns>
        private bool Simulating()
        {
            return simulatingBorder.style.display == DisplayStyle.Flex;
        }

        /// <summary>
        /// Set simulating mode.
        /// </summary>
        private void Simulating(bool value)
        {
            simulatingBorder.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Load all required Visual Elements.
        /// </summary>
        private void LoadVisualElements()
        {
            AITreeSettings settings = AITreeSettings.instance;

            rootVisualElement.Clear();

            VisualTreeAsset visualTree = settings.GetBehaviourTreeUXML();
            visualTree.CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(settings.GetBehaviourTreeUSS());

            graph = rootVisualElement.Q<BehaviourTreeGraph>();
            treeName = rootVisualElement.Q<Label>("tree-name");
            nodeDescription = rootVisualElement.Q<Label>("node-description");
            toolbarAssets = rootVisualElement.Q<ToolbarMenu>("toolbar-assets");
            nestedtreesButton = rootVisualElement.Q<ToolbarButton>("nested-trees");
            autoLiveLinkToggle = rootVisualElement.Q<ToolbarToggle>("auto-live-link");
            saveButton = rootVisualElement.Q<ToolbarButton>("save-button");
            autoSaveToggle = rootVisualElement.Q<ToolbarToggle>("auto-save-toggle");
            simulatingBorder = rootVisualElement.Q<VisualElement>("simulating-border");

            graph.FrameAll();
            graph.SetWindow(this);

            graph.UpdateSelection += OnNodeSelectionChanged;
            graph.AssetChanged += MarkAsChanged;

            nestedtreesButton.style.backgroundImage = EditorResources.Load<Texture2D>("Images/Icons/Other/BackParent.png");
#if UNITY_2022_3_OR_NEWER
            nestedtreesButton.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(BackgroundSizeType.Contain));
#else
            nestedtreesButton.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(ScaleMode.ScaleToFit);
#endif
            nestedtreesButton.style.width = 25;
            nestedtreesButton.RegisterCallback<MouseDownEvent>(OnSubTreeMenu, TrickleDown.TrickleDown);
            nestedtreesButton.visible = false;

            autoLiveLink = true;
            autoLiveLinkToggle.value = autoLiveLink;
            autoLiveLinkToggle.RegisterValueChangedCallback(OnAutoLiveLinkChanged);

            toolbarAssets.RegisterCallback<MouseDownEvent>(OnFillAssetToolbar, TrickleDown.TrickleDown);
            autoSaveToggle.RegisterValueChangedCallback(OnAutoSave);

            saveButton.SetEnabled(false);
            saveButton.clicked += SaveChanges;

            nodeDescription.visible = false;
            nodeDescription.text = string.Empty;
        }

        /// <summary>
        /// Check if window has new or unloaded visual elements.
        /// </summary>
        /// <returns>True if has new or unloaded visual elements, otherwise false.</returns>
        private bool HasUnloadedVisualElements()
        {
            return graph == null
                || treeName == null
                || toolbarAssets == null
                || saveButton == null
                || autoSaveToggle == null
                || simulatingBorder == null;
        }

        /// <summary>
        /// Update behaviour tree title label.
        /// </summary>
        private void UpdateTitleLabel()
        {
            if (treeName != null)
            {
                switch (settings.GetTreeNameMode())
                {
                    case AITreeSettings.TreeNameMode.Normal:
                        treeName.style.fontSize = 50;
                        treeName.visible = true;
                        break;
                    case AITreeSettings.TreeNameMode.Small:
                        treeName.style.fontSize = 25;
                        treeName.visible = true;
                        break;
                    case AITreeSettings.TreeNameMode.Disable:
                        treeName.style.fontSize = 50;
                        treeName.visible = false;
                        break;
                }
            }
        }

#if UNITY_2022_3_OR_NEWER
        /// <summary>
        /// Show a message if need to change the API.
        /// </summary>
        private static void ShowHotKeyNotification()
        {
            const string KEY = "RenownedGames.AITreeEditor.BehaviourTreeWindow.HotKeyNotification";
            if (!EditorPrefs.GetBool(KEY, false))
            {
                AITreeSettings settings = AITreeSettings.instance;
                if (!settings.HotKeyListener())
                {
                    bool change = EditorUtility.DisplayDialog("AI Tree", $"You are using the new version of Unity {Application.unityVersion}, we recommend switching to the new hotkey API, as some users have encountered hotkey problems in graph using the classic Unity API.\n\nYou can do this later in the settings.\n\nEdit/Preferences/AI Tree/Hot Key Listener", "Change", "Skip");
                    if (change)
                    {
                        settings.HotKeyListener(true);
                        settings.Save();

                        if (settings.HotKeyListener())
                        {
                            EditorUtility.DisplayDialog("AI Tree", "Settings successfully changed!", "Continue");
                        }
                    }
                    EditorPrefs.SetBool(KEY, true);
                }
            }
        }
#endif

        #region [Callbacks]
        /// <summary>
        /// Called when the play mode changes.
        /// </summary>
        private void OnPlayMoveStateChanged(PlayModeStateChange state)
        {
            //BehaviourRunner runner = EditorUtility.InstanceIDToObject(lastRunnerID) as BehaviourRunner;
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    TrackEditor(rootTarget != null ? rootTarget : GetTarget());
                    ResetNestedMode();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    TrackEditor(rootTarget != null ? rootTarget : GetTarget());
                    ResetNestedMode();
                    break;

            }
        }

        /// <summary>
        /// Called when the selected behavior tree is available to work with them.
        /// </summary>
        private void OnBecameNativeAsset(BehaviourTree behaviourTree)
        {
            behaviourTree.BecameNativeAsset -= OnBecameNativeAsset;
            TrackEditor(behaviourTree);
        }

        /// <summary>
        /// Called when a node is selected.
        /// </summary>
        private void OnNodeSelectionChanged(Node node)
        {
            nodeDescription.text = string.Empty;
            nodeDescription.visible = false;
            if (node != null)
            {
                if (node is RootNode)
                {
                    NotifyTrackEditor(GetTarget());
                    if (!HasOpenTrackers<NodeInspectorWindow>())
                    {
                        NodeInspectorWindow nodeInspector = GetOrCreateTracker<NodeInspectorWindow>();
                        nodeInspector.TrackEditor(GetTarget());
                    }
                }
                else
                {
                    NotifyTrackEditor(node);
                    if (!HasOpenTrackers<NodeInspectorWindow>())
                    {
                        NodeInspectorWindow nodeInspector = GetOrCreateTracker<NodeInspectorWindow>();
                        nodeInspector.TrackEditor(node);
                    }

                    if ((AITreeSettings.instance.GetNodeTooltipMode() & AITreeSettings.NodeTooltipMode.GraphOverlay) != 0)
                    {
                        if (NodeTypeCache.TryGetNodeInfo(node.GetType(), out NodeTypeCache.NodeInfo nodeInfo))
                        {
                            if (nodeInfo.tooltipAttribute != null)
                            {
                                nodeDescription.visible = true;
                                nodeDescription.text = nodeInfo.tooltipAttribute.text;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when clicking on sub tree back parent button.
        /// </summary>
        private void OnSubTreeMenu(MouseDownEvent evt)
        {
            if (evt.button == 0)
            {
                SubTreeOut();
            }
            else if (evt.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                for (int i = nestedTrees.Count - 1; i >= 0; i--)
                {
                    BehaviourTree tree = nestedTrees[i];
                    if (tree != null)
                    {
                        menu.AddItem(new GUIContent(tree.name), false, () => SubTreeOut(tree));
                    }
                    else
                    {
                        nestedTrees.RemoveAt(i);
                        i++;
                    }
                }

                Rect rect = nestedtreesButton.layout;
                rect.y += rect.height;
                menu.DropDown(rect);
            }
        }

        /// <summary>
        /// Called when clicking on Auto Live Link toggle button.
        /// </summary>
        /// <param name="evt"></param>
        private void OnAutoLiveLinkChanged(ChangeEvent<bool> evt)
        {
            autoLiveLink = evt.newValue;
        }

        /// <summary>
        /// Called after clicking on assets button to fill in the assets toolbar dropdown menu.
        /// </summary>
        private void OnFillAssetToolbar(MouseDownEvent evt)
        {
            // Clear toolbar
            toolbarAssets.menu.MenuItems().Clear();

            // Create new behaviour tree
            toolbarAssets.menu.AppendAction("Create new Behaviour Tree", a =>
            {
                BehaviourTree behaviourTree = BehaviourTree.Create("Behaviour Tree");

                Selection.activeObject = behaviourTree;
                EditorGUIUtility.PingObject(behaviourTree);
            });

            // Link to created behaviour trees
            HashSet<string> names = new HashSet<string>();
            string[] guids = AssetDatabase.FindAssets("t:BehaviourTree");
            for (int i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BehaviourTree behaviourTree = AssetDatabase.LoadAssetAtPath<BehaviourTree>(path);
                if (behaviourTree != null)
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    name = ObjectNames.NicifyVariableName(name);
                    if (!names.Add(name))
                    {
                        name += $" ({guid.Substring(0, Mathf.Min(8, guid.Length))}...)";
                    }

                    bool isSelected = TryGetBehaviourTree(out BehaviourTree tree) && tree == behaviourTree;
                    toolbarAssets.menu.AppendAction(name, a => NotifyTrackEditor(behaviourTree), isSelected ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
                }
            }
        }

        /// <summary>
        /// Called when auto save toggle changed.
        /// </summary>
        /// <param name="evt"></param>
        private void OnAutoSave(ChangeEvent<bool> evt)
        {
            saveButton.style.display = !evt.newValue ? DisplayStyle.None : DisplayStyle.Flex;
            if (evt.newValue && hasChanges)
            {
                SaveChanges();
            }
        }

        /// <summary>
        /// Called when AITreeSettings.Save() method performed.
        /// <br>It does not guarantee that the settings have been changed.</br>
        /// </summary>
        private void OnSettingsSaved(AITreeSettings settings)
        {
            UpdateTitleLabel();
        }
        #endregion

        #region [Static Methods]
        [MenuItem("Tools/AI Tree/Windows/Behaviour Tree", false, 20)]
        public static void Open()
        {
            Open<BehaviourTreeWindow>();
        }

        /// <summary>
        /// Allows you to open the behavior the editor by opening the behavior tree asset.
        /// </summary>
        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceId, int line)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is BehaviourTree)
            {
                BehaviourTreeWindow window = GetOrCreateTracker<BehaviourTreeWindow>();
                window.TrackEditor(asset);
                return true;
            }
            return false;
        }
        #endregion

        #region [Getter / Setter]
        /// <summary>
        /// Graph instance of this window.
        /// </summary>
        public BehaviourTreeGraph GetGraph()
        {
            return graph;
        }

        /// <summary>
        /// Behaviour tree label.
        /// </summary>
        public Label GetTreeName()
        {
            return treeName;
        }

        /// <summary>
        /// Check if Auto Live Link mode is enabled.
        /// </summary>
        internal bool AutoLiveLink()
        {
            return autoLiveLink;
        }
        #endregion
    }
}