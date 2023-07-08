using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
    {
        private float _startingPos, //This is the starting position of the sprites.
            _lengthOfSprite; //This is the length of the sprites.
        public float AmountOfParallax; //This is amount of parallax scroll. 
        public Camera MainCamera; //Reference of the camera.



        private void Start()
        {
            //Getting the starting X position of sprite.
            _startingPos = transform.position.x;
            //Getting the length of the sprites.
            _lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
        }



        private void Update()
        {
            Vector3 Position = transform.position;
            Vector3 NewPosition = new Vector3(Position.x + AmountOfParallax, transform.position.y, transform.position.z);

            



            if (transform.position.x  > Screen.width+940f )
            {
                NewPosition = new Vector3(-Screen.width+940f, transform.position.y, transform.position.z);
            }
            Debug.Log(transform.position.x);
            transform.position = NewPosition;
        }
    }