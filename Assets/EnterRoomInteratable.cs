using UnityEngine;

public class EnterRoomInteratable : DistanceInteractable
{
    public Transform objectToMove;
    public Transform destination;
    public bool once = true;

    public override void OnInteract()
    {
        if (objectToMove == null || destination == null) return;
        objectToMove.position = new Vector3(destination.position.x, destination.position.y, destination.position.z);

        if (once) gameObject.SetActive(false);
    }
}
