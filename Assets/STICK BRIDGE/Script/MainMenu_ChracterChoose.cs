using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu_ChracterChoose : MonoBehaviour {

	[Tooltip("The unique character ID")]
	public GameObject CharacterPrefab;

	public GameObject Locked;
	public Image ButImage;
	public Color pickColor = Color.yellow;

	public Text pricetxt;
	public Text state; 

	bool isUnlock;

	Player _player;
    
	void Start () {

		_player = CharacterPrefab.GetComponent<Player> ();
		if (_player.unlockDefault) {
//			PlayerPrefs.SetInt (GlobalValue.Character + characterID, 1);
			GlobalValue.UnlockCharacter (CharacterPrefab.GetInstanceID ());
			isUnlock = true;
		} else
			isUnlock = GlobalValue.CheckUnlockCharacter (CharacterPrefab.GetInstanceID ());
//			isUnlock = PlayerPrefs.GetInt (GlobalValue.Character + characterID, 0) == 1 ? true : false;

//		UnlockButton.SetActive (!isUnlock);
		Locked.SetActive (!isUnlock);
		state.color = isUnlock ? Color.white : Color.yellow;

		pricetxt.text =  _player.price.ToString ();
	}

	void Update(){
		if (!isUnlock)
			return;
		
		if (GlobalValue.PickedCharacter == CharacterPrefab.GetInstanceID ()) {
			state.text = "Picked";
			ButImage.color = pickColor;
		} else {
			state.text = "Choose";
			ButImage.color = Color.white;
		}
	}

	public void OnClick(){
		if (!isUnlock)
			Unlock ();
		else
			Pick ();
	}
	
	 void Unlock(){
//		SoundManager.Click ();

		var coins = GlobalValue.SavedCoins;
		if (coins >= _player.price) {
			coins -= _player.price;
			GlobalValue.SavedCoins = coins;
			//Unlock
			GlobalValue.UnlockCharacter (CharacterPrefab.GetInstanceID ());
//			PlayerPrefs.SetInt (GlobalValue.Character + characterID, 1);

			isUnlock = true;
//			if (anim)
//				anim.SetTrigger ("unlock");

//			CharacterHolder.Instance.UpdateUnlockCharacter ();
//			CharacterHolder.Instance.CharacterUnlocked.Add (CharacterPrefab.GetInstanceID ());
			Locked.SetActive (false);
			state.color = isUnlock ? Color.white : Color.yellow;

			SoundManager.PlaySfx (SoundManager.Instance.soundPurchased);
//			UnlockButton.SetActive (false);
		}
	}

	 void Pick(){
		SoundManager.Click ();

		if (GlobalValue.PickedCharacter == CharacterPrefab.GetInstanceID ())
			return;

		SoundManager.PlaySfx (SoundManager.Instance.soundClick);

		GlobalValue.PickedCharacter = CharacterPrefab.GetInstanceID ();


		GameManager.Instance.characterList.SetPickCharacter (CharacterPrefab);

		if (GameManager.Instance.State != GameManager.GameState.Dead)
			GameManager.Instance.SpawnPlayer ();
//		CharacterHolder.Instance.CharacterPicked = CharacterPrefab;
//		if (GameManager.Instance)
//			GameManager.Instance.SwitchPlayerCharacter ();

//		if (anim)
//			anim.SetTrigger ("pick");
	}
}
