using UnityEngine;

public static class PhysXUtility
{
    public static void ResetPhysX()
    {
        PhysX5ForUnity.PhysxActor[] actors = Object.FindObjectsOfType<PhysX5ForUnity.PhysxActor>(true);
        foreach (var actor in actors)
        {
            if (!actor.enabled) continue;
            switch (actor)
            {
                case PhysX5ForUnity.PhysxFluidActor fluidActor:
                    if (fluidActor.ParticleData != null) fluidActor.ResetObject();
                    break;
                case PhysX5ForUnity.PhysxTriangleMeshClothActor clothActor:
                    if (clothActor.ParticleData != null) clothActor.ResetObject();
                    break;
                case PhysX5ForUnity.PhysxFEMSoftBodyActor softBodyActor:
                    softBodyActor.ResetObject();
                    break;
                case PhysX5ForUnity.PhysxArticulationKinematicTree kinematicTree:
                    kinematicTree.ResetObject();
                    break;
            }
        }
    }
}
