/*
Purpose of the Script
The primary purpose of the CursorControl script is to ensure that the cursor is always visible and can move freely when the game starts. 
This is particularly useful in game menus or interfaces where the player needs to interact with UI elements using the cursor.
This script attached to game object called "CursorStuff" in the MainMenuScene
*/

using UnityEngine; // Imports the UnityEngine namespace, which contains essential classes and functions for Unity development.

namespace Survival // Declares a custom namespace named `Survival` to organize the code and avoid naming conflicts.
{
    public class CursorControl : MonoBehaviour // Defines a public class named `CursorControl` that inherits from `MonoBehaviour`.
    {
        private void Start() // Declares the `Start` method, which is called by Unity when the script instance is being loaded. This method is executed once at the beginning of the game or when the script is enabled.
        {
            // Make the cursor visible and unlock it
            Cursor.visible = true; // Sets the cursor's visibility to `true`, ensuring that the cursor is visible to the player. This is particularly important in games where the cursor might be hidden by default (e.g., first-person shooters).
            Cursor.lockState = CursorLockMode.None; // Sets the cursor's lock state to `None`, allowing the cursor to move freely around the screen. The `CursorLockMode` enumeration has three possible values:
                                                    // - CursorLockMode.None: The cursor behaves normally and can move freely.
                                                    // - CursorLockMode.Locked: The cursor is locked to the center of the game window.
                                                    // - CursorLockMode.Confined: The cursor is confined to the game window but can still move within it.
        }
    }
}
