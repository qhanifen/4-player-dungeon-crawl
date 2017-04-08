using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerController : MonoBehaviour 
{
	public float runSpeed = 10.0f;
	public float rotationSpeed = 8.0f;

    #region Input Variables
    public Player player;
	public Hero hero;
    public Animator anim;
    public bool active = false;

	public delegate void InteractEvent();
	public event InteractEvent Interact;
    #endregion 

    public PlayerController(int ID)
    {
        player = ReInput.players.GetPlayer(ID);
        active = true;
    }

    public void SetHero(Hero _hero)
    {
        hero = _hero;
    }

	// Use this for initialization
	void Start () 
	{
		hero = this.GetComponent<Hero>();
        anim = hero.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
        if (player != null)
        {
            float leftAxisX = player.GetAxisRaw("Move Horizontal");
            float leftAxisY = player.GetAxisRaw("Move Vertical");
            float rightAxisX = player.GetAxisRaw("Look Horizontal");
            float rightAxisY = player.GetAxisRaw("Look Vertical");

            Vector3 dir = new Vector3(leftAxisX, 0, leftAxisY);
            Vector3 lookDir = new Vector3(rightAxisX, 0, rightAxisY);

            Movement(dir, lookDir);

            if (player.GetButton("Fire"))
            {
                hero.Fire();
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
		transform.Translate(dir.normalized * runSpeed * Time.fixedDeltaTime, Space.World);
		if(lookDir != Vector3.zero)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir.normalized), rotationSpeed * Time.fixedDeltaTime);
			GetTarget();
		}
		else if(dir != Vector3.zero)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), rotationSpeed * Time.fixedDeltaTime);
		}
	}

	void GetTarget()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 10.0f) && hit.collider.GetComponent<MonoBehaviour>() is ITargetable)
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
