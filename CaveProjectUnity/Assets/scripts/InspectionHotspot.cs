using UnityEngine;

public class InspectionHotspot : MonoBehaviour
{
    public string description;
    [TextArea] public string detailedInfo;

    [Header("Centralização")]
    // Arraste para cá um objeto vazio que indique a "frente" deste hotspot
    public Transform faceDirection;

    private Renderer _renderer;
    private Color _originalColor;
    private bool _isHighlighted;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null) _originalColor = _renderer.material.color;
    }

    public void SetHighlight(bool state)
    {
        if (_renderer == null) return;
        _isHighlighted = state;
        _renderer.material.color = state ? Color.yellow : _originalColor;
    }
}