using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RoomBehavior : MonoBehaviour
{
    [SerializeField] List<DoorBehavior> doorList = new();
    [SerializeField] List<GameObject> enemyList = new();
    [SerializeField] TextMeshProUGUI enemiesLeftText;
    [SerializeField] int healthDropCount;
    int enemyCount;

    private void Awake()
    {
        enemyCount = enemyList.Count;
    }

    private void Update()
    {
        if (enemyCount > 0)
        {
            DisplayEnemyAmounts();
        }
    }

    private void DeployKeyDrop()
    {
        int rand = Random.Range(0, enemyCount);
        if (enemyList[rand].GetComponent<MeleeEnemy>() != null)
        {
            enemyList[rand].GetComponent<MeleeEnemy>().dropKey = true;
        }
        else
        {
            enemyList[rand].GetComponent<RangedEnemy>().dropKey = true;
        }
        GameManager.Instance.dropKey = false;
        GameManager.Instance.keyAlreadyDropped = true;
    }

    private void DeployHealthDrops()
    {
        while (healthDropCount > 0)
        {
            int rand = Random.Range(0, enemyCount);
            if (enemyList[rand].GetComponent<MeleeEnemy>() != null)
            {
                enemyList[rand].GetComponent<MeleeEnemy>().dropHealth = true;
            }
            else
            {
                enemyList[rand].GetComponent<RangedEnemy>().dropHealth = true;
            }
            healthDropCount--;
        }
    }

    public void EnterRoom()
    {
        if (enemyCount > 0)
        {
            LockAllDoors();

            DeployHealthDrops();

            if (GameManager.Instance.dropKey)
            {
                DeployKeyDrop();
            }
        }
    }

    public void LockAllDoors()
    {
        AudioManager.Instance.Play("Close", transform.position, gameObject, true);
        foreach (var door in doorList)
        {
            if (!door.gameObject.CompareTag("KeyDoor"))
            {
                door.LockDoor();
            }
        }
    }

    public void UnlockAllDoors()
    {
        AudioManager.Instance.Play("Open", transform.position, gameObject, true);
        foreach (var door in doorList)
        {
            door.UnlockDoor();
        }
    }

    public void DisplayEnemyAmounts()
    {
        enemiesLeftText.text = $"Enemies Left: {enemyCount}";
        foreach (var enemy in enemyList)
        {
            if (enemy == null)
            {
                enemyList.Remove(enemy);
                enemyCount--;

                if (enemyCount <= 0)
                {
                    UnlockAllDoors();
                    enemiesLeftText.text = $"Enemies Left: 0";
                }

                break;
            }
        }
    }
}
