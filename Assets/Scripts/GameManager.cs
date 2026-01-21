using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject instructionsMobil, instructionsPC;

    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.25f;
    public float gameSpeed { get; private set; }

    [SerializeField] private GameObject obstacleEngine;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;

    private Player player;
    private Spawner spawner;

    private float score;
    public float Score => score;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();
        obstacleEngine.SetActive(true);
        NewGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        obstacleEngine.SetActive(false);
    }

    public void isPc()
    {
        instructionsMobil.SetActive(false);
        instructionsPC.SetActive(true);
    }

    public void isMobile()
    {
        instructionsMobil.SetActive(true);
        instructionsPC.SetActive(false);
    }

    public void NewGame()
    {
        Time.timeScale = 1f;
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();

        foreach (var obstacle in obstacles) {
            Destroy(obstacle.gameObject);
        }
        obstacleEngine.SetActive(true);
        spawner = obstacleEngine.GetComponent<Spawner>();
        score = 0f;
        gameSpeed = initialGameSpeed;
        enabled = true;

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        UpdateHiscore();
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);

        UpdateHiscore();
    }

    private void Update()
    {
        if(gameSpeed < 15f)
        {
            gameSpeed += gameSpeedIncrease * Time.deltaTime;
        }
        
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }

}
