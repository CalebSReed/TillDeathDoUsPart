using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] DirectionResolver directionResolver;
    [SerializeField] HealthManager hp;
    [SerializeField] IntroManager intro;
    [SerializeField] GameObject stepVFX;
    public void StartInvincibility(AnimationEvent animEvent)
    {
        hp.isInvincible = true;
    }

    public void EndInvincibility(AnimationEvent animEvent)
    {
        hp.isInvincible = false;
    }

    public void EnableControls(AnimationEvent animEvent)
    {
        Player.Instance.controlsEnabled = true;
    }

    public void DisableControls(AnimationEvent animEvent)
    {
        Player.Instance.controlsEnabled = false;
    }

    public void StartSpecialAnimation(AnimationEvent animEvent)
    {
        directionResolver.showSpecial = true;
    }

    public void EndSpecialAnimation(AnimationEvent animEvent)
    {
        directionResolver.showSpecial = false;
    }

    public void EndThrow(AnimationEvent animEvent)
    {
        Player.Instance.throwing = false;
    }

    public void StopMoving(AnimationEvent animEvent)
    {
        GetComponent<RangedEnemy>().canMove = false;
    }

    public void ResumeMoving(AnimationEvent animEvent)
    {
        GetComponent<RangedEnemy>().canMove = true;
    }

    public void START(AnimationEvent animEvent)
    {
        AudioManager.Instance.Play("Start", transform.position, gameObject, true);
    }

    public void Shoot(AnimationEvent animEvent)
    {
        GetComponent<RangedEnemy>().Shoot();
    }

    public void LoadIntro()
    {
        SceneManager.LoadScene("Intro");
    }

    public void StopWaiting()
    {
        intro.WAIT = false;
    }

    public void TieRope()
    {
        intro.TieRope();
    }

    public void UntieRope()
    {
        intro.UntieRope();
    }

    public void ResetLevel()
    {
        GameManager.Instance.ResetLevel();
    }

    public void Die()
    {
        Player.Instance.cutscene = true;
        Player.Instance.movement.x = 0;
        Player.Instance.movement.y = 0;
    }

    public void PlayerStep()
    {
        AudioManager.Instance.Play("Step", transform.position, gameObject, true, false, false);
        Instantiate(stepVFX, Player.Instance.transform.position, Quaternion.identity);
    }

    public void EnemyStep()
    {
        AudioManager.Instance.Play("EnemyStep", transform.position, gameObject);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {

    }
}
