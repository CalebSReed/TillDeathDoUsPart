using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] List<GameObject> enemyList = new();
    public int enemiesKilled;
    public int killGoal;
    public bool dropKey;
    public bool keyAlreadyDropped;

    public GameObject keyObj;
    public GameObject hpObj;
    [SerializeField] bool title;
    [SerializeField] Animator start;
    [SerializeField] Animator rope;
    [SerializeField] string lvlName;
    [SerializeField] bool endGame;
    [SerializeField] GameObject pauseMenu;
    public bool lvl2;
    public bool intro;
    public bool lvl1;
    public bool titleScreen;
    bool paused;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (lvl2)
        {
            AudioManager.Instance.Play("Level", transform.position, gameObject, true, false, true);
        }
        if (titleScreen)
        {
            AudioManager.Instance.Play("Title", transform.position, gameObject, true, false, true);
        }
        if (intro || lvl1)
        {
            AudioManager.Instance.Play("Intro", transform.position, gameObject, true, false, true);
        }
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed && !Player.Instance.intro)
        {
            paused = !paused;
            Player.Instance.controlsEnabled = !paused;

            if (paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1f;
            }
            pauseMenu.SetActive(paused);
        }
    }

    public void PlayLevelMusic()
    {
        AudioManager.Instance.Play("Level", transform.position, gameObject, true, false, true);
    }

    public void SelectTitle(InputAction.CallbackContext context)
    {
        if (context.performed && title)
        {
            start.Play("StartPress");
            rope.Play("RopeTitle");
            title = false;
        }
    }

    public void DebugSpawnEnemy(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int rand = Random.Range(0, enemyList.Count);
            GameObject enemy = enemyList[rand];

            Instantiate(enemy, CalebUtils.RandomPositionInRadius(Player.Instance.transform.position, 10, 15), Quaternion.identity);
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(lvlName);
    }

    public void EndGame(InputAction.CallbackContext context)
    {
        if (context.performed && endGame)
        {
            Application.Quit();
        }
    }

    public void AddKill()
    {
        enemiesKilled++;
        if (enemiesKilled >= killGoal && !keyAlreadyDropped)
        {
            dropKey = true;
        }
    }
}
