using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberOfSamples : MonoBehaviour
{
    public GameObject ClassesContainer;

    void Update()
    {
        SetNumberOfClassesSamples();
    }

    void SetNumberOfClassesSamples()
    {
        int numClasses = ClassesContainer.transform.childCount;
        for (int i = 0; i < numClasses; i++)
        {
            Transform numberOfClasses = ClassesContainer.transform.GetChild(i).GetChild(0).GetChild(4);
            int numberOfSamples = ClassesContainer.transform.GetChild(i).GetChild(0).GetChild(5).GetChild(0).GetChild(0).childCount;
            numberOfClasses.GetComponent<TMPro.TextMeshProUGUI>().text = numberOfSamples > 1? numberOfSamples + " IMAGE" : numberOfSamples + " IMAGES";
        }
    }
}
