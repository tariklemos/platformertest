using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (CharacterZero))]
public class CharacterZeroUserControl : MonoBehaviour {

	private CharacterZero character;
	private bool jump;
	private bool attack;
	private bool dash;

	void Awake () {
		character = GetComponent<CharacterZero> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!dash) {
			dash = CrossPlatformInputManager.GetButtonDown("Run");
		}

		if (!jump) {
			// Read the jump input in Update so button presses aren't missed.
			jump = CrossPlatformInputManager.GetButtonDown("Jump");
		}

		if (!attack) {
			attack = CrossPlatformInputManager.GetButtonDown("Fire1");
		}
	}
	
	void FixedUpdate () {

		if (attack) {
			character.attack ();
			attack = false;
		} else {
			float h = CrossPlatformInputManager.GetAxis("Horizontal");
			
			character.move (h, jump, dash);
			jump = false;
			dash = false;
		}

	}


}
