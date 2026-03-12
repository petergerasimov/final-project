using UnityEngine;

namespace PhysX5ForUnity
{

    [DefaultExecutionOrder(0)]
    public class PhysicsReset : MonoBehaviour
    {
        private void OnEnable()
        {
            Physx.GetPhysXInitStatus();
            Physics.simulationMode = SimulationMode.FixedUpdate;
        }
    }
}
