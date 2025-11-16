using UnityEngine;

/// <summary>
/// Defines the contract for any object that can be interacted with by the player.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// The priority of the interaction. Higher values take precedence.
    /// </summary>
    public int Priority { get; }
    
    /// <summary>
    /// The core action to perform when the object is interacted with.
    /// </summary>
    public void Interact(Interactor interactor);

    /// <summary>
    /// Gets the text prompt to display to the user (e.g., "Open Door", "Rest").
    /// </summary>
    /// <returns>The interaction prompt string.</returns>
    public string GetInteractionPrompt();
}
