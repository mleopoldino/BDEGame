using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{

    public int idPlayer;
    public string namePlayer;

    public bool multiplayerOnline = false;
    public bool isLocalPlayer = false;


    public int healthMAX;
    public int healthCurrent;

    public bool isAlive = true;

    public float speed;
    public float jumpForce;
    public int[] damage;


    string[] inputs = new string[6];

    public string[] Inputs
    {
        get
        {
            return inputs;
        }

        set
        {
            inputs = value;
        }
    }

    private void Start()
    {
        string valuePlayer = (idPlayer + 1).ToString();

        inputs[0] = string.Format("Horizontal_P{0}", valuePlayer);
        inputs[1] = string.Format("Vertical_P{0}", valuePlayer);
        inputs[2] = string.Format("Jump_P{0}", valuePlayer);
        inputs[3] = string.Format("Fire1_P{0}", valuePlayer);
        inputs[4] = string.Format("Fire2_P{0}", valuePlayer);
        inputs[5] = "Cancel";
    }



    public void Init(bool gameOnline)
    {
        string valuePlayer = ((gameOnline) ? 1 : (idPlayer + 1)).ToString();



        inputs[0] = string.Format("Horizontal_P{0}", valuePlayer);
        inputs[1] = string.Format("Vertical_P{0}", valuePlayer);
        inputs[2] = string.Format("Jump_P{0}", valuePlayer);
        inputs[3] = string.Format("Fire1_P{0}", valuePlayer);
        inputs[4] = string.Format("Fire2_P{0}", valuePlayer);

    }


}
