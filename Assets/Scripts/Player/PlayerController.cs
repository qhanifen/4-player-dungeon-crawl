using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerController : MonoBehaviour
{
    #region Input Variables
    [HideInInspector]
    public int playerID = 0;
    public Player player;
    public Hero hero;
    [HideInInspector] public Animator anim;
    public bool active = false;

    public delegate void InteractEvent();
    public event InteractEvent Interact;
    #endregion
    
    // Use this for initialization
    void Start()
    {
        hero = this.GetComponent<Hero>();
        anim = hero.GetComponent<Animator>();
        //Inject Player later from PlayerManager
        player = ReInput.players.GetPlayer(playerID);
        active = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            float leftAxisX = player.GetAxis("Move Horizontal");
            float leftAxisY = player.GetAxis("Move Vertical");
            float rightAxisX = player.GetAxis("Look Horizontal");
            float rightAxisY = player.GetAxis("Look Vertical");

            Vector3 dir = new Vector3(leftAxisX, 0, leftAxisY);
            Vector3 lookDir = new Vector3(rightAxisX, 0, rightAxisY);

            Movement(dir, lookDir);

            if (player.GetButton("Fire"))
            {
                hero.Attack();
                anim.SetTrigger("Attack");
            }

            if (player.GetButtonDown("Interact") && Interact != null)
            {
                Interact();
            }
        }
    }

    void Movement(Vector3 dir, Vector3 lookDir)
    {
        //Handle character rotation
        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir.normalized), hero.rotationSpeed * Time.fixedDeltaTime);
        }
        else if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), hero.rotationSpeed * Time.fixedDeltaTime);
        }
        
        //Handle character movement
        if (dir != Vector3.zero)
        {
            transform.Translate(dir.normalized * hero.runSpeed * Time.fixedDeltaTime, Space.World);
            
            //Handle character animation
            float forwardVal = Vector3.Dot(transform.forward, dir);
            Vector3 crossVector = Vector3.Cross(lookDir, Vector3.up);
            float sideValue = Vector3.Dot(crossVector, dir);
            anim.SetBool("Moving", true);
            anim.SetFloat("Movement Forward", forwardVal);
            anim.SetFloat("Movement Side", sideValue);
        }
        else
        {
            anim.SetBool("Moving", false);
        }

        GetTarget();
    }

    void GetTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10.0f) && hit.collider.GetComponent<MonoBehaviour>() is ITargetable)
        {
            Debug.DrawRay(transform.position, transform.forward * Vector3.Distance(transform.position, hit.point), Color.red);
            hero.target = hit.collider.transform;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
        }
    }
}
