using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{




    public static NetworkManager instance;
    public SocketIOComponent socket;

    public string namePlayerLocal;


    public GameObject go_player;
    public InputField inputField_PlayerName;


    public List<ScoreBoardPlayers> listScoreBoardPlayers = new List<ScoreBoardPlayers>();



    public InputField inputField_UrlServer;
    public GameObject go_SocketIO;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        
    }


    // Use this for initialization
    void Start()
    {
        //socket.On("other player connected", OnOtherPlayerConnected);
        //socket.On("gameOver", OnGameOver);
        //socket.On("ready lobby", onReadyLobby);
        //socket.On("play", OnPlay);
        //socket.On("play2", OnPlay2);
        //socket.On("player move", OnPlayerMove);
        //socket.On("player shoot", OnPlayerShoot);
        //socket.On("player animation", OnPlayerAnimation);
        //socket.On("health", OnHealth);
        //socket.On("other player disconnect", OnOtherPlayerDisconnected);
        //socket.On("start game", OnStartGame);
    }


    public void JoinGame()
    {

        //go_SocketIO.GetComponent<SocketIOComponent>().url = inputField_UrlServer.text;
        //GameObject newSocketIO = Instantiate(go_SocketIO, transform.position, Quaternion.identity) as GameObject;
        //socket = newSocketIO.GetComponent<SocketIOComponent>();

        //socket.url = inputField_UrlServer.text;
        socket.Connect();
        

        socket.On("other player connected", OnOtherPlayerConnected);
        socket.On("gameOver", OnGameOver);
        socket.On("ready lobby", onReadyLobby);
        socket.On("play", OnPlay);
        socket.On("play2", OnPlay2);
        socket.On("player move", OnPlayerMove);
        socket.On("player shoot", OnPlayerShoot);
        socket.On("player animation", OnPlayerAnimation);
        socket.On("health", OnHealth);
        socket.On("other player disconnect", OnOtherPlayerDisconnected);
        socket.On("start game", OnStartGame);



        StartCoroutine(ConnectToServer());
    }

    #region Commands


    IEnumerator ConnectToServer()
    {

        if (namePlayerLocal == "")
        {
            if (inputField_PlayerName.text != "")
                namePlayerLocal = inputField_PlayerName.text;
            else
                namePlayerLocal = UnityEngine.Random.Range(1, 101).ToString();
        }

        //socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();


        int idPlayer = GameObject.Find("StartMenu").GetComponent<StartMenu>().selectedPlayer[0];

        UserJSON playerJSON = new UserJSON(namePlayerLocal, idPlayer);
        string data = JsonUtility.ToJson(playerJSON);



        yield return new WaitForSeconds(0.5f);
        socket.Emit("player connect", new JSONObject(data));
        //Application.ExternalCall("PlayerConnect", new JSONObject(data));
        yield return new WaitForSeconds(0.5f);



        //string playerName = inputField_PlayerName.text;
        //PlayerJSON playerJSON = new PlayerJSON(playerName);
        //string data = JsonUtility.ToJson(playerJSON);
        //socket.Emit("play", new JSONObject(data));
    }

    public void PlayGame()
    {
        UserJSON playerJSON = new UserJSON(namePlayerLocal);
        string data = JsonUtility.ToJson(playerJSON);
        socket.Emit("play2", new JSONObject(data));
        //Application.ExternalCall("Play2", new JSONObject(data));
        Debug.Log("Emit play2");
    }




    public void CommandMove(Vector3 vec3, string direction)
    {
        string data = JsonUtility.ToJson(new PositionJSON(vec3, direction));
        socket.Emit("player move", new JSONObject(data));
        //Application.ExternalCall("PlayerMove", new JSONObject(data));
    }

    public void CommandShoot(float angle)
    {
        ShootJSON shootJSON = new ShootJSON(namePlayerLocal, angle);

        socket.Emit("player shoot", new JSONObject(JsonUtility.ToJson(shootJSON)));
        //Application.ExternalCall("PlayerShoot", new JSONObject(JsonUtility.ToJson(shootJSON)));
    }

    public void CommandGameOver()
    {
        socket.Emit("gameOver");
        //Application.ExternalCall("GameOver");
    }

    public void CommandReadyLobby()
    {
        socket.Emit("ready lobby");
        //Application.ExternalCall("ReadyLobby");
    }

    public void CommandHealthChange(string name, int value)
    {
        PlayerDamageJSON playerDamageJSON = new PlayerDamageJSON(name, value);

        socket.Emit("health", new JSONObject(JsonUtility.ToJson(playerDamageJSON)));
        //Application.ExternalCall("Health", new JSONObject(JsonUtility.ToJson(playerDamageJSON)));
    }

    public void CommandAnimation(string playerName, string varName, bool value)
    {
        AnimationJSON animationJSON = new AnimationJSON(playerName, varName, (value ? "t" : "f"));
        socket.Emit("player animation", new JSONObject(JsonUtility.ToJson(animationJSON)));
        //Application.ExternalCall("PlayerAnimation", new JSONObject(JsonUtility.ToJson(animationJSON)));
    }

    public void CommandAnimation(string playerName, string varName, int value)
    {
        AnimationJSON animationJSON = new AnimationJSON(playerName, varName, value.ToString());
        socket.Emit("player animation", new JSONObject(JsonUtility.ToJson(animationJSON)));
        //Application.ExternalCall("PlayerAnimation", new JSONObject(JsonUtility.ToJson(animationJSON)));
    }

    public void CommandDisconnect()
    {
        socket.Emit("disconnect");
        //Application.ExternalCall("Disconnect");
    }

    #endregion


    #region Listening

    void OnOtherPlayerConnected(SocketIOEvent socketIoEvent)
    {
        string data = socketIoEvent.data.ToString();
        PlayerJSON userJSON = PlayerJSON.CreateFromJSON(data);
        //Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
        Vector3 position = Vector3.zero;

        GameObject p = Instantiate(go_player, position, Quaternion.identity) as GameObject;
        PlayerAttributes pa = p.GetComponent<PlayerAttributes>();
        pa.isLocalPlayer = false;
        pa.name = userJSON.name;
        pa.namePlayer = userJSON.name;
        pa.healthCurrent = userJSON.health;
        p.transform.Find("Canvas/txt_nomePlayer").GetComponent<Text>().text = pa.name;
        p.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;


    }

    void OnGameOver(SocketIOEvent socketIoEvent)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GameOver();
    }



    void onReadyLobby(SocketIOEvent socketIoEvent)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().LoadPhase();
    }


    void OnPlay(SocketIOEvent socketIoEvent)
    {
        string data = socketIoEvent.data.ToString();
        PlayerJSON currentUserJSON = PlayerJSON.CreateFromJSON(data);

        //Vector3 position = new Vector3(currentUserJSON.position[0], currentUserJSON.position[1], currentUserJSON.position[2]);
        Vector3 position = Vector3.zero;

        GameObject p = Instantiate(go_player, position, Quaternion.identity) as GameObject;
        PlayerAttributes pa = p.GetComponent<PlayerAttributes>();
        pa.isLocalPlayer = true;
        pa.name = currentUserJSON.name;
        pa.namePlayer = currentUserJSON.name;
        p.transform.Find("Canvas/txt_nomePlayer").GetComponent<Text>().text = pa.name;
        pa.healthCurrent = currentUserJSON.health;
    }


    void OnPlay2(SocketIOEvent socketIoEvent)
    {
        Debug.Log("Inicio OnPlay2");
        string data = socketIoEvent.data.ToString();
        PlayerJSON players = PlayerJSON.CreateFromJSON(data);

        string[] nomes = players.name.Split(';');
        string[] idPlayers = players.idPlayer.Split(';');



        for (int i = 0; i < nomes.Length; i++)
        {

            string sceneName = "Scene0";
            //Vector3 position = Vector3.zero;
            Vector3 position = GameObject.Find(string.Format("GameObjects/Scene/{0}/Respawns/p{1}", sceneName, i)).transform.position;

            string namePlayer_Sprite = (idPlayers[i] == "0" ? "Jorge" : (idPlayers[i] == "1" ? "Paulao" : (idPlayers[i] == "2" ? "Glutius" : "Yelva")));

            GameObject p = Instantiate(Resources.Load(string.Format("Characters/{0}", namePlayer_Sprite)), position, Quaternion.identity) as GameObject;

            if (nomes[i] == namePlayerLocal)
            {
                //GameObject p = Instantiate(Resources.Load(string.Format("Characters/{0}", namePlayer_Sprite)), position, Quaternion.identity) as GameObject;
                PlayerAttributes pa = p.GetComponent<PlayerAttributes>();
                //pa.Init(true);
                pa.isLocalPlayer = true;
                pa.name = nomes[i];
                //pa.idPlayer = i;
                pa.namePlayer = nomes[i];
                pa.multiplayerOnline = true;
                p.transform.Find("Canvas/txt_nomePlayer").GetComponent<Text>().text = pa.name;
                pa.healthCurrent = players.health;


                //listScoreBoardPlayers.Add(new ScoreBoardPlayers(pa.name, 0));
                AddlistScoreBoardPlayers(pa.name);

            }
            else
            {
                //GameObject p = Instantiate(Resources.Load(string.Format("Characters/{0}", namePlayer_Sprite)), position, Quaternion.identity) as GameObject;
                PlayerAttributes pa = p.GetComponent<PlayerAttributes>();
                //pa.Init(true);
                pa.isLocalPlayer = false;
                pa.name = nomes[i];
                //pa.idPlayer = i;
                pa.namePlayer = nomes[i];
                pa.multiplayerOnline = true;
                pa.healthCurrent = players.health;
                p.transform.Find("Canvas/txt_nomePlayer").GetComponent<Text>().text = pa.name;
                //p.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                //listScoreBoardPlayers.Add(new ScoreBoardPlayers(pa.name, 0));
                AddlistScoreBoardPlayers(pa.name);
            }

            p.GetComponent<PlayerMoviment>().direction = (i == 0 ? "D" : "E");
            p.transform.localScale = new Vector3((i == 0 ? 0.5f : -0.5f), 0.5f, 0);

        }
    }



    void AddlistScoreBoardPlayers(string name)
    {
        bool isExists = false;
        for (int i = 0; i < listScoreBoardPlayers.Count; i++)
        {
            if (listScoreBoardPlayers[i].name == name)
            {
                isExists = true;
                break;
            }
        }

        if (!isExists)
        {
            listScoreBoardPlayers.Add(new ScoreBoardPlayers(name, 0));
        }
    }

    void OnPlayerMove(SocketIOEvent socketIoEvent)
    {
        string data = socketIoEvent.data.ToString();
        PlayerJSON userJSON = PlayerJSON.CreateFromJSON(data);

        Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);

        if (userJSON.name == namePlayerLocal)
        {
            return;
        }
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
        if (p != null)
        {
            p.transform.position = position;
            p.GetComponent<PlayerMoviment>().ChangeDirection(userJSON.direction);
        }
    }

    void OnPlayerShoot(SocketIOEvent socketIoEvent)
    {
        string data = socketIoEvent.data.ToString();
        ShootJSON shootJSON = ShootJSON.CreateFromJSON(data);

        GameObject p = GameObject.Find(shootJSON.name);
        p.GetComponentInChildren<PlayerAttack>().CmdShootEnergyBall(0, shootJSON.angle);

    }

    void OnPlayerAnimation(SocketIOEvent socketIoEvent)
    {
        string data = socketIoEvent.data.ToString();
        AnimationJSON animJSON = AnimationJSON.CreateFromJSON(data);
        GameObject p = GameObject.Find(animJSON.playerName);

        p.GetComponent<PlayerMoviment>().ChangeAnimation(animJSON.variableName, animJSON.value);
    }



    void OnHealth(SocketIOEvent socketIoEvent)
    {
        string data = socketIoEvent.data.ToString();
        PlayerDamageJSON playerDamageJSON = PlayerDamageJSON.CreateFromJSON(data);
        GameObject p = GameObject.Find(playerDamageJSON.name);
        PlayerHealth ph = p.GetComponent<PlayerHealth>();

        ph.OnChangeHealth(playerDamageJSON.damage);
    }

    void OnOtherPlayerDisconnected(SocketIOEvent socketIoEvent)
    {
        string data = socketIoEvent.data.ToString();
        PlayerJSON userJSON = PlayerJSON.CreateFromJSON(data);
        int id = 0;
        for (int i = 0; i < listScoreBoardPlayers.Count; i++)
        {
            if (listScoreBoardPlayers[i].name == userJSON.name)
            {
                id = i;
                break;
            }
        }

        listScoreBoardPlayers.RemoveAt(id);
        if (listScoreBoardPlayers.Count == 1)
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().BackToStartMenu();
    }

    void OnStartGame(SocketIOEvent socketIoEvent)
    {
        GameObject.Find("StartMenu").GetComponent<StartMenu>().StartGame();
    }

    #endregion

    #region JSONMessageClasses

    [Serializable]
    public class UserJSON
    {
        public string name;
        public int idPlayer;

        public UserJSON(string _name)
        {
            name = _name;
        }

        public UserJSON(string _name, int _idPlayer)
        {
            name = _name;
            idPlayer = _idPlayer;
        }

    }

    [Serializable]
    public class PositionJSON
    {
        public float[] position;
        public string direction;
        public PositionJSON(Vector3 _position, string _dir)
        {
            position = new float[] { _position.x, _position.y, _position.z };
            direction = _dir;
        }
    }

    [Serializable]
    public class PlayerJSON
    {
        public string name;
        public string idPlayer;
        public float[] position;
        public string direction;
        public int health;

        public static PlayerJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<PlayerJSON>(data);
        }

        public static List<PlayerJSON> CreateFromJSONList(string data)
        {
            return JsonUtility.FromJson<List<PlayerJSON>>(data);
        }
    }

    [Serializable]
    public class PlayerDamageJSON
    {
        public string name;
        public int damage;

        public PlayerDamageJSON(string _name, int _damage)
        {
            name = _name;
            damage = _damage;
        }

        public static PlayerDamageJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<PlayerDamageJSON>(data);
        }
    }

    [Serializable]
    public class EnemiesJSON
    {
        public List<PlayerJSON> enemies;

        public static EnemiesJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<EnemiesJSON>(data);
        }
    }

    [Serializable]
    public class ShootJSON
    {
        public string name;
        public float angle;


        public ShootJSON(string _name, float _angle)
        {
            name = _name;
            angle = _angle;
        }

        public static ShootJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<ShootJSON>(data);
        }
    }

    [Serializable]
    public class AnimationJSON
    {
        public string playerName;
        public string variableName;
        public string value;


        public AnimationJSON(string _playerName, string _variableName, string _value)
        {
            playerName = _playerName;
            variableName = _variableName;
            value = _value;
        }

        public static AnimationJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<AnimationJSON>(data);
        }
    }

    #endregion


    public class ScoreBoardPlayers
    {
        public string name;
        public int idPlayer;
        public int score;

        public ScoreBoardPlayers(string _name, int _score)
        {
            name = _name;
            score = _score;
        }

        public ScoreBoardPlayers(string _name, int _idPlayer, int _score)
        {
            name = _name;
            idPlayer = _idPlayer;
            score = _score;
        }
    }

}
