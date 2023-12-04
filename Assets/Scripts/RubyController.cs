using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    public int score;
    
    public GameObject projectilePrefab;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI giftscoreText;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI restartText;
    public TextMeshProUGUI createdByText;
    public TextMeshProUGUI winText;

    public bool isGameActive;

    public int robotwin = 0;
    public int giftwin = 0;
    
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip npctalk;
    public AudioClip winscreen;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    public int winscore;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public bool boosting;
    public float boostTimer; //candy powerup - a

    public int giftscore; //giftbox score - c
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;

    public ParticleSystem hitEffect;
    public ParticleSystem healthPickup;
    public ParticleSystem dustEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        isGameActive = true;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        winText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameActive)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            
            Vector2 move = new Vector2(horizontal, vertical);
            
            if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }
        
            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(isGameActive)
            {
                Launch();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            if(isGameActive)
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
                if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                        PlaySound(npctalk); //npc talking sound effect -c
                    }
                }
            }
            
        }
        
        if(boosting)
            {
                boostTimer += Time.deltaTime;
                if(boostTimer >= 4)
                {
                    speed = 3.0f;
                    boostTimer = 0;
                    boosting = false;
                }

                dustEffect = Instantiate(dustEffect, transform.position, Quaternion.identity);
            } //candy power up timer -a

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(!isGameActive)
            {
                RestartGame();
            }
        }
    }
    
    void FixedUpdate()
    {
        if(isGameActive)
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;

            rigidbody2d.MovePosition(position);
        }
    }

    public void ChangeHealth(int amount)
    {
        if(isGameActive)
        {
            if (amount < 0)
            {
                if (isInvincible)
                    return;
                
                isInvincible = true;
                invincibleTimer = timeInvincible;
                // take damage effect
                animator.SetTrigger("Hit");
                hitEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                
                PlaySound(hitSound);
            }
        }
            
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            
         UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if(currentHealth <= 0)
        {
            GameOver();
        }
        
        
    }

    public void ChangeScore(int scoreAmount)
    {
        score += scoreAmount;
        scoreText.text = "Fixed Robots: " + score.ToString();

        if (score>= 6)
        {
           WinCondition(1);
        }
    
    }

    public void ChangeGiftScore(int GiftscoreAmount)
    {
        giftscore += GiftscoreAmount;
        giftscoreText.text = "Collected Gifts: " + giftscore.ToString(); //gift score ui -c
        if (giftscore >= 2)
        {
           WinCondition(1);
        }
    }

    public void WinCondition(int wincon)
    {
        winscore += wincon;
        if (winscore >=2)
        {
            WinGame();
        }
    
    }


    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void strawberryPickup()
    {
        healthPickup = Instantiate(healthPickup, transform.position, Quaternion.identity);
    }


    public void GameOver()
    {
        loseText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        createdByText.gameObject.SetActive(true);
        isGameActive = false;
    }

    public void RestartGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void WinGame()
    {
        winText.gameObject.SetActive(true);
        createdByText.gameObject.SetActive(true);
        PlaySound(winscreen); //win screen audio effect -c
        isGameActive = false;
    }

}
