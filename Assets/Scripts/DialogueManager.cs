using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] List<string> dialogueList = new();
    [SerializeField] bool intro;
    [SerializeField] IntroManager introClass;
    public Image textBox;
    [SerializeField] Sprite ani;
    [SerializeField] Sprite death;
    int index;

    private void Start()
    {

    }

    public void AdvanceText()
    {
        textBox.gameObject.SetActive(true);

        if (intro)
        {
            if (index == 1 || index == 3)
            {
                textBox.sprite = ani;
            }
            else
            {
                textBox.sprite = death;
            }
        }
        else
        {
            if (index == 5)
            {
                textBox.gameObject.SetActive(false);
            }

            if (index == 0 || index == 2 || index == 4 || index == 7 || index == 9)
            {
                textBox.sprite = ani;
            }
            else
            {
                textBox.sprite = death;
            }
        }

        AudioManager.Instance.Play("Text", transform.position, gameObject, true);

        if (index >= dialogueList.Count)
        {
            text.text = "";
            if (intro)
            {
                SceneManager.LoadScene("Lvl1");
            }
            else
            {
                textBox.gameObject.SetActive(false);
                introClass.EndIntro();
                Destroy(gameObject);
            }
            return;
        }
        text.text = dialogueList[index];
        index++;
    }
}
