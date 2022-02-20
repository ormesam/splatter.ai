# Splatter.AI
Splatter.AI is a code based behaviour tree for Unity projects big and small.

## Installation
To add to your Unity project go to the Package Manager, click the plus in the top left of the window. Select git URL and enter: `https://github.com/ormesam/splatter.ai.git?path=/src/Assets/Splatter.AI`

## Usage

1. Create new class deriving from BehaviourTree
2. Override `Awake` method (optional) and initiate blackboard values, make sure to call `base.Awake();` at the start of the method
3. Override `SetRoot` method, here you can build up your behaviour tree using the `BehaviourTreeBuilder` class as shown below
4. Attach the script to the GameObject
5. The tree will be executed every frame

## Example

```c#
using Splatter.AI;

public class ZombieBehaviourTree : BehaviourTree {
    public override void Awake() {
        base.Awake();
        
        Blackboard[ZombieKey] = GetComponent<Zombie>();
    }
    
    public override Node SetRoot() {
        return new BehaviourTreeBuilder(this)
            .Sequence("root", resetIfInterrupted: false)
            	.Do("custom action", () => {
                    return NodeResult.Success;
                })
            .End()
            .Build();
    }
}
```
