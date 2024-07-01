using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClearTextOnEnabled : MonoBehaviour
{
    GameObject ProjectTypesButtons;
    void OnEnable()
    {
        GetComponent<TMP_InputField>().text = "";
        this.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
    }
}
