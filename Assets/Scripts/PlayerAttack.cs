using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{

    VirtualJoystick joystick;

    public GameObject go_energyBall;
    public GameObject go_energyBall2;

    float dirHorizontal;
    float dirVertical;
    Vector3 directionEnergy;

    PlayerAttributes pa;

    Transform tf_parentEnergyBall;


    float angle;

    AudioSource soundAttack;

    Animator anim;


    // Use this for initialization
    void Start()
    {
        anim = GetComponentInParent<Animator>();

        pa = transform.GetComponentInParent<PlayerAttributes>();
        tf_parentEnergyBall = GameObject.Find("GameObjects/Power").transform;
        soundAttack = GetComponent<AudioSource>();

#if UNITY_ANDROID

        joystick = GameObject.Find("VirtualJoystickBackground").GetComponent<VirtualJoystick>();

        if (pa.isLocalPlayer)
        {
            GameObject.Find("btn_Attack").GetComponent<Button>().onClick.AddListener(comandAtack1);
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


#if UNITY_ANDROID
        dirHorizontal = joystick.Horizontal();
        dirVertical = joystick.Vertical();
#else
        dirHorizontal = Input.GetAxis(pa.Inputs[0]);
        dirVertical = Input.GetAxis(pa.Inputs[1]);
#endif

        //dirHorizontal = Input.GetAxis(pa.Inputs[0]);
        //dirVertical = Input.GetAxis(pa.Inputs[1]);

        if (dirVertical != 0 || dirHorizontal != 0)
        {

            angle = Mathf.Atan2(dirHorizontal, dirVertical) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }




        if (Input.GetButtonDown(pa.Inputs[3]))
        {

            comandAtack1();
        }



        //if (Input.GetButtonDown(pa.Inputs[4]))
        //{
        //    CmdShootEnergyBall(1);
        //}

    }


    public void comandAtack1()
    {
        if (pa.multiplayerOnline)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().CommandShoot(-angle);
        }
        else
        {
            CmdShootEnergyBall(0, -angle);
        }
    }



    public void CmdShootEnergyBall(int typeEnemyBall, float angle)
    {

        //pega a direção q o jogador esta movimentado


        //verifica para qual lado o jogador esta olhando
        if (dirHorizontal == 0 && dirVertical == 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, (GetComponentInParent<SpriteRenderer>().flipX ? 90 : -90));
        }
        transform.rotation = Quaternion.Euler(0, 0, angle);

        //instancia a bola de energia
        GameObject newEnergyBall = Instantiate((typeEnemyBall == 0 ? go_energyBall : go_energyBall2), transform.position, transform.rotation) as GameObject;
        newEnergyBall.name = "EnergyBall_" + pa.namePlayer;
        newEnergyBall.transform.SetParent(tf_parentEnergyBall);

        //aplica os valores a bola de energia
        EnergyBallBehaviour ebb = newEnergyBall.GetComponent<EnergyBallBehaviour>();
        ebb.idPlayerParent = pa.idPlayer;
        ebb.namePlayerParent = pa.namePlayer;
        ebb.damage = (typeEnemyBall == 0 ? pa.damage[0] : pa.damage[1]);
        ebb.isLocalPlayer = pa.isLocalPlayer;

        soundAttack.Play();

        anim.SetTrigger("attack");
    }



}
