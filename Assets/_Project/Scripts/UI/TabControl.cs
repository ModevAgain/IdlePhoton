
using System;
using System.Collections.Generic;
using UnityEngine;

public class TabControl : MonoBehaviour
{
    public List<CustomToggleButton> TabBtns;

    private void Awake()
    {
        foreach (var btn in TabBtns)
        {
            btn.Toggled += b => OnTabToggled(btn, b);
        }
    }

    private void Start()
    {
        TabBtns[0].OnToggle(true);
    }

    private void OnTabToggled(CustomToggleButton toggledBtn, bool value)
    {
        foreach (var btn in TabBtns)
        {
            if (btn != toggledBtn && btn.IsSelected)
            {
                btn.OnToggle(false);
            }
        }
    }
}
