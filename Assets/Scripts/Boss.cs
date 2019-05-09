using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;
using UnityEngine.Events;

public class Boss : MonoBehaviour
{/* 
    [System.Serializable]
    public class BossRound
    {
        public float platformSpeed = 1;
        //public MovingPlatform[] platforms;
        public GameObject[] enableOnProgress;
        public int bossHP = 10;
        public int shieldHP = 10;
    }

    
    public Transform target;

    public UnityEvent onDefeated;
    Animator animator;
    Root ai;

    int round = 0;

    void OnEnable() {
        round = 0;
        
        ai = BT.Root();
        ai.OpenBranch(
            //First Round
            BT.SetActive(beamLaser, false),
            BT.Repeat(rounds.Length).OpenBranch(
                BT.Call(NextRound),
                //grenade enabled is true only on 2 and 3 round, so allow to just test if it's the 1st round or not here
                BT.If(GrenadeEnabled).OpenBranch(
                    BT.Trigger(animator, "Enabled")
                    ),
                BT.Wait(delay),
                BT.Call(ActivateShield),
                BT.Wait(delay),
                BT.While(ShieldIsUp).OpenBranch(
                    BT.RandomSequence(new int[] { 1, 6, 4, 4 }).OpenBranch(
                        BT.Root().OpenBranch(
                            BT.Trigger(animator, "Walk"),
                            BT.Wait(0.2f),
                            BT.WaitForAnimatorState(animator, "Idle")
                            ),
                        BT.Repeat(laserStrikeCount).OpenBranch(
                            BT.SetActive(beamLaser, true),
                            BT.Trigger(animator, "Beam"),
                            BT.Wait(beamDelay),
                            BT.Call(FireLaser),
                            BT.WaitForAnimatorState(animator, "Idle"),
                            BT.SetActive(beamLaser, false),
                            BT.Wait(delay)
                        ),
                        BT.If(EllenOnFloor).OpenBranch(
                            BT.Trigger(animator, "Lightning"),
                            BT.Wait(lightningDelay),
                            BT.Call(ActivateLightning),
                            BT.Wait(lightningTime),
                            BT.Call(DeactivateLighting),
                            BT.Wait(delay)
                        ),
                        BT.If(GrenadeEnabled).OpenBranch(
                            BT.Trigger(animator, "Grenade"),
                            BT.Wait(grenadeDelay),
                            BT.Call(ThrowGrenade),
                            BT.WaitForAnimatorState(animator, "Idle")
                        )
                    )
                ),
                BT.SetActive(beamLaser, false),
                BT.Trigger(animator, "Grenade", false),
                BT.Trigger(animator, "Beam", false),
                BT.Trigger(animator, "Lightning", false),
                BT.Trigger(animator, "Disabled"),
                BT.While(IsAlive).OpenBranch(BT.Wait(0))
            ),
            BT.Trigger(animator, "Death"),
            BT.SetActive(damageable.gameObject, false),
            BT.Wait(cleanupDelay),
            BT.Call(Cleanup),
            BT.Wait(deathDelay),
            BT.Call(Die),
            BT.Terminate()
        );

        // BackgroundMusicPlayer.Instance.ChangeMusic(bossMusic);
        // BackgroundMusicPlayer.Instance.Play();
        // BackgroundMusicPlayer.Instance.Unmute(2.0f);
    }

    void FireLaser()
    {
        //laserFireAudioPlayer.PlayRandomSound();

        var p = Instantiate(projectile);
        var dir = -beamLaser.transform.right;
        p.transform.position = beamLaser.transform.position;
        p.initialForce = new Vector3(dir.x, dir.y) * 1000;
    }

    void ThrowGrenade()
    {
        grenadeThrowAudioPlayer.PlayRandomSound();

        var p = Instantiate(grenade);
        p.transform.position = grenadeSpawnPoint.position;
        p.initialForce = grenadeLaunchVelocity;
    }

    bool GrenadeEnabled()
    {
        return round > 1;
    }

    void ActivateLightning()
    {
        lightingAttackAudioPlayer.PlayRandomSound();

        var p = Instantiate(lightning) as GameObject;
        p.transform.position = transform.position;
        Destroy(p, lightningTime);
    }

    void DeactivateLighting()
    {
        lightingAttackAudioPlayer.Stop();
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            m_PreviousTargetPosition = target.position;
        }
    }

    void Update()
    {
        ai.Tick();
        if (beamLaser != null && target != null)
        {
            Vector2 targetMovement = (Vector2)target.position - m_PreviousTargetPosition;
            targetMovement.Normalize();
            Vector3 targetPos = target.position + Vector3.up * (1.0f + targetMovement.y * 0.5f);

            beamLaser.transform.rotation = Quaternion.RotateTowards(beamLaser.transform.rotation, Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.left, targetPos - beamLaser.transform.position, Vector3.forward)), laserTrackingSpeed * Time.deltaTime);
        }

        shield.transform.localScale = Vector3.Lerp(shield.transform.localScale, originShieldScale, Time.deltaTime);
    }

    void Die()
    {
        onDefeated.Invoke();
    }*/
}
