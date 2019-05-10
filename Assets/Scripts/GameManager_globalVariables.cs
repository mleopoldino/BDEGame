using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_globalVariables : MonoBehaviour {

    public int countPlayers;
    public int[] roundPlayerWin_total;

    public int countRound_total;
    public int countRound_current;


    public string[] players = new string[] {"",""};
    public bool multiplayerOnline = false;

    public int idSceneCurrent = -1;

    // Use this for initialization
    void Start () {

        DontDestroyOnLoad(this);


        roundPlayerWin_total = new int[countPlayers];
        for (int i = 0; i < roundPlayerWin_total.Length; i++)
        {
            roundPlayerWin_total[i] = 0;
        }

    }

}
