using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float stunDuration = 1.5f;
    public int damage = 20;
    public bool isDead = false;
    private Animator animator;


    public Transform player; // Sẽ tự động tìm Player có CharacterController
    private NavMeshAgent agent;
    private bool isAttacking = false;
    private bool wasRunning = false;

    private PlayerHealth playerHealth;
    private PlayerStun playerStun;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        // Tự động tìm Player có CharacterController
        if (player == null)
        {
            var cc = FindObjectOfType<CharacterController>();
            if (cc != null)
                player = cc.transform;
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = player.GetComponentInChildren<PlayerHealth>();

            playerStun = player.GetComponent<PlayerStun>();
            if (playerStun == null)
                playerStun = player.GetComponentInChildren<PlayerStun>();
        }
    }

    void Update()
    {
        if (player == null || agent == null || animator == null) return;
        if (!agent.enabled || health <= 0) return;

        // Nếu player đã chết thì zombie dừng lại
        if (playerHealth != null && playerHealth.IsDead())
        {
            agent.enabled = false;
            animator.SetBool("isRunning", false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);
        var audio = SoundManager.Instance.zombieAudioSource;

        if (distanceToPlayer > attackRange)
        {
            animator.SetBool("isRunning", true);
            isAttacking = false;
            if (!wasRunning)
            {
                if (audio.clip != SoundManager.Instance.zombieChasing)
                {
                    audio.clip = SoundManager.Instance.zombieChasing;
                    audio.loop = true;
                    audio.Play();
                }
                wasRunning = true;
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            if (wasRunning)
            {
                if (audio.isPlaying && audio.clip == SoundManager.Instance.zombieChasing)
                    audio.Stop();
                wasRunning = false;
            }
            if (!isAttacking)
            {
                audio.PlayOneShot(SoundManager.Instance.zombieAttack);
                animator.SetTrigger("Attack");
                isAttacking = true;
            }
        }
    }

    // Gọi hàm này từ Animation Event ở animation Attack
    public void DealDamageToPlayer()
    {
        if (isDead) return;
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("Zombie: PlayerHealth reference is null, cannot deal damage.");
        }

        if (playerStun != null)
        {
            playerStun.Stun(stunDuration);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // tránh trừ máu/đếm kill nhiều lần

        health -= damage;
        var audio = SoundManager.Instance.zombieAudioSource;
        if (audio.isPlaying && audio.clip == SoundManager.Instance.zombieChasing)
            audio.Stop(); // Tắt chasing khi bị hurt hoặc die
        if (health <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            audio.PlayOneShot(SoundManager.Instance.zombieDie);
            Die();
        }
        else if (animator != null)
        {
            audio.PlayOneShot(SoundManager.Instance.zombieHurt);
            animator.SetTrigger("Hurt");
        }
    }

    private void Die()
    {
        SoundManager.Instance.zombieAudioSource.PlayOneShot(SoundManager.Instance.zombieDie);
        if (animator != null)
            animator.SetTrigger("Die");
        
        if (agent != null)
            agent.enabled = false;

        // Báo cho GameManager để tăng số kill
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
        }
            
        Destroy(gameObject, 3f);
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }
}