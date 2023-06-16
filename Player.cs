using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class Player : MonoBehaviour {

   // public static Player instance { get; private set; }

    public float speedModifier = 0.05f;
    public float cameraSpeed = 0.05f;

    public Vector3 cameraRotation;
    public Vector3 cameraPosition;

    public float timeScoreMultiplier = 10f;
    public float crouchDuration = 1f;

    public float pursuerPositionOffset = 4f;

    public float initialHitSpeed = -10f;
    public float hitSpeedDecreaseSpeed = 80f;

    public LayerMask levelLayer;
 

    public BoxCollider[] boxCollider;
    public CapsuleCollider[] capsuleColliders;

    public float mouseAndTouchMoveDistance = 80f;

    public bool enableKeyboardMovement = true;
    public bool enableMouseMovement = true;

    [SerializeField]
    protected float m_GroundCheckDistance = 0.1f;
    protected float m_OrigGroundCheckDistance;
    protected Vector3 m_GroundNormal;
    [SerializeField]
    protected float m_JumpPower = 12f;
    [SerializeField]
    protected float stepDistance = 1.5f;

    public float speed = 3f;

    public int skinPrice = 1000;

    public Animator anim;

    protected float posX = 0.0f;
    protected float startTime;
    protected float jumpSpeed;
    protected bool m_Jump;
    protected Rigidbody m_Rigidbody;

    public bool m_PreviouslyGrounded, m_IsGrounded;
    protected Collider m_Collider;
    protected Vector3 m_GroundContactNormal;

    public bool dead;

    protected int coins;
    protected float time;
    protected bool crouching;
    protected float crouchDeltaHeight = 1f;
    public Manager manager;

    protected float jumpMultiplier = 1f;
    protected float timeMultiplier = 1f;
    protected float coinsMultiplier = 1f;

    protected float jumpMultiplierTime;
    protected float timeMultiplierTime;
    protected float coinsMultiplierTime;

   
    protected Vector2 fp;  // first finger position
    protected Vector2 lp;  // last finger position
    protected bool swiped = true;

    protected float animForward = 0.5f;
   
    protected float hitSpeedLoc;
    [HideInInspector]
    public Transform myTransform;

    public MySwipe mySwipe;

    public bool isShieldOn = false;

    protected virtual void Awake()
    {
        myTransform = transform;

        //if (instance == null)
        //    instance = this;
        //else
        //    Destroy(gameObject);
    }

    protected virtual void Start ()
    {       
        hitSpeedLoc = speed;
        anim = GetComponentInChildren<Animator>();
        mySwipe = GameObject.FindObjectOfType<MySwipe>();
        anim.applyRootMotion = false;

        m_Collider = GetComponent<CapsuleCollider>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_OrigGroundCheckDistance = m_GroundCheckDistance;
        manager = GameObject.Find("Manager").GetComponent<Manager>();

    }

    protected virtual void Crouch()
    {
       
        RunEventServices.GameMechanicAction.Crouch?.Invoke();
        anim.SetBool("Crouch", true);
        crouching = true;

        for (int i = 0; i < boxCollider.Length; i++)
        {
            boxCollider[i].size -= new Vector3(0, crouchDeltaHeight, 0);
            boxCollider[i].center -= new Vector3(0, crouchDeltaHeight / 2, 0);
        }
        for (int i = 0; i < capsuleColliders.Length; i++)
        {
            capsuleColliders[i].height -= crouchDeltaHeight;
            capsuleColliders[i].center -= new Vector3(0, crouchDeltaHeight / 2, 0);
        }

        Invoke("stopCrouching", crouchDuration);
    }

    protected virtual void stopCrouching()
    {
        anim.SetBool("Crouch", false);
        crouching = false;

        for (int i = 0; i < boxCollider.Length; i++)
        {
            boxCollider[i].size += new Vector3(0, crouchDeltaHeight, 0);
            boxCollider[i].center += new Vector3(0, crouchDeltaHeight / 2, 0);
        }
        for (int i = 0; i < capsuleColliders.Length; i++)
        {
            capsuleColliders[i].height += crouchDeltaHeight;
            capsuleColliders[i].center += new Vector3(0, crouchDeltaHeight / 2, 0);
        }
    }

    public void Die()
    {
        RunEventServices.GameMechanicAction.GameOver?.Invoke();

        dead = true;
        m_IsGrounded = true;
        anim.SetBool("GameOver", true);

        StartCoroutine(waitForDeathAnim());

        SaveRecords();
    }

    IEnumerator waitForDeathAnim()
    {
        yield return new WaitForSeconds(2f);
        manager.SettingsBtn.SetActive(true);
        manager.GamePanel.SetActive(false);
        manager.GameOverPanel.SetActive(true);
    }

    protected virtual void Jump()
    {
        RaycastHit hitInfo;
        if (!Physics.Raycast(myTransform.position + Vector3.up, Vector3.up, out hitInfo, 2f, levelLayer))
        {
            m_Jump = true;
            if (crouching)
            {
                CancelInvoke("stopCrouching");
                stopCrouching();
            }
        }
    }

    protected virtual void MoveSide(bool right)
    {
        if (m_IsGrounded)
        {
            anim.SetBool("Side", right);
            anim.SetTrigger("JumpSide");

            RunEventServices.GameMechanicAction.LeftMove?.Invoke();
            
        }
        if (crouching)
        {
            CancelInvoke("stopCrouching");
            stopCrouching();
        }
        
        RaycastHit hitInfo;

        if (!right)
        {
            if (!Physics.Raycast((myTransform.position + Vector3.up * 0.1f) + (Vector3.forward / 8f), Vector3.left, out hitInfo, 2f, levelLayer)
             && !Physics.Raycast((myTransform.position + Vector3.up * 0.1f) - (Vector3.forward / 8f), Vector3.left, out hitInfo, 2f, levelLayer))
                posX = posX - stepDistance;
        }
        else
        {
            if (!Physics.Raycast((myTransform.position + Vector3.up * 0.1f) + (Vector3.forward / 8f), Vector3.right, out hitInfo, 2f, levelLayer)
             && !Physics.Raycast((myTransform.position + Vector3.up * 0.1f) - (Vector3.forward / 8f), Vector3.right, out hitInfo, 2f, levelLayer))
                posX = posX + stepDistance;
        }
    }

    protected bool HasMouseMoved()
    {
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    protected virtual void Update()
    {
        if (!dead && manager.play && !manager.cameraLerp && Time.timeScale != 0)
        {
            if (Time.timeScale != 0)
            {
                time += Time.deltaTime * timeMultiplier;
                manager.timeScore.text = (time * timeScoreMultiplier).ToString("F0");
            }
           
            CheckGroundStatus();

            if (animForward < 1f)
            {               
                anim.SetFloat("Forward", animForward);
                animForward += Time.deltaTime * 2f;
            }
            else
                anim.SetFloat("Forward", 1f);

            if (!crouching)
                anim.SetBool("OnGround", m_IsGrounded);

            if (!m_IsGrounded)            
                anim.SetFloat("Jump", m_Rigidbody.velocity.y);            


            if (mySwipe.Tap)
            {
                swiped = false;
                fp = Input.mousePosition;
                lp = Input.mousePosition;
            }

            if (mySwipe.SwipeLeft)
            {
                swiped = true;
                MoveSide(false);
            }

            if (mySwipe.SwipeRight)
            {
                swiped = true;
                MoveSide(true);
            }
            if (mySwipe.SwipeDown)
            {
                if (m_IsGrounded && !crouching)
                {
                    swiped = true;
                    Crouch();
                }
            }

            if (mySwipe.SwipeUp)
            {
                if (!m_Jump)
                {
                    swiped = true;
                    Jump();
                }
            }


            if (m_IsGrounded)
            {
                HandleGroundedMovement(m_Jump);
            }                     
            
            speed += Time.deltaTime * speedModifier;
                        
            if(timeMultiplierTime > 0)
            {
                timeMultiplierTime -= Time.deltaTime;
                if (timeMultiplierTime <= 0)
                {
                    timeMultiplierTime = 0;
                    timeMultiplier = 1f;
                }                          
            }
            if (coinsMultiplierTime > 0)
            {
                coinsMultiplierTime -= Time.deltaTime;
                if (coinsMultiplierTime <= 0)
                {
                    coinsMultiplierTime = 0;
                    coinsMultiplier = 1f;
                }               
            }
            if (jumpMultiplierTime > 0)
            {
                jumpMultiplierTime -= Time.deltaTime;
                if (jumpMultiplierTime <= 0)
                {
                    jumpMultiplierTime = 0;
                    jumpMultiplier = 1f;
                }               
            }

            m_Jump = false;
        }
        if(manager.cameraLerp)
        {
            anim.SetFloat("Forward", 0.5f);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!dead && manager.play)
        {
            if(!m_IsGrounded)
            {
                HandleAirborneMovement();
            }

            if (!manager.cameraLerp)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, new Vector3(posX, myTransform.position.y, myTransform.position.z), 0.15f);
               
                if (hitSpeedLoc >= speed)
                {
                    myTransform.Translate(0, 0, speed * Time.deltaTime);
                }
                else
                {
                    myTransform.Translate(0, 0, hitSpeedLoc * Time.deltaTime);
                    hitSpeedLoc += Time.fixedDeltaTime * hitSpeedDecreaseSpeed;
                }
            }

            if (!manager.cameraLerp)
                manager.cameraTransform.position = new Vector3(manager.cameraTransform.position.x, manager.cameraTransform.position.y, transform.position.z + cameraPosition.z);
            else           
                manager.cameraTransform.position = Vector3.Lerp(manager.cameraTransform.position, new Vector3(manager.cameraTransform.position.x, manager.cameraTransform.position.y, transform.position.z + cameraPosition.z), cameraSpeed * 3f);                          

            manager.cameraTransform.position = Vector3.Lerp(manager.cameraTransform.position, new Vector3(myTransform.position.x, myTransform.position.y + cameraPosition.y, myTransform.position.z + cameraPosition.z), cameraSpeed);
        }
        if (manager.play)
        {
            if (!manager.cameraLerp)
            {                                
                if (manager.pursuitTimeLoc > 0)
                {
                    if(!dead)
                    {
                        manager.pursuerTransform.position = Vector3.Lerp(manager.pursuerTransform.position, new Vector3(manager.pursuerTransform.position.x, manager.pursuerTransform.position.y, transform.position.z - pursuerPositionOffset), 0.4f);
                        manager.pursuerTransform.position = Vector3.Lerp(manager.pursuerTransform.position, new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z - pursuerPositionOffset), 0.1f);
                    }
                    else
                    {
                        manager.pursuerTransform.position = Vector3.Lerp(manager.pursuerTransform.position, new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z - (pursuerPositionOffset - 1f)), 0.1f);
                    }
                }
            }
            else
                manager.pursuerTransform.position = Vector3.Lerp(manager.pursuerTransform.position, new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z - pursuerPositionOffset), 0.1f);
        }
    }

    protected virtual void CheckGroundStatus()
    {
        RaycastHit hitInfo;

        // 0.3f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(myTransform.position + (Vector3.up * 0.3f), Vector3.down, out hitInfo, m_GroundCheckDistance + 0.3f, levelLayer))
        {            
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            //m_Animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            //m_Animator.applyRootMotion = false;
        }
    }

    protected virtual void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * 9f) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);

        m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
    }

    protected virtual void HandleGroundedMovement(bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump)
        {
            // jump!
            RunEventServices.GameMechanicAction.Jump?.Invoke();
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower * jumpMultiplier, m_Rigidbody.velocity.z);
            m_IsGrounded = false;
            m_GroundCheckDistance = 0.1f;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!dead)
        {
            if (other.tag == "Bonus")
            {
                Bonus bonus = other.GetComponent<Bonus>();
                Transform effect = Instantiate(bonus.effect, myTransform.position + Vector3.up * capsuleColliders[0].center.y, Quaternion.identity).transform;
                effect.SetParent(myTransform);

                if (bonus.sound != null)
                {
                    RunEventServices.GameMechanicAction.BonusSound?.Invoke(bonus.sound);
                }

                if (bonus.isCoin)
                {
                    coins += (int)(bonus.multiplier * coinsMultiplier);
                }

                other.GetComponent<Collider>().enabled = false;
                Destroy(other.gameObject);

                manager.coinsScore.text = coins.ToString();
            }
        }
    }

    public virtual void SaveRecords()
    {                
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coins);
        manager.CoinsTxt.text = (PlayerPrefs.GetInt("Coins")).ToString();

        manager.GameOverCoinsTxt.text = manager.CoinsTxt.text;

        coins = 0;

        manager.gameOverScoreTxt.text = (time * timeScoreMultiplier).ToString("F0");

        if (float.Parse(manager.maxTimeScore.text) < (time * timeScoreMultiplier))
        {
            manager.maxTimeScore.text = (time * timeScoreMultiplier).ToString("F0");
            manager.congratulationsTxt.enabled = true;

            PlayerPrefs.SetFloat("MaxTimeScore", (time * timeScoreMultiplier));
        }
        else
            manager.congratulationsTxt.enabled = false;

        PlayerPrefs.Save();
    }

}
