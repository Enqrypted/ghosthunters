using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClientController : MonoBehaviourPunCallbacks
{
    
    GameObject character;

    [SerializeField]
    private Joystick joystick;

    [SerializeField] private Transform camera;
    // Start is called before the first frame update

    private float speed = 3f;
    
    IEnumerator spawn()
    {
        yield return new WaitForSeconds(.5f);
        character = PhotonNetwork.Instantiate("Character", new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f),
            Quaternion.identity);

        character.transform.Find("Canvas").Find("pName").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
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
            PhotonNetwork.Instantiate("Ghost", new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f),
                Quaternion.identity);
        }
        yield return null;
    }

    void Start()
    {
        StartCoroutine(spawn());
        StartCoroutine(setCharacterNames());

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(manageGhostSpawns());
        }
        
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
