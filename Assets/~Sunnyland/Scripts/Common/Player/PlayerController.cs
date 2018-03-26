﻿using System.Collections;
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
        [Header("Grounding")]
        public float rayDistance = .5f;
        public float maxSlopeAngle = 45f;
        public bool isGrounded = false;
        [Header("Crouch")]
        public bool isCrouching = false;
        [Header("Jump")]
        public float jumpHeight = 2f;
        public int maxJumpCount = 2;
        public bool isJumping = false;
        [Header("Climb")]
        public float climbSpeed = 5f;
        public bool isClimbing = false;
        public bool isOnSlope = false;
        [Header("References")]
        public Collider2D defaultCollider;
        public Collider2D crouchCollider;

        // Delegates
        public EventCallback onJump;
        public EventCallback onHurt;
        public BoolCallback onCrouchChanged;
        public BoolCallback onGroundedChanged;
        public BoolCallback onSlopeChanged;
        public BoolCallback onClimbChanged;
        public FloatCallback onMove;
        public FloatCallback onClimb;

        private Vector3 groundNormal = Vector3.up;
        private int currentJump = 0;

        private float inputH, inputV;

        // References
        private SpriteRenderer rend;
        private Animator anim;
        private Rigidbody2D rigid;

        #region Unity Functions
        void Start()
        {
            rend = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
        }
        void Update()
        {
            PerformMove();
            PerformJump();
        }
        void FixedUpdate()
        {
            // Feel for the ground
            DetectGround();
        }
        void OnDrawGizmos()
        {
            Ray groundRay = new Ray(transform.position, Vector3.down);
            Gizmos.DrawLine(groundRay.origin, groundRay.origin + groundRay.direction * rayDistance);
        }
        #endregion

        #region Custom Functions

        void PerformMove()
        {
            if (isOnSlope &&
                inputH == 0 &&
                isGrounded)
            {
                //Cancel Velocity
                rigid.velocity = Vector3.zero;
            }

            Vector3 right = Vector3.Cross(groundNormal, Vector3.back);
            rigid.AddForce(right * inputH * speed);

            //Limit the velocity mac celocity
            LimitVelocity();
        }

        void PerformJump()
        {
            if (isJumping)
            {
                //if player is jumping
                if (currentJump < maxJumpCount)
                {
                    //increase player is allowed to jump
                    currentJump++;
                    //perform jump logic
                    rigid.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                }
            }
            // reset jump input
            isJumping = false;
        }

        bool CheckSlope(RaycastHit2D hit)
        {
            // Grab the angle in degrees of the surface we're standing on
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            // If the angle is greater than max
            if (slopeAngle >= maxSlopeAngle)
            {
                // Make player slide down surface
                rigid.AddForce(Physics.gravity);
            }
            if (slopeAngle > 0 && slopeAngle < maxSlopeAngle)
            {
                return true;
            }
            return false;
        }

        // Check to see if ray hit object is ground
        bool CheckGround(RaycastHit2D hit)
        {
            // Check if:
            if (hit.collider != null && hit.collider.name != name && !hit.collider.isTrigger)
            {
                // Reset the jump count
                currentJump = 0;
                // Is grounded!
                isGrounded = true;
                // Set ground normal now that we're grounded
                groundNormal = -hit.normal;

                // Record 'isOnSlope' value 
                bool wasOnSlope = isOnSlope;
                // Check if we're on a slope!
                isOnSlope = CheckSlope(hit);
                // Has the 'isOnSlope' value changed?
                if (wasOnSlope != isOnSlope)
                {
                    // Invoke event
                    if (onSlopeChanged != null)
                    {
                        onSlopeChanged.Invoke(isOnSlope);
                    }
                }

                // We have found our ground so exit the function
                // (No need to check any more hits)
                return true;
            }
            else
            {
                // We are no longer grounded
                isGrounded = false;
            }

            // Haven't found the ground (so keep looking)
            return false;
        }

        void DetectGround()
        {
            // Record a copy of what isGrounded was
            bool wasGrounded = isGrounded;

            #region Ground Detection Logic
            // Create a ray going down
            Ray groundRay = new Ray(transform.position, Vector3.down);
            // Set Hit to 2D Raycast
            RaycastHit2D[] hits = Physics2D.RaycastAll(groundRay.origin, groundRay.direction, rayDistance);

            foreach (var hit in hits)
            {
                if (Mathf.Abs(hit.normal.x) > 0.1f)
                {
                    rigid.gravityScale = 0;
                }
                else
                {
                    rigid.gravityScale = 1;
                }

                if (CheckGround(hit))
                {
                    // We found the ground! So exit the loop
                    break;
                }

                // If hit collider is not null
                // Reset currentJump
            }
            #endregion

            // Check if:
            if (wasGrounded != isGrounded && // IsGrounded has changed since before the detection AND
                onGroundedChanged != null) // Something is subscribed to this event
            {
                // Run all the things subscribed to event and give it "isGrounded" value
                onGroundedChanged.Invoke(isGrounded);
            }
        }
        void LimitVelocity()
        {
            // If Rigid's velocity (magnitude) is greater than maxVelocity
            if (rigid.velocity.magnitude > maxVelocity)
            {
                // Set Rigid velocity to velocity normalized x maxVelocity
                rigid.velocity = rigid.velocity.normalized * maxVelocity;
            }
        }

        void EnablePhysics()
        {
            rigid.simulated = true;
            rigid.gravityScale = 1;
        }

        void DisablePhysics()
        {
            rigid.simulated = false;
            rigid.gravityScale = 0;
        }

        public void Jump()
        { 
            //Alternate way
            isJumping = true;

            if (onJump != null)
            {
                onJump.Invoke();
            }
        }

        public void Move(float horizontal)
        {
            if (horizontal != 0)
            {
                rend.flipX = horizontal > 0;
            }

            inputH = horizontal;

            //invoke event
            if (onMove != null)
            {
                onMove.Invoke(inputH);
            }
        }
        public void Climb()
        {
            // CHALLENGE
        }

        public void Crouch()
        {

        }
        #endregion
    }
}