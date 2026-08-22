using System.Collections.Generic;
using System.Linq;
using Splatter.AI.Leaves;
using UnityEngine.UIElements;
using BTNode = Splatter.AI.Node;

namespace Splatter.AI.Editor {
    /// <summary>
    /// Live view of a tree's blackboard, plus one section per subtree blackboard — subtrees keep
    /// their own scoped blackboard, and the panel surfaces that directly. Values are polled each
    /// editor update: <see cref="Blackboard.Subscribe"/> is per-key, so key additions are
    /// unobservable without polling anyway, and polling keeps editor delegates out of runtime
    /// object lifetimes.
    /// </summary>
    public class BlackboardPanel : ScrollView {
        private class Section {
            public Blackboard Blackboard;
            public Foldout Foldout;
            public readonly List<string> Keys = new List<string>();
            public readonly List<Label> ValueLabels = new List<Label>();
        }

        private readonly List<Section> sections = new List<Section>();

        public BlackboardPanel() {
            AddToClassList("blackboard-panel");
        }

        /// <summary>
        /// Rebuilds the panel's sections for the given tree. Pass null to clear.
        /// </summary>
        public void SetTree(BehaviourTree tree) {
            Clear();
            sections.Clear();

            if (tree == null) {
                return;
            }

            AddSection("Blackboard", tree.Blackboard);

            if (tree.Root == null) {
                return;
            }

            var seenNodes = new HashSet<BTNode>();
            var seenBlackboards = new HashSet<Blackboard> { tree.Blackboard };
            CollectSubtrees(tree.Root, seenNodes, seenBlackboards);
        }

        /// <summary>
        /// Refreshes every section's rows from its blackboard. Rows are rebuilt only when a
        /// section's key set changed; otherwise just the value text is updated.
        /// </summary>
        public void UpdateValues() {
            foreach (var section in sections) {
                var keys = section.Blackboard.Keys.OrderBy(key => key).ToList();

                if (!keys.SequenceEqual(section.Keys)) {
                    RebuildRows(section, keys);
                }

                for (int i = 0; i < section.Keys.Count; i++) {
                    section.Blackboard.TryGetValue(section.Keys[i], out var value);

                    var label = section.ValueLabels[i];
                    string text = FormatValue(value);

                    if (label.text != text) {
                        label.text = text;
                        label.tooltip = value?.GetType().Name ?? "null";
                    }
                }
            }
        }

        private void CollectSubtrees(BTNode node, HashSet<BTNode> seenNodes, HashSet<Blackboard> seenBlackboards) {
            if (!seenNodes.Add(node)) {
                return;
            }

            if (node is SubtreeNode subtree && subtree.Tree != null
                && seenBlackboards.Add(subtree.Tree.Blackboard)) {

                AddSection($"{subtree.Name} — Blackboard", subtree.Tree.Blackboard);
            }

            foreach (var child in BehaviourTree.GetChildren(node)) {
                CollectSubtrees(child, seenNodes, seenBlackboards);
            }
        }

        private void AddSection(string label, Blackboard blackboard) {
            var section = new Section {
                Blackboard = blackboard,
                Foldout = new Foldout { text = label, value = true },
            };

            sections.Add(section);
            Add(section.Foldout);
        }

        private static void RebuildRows(Section section, List<string> keys) {
            section.Foldout.Clear();
            section.Keys.Clear();
            section.ValueLabels.Clear();

            foreach (var key in keys) {
                var row = new VisualElement();
                row.AddToClassList("bb-row");

                var keyLabel = new Label(key);
                keyLabel.AddToClassList("bb-key");
                row.Add(keyLabel);

                var valueLabel = new Label();
                valueLabel.AddToClassList("bb-value");
                row.Add(valueLabel);

                section.Foldout.Add(row);
                section.Keys.Add(key);
                section.ValueLabels.Add(valueLabel);
            }
        }

        private static string FormatValue(object value) {
            if (value == null) {
                return "null";
            }

            if (value is UnityEngine.Object unityObject) {
                return unityObject != null ? unityObject.name : "<destroyed>";
            }

            return value.ToString();
        }
    }
}
