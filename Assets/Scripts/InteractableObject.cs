using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour, IInteractible {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IInteractible implementation

	public void OnTriggerEnter (Collider other)
	{
		PlayerController player = other.GetComponent<PlayerController>();
		if(player != null)
		{
			player.Interact += Activate;
		}
	}

	public void OnTriggerExit (Collider other)
	{
		PlayerController player = other.GetComponent<PlayerController>();
		if(player != null)
		{
			player.Interact -= Activate;
		}
	}

	public void Activate()
	{
		Debug.Log("Activated Trap Card!");	
	}
	#endregion
}
