using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveImage : MonoBehaviour
{
    public List<Texture2D> capturedImages;
    public int ImageIndex;
    public bool isLoad = false;
    private Transform GrandParentObject;

    void Start()
    {
        GrandParentObject = transform.parent.parent.parent.parent;
        // if true => initialize remove image of the outside panel 
        // so we want to get the captured images from the inside panel ()
        if (GrandParentObject.name == "ClassesBox" && isLoad) // in case the load of the saved project instentiate images
        {
            return;
        }
        else if (GrandParentObject.name != "CameraPanel") // in case remove image from outside panel
        {
            GrandParentObject = GrandParentObject.parent.parent.parent.parent.GetChild(1);
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
            Debug.Log(parent.GetChild(i).name);
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
        Debug.Log("ImageIndex: " + ImageIndex + " capturedImages.Count: " + capturedImages.Count);

        capturedImages.RemoveAt(ImageIndex);
    }
}
