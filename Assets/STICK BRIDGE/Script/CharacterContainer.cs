using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterContainer : MonoBehaviour {
	public static CharacterContainer Instance;
	public GameObject defaultPlayer;
	public GameObject[] listPlayers;
	GameObject pickedPlayer;

	void Awake(){
		Instance = this;

		int pickedCharID = GlobalValue.PickedCharacter;
		if (pickedCharID != 0) {
			foreach(var _char in listPlayers){
				if(_char.GetInstanceID() == pickedCharID){
					SetPickCharacter(_char);
					break;
				}
			}
		}
	}

	public GameObject GetPlayer(){
		return pickedPlayer != null ? pickedPlayer : defaultPlayer;
	}

	public void SetPickCharacter(GameObject _player){
		foreach (var obj in listPlayers) {
			if (obj == _player) {
				pickedPlayer = obj;
				Debug.Log ("Set new player successfully: " + _player.name);
				return;
			}
		}

		Debug.LogError ("No player! Please place the player prefab to CharacterContainer");
	}
}
