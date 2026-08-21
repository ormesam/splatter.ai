# Splatter.AI

Splatter.AI is a code-based behaviour tree for Unity. Trees are written in C# rather than assembled in a graph editor, and the engine core has no engine references at all, so node logic is plain C# and unit testable on its own.

Requires **Unity 6** (`6000.0` or newer). This is an early preview — expect breaking changes between versions.

See **[REFERENCE.md](REFERENCE.md)** for the node reference, reactivity and abort modes, ticking, the behaviour tree viewer, and tests.

## Installation

To add to your Unity project go to the Package Manager, click the plus in the top left of the window. Select git URL and enter:

```
https://github.com/ormesam/splatter.ai.git?path=/src/Assets/Splatter.AI
```

The package itself has no dependencies. The samples can be imported from the package's Samples section in the Package Manager; they additionally need `com.unity.ai.navigation` for the NavMesh.

## Quick Start

1. Create a class deriving from `BehaviourTreeRunner`, which is a `MonoBehaviour`.
2. Override `CreateRoot()` and build the tree there. There is no builder API — nodes are composed with ordinary C# initialisers: composites take a collection initialiser, decorators take a `Child`.
3. Optionally override `Awake()` to seed the blackboard. It is `protected`, and `base.Awake()` is what creates `Tree`, so call it first.
4. Attach the script to a GameObject. Set **Tick Interval** in the inspector if the tree does not need to run every frame.
5. On `Start` the tree registers itself with `BehaviourTreeManager` and is [ticked centrally](REFERENCE.md#ticking) from then on. Disabling the component pauses the tree in place; re-enabling resumes it.

To give a tree a strongly typed context instead of, or alongside, the blackboard, override `CreateTree()` and return a `ContextBehaviourTree<T>`.

## Example

An enemy that wanders until it sees the player, chases while it can see them, and returns to wandering when it loses them.

```c#
using Splatter.AI;
using UnityEngine;

public class Zombie : BehaviourTreeRunner {
    private const string PlayerVisibleKey = "PlayerVisible";

    public GameObject Player;
    public float SightRange = 10f;

    private void Update() {
        // Sensors can write every frame; observers are only notified when the value changes.
        Tree.Blackboard[PlayerVisibleKey] = CanSeePlayer();
    }

    protected override Node CreateRoot() {
        var chase = new BlackboardObserverDecorator(Tree.Blackboard, PlayerVisibleKey, true, AbortMode.Both) {
            Child = new Leaf("Chase player", ChasePlayer),
        };

        var wander = new Sequencer("Wander") {
            new Leaf("Pick a spot", PickSpot),
            new WaitNode("Pause", 1f, 3f),
        };

        return new Repeater {
            Child = new Selector {
                chase,
                wander,
            },
        };
    }

    private NodeResult ChasePlayer() {
        // Move towards the player; never finishes on its own.
        return NodeResult.Running;
    }

    private NodeResult PickSpot() {
        return NodeResult.Success;
    }

    private bool CanSeePlayer() {
        return Vector3.Distance(transform.position, Player.transform.position) < SightRange;
    }
}
```

Because the player-visible check is an observer with `AbortMode.Both`, spotting the player aborts the wander mid-step to start chasing, and losing them stops the chase and resumes wandering — without either branch polling for the change. See [Reactivity](REFERENCE.md#reactivity) for the other abort modes.

## Samples

A patrol / chase demo lives in `Samples/Scenes/PatrolDemo.unity`. Open it and press play — the enemy walks between waypoints, pausing at each one, and abandons the patrol to chase the player when it gets line of sight. `Samples/Scripts/PatrolChaseEnemy.cs` is the tree, and is a good companion to [Reactivity](REFERENCE.md#reactivity). The scene needs `com.unity.ai.navigation`.

https://user-images.githubusercontent.com/8319419/155850237-8c4a9a6e-f704-4711-b386-8e6ffe98d848.mp4
