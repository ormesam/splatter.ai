# Splatter.AI
Splatter.AI is a code based behaviour tree for Unity projects. Current version v0.0.4 can be found in the releases, note there may be breaking changes while in early preview.

## Installation
To add to your Unity project go to the Package Manager, click the plus in the top left of the window. Select git URL and enter: `https://github.com/ormesam/splatter.ai.git?path=/src/Assets/Splatter.AI`

## Quick Start [More Docs...](https://github.com/ormesam/splatter.ai/wiki)

1. Create new class deriving from BehaviourTree
2. Override `Awake` method (optional) and initiate blackboard values, make sure to call `base.Awake();` at the start of the method
3. Override `CreateRoot` method, here you can build up your behaviour tree using the `BehaviourTreeBuilder` class as shown below
4. Attach the script to the GameObject
5. The tree will be executed every frame

View the [wiki](https://github.com/ormesam/splatter.ai/wiki) for more documentation.

## Example

```c#
using Splatter.AI;

public class ZombieBehaviourTree : BehaviourTree {
    public override void Awake() {
        base.Awake();
        
        Blackboard[ZombieKey] = GetComponent<Zombie>();
    }
    
    protected override Node CreateRoot() {
        return new BehaviourTreeBuilder(this)
            .Sequence()
                .Name("root")
            	.Do("custom action", () => {
                    return NodeResult.Success;
                })
            .End()
            .Build();
    }
}
```
## Patrol / Chase Demo
Found under Samples

https://user-images.githubusercontent.com/8319419/155850237-8c4a9a6e-f704-4711-b386-8e6ffe98d848.mp4


