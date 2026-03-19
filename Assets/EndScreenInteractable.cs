using UnityEngine;

public class EndScreenInteractable : DistanceInteractable
{
    public DeathScreen finalScreen;
    public string message = "YOU WON!";

    public override void OnInteract()
    {
        base.OnInteract();
        if (finalScreen != null)
        {
            finalScreen.Show(message);
        }
    }
}
