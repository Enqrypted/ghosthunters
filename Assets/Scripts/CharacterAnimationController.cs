using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private float lastWalkFrame = 0;

    private Vector3 lastPos;

    private int currentIndex = 0;
    [SerializeField] private Sprite[] walkSprites = new Sprite[2];

    // Update is called once per frame
    void Update()
    {
        
        float velocity = (lastPos - transform.position).magnitude;
        lastPos = transform.position;

        if (velocity > 0f)
        {
            if (Time.time - lastWalkFrame > .25f)
            {
                lastWalkFrame = Time.time;

                currentIndex++;

                if (currentIndex >= walkSprites.Length)
                {
                    currentIndex = 0;
                }

                GetComponent<SpriteRenderer>().sprite = walkSprites[currentIndex];
            }
        }
        
    }
}
