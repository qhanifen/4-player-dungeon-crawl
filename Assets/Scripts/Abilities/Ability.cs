using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour, ICastable {

	public int abilityID;
	public string abilityName;
	public bool onCooldown = false;
	public float cooldownTimer = 5.0f;

	ITargetable target;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {

	}

	#region ICastable implementation

	public void Cast()
	{
		if(!onCooldown)
		{
			CoolDown();
		}
	}

	public Transform Target ()
	{
		return target.Target;
	}

	public IEnumerator CoolDown ()
	{
		onCooldown = true;
		yield return new WaitForSeconds(cooldownTimer);
		onCooldown = false;
	}

	#endregion
}
