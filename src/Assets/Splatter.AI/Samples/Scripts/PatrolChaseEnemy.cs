using UnityEngine;
using UnityEngine.AI;

namespace Splatter.AI {
    /// <summary>
    /// Patrol / chase demo. The enemy walks between waypoints, pausing at each one. Every
    /// frame a sensor writes whether the player is in line of sight to the blackboard, and a
    /// blackboard observer reacts to that value changing: spotting the player aborts the
    /// patrol to start chasing, and losing them stops the chase and resumes the patrol.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PatrolChaseEnemy : BehaviourTreeRunner {
        private const string PlayerVisibleKey = "PlayerVisible";

        private NavMeshAgent agent;
        private int currentWaypointIdx;

        public GameObject[] Waypoints;
        public GameObject Player;
        public float MaxDistance = 5f;

        protected override void Awake() {
            base.Awake();

            agent = GetComponent<NavMeshAgent>();
        }

        private void Update() {
            // Sensors write to the blackboard; observers are only notified when the value
            // actually changes, so writing every frame is cheap.
            Tree.Blackboard[PlayerVisibleKey] = CanSeePlayer();
        }

        protected override Node CreateRoot() {
            return new Repeater() {
                Child = new Selector() {
                    new BlackboardObserverDecorator(Tree.Blackboard, PlayerVisibleKey, true, AbortMode.Both) {
                        Child = new Leaf("Chase player", ChasePlayer),
                    },
                    new Sequencer("Patrol") {
                        new Leaf("Set next waypoint", SetNextWaypoint),
                        new WaitUntilNode("Move to waypoint", HasReachedDestination),
                        new WaitNode("Pause", 1, 3),
                    },
                },
            };
        }

        private NodeResult ChasePlayer() {
            agent.SetDestination(Player.transform.position);

            return NodeResult.Running;
        }

        private NodeResult SetNextWaypoint() {
            agent.SetDestination(Waypoints[currentWaypointIdx].transform.position);

            currentWaypointIdx = (currentWaypointIdx + 1) % Waypoints.Length;

            return NodeResult.Success;
        }

        private bool HasReachedDestination() {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        }

        private bool CanSeePlayer() {
            var rayDirection = Player.transform.position - transform.position;

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, MaxDistance)) {
                return hit.transform == Player.transform;
            }

            return false;
        }
    }
}
