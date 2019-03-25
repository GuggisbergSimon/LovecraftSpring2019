using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LevelBuilder : MonoBehaviour
{
	[SerializeField] private int maxFloors = 5;
	[SerializeField] private GameObject[] wallPrefabs = null;
	[SerializeField] private GameObject[] doorPrefabs = null;
	[SerializeField] private GameObject[] groundPrefabs = null;
	[SerializeField] private GameObject[] interiorRampPrefabs = null;
	[SerializeField] private GameObject[] bridgePrefabs = null;
	[SerializeField] private float doorWeight = 0.25f;
	[SerializeField] private float wallWeight = 0.75f;
	[SerializeField] private int minDoorsPerFloor = 2;
	[SerializeField] private int maxDoorsPerFloor = 6;
	[SerializeField] private LayerMask linkLayer = 0;
	[SerializeField] private float maxDistanceRayCastLink = 30.0f;
	private float _sizeHexa;
	private float _widthHexa = 42.784f;
	private float _heightHexa;
	private float _heightFloor = 9.0f;
	private List<Transform> _openList = new List<Transform>();
	private List<Transform> _closedList = new List<Transform>();
	private List<GameObject> _towers = new List<GameObject>();

	private void Start()
	{
		_sizeHexa = 2 * _widthHexa / Mathf.Sqrt(3);
		_heightHexa = _sizeHexa / 2;

		//test
		Build(transform.position);
	}

	private void Build(Vector3 pos)
	{
		BuildTowerBase(pos);
		int currentFloor = 1;

		//build the walls and make sure there is at least 2 doors
		//TODO add check wether it's possible to place doors (ie: if there is enough room) and if there is a door to link in direction (raycast link tag)
		//register every door as a possible link

		float currentDoorWeight = doorWeight;
		float currentWallWeight = wallWeight;

		for (int i = 0; i < 6; i++)
		{
			if (Random.Range(0.0f, 1.0f) * currentDoorWeight > Random.Range(0.0f, 1.0f) * currentWallWeight)
			{
				GameObject door = InstantiateDoor(pos, currentFloor, i);
				_openList.Add(door.transform);
				currentDoorWeight -= doorWeight / (maxDoorsPerFloor);
			}
			else
			{
				RaycastHit hit;
				//todo remove that debug once it works fine
				Debug.DrawLine(pos + Vector3.up * currentFloor * _heightFloor + Vector3.up * 4.0f,
					pos + Vector3.up * currentFloor * _heightFloor + Vector3.up * 4.0f +
					Quaternion.AngleAxis((i * 60) + 180, Vector3.up) * Vector3.forward * maxDistanceRayCastLink,
					Color.red, 3);
				//---------
				if (Physics.Raycast(pos + Vector3.up * currentFloor * _heightFloor + Vector3.up * 4.0f,
					Quaternion.AngleAxis((i * 60) + 180, Vector3.up) * Vector3.forward, out hit, linkLayer))
				{
					Debug.Log("hit");
					if (hit.transform.CompareTag("Link"))
					{
						Debug.Log("bravo");
						InstantiateDoor(pos, currentFloor, i);
					}
				}
				else
				{
					Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)],
						pos + Vector3.up * currentFloor * _heightFloor, Quaternion.Euler(0, i * 60, 0),
						_towers[_towers.Count - 1].transform);
					currentWallWeight -= wallWeight / (6 - minDoorsPerFloor);
				}
			}
		}

		//build the bridges from the doors
		foreach (var link in _openList)
		{
			Vector3 baseNextTower = GetHexPos(link.position, link.eulerAngles.y);
			baseNextTower.y = 0;
			BuildTowerBase(baseNextTower);
		}
	}

	private GameObject InstantiateDoor(Vector3 pos, int currentFloor, int i)
	{
		return Instantiate(bridgePrefabs[Random.Range(0, bridgePrefabs.Length)],
			pos + Vector3.up * currentFloor * _heightFloor, Quaternion.Euler(0, i * 60, 0),
			_towers[_towers.Count - 1].transform);
	}

	private void BuildTowerBase(Vector3 pos)
	{
		_towers.Add(new GameObject("Tower"));
		_towers[_towers.Count - 1].transform.position = pos;
		_towers[_towers.Count - 1].transform.parent = transform;

		//build ground first floor
		Instantiate(groundPrefabs[Random.Range(0, groundPrefabs.Length)], pos,
			Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), _towers[_towers.Count - 1].transform);

		//build walls and make sure there is at least two doors
		float currentDoorWeight = doorWeight;
		float currentWallWeight = wallWeight;

		for (int i = 0; i < 6; i++)
		{
			if (Random.Range(0.0f, 1.0f) * currentDoorWeight > Random.Range(0.0f, 1.0f) * currentWallWeight)
			{
				GameObject door = Instantiate(doorPrefabs[Random.Range(0, doorPrefabs.Length)], pos,
					Quaternion.Euler(0, i * 60, 0),
					_towers[_towers.Count - 1].transform);
				currentDoorWeight -= doorWeight / (maxDoorsPerFloor);
			}
			else
			{
				Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], pos, Quaternion.Euler(0, i * 60, 0),
					_towers[_towers.Count - 1].transform);
				currentWallWeight -= wallWeight / (6 - minDoorsPerFloor);
			}
		}

		float angleStairs = Random.Range(0, 6) * 60;

		//build ground second floor
		Instantiate(interiorRampPrefabs[Random.Range(0, interiorRampPrefabs.Length)],
			pos + Vector3.up * _heightFloor, Quaternion.Euler(0, angleStairs, 0), _towers[_towers.Count - 1].transform);
	}

	private Vector3 GetHexPos(Vector3 initPos, float angle)
	{
		if (Mathf.Approximately(angle, 0))
		{
			return initPos + Vector3.forward * -_widthHexa;
		}
		else if (Mathf.Approximately(angle, 60))
		{
			return initPos + Vector3.right * -(3.0f / 2.0f) * _heightHexa + Vector3.forward * -_widthHexa / 2;
		}
		else if (Mathf.Approximately(angle, 120))
		{
			return initPos + Vector3.right * -(3.0f / 2.0f) * _heightHexa + Vector3.forward * _widthHexa / 2;
		}
		else if (Mathf.Approximately(angle, 180))
		{
			return initPos + Vector3.forward * _widthHexa;
		}
		else if (Mathf.Approximately(angle, 240))
		{
			return initPos + Vector3.right * (3.0f / 2.0f) * _heightHexa + Vector3.forward * _widthHexa / 2;
		}
		else
		{
			return initPos + Vector3.right * (3.0f / 2.0f) * _heightHexa + Vector3.forward * -_widthHexa / 2;
		}
	}
}