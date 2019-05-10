using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoviment : MonoBehaviour
{

    VirtualJoystick joystick;


    Vector3 oldPosition;
    Vector3 currentPosition;

    public string direction;


    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    float inputHorizontal;

    PlayerAttributes pa;



    bool walk;


    //Jump
    Vector3 distanceFloor = new Vector3(0, 0.95f, 0);
    //bool onFloor = true;
    int countJump;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        pa = GetComponent<PlayerAttributes>();


        oldPosition = transform.position;
        currentPosition = oldPosition;



#if UNITY_ANDROID

        joystick = GameObject.Find("VirtualJoystickBackground").GetComponent<VirtualJoystick>();

        if (pa.isLocalPlayer)
        {
            GameObject.Find("btn_Jump").GetComponent<Button>().onClick.AddListener(CommandJump);
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {

        if (!pa.isLocalPlayer)
            return;

        if (GameManager.instance.stateGameCurrent != GameManager.StateGame.Running)
            return;

        if (pa.isAlive)
        {


            //Debug.DrawLine(transform.position, transform.position - Vector3.up + distanceFloor);
            RaycastHit2D ray;
            ray = Physics2D.Linecast(transform.position, transform.position - Vector3.up + distanceFloor, -1);


            //verifica se existe algum objeto abaixo do jogador
            if (ray)
            {
                //verifica se o player esta no chao
                if (ray.transform.CompareTag("Scene") && countJump > 0)
                {
                    countJump = 0;
                    ChangeAnimationNet("jump", countJump);
                    //ChangeAnimationNet("walk", false);

                }
            }


            //pega o input horizontal do jogador
            //inputHorizontal = Input.GetAxis(pa.Inputs[0]);
            //Android
            //inputHorizontal = joystick.Horizontal();
#if UNITY_ANDROID
            inputHorizontal = joystick.Horizontal();
#else
            inputHorizontal = Input.GetAxis(pa.Inputs[0]);
#endif

            //anima o personagem e transforma a imagem
            if (inputHorizontal != 0)
            {
                if (!walk)
                {
                    walk = true;
                    //anim.SetBool("walk", walk);
                    ChangeAnimationNet("walk", walk);
                }

                //if (inputHorizontal > 0 && sr.flipX)
                if (inputHorizontal > 0 && direction == "E")
                {
                    //sr.flipX = false;
                    //direction = "E";
                    ChangeDirection("D");
                }
                //else if (inputHorizontal < 0 && !sr.flipX)
                else if (inputHorizontal < 0 && direction == "D")
                {
                    //sr.flipX = true;
                    //direction = "D";
                    ChangeDirection("E");
                }

            }
            else
            {
                if (walk)
                {
                    walk = false;
                    //anim.SetBool("walk", walk);
                    ChangeAnimationNet("walk", walk);
                }
            }


            //verifica o botao de pulo
            //o maximo de pulo que pode dar é 2
            if (Input.GetButtonDown(pa.Inputs[2]))
            {
                CommandJump();

            }


            if (Input.GetButtonDown("Cancel"))
            {
                GameManager.instance.ChangeStateGamePause("s");
            }


        }

    }



    void CommandJump()
    {

        if (countJump < 2)
        {

            countJump++;
            //anim.SetInteger("jump",countJump);
            ChangeAnimationNet("jump", countJump);
            if (countJump == 2)
            {
                Vector2 newVelocity = rb.velocity;
                newVelocity.y = 0;
                rb.velocity = newVelocity;
            }

            rb.AddForce(new Vector2(0, pa.jumpForce));

        }

    }

    void FixedUpdate()
    {
        if (!pa.isLocalPlayer)
            return;


        //transforma o personagem na horizontal
        rb.velocity = new Vector2(inputHorizontal * pa.speed, rb.velocity.y);
        //rb.velocity = new Vector2(joystick.Horizontal() * pa.speed, rb.velocity.y);

        if (pa.multiplayerOnline)
        {

            currentPosition = transform.position;

            if (currentPosition != oldPosition)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().CommandMove(transform.position, direction);
                oldPosition = currentPosition;
            }

        }

    }

    public void ChangeDirection(string newDirection)
    {

        if (direction != newDirection)
        {
            direction = newDirection;
            //sr.flipX = (direction == "D" ? true : false);
            //Vector3 newScale = transform.localScale;
            //newScale.x = (direction == "D" ? 0.5f : -0.5f);
            //transform.localScale = newScale;

            transform.localScale = new Vector3((direction == "D" ? 0.5f : -0.5f), 0.5f, 1f);
            //Debug.Log("Nova Escala: " + new Vector3((direction == "D" ? 0.5f : -0.5f), 0.5f, 1f));
        }
    }

    public void ChangeAnimationNet(string nameVar, bool value)
    {
        anim.SetBool(nameVar, value);

        if (pa.multiplayerOnline)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().CommandAnimation(gameObject.name, nameVar, value);
        }
    }

    public void ChangeAnimationNet(string nameVar, int value)
    {
        anim.SetInteger(nameVar, value);
        if (pa.multiplayerOnline)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().CommandAnimation(gameObject.name, nameVar, value);
        }
    }


    public void ChangeAnimation(string nameVar, string value)
    {
        if (value == "t")
        {
            anim.SetBool(nameVar, true);
        }
        else if (value == "f")
        {
            anim.SetBool(nameVar, false);
        }
        else
        {
            anim.SetInteger(nameVar, int.Parse(value));
        }


    }
}
