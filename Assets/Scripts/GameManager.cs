﻿using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {
	PlayerTurn,
	EnemyTurn,
	Paused
}

/// <summary>
/// Handles and oversees processes of the Grid Scene and the battle gameplay that takes place within.
/// Handles opening and closing menus in the screen such as the Terrain Editor, and sets the Game State accordingly.
/// </summary>
public class GameManager : MonoBehaviour {

	// Refernces to other manager scripts (drag in inspector)
	public UnitController UnitControllerManagerScript;
	public HUDManager HUDManagerScript;
	public TerrainEditor TerrainEditorScript;
	public HexGrid HexGridScript;

	// Gameplay properties
	public GameState currentGameState;
	public GameState lastGameState;

	public bool terrainEditorOpen;
	public bool battleMenuOpen;


	void Start() {
		// Terrain Editor should be inactive from the start
		TerrainEditorScript.gameObject.SetActive (false);
		terrainEditorOpen = false;
		battleMenuOpen = false;

		// ignore collisions between layer 0 and 8
		Physics.IgnoreLayerCollision(0,8);
	}

	void Update() {
		// toggle terrain editor menu
		if (Input.GetKeyDown (KeyCode.T)) {
			
			if (terrainEditorOpen == true) {
				CloseTerrainEditor ();
			}
			else {
				// TO DO: UPON CLOSING, SAVE THIS 
				OpenTerrainEditor ();
			}
		}


		// Check for adjacency to enable the battle menu
		if (UnitControllerManagerScript.AreThereAdjacentUnits () == true && battleMenuOpen == false) {
			OpenBattleMenu ();
			Debug.Log("Adjacent units!");
		} 
		else if (UnitControllerManagerScript.AreThereAdjacentUnits () == false && battleMenuOpen == true) {
			CloseBattleMenu ();
			Debug.Log("No adjacent units.");
		}
	}


	// Menu loading methods

	/// <summary>
	/// Opens the terrain editor menu and pauses the gameplay.
	/// </summary>
	void OpenTerrainEditor() {
		TerrainEditorScript.gameObject.SetActive (true);
		terrainEditorOpen = true;

		UnitControllerManagerScript.DisableController();
		HexGridScript.HexClickEnabled = false;
		lastGameState = currentGameState;
		currentGameState = GameState.Paused;

	}

	void CloseTerrainEditor() {
		TerrainEditorScript.gameObject.SetActive (false);
		terrainEditorOpen = false;

		UnitControllerManagerScript.EnableController();
		HexGridScript.HexClickEnabled = true;
		currentGameState = lastGameState;

	}

	void OpenBattleMenu() {
		// TO DO: make battle menu active
		battleMenuOpen = true;
	}

	void CloseBattleMenu() {
		// TO DO: make battle menu active
		battleMenuOpen = false;
	}


}
