using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if(currentHealth > 0)
        {
            //player hurt
            anim.SetTrigger("hurt");
            //iframes
            StartCoroutine(Invunerability());
            SoundManager.instance.PlaySound(hurtSound);
        }
        else
        {
            //player dead
            if(!dead)
            {
                //Deactivate
                foreach(Behaviour component in components)
                    component.enabled = false;

                anim.SetBool("grounded", true);
                anim.SetTrigger("die");

                dead = true;
                SoundManager.instance.PlaySound(deathSound);
            }
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    //Respawn
    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
        StartCoroutine(Invunerability());

        //Activate all attached component classes
        foreach (Behaviour component in components)
            component.enabled = true;
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;

        Physics2D.IgnoreLayerCollision(7,8,true);

        //invunerabity duration
        //Tab + 4
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        
        Physics2D.IgnoreLayerCollision(7, 8, false);
        invulnerable = false;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
