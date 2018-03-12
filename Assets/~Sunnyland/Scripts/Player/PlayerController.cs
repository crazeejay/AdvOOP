using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]

    public class PlayerController : MonoBehaviour
    {
        public float speed = 5f;
        public float maxVelocity = 2f;
        public float rayDistance = .5f;
        public float jumpHeight = 2f;
        public int maxJumpCount = 2;
        public LayerMask groundLayer;

        private Vector3 moveDirection;
        private int currentJump = 0;

        //References
        private SpriteRenderer rend;
        private Animator anim;
        private Rigidbody2D rigid;

        #region Unity Functions
        // Use this for initialization
        void Start()
        {
            rend = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();

        }

        // Update is called once per frame
        void Update()
        {
            moveDirection.y += Physics.gravity.y * Time.deltaTime;

        }
        void FixedUpdate()
        {

        }
        void OnDrawGizmos()
        {
            Ray groundRay = new Ray(transform.position, Vector3.down);
            Gizmos.DrawLine(groundRay.origin, groundRay.origin + groundRay.direction * rayDistance);

        }
        #endregion

        #region Custom Functions
        void DetectGround()
        {
             //Create a ray going down
             //set hit to 2D
             //If hit collider is not null
               //Reset currentJump
        }
        void LimitVelocity()
        {
            //If Rigid's velocity (magnitude) is greater than maxVelocity
              //Set Rigid velocity to velocity normalized x maxVelocity
        }
        void jump()
        {
          //If currentJump is less than maxJump
            //Increment currentJump
            //addForce to player (using Impulse)
        }
        public void Climb()
        {
          //CHALLENGE
        }
        public void Move(float horizontal)
        {
          //If horizontal > 0
            //Flip Character
          //If horizontal < 0
            //Flip Character

          //Addforce to player in the right direction
          //limit Velocity
        }
        #endregion
    }
}
