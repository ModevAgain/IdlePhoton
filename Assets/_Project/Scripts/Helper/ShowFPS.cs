using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ShowFPS : MonoBehaviour
{
    public TMP_Text FpsText;
    public float updateInterval = 0.5f;

    private float _accumulated = 0f;
    private int _frames = 0;
    private float _timer = 0f;

    void Update()
    {
        _accumulated += Time.unscaledDeltaTime;
        _frames++;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= updateInterval)
        {
            float fps = _frames / _accumulated;
            FpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";

            _timer = 0f;
            _accumulated = 0f;
            _frames = 0;
        }
    }
}
