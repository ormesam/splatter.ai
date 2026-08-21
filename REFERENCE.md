# Splatter.AI Reference

Reference for Splatter.AI. See the [README](README.md) for installation, a quick start, and a worked example.

- [Ticking](#ticking)
- [Reactivity](#reactivity)
- [Nodes](#nodes)
- [Behaviour Tree Viewer](#behaviour-tree-viewer)
- [Tests](#tests)

## Ticking

Trees are ticked by a central manager (`BehaviourTreeManager`) rather than per-component `Update` calls. Each `BehaviourTreeRunner` has a `Tick Interval` field: `1` (default) ticks every frame, while a higher value ticks the tree every that-many frames. Trees sharing an interval are automatically staggered so they don't all tick on the same frame, which makes large numbers of agents cheap to run.

## Reactivity

Conditions can be polled or observed.

`Blackboard` notifies per-key subscribers, and only when a value actually changes. Sensor code can therefore assign every frame, as in the [README example](README.md#example), without waking anything until the value is genuinely different.

`BlackboardObserverDecorator` gates its child on a blackboard entry and re-checks it only on those change notifications. What a change then does depends on its `AbortMode`:

| Mode | Effect |
| --- | --- |
| `None` | Never aborts. The condition gates entry only, evaluated when the decorator starts. |
| `Self` | While the child is running, a change that falsifies the condition stops the child and returns `Failure`. |
| `LowerPriority` | While the branch is not running, a change that would flip its last result makes the parent composite stop lower-priority children and re-evaluate from this branch. |
| `Both` | Both of the above. |

Two rules are worth knowing:

- **Aborts are applied on the next tick, never at notification time.** A value that changes mid-tick, or that changes and reverts before the next tick, can never tear down a node that is currently executing.
- **`LowerPriority` observers must be a direct child of a `Selector` or `Sequencer`**, since it is that composite's memory they interrupt. The tree validates this on its first tick and throws an explanatory exception otherwise.

For polled conditions instead, use `GuardDecorator`, which re-evaluates every update, or `ReactiveSelector` / `ReactiveSequencer`, which drop the memory of their ordinary counterparts and re-tick from the first child every update.

## Nodes

### Composites

| Node | Behaviour |
| --- | --- |
| `Selector` | Runs children in order until one succeeds, failing if none do. Has memory: resumes at the running child, so a chosen branch runs to completion. |
| `Sequencer` | Runs children in order until one fails, succeeding if all do. Has memory: completed steps run exactly once per pass. |
| `ReactiveSelector` | Selector without memory. Re-ticks from the first child every update, so a higher-priority child becoming available interrupts a later running one. |
| `ReactiveSequencer` | Sequence without memory. An earlier child changing its result interrupts a later running one. |
| `RandomSelector` | `Selector` that tries its children in a random order, reshuffled each time it starts. |
| `RandomSequencer` | `Sequencer` that runs its children in a random order, reshuffled each time it starts. |
| `WeightedRandomSelector` | Selector with a weighted order — the higher the weight, the earlier a child is likely to be tried. Use `Add(node, weight)`; unweighted children default to `1`. |
| `Parallel` | Ticks all children each update until its `ParallelMode` condition is met. Completed children are not re-ticked, and any still running when the node completes are stopped. |

`ParallelMode` values:

| Mode | Completes when |
| --- | --- |
| `ExitOnAnySuccess` | A child succeeds, or all children complete without succeeding. |
| `ExitOnAnyFailure` | A child fails, or all children complete without failing. |
| `ExitOnAnyCompletion` | Any child returns `Success` or `Failure`, whose result is passed through. |
| `WaitForAllToComplete` | Every child has finished, whatever their results. |
| `WaitForAllToSucceed` | All children have succeeded, or as soon as one fails. |

### Decorators

| Node | Behaviour |
| --- | --- |
| `Repeater` | Restarts the child each time it completes, whatever the result. Succeeds after a given number of repeats, or repeats forever if no count is given. |
| `RepeatUntilFailure` | Restarts the child each time it succeeds, succeeding once the child fails. |
| `RetryUntilSuccess` | Restarts the child each time it fails, succeeding once the child succeeds. Fails after a maximum number of attempts, or retries forever if none is given. |
| `InvertDecorator` | Swaps the child's `Success` and `Failure`, passing `Running` through. |
| `SuccessDecorator` | Returns `Success` when the child completes, whatever the result. |
| `FailureDecorator` | Returns `Failure` when the child completes, whatever the result. |
| `RunningDecorator` | Runs the child to completion once, then returns `Running` indefinitely until the decorator is stopped. |
| `GuardDecorator` | Runs the child only while a condition holds, re-evaluated every update. If it becomes false mid-run the child is stopped and `Failure` returned. |
| `CooldownDecorator` | Passes the child's result through, then locks it out for a number of seconds, returning `Failure` while cooling down. A running child is never interrupted. |
| `TimeLimitDecorator` | Passes the child's result through, but stops it and fails if it runs longer than a number of seconds, measured from each activation. |
| `BlackboardObserverDecorator` | Event-driven guard on a blackboard entry, re-checked only when the value changes and aborting per its `AbortMode`. See [Reactivity](#reactivity). |

### Leaves

| Node | Behaviour |
| --- | --- |
| `Leaf` | Wraps a `Func<NodeResult>`, instead of deriving a node type. |
| `ConditionNode` | Evaluates a condition and immediately returns `Success` or `Failure`. |
| `WaitUntilNode` | Returns `Running` until a condition becomes true. |
| `WaitNode` | Waits a random duration between a minimum and maximum number of seconds, then succeeds. |
| `BlackboardConditionNode` | Succeeds if a blackboard key is set and, when given, equal to an expected value. |
| `SetBlackboardValueNode` | Sets a blackboard value. |
| `ChanceNode` | Succeeds with a given probability, re-rolled each update. |
| `IdleNode` | Runs forever, never completing on its own. A "do nothing" branch that holds until interrupted. |
| `SubtreeNode` | Ticks another `BehaviourTree`, for composing and reusing whole trees. The subtree keeps its own blackboard, scoped separately from its parent's. |

## Behaviour Tree Viewer

Open **Window > Splatter > Behaviour Tree Viewer** to watch a tree run.

Trees are built in code at `Start`, so the viewer is play-mode only. It follows the selected GameObject's `BehaviourTreeRunner`, or you can pick a tree from the toolbar dropdown; **Lock** pins the current tree so it stops following the selection, and **Frame** (or the **F** key) re-centres the graph. Drag anywhere with the left or middle mouse button to pan, and scroll to zoom on the cursor. The right-hand pane lists the tree's blackboard, plus a foldout per `SubtreeNode` for its scoped blackboard.

Nodes are coloured by what they are doing:

| Colour | Meaning |
| --- | --- |
| Amber | Currently running — the edges along the active branch tint amber too |
| Green | Stopped with `Success` |
| Red | Stopped with `Failure` |
| Violet | Aborted |

Completed states fade out over a couple of seconds, so a branch that succeeds and immediately restarts still registers visually. The graph is read-only — it can be panned, zoomed and framed, but not rearranged. A node instance shared between branches is drawn once, with an edge from each parent. Trees ticked by hand rather than registered with `BehaviourTreeManager` do not appear in the dropdown.

## Tests

The node logic is covered by an EditMode NUnit suite under `Tests/`, runnable from **Window > General > Test Runner**, or headless:

```
Unity.exe -batchmode -projectPath src -runTests -testPlatform EditMode -testResults results.xml
```

The tests reference only the core assembly, which has no engine references. That is why the time-dependent decorators take an injected clock (`Func<float>`) rather than reading `Time.time` themselves — `WaitNode`, which does, is the one node that lives in the Unity assembly.
