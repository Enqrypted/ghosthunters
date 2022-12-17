using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public bool isEvil = false;
    private float speed = 3f;

    private Vector3 moveDirection;

    IEnumerator walkControl()
    {

        bool stayIdle = Random.Range(0f, 1f) > .5f;

        if (stayIdle)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        }
        
        yield return new WaitForSeconds(Random.Range(.5f, 2f));

        StartCoroutine(walkControl());

    }
    
    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            isEvil = Random.Range(0f, 1f) > .5f;
            PhotonView.Get(this).RPC("SetState", RpcTarget.All, isEvil);
            StartCoroutine(walkControl());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((moveDirection != null) && (moveDirection.magnitude > 0f))
        {
            transform.position += moveDirection * Time.deltaTime * speed;
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime*5f);
        }
    }

    [PunRPC]
    public void SetState(bool _isEvil)
    {
        this.isEvil = _isEvil;
        GetComponent<SpriteRenderer>().color = isEvil ? new Color(.9f, .4f, .5f) : new Color(.4f, .6f, .8f);
    }
}
