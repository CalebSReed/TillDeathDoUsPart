using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject death;
    [SerializeField] DialogueManager dialogue;
    [SerializeField] GameObject deathCutscene;
    [SerializeField] GameObject ropeTie;
    [SerializeField] GameObject ropeIntro;
    [SerializeField] GameObject realRope;
    [SerializeField] GameObject advance;

    public bool WAIT;
    public bool lvl1;

    int index;
    public void AdvanceIntro(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (lvl1)
            {
                if (WAIT)
                {
                    return;
                }

                if (index < 5)
                {
                    dialogue.AdvanceText();
                }
                else if (index == 5)
                {
                    dialogue.AdvanceText();
                    player.frontAnimator.Play("HoldRope");
                    ropeIntro.SetActive(true);
                    WAIT = true;
                }
                if (index > 5)
                {
                    dialogue.AdvanceText();
                }

                index++;
            }
            else
            {
                if (WAIT)
                {
                    return;
                }

                if (index == 0)
                {
                    WAIT = true;
                    StartCoroutine(MoveDeath());
                }
                else if (index > 0)
                {
                    dialogue.AdvanceText();
                }
                index++;
            }
        }
    }

    public void TieRope()
    {
        deathCutscene.SetActive(false);
        player.gameObject.SetActive(false);
        ropeTie.SetActive(true);
        ropeIntro.SetActive(false);
    }

    public void UntieRope()
    {
        death.SetActive(true);
        player.gameObject.SetActive(true);
        ropeTie.SetActive(false);
        realRope.SetActive(true);
        WAIT = false;
        dialogue.AdvanceText();
    }

    IEnumerator MoveDeath()
    {
        int go = 25;
        while (go > 0)
        {
            death.transform.position = new Vector3(death.transform.position.x - 1, death.transform.position.y, 0);
            go--;
            yield return null;
        }
        WAIT = false;
    }

    public void EndIntro()
    {
        GameManager.Instance.PlayLevelMusic();
        AudioManager.Instance.Stop("Intro");
        player.controlsEnabled = true;
        player.intro = false;
        advance.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SkipIntro()
    {
        if (!lvl1)
        {
            SceneManager.LoadScene("Lvl1");
        }
        else
        {
            if (dialogue != null)
            {
                Destroy(dialogue.textBox.gameObject);
                Destroy(dialogue.gameObject);
            }
            EndIntro();
        }
    }
}
