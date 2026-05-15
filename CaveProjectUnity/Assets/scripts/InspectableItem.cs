using UnityEngine;

public class InspectableItem : MonoBehaviour
{
    [Header("Configurações de Inspeção")]
    public string itemName;
    [TextArea] public string description;
    public Vector3 inspectionRotationOffset; // Ajuste se o item aparecer de lado

    // Guarda a posição original para quando sair da inspeção
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Quaternion originalRotation;
    [HideInInspector] public Transform originalParent;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalParent = transform.parent;
    }
}