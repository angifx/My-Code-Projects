using UnityEngine;
using UnityEngine.SceneManagement;

public class GemManager : MonoBehaviour
{
    public static GemManager Instance;

    [SerializeField] private int gemsNeededForBoss = 25;
    [SerializeField] private string bossSceneName = "BossArena";

    private int currentGems = 0;
    private bool bossTriggered = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        gem.OnGemCollect += HandleGemCollected;
    }

    void OnDisable()
    {
        gem.OnGemCollect -= HandleGemCollected;
    }

    private void HandleGemCollected(int worth)
    {
        currentGems += worth;
        Debug.Log("Gems gesammelt: " + currentGems + "/" + gemsNeededForBoss);

        if (currentGems >= gemsNeededForBoss && !bossTriggered)
        {
            bossTriggered = true;
            SceneManager.LoadScene(bossSceneName);
        }
    }

    public int GetCurrentGems() => currentGems;
}