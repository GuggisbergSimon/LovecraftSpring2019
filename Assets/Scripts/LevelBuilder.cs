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
	[SerializeField] private float doorWeightFirst = 0.25f;
	[SerializeField] private float wallWeightFirst = 0.75f;
	[SerializeField] private float doorWeightOthers = 0.5f;
	[SerializeField] private float wallWeightOthers = 0.5f;
	[SerializeField] private int minDoorsPerFloor = 2;
	[SerializeField] private int maxDoorsPerFloor = 6;
	[SerializeField] private LayerMask linkLayer = 0;
	[SerializeField] private LayerMask notLinkLayer = 0;
	[SerializeField] private float maxDistanceRayCastLink = 30.0f;
	[SerializeField] private int maxIteration = 10;
	private float _sizeHexa;
	private float _widthHexa = 42.784f;
	private float _heightHexa;
	private float _heightFloor = 9.0f;
	private List<Transform> _openList = new List<Transform>();
	private List<Transform> _closedList = new List<Transform>();
	private List<GameObject> _towers = new List<GameObject>();
	private List<Vector3> _specialRooms = new List<Vector3>();

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
		List<Transform> tempOpenList = new List<Transform>();
		BuildTowerFirstLevel(tempOpenList, pos, currentFloor, doorWeightFirst, wallWeightFirst);
		foreach (var item in tempOpenList)
		{
			_openList.Add(item);
		}

		tempOpenList.Clear();
		int currentIteration = 0;

		float doorWeightLocal = doorWeightFirst;
		float wallWeightLocal = wallWeightFirst;
		while (_openList.Count > 0)
		{
			//build the base of a tower in front of each door
			foreach (var link in _openList)
			{
				Vector3 baseNextTower = GetHexPos(link.position, link.eulerAngles.y);
				bool canBuild = true;
				foreach (var closedLink in _closedList)
				{
					if (GetHexPos(closedLink.position, closedLink.eulerAngles.y).Equals(baseNextTower))
					{
						canBuild = false;
					}
				}

				if (canBuild)
				{
					baseNextTower.y = 0;

					BuildTowerBase(baseNextTower);
					BuildTowerFirstLevel(tempOpenList, baseNextTower, currentFloor, doorWeightLocal, wallWeightLocal);
				}

				_closedList.Add(link);
			}

			foreach (var item in _closedList)
			{
				_openList.Remove(item);
			}

			foreach (var item in tempOpenList)
			{
				_openList.Add(item);
			}

			tempOpenList.Clear();
			currentIteration++;
			//generate closed rooms for next iteration
			if (currentIteration > maxIteration)
			{
				doorWeightLocal = 0;
				wallWeightLocal = 1;
			}
		}

		currentFloor++;
		while (currentFloor < maxFloors)
		{
			for (int i = 0; i < _towers.Count; i++)
			{
				BuildTowerOtherLevels(_towers[i].transform.position,currentFloor,i);
			}
		}
	}

	private GameObject InstantiateDoor(Vector3 pos, int currentFloor, int i)
	{
		return Instantiate(bridgePrefabs[Random.Range(0, bridgePrefabs.Length)],
			pos + Vector3.up * currentFloor * _heightFloor, Quaternion.Euler(0, i * 60, 0),
			_towers[_towers.Count - 1].transform);
	}

	private GameObject InstantiateWall(Vector3 pos, int currentFloor, int i)
	{
		return Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)],
			pos + Vector3.up * currentFloor * _heightFloor, Quaternion.Euler(0, i * 60, 0),
			_towers[_towers.Count - 1].transform);
	}

	private void BuildTowerOtherLevels(Vector3 pos, int currentFloor, int towersIndex)
	{
		//build the ground
		Vector3 currentPos = pos + Vector3.up * currentFloor * _heightFloor;
		if (Random.Range(0f, currentFloor) < Random.Range(0f, maxFloors))
		{
			RaycastHit hitSelf;
			if (Physics.Raycast(currentPos + Vector3.up * 4.0f, Vector3.down,
				out hitSelf, _heightFloor - 0.1f))
			{
				Instantiate(interiorRampPrefabs[Random.Range(0, interiorRampPrefabs.Length)],
					pos + Vector3.up * currentFloor * _heightFloor,
					Quaternion.Euler(0, hitSelf.transform.eulerAngles.y, 0), _towers[towersIndex].transform);
				int doorsQuantity = 0;
				float currentDoorWeight = doorWeightOthers;
				float currentWallWeight = wallWeightOthers;

				for (int i = 0; i < 6; i++)
				{
					RaycastHit hitLink;
					//checks for link and build a bridge if there is
					if (Physics.Raycast(currentPos + Vector3.up * 4.0f,
						Quaternion.AngleAxis((i * 60) + 180, Vector3.up) * Vector3.forward, out hitLink,
						maxDistanceRayCastLink, linkLayer))
					{
						if (hitLink.transform.CompareTag("Link"))
						{
							InstantiateDoor(pos, currentFloor, i);
							currentDoorWeight -= doorWeightOthers / (maxDoorsPerFloor);
							doorsQuantity++;
						}
					}
					else if (Physics.Raycast(pos + Vector3.up * currentFloor * _heightFloor + Vector3.up * 4.0f,
						Quaternion.AngleAxis((i * 60) + 180, Vector3.up) * Vector3.forward, out hitLink,
						maxDistanceRayCastLink,
						notLinkLayer))
					{
						if (hitLink.transform.CompareTag("NotLink"))
						{
							InstantiateWall(pos, currentFloor, i);
							currentWallWeight -= wallWeightOthers / (6 - minDoorsPerFloor);
						}
					}
					else
					{
						RaycastHit hitNeighbor;
						if (Physics.Raycast(GetHexPos(currentPos, (i * 60)),
							Vector3.down, out hitNeighbor, _heightFloor - 0.1f))
						{
							if (Random.Range(0.0f, 1.0f) * currentDoorWeight >
							    Random.Range(0.0f, 1.0f) * currentWallWeight)
							{
								GameObject door = InstantiateDoor(pos, currentFloor, i);
								currentDoorWeight -= doorWeightOthers / (maxDoorsPerFloor);
								doorsQuantity++;
							}
							else
							{
								InstantiateWall(pos, currentFloor, i);
								currentWallWeight -= wallWeightOthers / (6 - minDoorsPerFloor);
							}
						}
					}
				}

				if (doorsQuantity < minDoorsPerFloor)
				{
					_specialRooms.Add(pos);
				}
			}
		}
		else
		{
			//build the top of a tower
			Instantiate(groundPrefabs[Random.Range(0, groundPrefabs.Length)],
				currentPos - Vector3.up * 0.5f, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0),
				_towers[towersIndex].transform);
			//todo remove that tower so that we do not iterate on it again !!!
		}
	}

	private void BuildTowerFirstLevel(List<Transform> openList, Vector3 pos, int currentFloor, float doorWeight,
		float wallWeight)
	{
		//build the walls and make sure there is at least 2 doors
		float currentDoorWeight = doorWeight;
		float currentWallWeight = wallWeight;
		int doorsQuantity = 0;

		for (int i = 0; i < 6; i++)
		{
			RaycastHit hit;
			//checks for link and build a bridge if there is
			if (Physics.Raycast(pos + Vector3.up * currentFloor * _heightFloor + Vector3.up * 4.0f,
				Quaternion.AngleAxis((i * 60) + 180, Vector3.up) * Vector3.forward, out hit, maxDistanceRayCastLink,
				linkLayer))
			{
				if (hit.transform.CompareTag("Link"))
				{
					InstantiateDoor(pos, currentFloor, i);
					currentDoorWeight -= doorWeight / (maxDoorsPerFloor);
					doorsQuantity++;
				}
			}
			//checks for no link (wall/window) and builds a no link if there is
			else if (Physics.Raycast(pos + Vector3.up * currentFloor * _heightFloor + Vector3.up * 4.0f,
				Quaternion.AngleAxis((i * 60) + 180, Vector3.up) * Vector3.forward, out hit, maxDistanceRayCastLink,
				notLinkLayer))

			{
				if (hit.transform.CompareTag("NotLink"))
				{
					InstantiateWall(pos, currentFloor, i);
					currentWallWeight -= wallWeight / (6 - minDoorsPerFloor);
				}
			}
			else
			{
				if (Random.Range(0.0f, 1.0f) * currentDoorWeight > Random.Range(0.0f, 1.0f) * currentWallWeight)
				{
					GameObject door = InstantiateDoor(pos, currentFloor, i);
					openList.Add(door.transform);
					currentDoorWeight -= doorWeight / (maxDoorsPerFloor);
					doorsQuantity++;
				}
				else
				{
					InstantiateWall(pos, currentFloor, i);
					currentWallWeight -= wallWeight / (6 - minDoorsPerFloor);
				}
			}
		}

		if (doorsQuantity < minDoorsPerFloor)
		{
			_specialRooms.Add(pos);
		}
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
		float currentDoorWeight = doorWeightFirst;
		float currentWallWeight = wallWeightFirst;

		for (int i = 0; i < 6; i++)
		{
			if (Random.Range(0.0f, 1.0f) * currentDoorWeight > Random.Range(0.0f, 1.0f) * currentWallWeight)
			{
				GameObject door = Instantiate(doorPrefabs[Random.Range(0, doorPrefabs.Length)], pos,
					Quaternion.Euler(0, i * 60, 0),
					_towers[_towers.Count - 1].transform);
				currentDoorWeight -= doorWeightFirst / (maxDoorsPerFloor);
			}
			else
			{
				Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], pos, Quaternion.Euler(0, i * 60, 0),
					_towers[_towers.Count - 1].transform);
				currentWallWeight -= wallWeightFirst / (6 - minDoorsPerFloor);
			}
		}

		//build ground second floor
		Instantiate(interiorRampPrefabs[Random.Range(0, interiorRampPrefabs.Length)],
			pos + Vector3.up * _heightFloor, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0),
			_towers[_towers.Count - 1].transform);
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