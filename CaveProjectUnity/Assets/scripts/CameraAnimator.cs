using UnityEngine;
using UnityEngine.InputSystem; // <--- ADICIONE ESTA LINHA

public class CameraAnimator : MonoBehaviour
{
    [Header("Configurações de Respiração (Idle)")]
    public float idleAmplitude = 0.01f; // Força do movimento
    public float idleFrequency = 1.5f;  // Velocidade da respiração

    [Header("Configurações de Caminhada (Head Bob)")]
    public float walkAmplitude = 0.05f;
    public float walkFrequency = 10f;
    public float leanAngle = 2f; // Inclinação lateral ao andar

    private float _timer;
    private Vector3 _initialPosition;
    private CharacterController _controller;

    void Start()
    {
        _initialPosition = transform.localPosition;
        // Pega o controller no objeto pai (o Player)
        _controller = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        // Se estivermos em pausa ou inspecionando, não anima
        if (Time.timeScale == 0) return;

        // Detecta se o jogador está tentando se mover (W, A, S, D)
        // Se você usa o Input System padrão:
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed ||
            Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            moveInput = Vector2.one; // Valor simbólico para indicar movimento
        }

        // Se houver input de movimento, aplica Head Bob, senão aplica Idle
        if (moveInput.magnitude > 0.1f)
        {
            ApplyHeadBob();
        }
        else
        {
            ApplyIdleSway();
        }
    }

    // Efeito de respiração quando parado
    void ApplyIdleSway()
    {
        _timer += Time.deltaTime * idleFrequency;

        float moveY = Mathf.Sin(_timer) * idleAmplitude;
        float moveX = Mathf.Cos(_timer * 0.5f) * idleAmplitude; // Movimento em "8" suave

        transform.localPosition = _initialPosition + new Vector3(moveX, moveY, 0);
    }

    // Efeito de caminhar
    // Removamos o parâmetro 'speed' para simplificar o teste
    void ApplyHeadBob()
    {
        _timer += Time.deltaTime * walkFrequency;

        // Movimento vertical (Seno cria o sobe e desce)
        float moveY = Mathf.Sin(_timer) * walkAmplitude;

        // Movimento lateral (Cosseno na metade da velocidade cria o balanço em '8')
        float moveX = Mathf.Cos(_timer * 0.5f) * walkAmplitude;

        // Inclinação lateral (Z)
        float tiltZ = Mathf.Cos(_timer * 0.5f) * leanAngle;

        Vector3 targetPos = _initialPosition + new Vector3(moveX, moveY, 0);

        // Suavização para evitar movimentos bruscos
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 15f);

        // Aplica a inclinação suavemente
        Quaternion targetRot = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, tiltZ);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 15f);
    }
}