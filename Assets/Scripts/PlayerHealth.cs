using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    PlayerAttributes pa;
    public Text txt_health;

    public RectTransform rectTransform_HealthBar;
    float initialWidth = 0f;


    // Use this for initialization
    void Start()
    {
        pa = GetComponent<PlayerAttributes>();
        //txt_health.text = pa.healthCurrent.ToString();
        initialWidth = rectTransform_HealthBar.sizeDelta.x;
    }

    //aplica dano ao jogador
    public void TakeDamage(int damage)
    {

        if (pa.healthCurrent > 0)
        {
            //pa.healthCurrent -= damage;
            //pa.healthCurrent = (pa.healthCurrent < 0 ? 0 : pa.healthCurrent);
            //txt_health.text = pa.healthCurrent.ToString();        
            if (pa.multiplayerOnline)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().CommandHealthChange(gameObject.name, damage);
            }
            else
            {
                OnChangeHealth(damage);
            }
            
        }
    }



    public void OnChangeHealth(int damage)
    {

        pa.healthCurrent -= damage;
        pa.healthCurrent = (pa.healthCurrent < 0 ? 0 : pa.healthCurrent);

        rectTransform_HealthBar.sizeDelta = new Vector2(((float)pa.healthCurrent / pa.healthMAX) * initialWidth, rectTransform_HealthBar.sizeDelta.y);
        
        //jogador morreu
        if (pa.healthCurrent == 0)
        {
            //GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SetDefeatPlayer(pa.idPlayer);
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SetDefeatPlayer(pa.name);
            pa.isAlive = false;
            //desabilita as colisões
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
        }

    }



}
