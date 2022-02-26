using UnityEngine;
using UnityEngine.AI;

namespace Splatter.AI.Examples {
    public class PatrollingBehaviourTree : BehaviourTree {
        public GameObject[] Waypoints;
        public GameObject Player;
        public float MaxDistance;

        protected override void Awake() {
            base.Awake();

            // Cache values
            Blackboard.Add("CurrentWaypointIdx", 0);
        }

        protected override Node CreateRoot() {
            return new BehaviourTreeBuilder(this)
                .AlwaysRunning("Root")
                    .Selector("")
                        .Sequence("Player detection", resetIfInterrupted: true, abortType: AbortType.SelfAndLower, () => CanSeePlayer())
                            .Do("Go to player", () => {
                                var navMeshAgent = GetComponent<NavMeshAgent>();
                                navMeshAgent.SetDestination(Player.transform.position);

                                return NodeResult.Running;
                            })
                        // Other actions...
                        .End()
                        .Sequence("Patrol")
                            .Do("Set next waypoint", () => {
                                int currentWaypointIdx = GetItem<int>("CurrentWaypointIdx");
                                var navMeshAgent = GetComponent<NavMeshAgent>();

                                currentWaypointIdx++;

                                if (currentWaypointIdx >= Waypoints.Length) {
                                    currentWaypointIdx = 0;
                                }

                                Blackboard["CurrentWaypointIdx"] = currentWaypointIdx;

                                navMeshAgent.SetDestination(Waypoints[currentWaypointIdx].transform.position);

                                return NodeResult.Success;
                            })
                            .WaitUntil("Move to waypoint", () => {
                                var navMeshAgent = GetComponent<NavMeshAgent>();

                                return Vector3.Distance(navMeshAgent.destination, transform.position) <= navMeshAgent.stoppingDistance;
                            })
                            .Wait("Pause", 1, 3)
                        .End()
                    .End()
                .End()
                .Build();
        }

        private bool CanSeePlayer() {
            var rayDirection = Player.transform.position - transform.position;

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit)) {
                return hit.transform == Player.transform && hit.distance <= MaxDistance;
            }

            return false;
        }
    }
}
