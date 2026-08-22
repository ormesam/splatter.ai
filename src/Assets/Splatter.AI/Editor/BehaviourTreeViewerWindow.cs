using System.Collections.Generic;
using System.Linq;
using Splatter.AI.Unity;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Splatter.AI.Editor {
    /// <summary>
    /// Live viewer for running behaviour trees: a read-only node graph with running highlights
    /// and stop-reason fades, a tree picker fed by <see cref="BehaviourTreeManager.Trees"/>, and
    /// a blackboard side panel. Runtime-only — trees are built in code, so outside Play Mode the
    /// window shows an empty state.
    /// </summary>
    public class BehaviourTreeViewerWindow : EditorWindow {
        private BehaviourTreeCanvas graphView;
        private BlackboardPanel blackboardPanel;
        private ToolbarMenu treeMenu;
        private ToolbarToggle lockToggle;
        private Label emptyState;

        // Runtime objects, intentionally not serialized: they cannot survive play-mode exit.
        private BehaviourTree selectedTree;
        private readonly List<BehaviourTree> menuTrees = new List<BehaviourTree>();

        [MenuItem("Window/Splatter/Behaviour Tree Viewer")]
        public static void ShowWindow() {
            var window = GetWindow<BehaviourTreeViewerWindow>();
            window.titleContent = new GUIContent("Behaviour Tree Viewer");
        }

        private void OnEnable() {
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable() {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void CreateGUI() {
            var stylesheet = Resources.Load<StyleSheet>("BehaviourTreeStyle");

            if (stylesheet != null) {
                rootVisualElement.styleSheets.Add(stylesheet);
            }

            var toolbar = new Toolbar();

            treeMenu = new ToolbarMenu { text = "No tree" };
            toolbar.Add(treeMenu);

            lockToggle = new ToolbarToggle { text = "Lock" };
            lockToggle.tooltip = "Keep showing this tree instead of following the selected GameObject.";
            toolbar.Add(lockToggle);

            toolbar.Add(new ToolbarButton(() => graphView.FrameAll()) { text = "Frame" });

            rootVisualElement.Add(toolbar);

            var split = new TwoPaneSplitView(1, 280, TwoPaneSplitViewOrientation.Horizontal);
            split.style.flexGrow = 1;

            var graphContainer = new VisualElement();
            graphContainer.style.flexGrow = 1;

            graphView = new BehaviourTreeCanvas();
            graphView.StretchToParentSize();
            graphContainer.Add(graphView);

            emptyState = new Label();
            emptyState.AddToClassList("empty-state");
            graphContainer.Add(emptyState);

            split.Add(graphContainer);

            blackboardPanel = new BlackboardPanel();
            split.Add(blackboardPanel);

            rootVisualElement.Add(split);
        }

        private void OnEditorUpdate() {
            if (graphView == null) {
                return;
            }

            if (!Application.isPlaying) {
                SelectTree(null);
                ShowEmptyState("Enter Play Mode to inspect running behaviour trees.");

                return;
            }

            UpdateMenu();
            FollowSelection();

            if (selectedTree == null) {
                SelectTree(BehaviourTreeManager.Trees.FirstOrDefault(tree => tree.Root != null));
            }

            if (selectedTree == null) {
                ShowEmptyState("No behaviour trees registered.");

                return;
            }

            emptyState.style.display = DisplayStyle.None;

            // An unregistered tree stays inspectable: disabling a runner pauses in place, and
            // frozen state is exactly what the user wants to look at.
            bool registered = BehaviourTreeManager.Trees.Contains(selectedTree);
            treeMenu.text = GetDisplayName(selectedTree) + (registered ? string.Empty : " (paused)");

            if (!graphView.MatchesStructure(selectedTree)) {
                graphView.SetTree(selectedTree);
                blackboardPanel.SetTree(selectedTree);
            }

            graphView.UpdateNodeStates(EditorApplication.timeSinceStartup);
            blackboardPanel.UpdateValues();
        }

        private void OnPlayModeChanged(PlayModeStateChange change) {
            // Mandatory with domain reload disabled: window state survives play-mode exit and
            // would otherwise dangle stale node references.
            if (change == PlayModeStateChange.ExitingPlayMode) {
                SelectTree(null);
                menuTrees.Clear();
                treeMenu?.menu.MenuItems().Clear();
            }
        }

        private void FollowSelection() {
            if (lockToggle.value) {
                return;
            }

            var selection = Selection.activeGameObject;

            if (selection != null && selection.TryGetComponent<BehaviourTreeRunner>(out var runner)
                && runner.Tree?.Root != null) {

                SelectTree(runner.Tree);
            }
        }

        private void SelectTree(BehaviourTree tree) {
            if (selectedTree == tree) {
                return;
            }

            selectedTree = tree;
            graphView.SetTree(tree);
            blackboardPanel.SetTree(tree);
            treeMenu.text = tree != null ? GetDisplayName(tree) : "No tree";
        }

        private void UpdateMenu() {
            var trees = BehaviourTreeManager.Trees.ToList();

            if (trees.SequenceEqual(menuTrees)) {
                return;
            }

            menuTrees.Clear();
            menuTrees.AddRange(trees);

            treeMenu.menu.MenuItems().Clear();

            var nameCounts = new Dictionary<string, int>();

            foreach (var tree in trees) {
                string name = GetDisplayName(tree);

                // Disambiguate duplicate names by registration order: "Enemy", "Enemy (2)", ...
                if (nameCounts.TryGetValue(name, out int count)) {
                    nameCounts[name] = count + 1;
                    name = $"{name} ({count + 1})";
                } else {
                    nameCounts[name] = 1;
                }

                var captured = tree;
                treeMenu.menu.AppendAction(name, _ => SelectTree(captured),
                    _ => captured == selectedTree
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal);
            }
        }

        private void ShowEmptyState(string message) {
            emptyState.text = message;
            emptyState.style.display = DisplayStyle.Flex;
        }

        private static string GetDisplayName(BehaviourTree tree) {
            if (!string.IsNullOrEmpty(tree.Name)) {
                return tree.Name;
            }

            return string.IsNullOrEmpty(tree.Root?.Name) ? "Tree" : tree.Root.Name;
        }
    }
}
