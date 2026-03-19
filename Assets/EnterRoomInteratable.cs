using UnityEngine;

public class EnterRoomInteratable : DistanceInteractable
{
    public Transform ObjectToMove;
    public Transform Destination;
    public bool IsOneTime = true;

    public override void OnInteract()
    {
        if (ObjectToMove == null || Destination == null) return;
        ObjectToMove.position = new Vector3(Destination.position.x, Destination.position.y, Destination.position.z);

        if (IsOneTime) gameObject.SetActive(false);
    }
}
