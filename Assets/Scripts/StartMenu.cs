using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SocketIO;

public class StartMenu : MonoBehaviour
{


    public GameObject[] panels;
    int panelCurrentId;
    public int[] selectedPlayer;
    string comandSelectPlayer = "";
    public GameManager_globalVariables gm_gv;
    //bool multiplayerOnline = false;

    public GameObject[] go_SelectPlayer;


    public GameObject go_FirstButton;
    public InputField inputField_UrlServer;
    public InputField inputField_PlayerName;


    public SocketIOComponent socket;

    public void Start()
    {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Debug.Log("socket.url: " + socket.url);
        //inputField_UrlServer.text = socket.url;
        //Debug.Log("txt_UrlServer: " + inputFIeld_UrlServer.text);

        //StartCoroutine(Select2PlayerAutomatic());

        inputField_PlayerName.text = PlayerPrefs.GetString("PlayerName");
    }

    public void StartGame()
    {

        //selecao de personagem
        for (int i = 0; i < gm_gv.players.Length; i++)
        {
            if (selectedPlayer[i] == 0)
            {
                gm_gv.players[i] = "Jorge";
            }
            else if (selectedPlayer[i] == 1)
            {
                gm_gv.players[i] = "Paulao";
            }
            else if (selectedPlayer[i] == 2)
            {
                gm_gv.players[i] = "Glutius";
            }
            else
            {
                gm_gv.players[i] = "Yelva";
            }

        }


        SceneManager.LoadScene("Phase0", LoadSceneMode.Single);

        EventSystem.current.SetSelectedGameObject(go_FirstButton);

    }


    public void ChangePanel(int id)
    {

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        if (id == 4)
        {
            if (gm_gv.multiplayerOnline)
            {
                panelCurrentId = id;
                panels[id].SetActive(true);



                //socket.url = inputField_UrlServer.text;
                //Debug.Log(inputField_UrlServer.text);

                //socket.Connect();
                NetworkManager.instance.JoinGame();

            }
            else
            {
                StartGame();
                //Destroy(GameObject.Find("Network Manager"));
            }
        }
        else
        {
            panelCurrentId = id;
            panels[id].SetActive(true);
        }


    }


    public void SelectPlayer(int idPlayer)
    {
        selectedPlayer[0] = idPlayer;
    }

    public void SelectGameMode(string mode)
    {
        if (mode == "Online")
        {

            if (inputField_PlayerName.text == "")
                inputField_PlayerName.text = "Player" + Random.Range(0, 100);

            PlayerPrefs.SetString("PlayerName", inputField_PlayerName.text);

            gm_gv.multiplayerOnline = true;
            //NetworkManager.instance.JoinGame();
            //NetworkManager.instance.PlayGame();

            go_SelectPlayer[1].SetActive(false);

        }
        else
        {
            gm_gv.multiplayerOnline = false;
            go_SelectPlayer[1].SetActive(true);
        }

        ChangePanel(3);
        //StartCoroutine(SelectPlayerReadyAutomatic());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ChangePanel(0);
        }


        if (panelCurrentId == 0)
        {
            if (Input.GetButtonDown("Fire1_P1"))
            {
                SelectGameMode("2Player");
            }

            if (Input.GetButtonDown("Fire2_P1"))
            {
                SelectGameMode("Online");
            }
        }
        else if (panelCurrentId == 3)
        {
            SelectPlayer1();
            if (!gm_gv.multiplayerOnline)
            {
                SelectPlayer2();
            }

            if (Input.GetButtonDown("Fire1_P1"))
            {
                ChangePanel(4);
            }
        }
    }


    void SelectPlayer1()
    {

        float h = Input.GetAxis("Horizontal_P1");
        float v = Input.GetAxis("Vertical_P1");


        if (comandSelectPlayer == string.Empty)
        {
            if (h != 0)
            {
                comandSelectPlayer = (Mathf.Sign(h) == 1 ? "D" : "E");

                if (comandSelectPlayer == "D" && (selectedPlayer[0] == 0 || selectedPlayer[0] == 2))
                {
                    selectedPlayer[0] += 1;
                }
                else if (comandSelectPlayer == "E" && (selectedPlayer[0] == 1 || selectedPlayer[0] == 3))
                {
                    selectedPlayer[0] -= 1;
                }

                go_SelectPlayer[0].transform.SetParent(GameObject.Find(string.Format("btn_Player{0}", selectedPlayer[0])).transform);
                go_SelectPlayer[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(-40, 60, 0);
            }
            else if (v != 0)
            {
                comandSelectPlayer = (Mathf.Sign(v) == 1 ? "C" : "B");
                if (comandSelectPlayer == "C" && (selectedPlayer[0] == 2 || selectedPlayer[0] == 3))
                {
                    selectedPlayer[0] -= 2;
                }
                else if (comandSelectPlayer == "B" && (selectedPlayer[0] == 0 || selectedPlayer[0] == 1))
                {
                    selectedPlayer[0] += 2;
                }
                go_SelectPlayer[0].transform.SetParent(GameObject.Find(string.Format("btn_Player{0}", selectedPlayer[0])).transform);
                go_SelectPlayer[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(-40, 60, 0);
            }

        }


        if (h == 0 && v == 0 && comandSelectPlayer != string.Empty)
        {
            comandSelectPlayer = string.Empty;
        }

    }



    void SelectPlayer2()
    {

        float h = Input.GetAxis("Horizontal_P2");
        float v = Input.GetAxis("Vertical_P2");


        if (comandSelectPlayer == string.Empty)
        {
            if (h != 0)
            {
                comandSelectPlayer = (Mathf.Sign(h) == 1 ? "D" : "E");

                if (comandSelectPlayer == "D" && (selectedPlayer[1] == 0 || selectedPlayer[1] == 2))
                {
                    selectedPlayer[1] += 1;
                }
                else if (comandSelectPlayer == "E" && (selectedPlayer[1] == 1 || selectedPlayer[1] == 3))
                {
                    selectedPlayer[1] -= 1;
                }

                go_SelectPlayer[1].transform.SetParent(GameObject.Find(string.Format("btn_Player{0}", selectedPlayer[1])).transform);
                go_SelectPlayer[1].GetComponent<RectTransform>().anchoredPosition = new Vector3(40, 60, 0);
            }
            else if (v != 0)
            {
                comandSelectPlayer = (Mathf.Sign(v) == 1 ? "C" : "B");
                if (comandSelectPlayer == "C" && (selectedPlayer[1] == 2 || selectedPlayer[1] == 3))
                {
                    selectedPlayer[1] -= 2;
                }
                else if (comandSelectPlayer == "B" && (selectedPlayer[1] == 0 || selectedPlayer[1] == 1))
                {
                    selectedPlayer[1] += 2;
                }
                go_SelectPlayer[1].transform.SetParent(GameObject.Find(string.Format("btn_Player{0}", selectedPlayer[1])).transform);
                go_SelectPlayer[1].GetComponent<RectTransform>().anchoredPosition = new Vector3(40, 60, 0);
            }

        }


        if (h == 0 && v == 0 && comandSelectPlayer != string.Empty)
        {
            comandSelectPlayer = string.Empty;
        }

    }


    IEnumerator Select2PlayerAutomatic()
    {
        yield return new WaitForSeconds(10);
        if (panelCurrentId == 0)
            SelectGameMode("2Player");
    }

    IEnumerator SelectPlayerReadyAutomatic()
    {
        yield return new WaitForSeconds(10);
        if (panelCurrentId == 3)
            ChangePanel(4);
    }

}
