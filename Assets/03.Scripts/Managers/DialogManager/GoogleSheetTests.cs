using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoogleSheetTest : MonoBehaviour
{
    private GoogleSheetLoader loader;

    void Start()
    {
        loader = gameObject.AddComponent<GoogleSheetLoader>();
        loader.LoadAllSheets();
    }

}