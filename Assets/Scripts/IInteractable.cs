public interface IInteractable
{
    /// <summary>
    /// Called when a player interacts with this object.
    /// Return true if the interaction succeeded (for example, purchase or repair succeeded).
    /// </summary>
    bool Interact(UnityEngine.GameObject interactor);
}
