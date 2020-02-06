// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer.Examples.FineControl
{
    /// <summary>An object that can be interacted with.</summary>
    public interface IInteractable
    {
        /************************************************************************************************************************/

        void Interact();

        /************************************************************************************************************************/
    }

    /// <summary>
    /// Attempts to interact with whatever <see cref="IInteractable"/> the cursor is pointing at when the user clicks
    /// the mouse.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Fine Control - Click To Interact")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.FineControl/ClickToInteract")]
    public sealed class ClickToInteract : MonoBehaviour
    {
        /************************************************************************************************************************/

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                var interactable = raycastHit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                    interactable.Interact();
            }
        }

        /************************************************************************************************************************/
    }
}
