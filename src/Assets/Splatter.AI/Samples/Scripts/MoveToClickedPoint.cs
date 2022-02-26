using UnityEngine;
using UnityEngine.AI;

namespace Splatter.AI {
    public class MoveToClickedPoint : MonoBehaviour {
        private NavMeshAgent agent;

        private void Start() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
                    agent.destination = hit.point;
                }
            }
        }
    }
}