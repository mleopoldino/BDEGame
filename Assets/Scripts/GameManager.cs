using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    GameManager_globalVariables gmGlobalVar;




    [Header("Game Manager")]
    public int[] roundPlayerWin_current;
    bool lastRound = false;

    bool readyLobby = false;


    [Header("UI Manager")]
    public Animator anim;

    public enum StateGame
    {
        BeginRound,
        Running,
        Pause,
        ShowScoreBoard,
        GameOver
    }
    public StateGame stateGameCurrent;
    public Text txt_Round;
    public Text txt_ScoreBoard;

    public GameObject panel_Pause;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }


    // Use this for initialization
    void Start()
    {

        //se for android
#if !UNITY_ANDROID
        GameObject.Find("VirtualButtons").SetActive(false);
#endif

        //Carrega as informações Globais-----------------------------------------------

        gmGlobalVar = GameObject.Find("Gm_GlobalVariables").GetComponent<GameManager_globalVariables>();
        //incrementa a quantidade de round
        gmGlobalVar.countRound_current++;

        //verifica se é o ultimo round
        if (gmGlobalVar.countRound_total != -1)
        {
            if (gmGlobalVar.countRound_total == gmGlobalVar.countRound_current)
            {
                lastRound = true;
            }
        }


        //carrega o cenario-----------------------------------------------

        int countScenesAvailable = 4;//Directory.GetFiles(Application.dataPath + "/Resources/Scenes", "*.prefab").Length;

        gmGlobalVar.idSceneCurrent++;
        if (gmGlobalVar.idSceneCurrent >= countScenesAvailable)
        {
            gmGlobalVar.idSceneCurrent = 0;
        }

        string sceneName = string.Format("Scene{0}", gmGlobalVar.idSceneCurrent);
        //string sceneName = "Scene0";


        GameObject newScene = Instantiate(Resources.Load(string.Format("Scenes/{0}", sceneName))) as GameObject;
        newScene.transform.SetParent(GameObject.Find("GameObjects/Scene").transform);
        newScene.transform.position = new Vector3(0, 0, 0);
        newScene.name = sceneName;
        Debug.Log("Load Scene: " + sceneName);

        //carrega os jogadores, conforme foi selecionado no menu seleção de personagem----------------------------------------
        if (gmGlobalVar.multiplayerOnline)
        {
            NetworkManager.instance.PlayGame();
        }
        else
        {
            for (int i = 0; i < gmGlobalVar.countPlayers; i++)
            {
                GameObject newPlayer = Instantiate(Resources.Load(string.Format("Characters/{0}", gmGlobalVar.players[i]))) as GameObject;
                newPlayer.name = string.Format("P{0}", i + 1);
                newPlayer.GetComponent<PlayerAttributes>().idPlayer = i;
                newPlayer.GetComponent<PlayerAttributes>().isLocalPlayer = true;
                newPlayer.GetComponent<PlayerAttributes>().namePlayer = newPlayer.name;
                newPlayer.GetComponent<PlayerAttributes>().multiplayerOnline = false;
                newPlayer.transform.Find("Canvas/txt_nomePlayer").GetComponent<Text>().text = newPlayer.name;
                //newPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                newPlayer.transform.position = GameObject.Find(string.Format("GameObjects/Scene/{0}/Respawns/p{1}", sceneName, i)).transform.position;

                newPlayer.GetComponent<PlayerMoviment>().direction = (i == 0 ? "D" : "E");
                newPlayer.transform.localScale = new Vector3((i == 0 ? 0.5f : -0.5f), 0.5f, 0);

            }
        }




        //Coloca todos os jogadores como vivo
        roundPlayerWin_current = new int[gmGlobalVar.countPlayers];
        for (int i = 0; i < roundPlayerWin_current.Length; i++)
        {
            roundPlayerWin_current[i] = 1;
        }




        stateGameCurrent = StateGame.BeginRound;
        txt_Round.text = string.Format("Round {0}", gmGlobalVar.countRound_current.ToString());




        anim.SetTrigger("BeginRound");



    }


    public void SetDefeatPlayer(int id)
    {
        roundPlayerWin_current[id] = 0;


        //verifica quantos jogadores ainda estão jogando
        int qtd = 0;
        for (int i = 0; i < roundPlayerWin_current.Length; i++)
        {
            qtd += roundPlayerWin_current[i];
        }

        if (qtd == 1)
        {
            if (gmGlobalVar.multiplayerOnline)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().CommandGameOver();
            }
            else
            {
                GameOver();
            }
        }

    }

    public void SetDefeatPlayer(string nameDefeated)
    {

        //verifica quantos jogadores ainda estão jogando
        int qtd = 0;

        if (gmGlobalVar.multiplayerOnline)
        {
            for (int i = 0; i < NetworkManager.instance.listScoreBoardPlayers.Count; i++)
            {
                if (NetworkManager.instance.listScoreBoardPlayers[i].name == nameDefeated)
                {
                    roundPlayerWin_current[i] = 0;
                }
                qtd += roundPlayerWin_current[i];
            }

            if (qtd == 1)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().CommandGameOver();
            }



        }
        else
        {
            for (int i = 0; i < roundPlayerWin_current.Length; i++)
            {
                if ((i + 1).ToString() == nameDefeated.Substring(1, 1))
                {
                    roundPlayerWin_current[i] = 0;
                }
                qtd += roundPlayerWin_current[i];
            }


            if (qtd == 1)
            {
                GameOver();

            }
        }

    }



    private void Update()
    {
        if (stateGameCurrent == StateGame.BeginRound && anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {

            GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < gos.Length; i++)
            {
                gos[i].GetComponent<PlayerMoviment>().enabled = true;
                gos[i].transform.Find("go_attackPos").GetComponent<PlayerAttack>().enabled = true;
                if (gos[i].GetComponent<PlayerAttributes>().isLocalPlayer)
                {
                    gos[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                else
                {
                    gos[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                }

            }


            stateGameCurrent = StateGame.Running;
        }
        if (stateGameCurrent == StateGame.GameOver)
        {
            if (Input.anyKey && !readyLobby)
            {
                readyLobby = true;
                if (gmGlobalVar.multiplayerOnline)
                {
                    NetworkManager.instance.CommandReadyLobby();
                }
                else
                {
                    LoadPhase();
                }
            }

        }
        else if (stateGameCurrent == StateGame.Pause)
        {
            if (Input.GetButtonDown("Fire1_P1"))
            {
                BackToStartMenu();
            }

            if (Input.GetButtonDown("Fire2_P1"))
            {
                ChangeStateGamePause("n");
            }

            if (Input.GetButtonDown("Cancel"))
            {
                ChangeStateGamePause("n");
            }
        }




    }

    public void LoadPhase()
    {
        SceneManager.LoadScene("Phase0", LoadSceneMode.Single);
    }

    public void BackToStartMenu()
    {
        Time.timeScale = 1;

        Destroy(GameObject.Find("Gm_GlobalVariables"));
        Destroy(GameObject.Find("SocketIO"));
        if (gmGlobalVar.multiplayerOnline)
        {
            NetworkManager.instance.CommandDisconnect();
        }
        //Destroy(GameObject.Find("Network Manager"));
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);

    }


    public void GameOver()
    {

        //Monta o texto do quadro de resultados
        string txt_sb = string.Empty;


        if (gmGlobalVar.multiplayerOnline)
        {
            for (int i = 0; i < gmGlobalVar.roundPlayerWin_total.Length; i++)
            {
                //gmGlobalVar.roundPlayerWin_total[i] += roundPlayerWin_current[i];
                //txt_sb += string.Format("<color=#ffffffff>Player {0}:</color> <color=#ffff00ff><b>{1}</b></color>", (i + 1).ToString(), gmGlobalVar.roundPlayerWin_total[i].ToString()) + "\n";

                NetworkManager.instance.listScoreBoardPlayers[i].score += roundPlayerWin_current[i];
                txt_sb += string.Format("<color=#ffffffff>{0}:</color> <color=#ffff00ff><b>{1}</b></color>", NetworkManager.instance.listScoreBoardPlayers[i].name, NetworkManager.instance.listScoreBoardPlayers[i].score.ToString()) + "\n";
            }
        }

        else
        {

            for (int i = 0; i < gmGlobalVar.roundPlayerWin_total.Length; i++)
            {
                //gmGlobalVar.roundPlayerWin_total[i] += roundPlayerWin_current[i];
                //txt_sb += string.Format("<color=#ffffffff>Player {0}:</color> <color=#ffff00ff><b>{1}</b></color>", (i + 1).ToString(), gmGlobalVar.roundPlayerWin_total[i].ToString()) + "\n";

                gmGlobalVar.roundPlayerWin_total[i] += roundPlayerWin_current[i];
                txt_sb += string.Format("<color=#ffffffff>P{0}:</color> <color=#ffff00ff><b>{1}</b></color>", (i + 1).ToString(), gmGlobalVar.roundPlayerWin_total[i].ToString()) + "\n";
            }

        }






        //stateGameCurrent = StateGame.GameOver;
        stateGameCurrent = StateGame.ShowScoreBoard;

        txt_ScoreBoard.text = txt_sb;

        anim.SetTrigger("ShowScoreBoard");
        StartCoroutine(ChangeStateGameCurrent(StateGame.GameOver, 3f));
    }


    public void ChangeStateGamePause(string status)
    {
        if (status == "s")
        {
            StartCoroutine(ChangeStateGameCurrent(StateGame.Pause, 0f));
        }
        else
        {
            StartCoroutine(ChangeStateGameCurrent(StateGame.Running, 0f));
        }
    }



    IEnumerator ChangeStateGameCurrent(StateGame newState, float time)
    {
        yield return new WaitForSeconds(time);

        if (newState == StateGame.Pause)
        {
            Time.timeScale = 0;
            panel_Pause.SetActive(true);
        }
        else if (stateGameCurrent == StateGame.Pause && newState != StateGame.Pause)
        {
            Time.timeScale = 1;
            panel_Pause.SetActive(false);
        }

        stateGameCurrent = newState;
    }

}
