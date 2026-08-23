# Splatter.AI
Splatter.AI is a code based behaviour tree for Unity projects. Requires Unity 6 (`6000.0` or newer).

## Installation
To add to your Unity project go to the Package Manager, click the plus in the top left of the window. Select git URL and enter: `https://github.com/ormesam/splatter.ai.git?path=/src/Assets/Splatter.AI`

The package itself has no dependencies. The samples additionally need `com.unity.ai.navigation`.

## Quick Start

1. Create new class deriving from `BehaviourTreeRunner`, which is a `MonoBehaviour`
2. Override `CreateRoot` method, here you can build up your behaviour tree as shown below - composites take a collection initialiser, decorators take a `Child`
3. Override `Awake` method (optional) and initiate blackboard values, make sure to call `base.Awake();` at the start of the method as that is what creates `Tree`
4. Attach the script to the GameObject, and set **Tick Interval** in the inspector if the tree does not need to run every frame
5. On `Start` the tree registers itself with `BehaviourTreeManager` and is ticked centrally from then on. Disabling the component pauses the tree in place; re-enabling resumes it

See [REFERENCE.md](REFERENCE.md) for the node reference, reactivity and abort modes, ticking, the behaviour tree viewer, and tests.

## Example

An enemy that wanders until it sees the player, chases while it can see them, and returns to wandering when it loses them.

```c#
using Splatter.AI;
using Splatter.AI.Composites;
using Splatter.AI.Decorators;
using Splatter.AI.Leaves;
using Splatter.AI.Unity;
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
        return new Repeater {
            Child = new Selector {
                new BlackboardObserverDecorator(Tree.Blackboard, PlayerVisibleKey, true, AbortMode.Both) {
                    Child = new Leaf("Chase player", ChasePlayer),
                },
                new Sequencer("Wander") {
                    new Leaf("Pick a spot", PickSpot),
                    new WaitNode("Pause", 1f, 3f),
                },
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
## Patrol / Chase Demo
Found under Samples, in `Samples/Scenes/PatrolDemo.unity`. `Samples/Scripts/PatrolChaseEnemy.cs` is the tree.

https://user-images.githubusercontent.com/8319419/155850237-8c4a9a6e-f704-4711-b386-8e6ffe98d848.mp4
