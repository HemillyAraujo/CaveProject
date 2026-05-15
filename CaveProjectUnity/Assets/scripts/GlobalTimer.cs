using UnityEngine;
using TMPro;

public class GlobalTimer : MonoBehaviour
{
    public static GlobalTimer Instance { get; private set; }

    [Header("Configurações de Tempo")]
    [SerializeField] private float timeRemaining = 900f; // 15 minutos em segundos (15 * 60)

    public float TimeRemaining => timeRemaining;

    [Header("Configurações de UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private bool _isRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _isRunning = true;
    }

    void Update()
    {
        if (_isRunning)
        {
            if (timeRemaining > 0)
            {
                // Subtrai o tempo passado no último frame
                timeRemaining -= Time.deltaTime;
                UpdateDisplay(timeRemaining);
            }
            else
            {
                // O tempo acabou
                timeRemaining = 0;
                _isRunning = false;
                OnTimerEnd();
            }
        }
    }

    void UpdateDisplay(float timeToDisplay)
    {
        // Garante que não exiba valores negativos
        if (timeToDisplay < 0) timeToDisplay = 0;

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Formato clássico MM:SS
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTimerEnd()
    {
        Debug.Log("O tempo acabou!");
        // Aqui você pode chamar eventos de Game Over, por exemplo.
    }

    // Métodos para controle externo
    public void StopTimer() => _isRunning = false;
    public void ResumeTimer() => _isRunning = true;
}