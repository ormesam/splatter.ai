# Sequencer

## What is a Sequencer?

The **Sequencer** is a composite node. It will execute each child in sequence, returning failure if a child fails, and returning success if every child returned a successful status.

## Example

Simple example of a patrolling NPC sequence

```c#
.Sequence("Sequence")
    .Do("Select random waypoint", () => {
        ...

        return NodeResult.Success;
    })
    .Do("Go to waypoint", () => {
        ...

        return arrived ? NodeResult.Success : NodeResult.Running;
    })
    .Wait("Random pause", 1, 5)
.End()
```

## Optional Parameters

**resetIfInterrupted** - boolean indicating if the sequence should start again if it gets interrupted.

**abortType** - See Composites

**condition** - see Composites