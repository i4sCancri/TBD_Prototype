﻿// Aiden
using UnityEngine;
using UnityEngine.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the Player controls for units on the grid map.
/// Since the controlled unit changed, which one getting controlled is changed here.
/// </summary>
public class UnitController : MonoBehaviour {
	
	public float speed = 1f;

	private HUDManager HUDManagerScript;

	public HexGrid hexGrid;

	//[HideInInspector] 
	public List<Unit> units = new List<Unit> ();
	public List<Enemy> enemies = new List<Enemy> ();

	private bool controllerOn = true;					// determines if controller is allowed to work, ***disable (false) on layover screens, etc.***

	public GameObject unitHolderObj;					// gameobject called "Units" that holds all the unit in play
	public GameObject enemyHolderObj;
	public Unit controlledUnit;							// the Unit controlled currently
	private Transform unitTransform;
	private int currentUnitIndex = 0;	

	// Coordinates for pathfinding/movement
	public HexCoordinates initialCoord;														// first position of unit since last move apply or since switched to
	public Stack<HexCoordinates> currentPath = new Stack<HexCoordinates>();					// Stack of current walked path
		// NOTE: THIS SHOULD NEVER INCLUDE THE INITIAL COORD (at least not for now)

	public Unit target;					// target of action

	// Properties
	public Transform UnitTransform {
		get { return UnitTransform; }
	}

	public bool ControllerOn {
		set { controllerOn = value; }
		get { return controllerOn; }
	}

	void Start () {
		HUDManagerScript = GameObject.Find ("UICanvas").GetComponent<HUDManager> ();

		// Populate the Units list
		foreach (Transform child in unitHolderObj.transform) {
			units.Add (child.gameObject.GetComponent<Unit>());
		}

		// Populate Enemies list
		foreach (Transform child in enemyHolderObj.transform) {
			enemies.Add (child.gameObject.GetComponent<Enemy> ());
		}


		// Default the first in the list to the current controlled unit
		controlledUnit = units[0];
		controlledUnit.gameObject.layer = 0;		// CURRENT CONTROLLED UNIT MUST BE ON LAYER 0, OTHERS MUST BE ON 8
		unitTransform = controlledUnit.transform;
		HUDManagerScript.UpdateActiveUnitText(controlledUnit.Name);
	}

	/// Cycles through controllable units (skips over dead units)
	public void CycleUnit() {
		// Apply latest path first if there is one
		if (currentPath.Count != 0 )
			ApplyMove();

		// set the coordinate of the unit from which you are switching to occupied
		hexGrid.OccupyCell(HexCoordinates.GetIndexOfCoordinate(GetCurrentCoordinate(), hexGrid.width));

		// Now increment to next unit in list

		// Skip if dead
		do {
			currentUnitIndex++;
			if (currentUnitIndex > units.Count - 1)	// loop through list
				currentUnitIndex = 0;
		} while (units [currentUnitIndex].status == Status.Dead);


		if (currentUnitIndex > units.Count - 1)	// loop through list
			currentUnitIndex = 0;

		controlledUnit = units[currentUnitIndex];

		
		unitTransform = controlledUnit.transform;

		// set initial coord
		initialCoord = GetCurrentCoordinate();


		// Change all of the unit's layers to 8 before changing the current to 0
		foreach (Unit u in units) {
			u.gameObject.layer = 8;
		}
		controlledUnit.gameObject.layer = 0;

		// Update UI with Current Unit data
		HUDManagerScript.UpdateActiveUnitText(controlledUnit.Name);
		HUDManagerScript.UpdateAPBar();
		// CHANGE CORRESPONDING UNITS ACTIONS LIST HERE
	}

	void Update () {
		// Movement
		if (controllerOn)
			unitTransform.Translate (Input.GetAxis ("Horizontal_Player") * speed, Input.GetAxis ("Vertical_Player"), 0);

		// if character runs out of AP, disable controller
		if (controlledUnit.currentAP <= 0)
			controllerOn = false;
		else
			controllerOn = true;

	}

	// When the terrain editor is open, disable units and mobility
	public void DisableController() {
		controllerOn = false;
		Debug.Log ("Controller DISABLED");
	}

	public void EnableController() {
		controllerOn = true;
		Debug.Log ("Controller ENABLED");
	}

	// MOVEMENT---------------------------------------------------------

	/// <summary>
	/// Confirms the last path movement and consumes the appropriate amount of AP <-- this is handled in TouchCell() of HexGrid.cs
	/// according to the size of the currentPath stack
	/// </summary>
	public void ApplyMove() {
		// clears current stack of moves
		currentPath.Clear ();

		// set NEW initial coord
		initialCoord = GetCurrentCoordinate();

		// TO DO: clear the grid colors for walking (USE RESTORE GRID LATER)
		hexGrid.ResetGridColorToBlank();
	}

	public void UndoMove(){
		// TO DO: return unit position to the initial coordinate position (cell's transform.position)
		//			clear currentPath stack
		currentPath.Clear ();
	}

	/// <summary>
	/// Checkes if there are units adjacent to the currently controlled unit.
	/// ONLY APPLIES TO CHECKING FOR ENEMIES RIGHT NOW.
	/// </summary>
	/// <returns><c>true</c> units are adjacent <c>false</c> no adjacent units.</returns>
	public bool AreThereAdjacentUnits() {
		// store surrounding hexes in a list
		List<HexCoordinates> surroundingHexes = new List<HexCoordinates>();
		HexCoordinates current = GetCurrentCoordinate ();
		surroundingHexes.Add (new HexCoordinates(current.X + 1, current.Z));
		surroundingHexes.Add (new HexCoordinates(current.X, current.Z + 1));
		surroundingHexes.Add (new HexCoordinates(current.X - 1, current.Z));
		surroundingHexes.Add (new HexCoordinates(current.X, current.Z - 1));
		surroundingHexes.Add (new HexCoordinates(current.X - 1, current.Z + 1));
		surroundingHexes.Add (new HexCoordinates(current.X + 1, current.Z - 1));

		// if there are enemies, return true
		foreach (HexCoordinates h in surroundingHexes) {
			// if any adjacent cells are occupied, return true
			int index = HexCoordinates.GetIndexOfCoordinate(h, hexGrid.width);
			if (index >= 0) {	// needed to avoid index out of range errors for edge hexes
				if (hexGrid.Cells [index].isOccupied == true) {
					return true;
				}
			}

		}
		//otherwise return false
		return false;
	}

	/// <summary>
	/// Helper function to get the current HexCoord
	/// </summary>
	/// <returns>The current location.</returns>
	HexCoordinates GetCurrentCoordinate() {
		return HexCoordinates.FromPosition(unitTransform.position);
	}

		
}
