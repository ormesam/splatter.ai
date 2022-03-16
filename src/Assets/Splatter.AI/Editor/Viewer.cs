#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Splatter.AI.Editor {
    public partial class Viewer : EditorWindow {
        private Graph graph;

        [MenuItem("Window/Splatter/Behaviour Tree Viewer")]
        public static void ShowEditor() {
            var window = GetWindow<Viewer>();
            window.titleContent = new GUIContent("Behaviour Tree Viewer");
        }

        private void CreateGUI() {
            AddGraph();
            AddStyles();

            OnSelectionChange();
        }

        private void AddStyles() {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("BehaviourTreeStyle"));
        }

        private void AddGraph() {
            graph = new Graph();
            graph.StretchToParentSize();

            rootVisualElement.Add(graph);
        }

        private void OnSelectionChange() {
            if (!Application.isPlaying) {
                return;
            }

            var tree = GetTree();

            if (!tree) {
                return;
            }

            graph.Draw(tree);
        }

        private BehaviourTree GetTree() {
            if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent(out BehaviourTree treeComponent)) {
                return treeComponent;
            }

            return null;
        }

        private void OnInspectorUpdate() {
            if (!Application.isPlaying) {
                return;
            }

            graph?.UpdateNodeStates();
        }
    }
}
#endif