using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallBehaviour : MonoBehaviour
{

    float speed = 10f;

    public int idPlayerParent;
    public string namePlayerParent;
    public int damage;

    Rigidbody2D rb;


    public GameObject particleExplosion;


    public bool isLocalPlayer;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }



    private void FixedUpdate()
    {
        //movimenta o gameobject
        rb.MovePosition(transform.position + transform.up * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        //verifica se teve collision com algum jogador
        if (collision.CompareTag("Player"))
        {
            
            //verifica se é diferente o jogador que soltou o poder com o jogador que colidiu com o poder
            if (namePlayerParent != collision.GetComponent<PlayerAttributes>().namePlayer)
            {
                if (isLocalPlayer)
                {
                    collision.GetComponent<PlayerHealth>().TakeDamage(damage);
                }
                
                Destroy(this.gameObject, 0);


                //Instancia a particula de explosão
                GameObject ps = Instantiate(particleExplosion, transform.position, Quaternion.identity) as GameObject;
                Destroy(ps, 0.5f);
            }
        }
        else
        {
            Destroy(this.gameObject, 0);

            //Instancia a particula de explosão
            GameObject ps = Instantiate(particleExplosion, transform.position, Quaternion.identity) as GameObject;
            Destroy(ps, 0.5f);
        }
    }
}
