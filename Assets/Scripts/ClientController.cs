using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClientController : MonoBehaviourPunCallbacks
{
    
    GameObject character;

    [SerializeField]
    private Joystick joystick;

    [SerializeField] private Transform camera;

    [SerializeField] private TextMeshProUGUI timerTxt;

    [SerializeField] private TextMeshProUGUI pointsTxt;

    [SerializeField] private GameObject gameWinPanel;

    private Dictionary<string, int> playerPoints = new Dictionary<string, int>();

    private float speed = 3f;

    public int secondsTimer = 120;
    
    IEnumerator spawn()
    {
        yield return new WaitForSeconds(.5f);
        character = PhotonNetwork.Instantiate("Character", new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f),
            Quaternion.identity);

        character.transform.Find("Canvas").Find("pName").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
        character.name = PhotonNetwork.LocalPlayer.UserId;
    }

    IEnumerator setCharacterNames()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            string ownerName = g.GetComponent<PhotonView>().Owner.NickName;
            g.name = ownerName;
            g.transform.Find("Canvas").Find("pName").GetComponent<TextMeshProUGUI>().text = ownerName;
        }
        
    }

    IEnumerator manageGhostSpawns()
    {
        yield return new WaitForSeconds(1f);
        
        //spawn ghosts if is host
        for (int i = 0; i < 50; i++)
        {
            PhotonNetwork.Instantiate("Ghost", new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0f),
                Quaternion.identity);
        }
        yield return null;
    }
    
    IEnumerator manageCoinSpawns()
    {
        yield return new WaitForSeconds(1f);
        
        //spawn coins if is host

        while (true)
        {
            int coins = GameObject.FindGameObjectsWithTag("Coins").Length;
        
            for (int i = 0; i < 50-coins; i++)
            {
                PhotonNetwork.Instantiate("Coin", new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0f),
                    Quaternion.identity);
            }

            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Coins").Length < 50);
        }
    }

    IEnumerator RoundTimer()
    {

        yield return new WaitForSeconds(1f);
        secondsTimer--;
        PhotonView.Get(this).RPC("UpdateTimerTxt", RpcTarget.All, secondsTimer.ToString());

        if (secondsTimer > 0)
        {
            StartCoroutine(RoundTimer());
        }
        else
        {
            //TODO
            //round end functionality
        }
    }

    IEnumerator setupPlayerPoints()
    {

        yield return new WaitForSeconds(1f);
        
        //setup player points
        foreach (Player plr in PhotonNetwork.PlayerList)
        {
            playerPoints.Add(plr.NickName, 0);
        }
    }

    [PunRPC]
    public void UpdateTimerTxt(string timeLeft)
    {
        
        TimeSpan time = TimeSpan.FromSeconds(Int32.Parse(timeLeft));
        string str = time .ToString(@"mm\:ss");
        
        timerTxt.text = str;
    }

    void Start()
    {
        StartCoroutine(spawn());
        StartCoroutine(setCharacterNames());

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(RoundTimer());
            StartCoroutine(manageGhostSpawns());
            StartCoroutine(manageCoinSpawns());
            StartCoroutine(setupPlayerPoints());
        }
    }

    [PunRPC]
    public void IncreasePlayerPoints(string pName, int points)
    {
        playerPoints[pName]+=points;
        foreach (Player plr in PhotonNetwork.PlayerList)
        {

            if (plr.NickName == pName)
            {
                PhotonView.Get(this).RPC("UpdatePointsText", plr, playerPoints[pName].ToString());
            }
        }
    }

    [PunRPC]
    public void UpdatePointsText(string points)
    {
        pointsTxt.text = "Points: " + points;
    }

    [PunRPC]
    public void RemovePlrHeart(string username)
    {
        Transform lives = GameObject.Find(username).transform.Find("Canvas").Find("Lives");
        Destroy(lives.Find("H" + lives.childCount).gameObject);
    }

    [PunRPC]
    public void GameOver(string winnerName, int points)
    {
        gameWinPanel.SetActive(true);
        gameWinPanel.transform.Find("pWinnerTxt").GetComponent<TextMeshProUGUI>().text = winnerName + " wins!";
        gameWinPanel.transform.Find("pointsTxt").GetComponent<TextMeshProUGUI>().text = points + " points";
        
    }

    public void PlayerKilledGhost(Player plr)
    {
        PhotonView.Get(this).RPC("IncreasePlayerPoints", PhotonNetwork.MasterClient, plr.NickName, 5);
    }

    public void GhostKilledPlayer(Player plr)
    {
        
        Transform lives = GameObject.Find(plr.NickName).transform.Find("Canvas").Find("Lives");
        if (lives.childCount <= 1)
        {

            string topPlayer = "";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if ((topPlayer == "") || (playerPoints[topPlayer] < playerPoints[player.NickName]))
                {
                    topPlayer = player.NickName;
                }
            }
            
            PhotonView.Get(this).RPC("GameOver", RpcTarget.All, topPlayer, playerPoints[topPlayer]);
        }

        PhotonView.Get(this).RPC("RemovePlrHeart", RpcTarget.All, plr.NickName);
    }

    public void PlayerGotCoin(Player plr)
    {
        PhotonView.Get(this).RPC("IncreasePlayerPoints", PhotonNetwork.MasterClient, plr.NickName, 1);
    }

    private void Update()
    {

        if (character)
        {
            camera.position = Vector3.Lerp(camera.position, character.transform.position-new Vector3(0,0, 10f), Time.deltaTime*5f);
            Vector3 walkMovement = new Vector3(joystick.Direction.x, joystick.Direction.y, 0f);
            
            character.transform.position += walkMovement*Time.deltaTime*speed;
            float angle = Mathf.Atan2(walkMovement.y, walkMovement.x) * Mathf.Rad2Deg;

            if (walkMovement.magnitude > 0f)
            {
                character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime*10f);
            }
            
            character.transform.Find("Canvas").rotation = Quaternion.identity;

        }
    }
}
