using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggleButton : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _label;
    [SerializeField]
    private bool _toggleLabel;
    [SerializeField]
    private Color _selectedLabelColor;
    [SerializeField]
    private Color _defaultLabelColor;
    
    [SerializeField]
    private Image _imageBG;
    [SerializeField]
    private bool _toggleBG;
    [SerializeField]
    private Color _selectedBGColor;
    [SerializeField]
    private Color _defaultBGColor;

    public bool IsSelected;
    public Action<bool> Toggled;

    private Button _btn;


    private void Awake()
    {
        _btn = GetComponentInChildren<Button>();
        _btn.onClick.AddListener(OnClick);

        Toggled += OnToggle;
    }

    private void OnClick()
    {
        if (IsSelected)
            return;
        Toggled?.Invoke(!IsSelected);
    }

    public void OnToggle(bool on)
    {
        IsSelected = on;
        
        if (_toggleLabel)
        {
            _label.color = on ? _selectedLabelColor : _defaultLabelColor;
        }
        
        if (_toggleBG)
        {
            _imageBG.color = on ? _selectedBGColor : _defaultBGColor;
        }
        
        _btn.interactable = !on;
    }
}
