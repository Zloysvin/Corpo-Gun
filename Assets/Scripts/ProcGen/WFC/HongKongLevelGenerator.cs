using System.Collections.Generic;
using UnityEngine;

public class HongKongLevelGenerator : MonoBehaviour
{
    // I am using plane as a base floor, due to simplicity in rotation. Plane at scale 1 is actually 10 by 10
    // so, to make it 1 by 1, scale need to be decreased to 0.1
    private const float FLOOR_SCALE = 0.1f;
    private int[,] _levelMap;
    private Vector2 _mapCenter;
    private bool _mapIsGenerated;

    [Header("Prefab References")]
    [SerializeField]
    private GameObject _floorPrefab;

    [Header("Base Floor Settings")]
    [SerializeField]
    private int _minBaseFloorSize = 1;
    [SerializeField]
    private int _maxBaseFloorSize = 1;

    [Header("Additional Floor Settings")]
    [SerializeField] 
    private int _numOfAdditionalFloors = 0;
    [SerializeField]
    private int _minAdditionalFloorPosDeviation = 0;
    [SerializeField]
    private int _maxAdditionalFloorPosDeviation = 0;
    [SerializeField]
    private int _minAdditionalFloorSize = 1;
    [SerializeField]
    private int _maxAdditionalFloorSize = 1;

    void Start()
    {
        GenerateFloors();
    }

    private void GenerateFloors()
    {
        var baseFloor = Instantiate(_floorPrefab, Vector3.zero, Quaternion.identity);
        baseFloor.transform.localScale = Vector3.one * FLOOR_SCALE * Random.Range(_minBaseFloorSize, _maxBaseFloorSize);

        float checkScale = baseFloor.transform.localScale.x * 10f;


        // This check, and similar check below, is required because if floor has a scale that's odd, then it's edges
        // will have coordinates that are not whole numbers. Issue is that all squares will have different scales,
        // so I had to unify them. Plus it's much easier to work when your coordinate system is based on whole numbers
        if (checkScale % 2 == 0)
        {
            baseFloor.transform.localScale = Vector3.one * (baseFloor.transform.localScale.x + FLOOR_SCALE);
        }

        List<GameObject> additionalFloors = new List<GameObject>();

        for (int i = 0; i < _numOfAdditionalFloors; i++)
        {
            int posX = (1 + 2 * (Random.Range(0, 2) * -1)) *
                       Random.Range(_minAdditionalFloorPosDeviation, _maxAdditionalFloorPosDeviation);
            int posY = (1 + 2 * (Random.Range(0, 2) * -1)) *
                       Random.Range(_minAdditionalFloorPosDeviation, _maxAdditionalFloorPosDeviation);

            GameObject floor = Instantiate(_floorPrefab, new Vector3(posX, 0f, posY), Quaternion.identity);
            floor.transform.localScale = Vector3.one * FLOOR_SCALE * Random.Range(_minAdditionalFloorSize, _maxAdditionalFloorSize);

            checkScale = floor.transform.localScale.x * 10f;

            if (checkScale % 2 == 0)
            {
                floor.transform.localScale = Vector3.one * (floor.transform.localScale.x + FLOOR_SCALE);
            }

            additionalFloors.Add(floor);
        }

        GenerateMap(baseFloor, additionalFloors);
        //SetEdges();
    }

    private void GenerateMap(GameObject baseFloor, List<GameObject> AdditionalFloors)
    {
        var bounds = GetMapBounds(baseFloor, AdditionalFloors);
        var boundsX = bounds.Item1;
        var boundsY = bounds.Item2;

        // Displaying Map Bounds
        Debug.DrawLine(new Vector3(boundsX.Item1, 0, boundsY.Item2), new Vector3(boundsX.Item2, 0, boundsY.Item2), Color.green, 120f);
        Debug.DrawLine(new Vector3(boundsX.Item2, 0, boundsY.Item2), new Vector3(boundsX.Item2, 0, boundsY.Item1), Color.green, 120f);
        Debug.DrawLine(new Vector3(boundsX.Item2, 0, boundsY.Item1), new Vector3(boundsX.Item1, 0, boundsY.Item1), Color.green, 120f);
        Debug.DrawLine(new Vector3(boundsX.Item1, 0, boundsY.Item1), new Vector3(boundsX.Item1, 0, boundsY.Item2), Color.green, 120f);

        _levelMap = new int[(int)(-boundsX.Item1 + boundsX.Item2 + 1), (int)(-boundsY.Item1 + boundsY.Item2 + 1)];
        _mapCenter = new Vector2(-boundsX.Item1, -boundsY.Item1);

        for (int i = 0; i < _levelMap.GetLength(0); i++)
        {
            for (int j = 0; j < _levelMap.GetLength(1); j++)
            {
                _levelMap[i, j] = 0;
            }
        }

        SetFloorOnMap(ConvertToMapCoords(baseFloor, _mapCenter), baseFloor.transform.localScale);

        foreach (var floor in AdditionalFloors)
        {
            SetFloorOnMap(ConvertToMapCoords(floor, _mapCenter), floor.transform.localScale);
        }

        _mapIsGenerated = true;
    }

    private ((float, float), (float, float)) GetMapBounds(GameObject baseFloor, List<GameObject> AdditionalFloors)
    {
        // I am using scale.x because all floors are shaped as squares, so all scale axis are equal
        float baseBounds = (baseFloor.transform.localScale.x - FLOOR_SCALE) / 2f * 10f;
        (float, float) boundsX = new(-baseBounds, baseBounds);
        (float, float) boundsY = new(-baseBounds, baseBounds);

        foreach (var floor in AdditionalFloors)
        {
            baseBounds = (floor.transform.localScale.x - FLOOR_SCALE) / 2f * 10f;

            if (floor.transform.position.x - baseBounds < boundsX.Item1)
            {
                boundsX.Item1 = floor.transform.position.x - baseBounds;
            }

            if (floor.transform.position.x + baseBounds > boundsX.Item2)
            {
                boundsX.Item2 = floor.transform.position.x + baseBounds;
            }

            if (floor.transform.position.z - baseBounds < boundsY.Item1)
            {
                boundsY.Item1 = floor.transform.position.z - baseBounds;
            }

            if (floor.transform.position.z + baseBounds > boundsY.Item2)
            {
                boundsY.Item2 = floor.transform.position.z + baseBounds;
            }
        }

        return (boundsX, boundsY);

    }

    private Vector2 ConvertToMapCoords(GameObject obj, Vector2 center)
    {
        return new Vector2(center.x + obj.transform.position.x, center.y + obj.transform.position.z);
    }

    private void SetFloorOnMap(Vector2 origin, Vector3 size)
    {
        float baseBounds = (size.x - FLOOR_SCALE) / 2f * 10f;

        for (int x = (int)(origin.x - baseBounds); x <= (int)(origin.x + baseBounds); x++)
        {
            for (int y = (int)(origin.y - baseBounds); y <= (int)(origin.y + baseBounds); y++)
            {
                _levelMap[x, y] = 1;
            }
        }
    }

    private void SetEdges()
    {
        for (int x = 0; x < _levelMap.GetLength(0); x++)
        {
            for (int y = 0; y < _levelMap.GetLength(1); y++)
            {
                int emptyCount = 0;

                if (x - 1 >= 0)
                {
                    if (_levelMap[x - 1, y] == 0)
                    {
                        emptyCount++;
                    }
                }
                else
                {
                    emptyCount++;
                }

                if (x + 1 < _levelMap.GetLength(0))
                {
                    if (_levelMap[x + 1, y] == 0)
                    {
                        emptyCount++;
                    }
                }
                else
                {
                    emptyCount++;
                }

                if (y - 1 >= 0)
                {
                    if (_levelMap[x, y - 1] == 0)
                    {
                        emptyCount++;
                    }
                }
                else
                {
                    emptyCount++;
                }

                if (y + 1 < _levelMap.GetLength(1))
                {
                    if (_levelMap[x, y + 1] == 0)
                    {
                        emptyCount++;
                    }
                }
                else
                {
                    emptyCount++;
                }

                if (emptyCount != 4 && emptyCount != 0)
                {
                    _levelMap[x, y] = 2;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (_mapIsGenerated)
            {
                for (int i = 0; i < _levelMap.GetLength(0); i++)
                {
                    for (int j = 0; j < _levelMap.GetLength(1); j++)
                    {
                        if (_levelMap[i, j] != 0)
                        {
                            if (_levelMap[i, j] == 1)
                            {
                                Gizmos.color = Color.yellow;
                            }
                            if (_levelMap[i, j] == 2)
                            {
                                Gizmos.color = Color.blue;
                            }

                            Gizmos.DrawCube(new Vector3(-_mapCenter.x + i, 0f, -_mapCenter.y + j),
                                new Vector3(0.9f, 0.1f, 0.9f));
                        }
                    }
                }
            }
        }
    }
}
