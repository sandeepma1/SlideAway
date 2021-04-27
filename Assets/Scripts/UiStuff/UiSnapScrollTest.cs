using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
public class UiSnapScrollTest : MonoBehaviour
{
    private HorizontalScrollSnap scrollSnap;

    private void Start()
    {
        scrollSnap = GetComponent<HorizontalScrollSnap>();
        //scrollSnap.OnCurrentScreenChange += OnCurrentScreenChange;
    }

    private void OnDestroy()
    {
        //scrollSnap.OnCurrentScreenChange -= OnCurrentScreenChange;
    }

    private void OnPageChange(int page)
    {
        print(page);
    }
}
