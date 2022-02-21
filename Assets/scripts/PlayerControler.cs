using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class PlayerControler : MonoBehaviour{
    

    //variaveis 
    private Animator playerAnimator;
    private Rigidbody2D playerRigidbody2d;
    private SpriteRenderer srPlayer;
    private bool playerInvencivel;

    public GameObject PlayerDie;

    public Transform GroundCheck;
    public bool isGround = false;
    
    public float speed;

    public float touchRun = 0.0f;

    public bool facingRight = true;

    public int vidas = 3;
    public Color hitColor;
    public Color noHitColor;

    //pulo
    public bool  jump =          false;
    public int   numberJumps =   0;
    public int   maximoJump =    2;
    public float jumpForce;

    private ControllerGame _ControleGame;

    public AudioSource fxGame;
    public AudioClip fxPulo;

    public ParticleSystem Poeira;

    // Start is called before the first frame update
    void Start()
    {

        playerAnimator = GetComponent<Animator>();
        playerRigidbody2d = GetComponent<Rigidbody2D>();
        srPlayer = GetComponent<SpriteRenderer>();

        _ControleGame = FindObjectOfType(typeof(ControllerGame)) as ControllerGame;
        
    }

    // Update is called once per frame
    void Update()
    {
        isGround = Physics2D.Linecast(transform.position, GroundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        playerAnimator.SetBool("isGrounded", isGround);

        touchRun = Input.GetAxisRaw("Horizontal");
        //Debug.Log(touchRun.ToString());

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        SetaMovimentos();
        
    }

    void MovePlayer(float movimentoH)
    {
        playerRigidbody2d.velocity = new Vector2(movimentoH * speed, playerRigidbody2d.velocity.y);

        if(movimentoH < 0 && facingRight || (movimentoH > 0 && !facingRight))
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer(touchRun);

        if (jump)
        {
            JumpPlayer();
        }
    }

    void JumpPlayer()
    {
       
        if (isGround)
        {
            numberJumps = 0;
            CriarPoeira();
        }


        if (isGround || numberJumps < maximoJump)
        {
            playerRigidbody2d.AddForce(new Vector2(0f, jumpForce));
            isGround = false;
            numberJumps++;

            //som do pulo
            fxGame.PlayOneShot(fxPulo);
            CriarPoeira();

        }
        jump = false;
    }

    void Flip()
    {
        CriarPoeira();

        facingRight = !facingRight;
        //Vector3 theScale = transform.localScale;
        //theScale.x *= -1;

        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void SetaMovimentos()
    {
        playerAnimator.SetBool("Walk", playerRigidbody2d.velocity.x != 0 && isGround);

        playerAnimator.SetBool("Jump", !isGround);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "coletaveis":

                _ControleGame.Pontuacao(1);

                Destroy(collision.gameObject);
                break;

            case "Inimigo":

                //animação
                GameObject tempExplosao = Instantiate(_ControleGame.hitprefab, transform.position, transform.localRotation);
                Destroy(tempExplosao, 0.5f);

                //adiciona força ao pulo
                Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, 800));

                //som da explosão
                _ControleGame.fxGame.PlayOneShot(_ControleGame.fxExplosao);

                //destroi o inimigo
                Destroy(collision.gameObject);
                    
                break;

            case "Damage":

                Hurt();

                break;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Plataforma":
                this.transform.parent = collision.transform;
                break;

            case "Inimigo":
                Hurt();
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Plataforma":
                this.transform.parent = null;
                break;
        }
    }

    void Hurt()
    {
        if (!playerInvencivel)
        {

            playerInvencivel = true;

            vidas--;
            StartCoroutine("Dano");

            _ControleGame.BarraVida(vidas);

            if(vidas < 1)
            {
                GameObject pDieTemp = Instantiate(PlayerDie, transform.position, Quaternion.identity);
                Rigidbody2D rbDie = pDieTemp.GetComponent<Rigidbody2D>();
                rbDie.AddForce(new Vector2(150f, 500f));

                _ControleGame.fxGame.PlayOneShot(_ControleGame.fxDie);

                Invoke("CarregaoJogo", 4f);
                gameObject.SetActive(false);

            }
        }
    }

    void CarregaoJogo()
    {
        EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Dano()
    {

        srPlayer.color = noHitColor;
        yield return new WaitForSeconds(0.1f);

        for (float i = 0; i<1; i += 0.1f)
        {
            srPlayer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            srPlayer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        srPlayer.color = Color.white;
        playerInvencivel = false;
    }

    void CriarPoeira()
    {
        Poeira.Play();
    }

}
