using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveImage : MonoBehaviour
{
    public List<Texture2D> capturedImages;
    public int ImageIndex;

    private Transform GrandParentObject;

    void Start()
    {
        GrandParentObject = transform.parent.parent.parent.parent;
        if (GrandParentObject.name != "CameraPanel")
        {
            GrandParentObject = GrandParentObject.parent.GetChild(1);
        }
        capturedImages = GrandParentObject.GetComponent<WebcamController>().capturedImages;
    }

    public void RemoveThisImage()
    {
        // Get the parent of the current object
        Transform parent = transform.parent;

        // Shift the positions of the prefabs after the removed one
        for (int i = parent.childCount - 1; i > ImageIndex; i--)
        {
            // Get the RectTransform of the child
            RectTransform childRectTransform = parent.GetChild(i).GetComponent<RectTransform>();

            // Get the RectTransform of the preceding sibling
            RectTransform prevSiblingRectTransform = parent.GetChild(i - 1).GetComponent<RectTransform>();

            // Set the localPosition of the child to match the preceding sibling
            childRectTransform.localPosition = prevSiblingRectTransform.localPosition;

            parent.GetChild(i).GetComponent<RemoveImage>().ImageIndex = i - 1;
        }

        // Remove the last child (empty space)
        Destroy(gameObject);

        // Update the capturedImages list
        capturedImages.RemoveAt(ImageIndex);
    }
}
