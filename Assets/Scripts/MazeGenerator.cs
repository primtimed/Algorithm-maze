using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    #region ||Unity Inspector||
    [SerializeField] private Vector2Int _mazeSize = new Vector2Int(10, 10); // set the size of the maze
    [SerializeField] private GameObject _mazeNodePrefab; // Object node

    private List<GameObject> _grid = new List<GameObject>(); // Save grid in list
    private HashSet<GameObject> _visitedNodes = new HashSet<GameObject>(); // Track visited nodes
    private List<GameObject> _path = new List<GameObject>();  // Save the path you already have

    private Vector2Int[] _directions = {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0)  // Left
    }; // Direction the maze can go

    private GameObject _currentNode; // The point you are now
    #endregion

    #region ||Unity start void||
    private void Start() // Start the Grid create
    {
        _empty = new GameObject(); // Create a empty
        _empty.transform.parent = transform;

        StartCoroutine(CreateGrid());
    }
    #endregion

    #region ||Maze generate||
    IEnumerator CreateGrid() // Create the grid
    {
        for (int x = 0; x < _mazeSize.x; x++)
        {
            for (int y = 0; y < _mazeSize.y; y++)
            {
                GameObject node = Instantiate(_mazeNodePrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                _grid.Add(node);
            }

            yield return null;
        }

        yield return StartCoroutine(CreateMaze());
    }
    
    public int _delay = 0; // Is for that you can speedup the maze building. !IT WIL BE LOOKING BUGY HOW HIGHER IT GO!
    int _saveDelay;
    IEnumerator CreateMaze() // Create the paths
    {
        _currentNode = _grid[Random.Range(0, _grid.Count)];
        _currentNode.GetComponent<Renderer>().material.color = Color.green; // End node Color

        _currentNode = _grid[Random.Range(0, _grid.Count)];
        _path.Add(_currentNode);
        _visitedNodes.Add(_currentNode); // Mark as visited
        _currentNode.GetComponent<Renderer>().material.color = Color.red; // Starting node Color

        _saveDelay = _delay;

        while (_path.Count > 0) // Continue until all paths are made
        {
            if(_delay > 0)
            {
                List<GameObject> neighbors = GetUnvisitedNeighbors(_currentNode);

                if (neighbors.Count > 0)
                {
                    GameObject nextNode = neighbors[Random.Range(0, neighbors.Count)];

                    CreatePath(_currentNode, nextNode);

                    _currentNode = nextNode;
                    _path.Add(_currentNode);
                    _visitedNodes.Add(_currentNode);
                }

                else // Dead end. Backtrack to the previous node
                {
                    _path.RemoveAt(_path.Count - 1); // Remove the current node from the stack

                    if (_path.Count > 0)
                    {
                        _currentNode = _path[_path.Count - 1]; // Move back to the previous node
                    }
                }

                _delay--;
            }

            else
            {
                yield return null;
                _delay = _saveDelay;
            }

        }

        yield return null;
        Debug.Log("Maze complete!");
    }

    List<GameObject> GetUnvisitedNeighbors(GameObject node) // Get all info from neighbors
    {
        List<GameObject> neighbors = new List<GameObject>();
        Vector3 nodePosition = node.transform.position;

        foreach (var direction in _directions) // For all direction the maze can be
        {
            Vector3 neighborPos = nodePosition + new Vector3(direction.x, 0, direction.y);
            GameObject neighbor = _grid.Find(g => g.transform.position == neighborPos);

            if (neighbor != null && !_visitedNodes.Contains(neighbor)) // Check if not visited
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    GameObject _empty;
    void CreatePath(GameObject fromNode, GameObject toNode) // Create lineRenderer for the path
    {
        // Create a new LineRenderer for each path
        GameObject LineObj = Instantiate(_empty, fromNode.transform.position, fromNode.transform.rotation, transform); // Create a Empty for the lineRenderer
        LineRenderer line = LineObj.AddComponent<LineRenderer>(); // Add lineRenderer to Empty

        line.SetPosition(0, fromNode.transform.position);
        line.SetPosition(1, toNode.transform.position);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        // Set up the material for the LineRenderer
        Material lineMaterial = new Material(Shader.Find("Unlit/Color")); // create material for lineRenderer

        lineMaterial.color = Color.white; // Set color of the lineRenderer material
        line.material = lineMaterial; // Add the material to lineRenderer
    }
    #endregion
}