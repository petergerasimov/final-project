using UnityEngine;

public class EndScreenInteractable : DistanceInteractable
{
    public DeathScreen FinalScreen;
    public string Message = "YOU WON!";

    public override void OnInteract()
    {
        base.OnInteract();
        if (FinalScreen != null)
        {
            FinalScreen.Show(Message);
        }
    }
}
