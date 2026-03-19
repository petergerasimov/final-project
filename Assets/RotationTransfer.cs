using UnityEngine;

public class RotationTransfer : MonoBehaviour
{
    [SerializeField] private Transform source;
    [SerializeField] private Transform target;

    void Update()
    {
        if (source == null || target == null) return;
        target.rotation = source.rotation;
    }
}
