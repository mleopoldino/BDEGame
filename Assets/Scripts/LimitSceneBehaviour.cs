using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitSceneBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //verifica se teve collision com algum jogador
        if (collision.CompareTag("Player"))
        {

            if (collision.GetComponent<PlayerAttributes>().isLocalPlayer)
            {
                collision.GetComponent<PlayerHealth>().TakeDamage(5);
            }

        }
    }
}
