using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class Module
{
    public GameObject value;
    public int[] Sockets = new int[4];
}

public class Tile
{
    public List<Module> Modules { get; set; }

    public bool hasDeveloped = false;

    public Tile()
    {
    }
}

public class WFC : MonoBehaviour
{
    public List<Module> allModules = new List<Module>();

    public int height;
    public int width;

    private Tile[,] map;

    private Module[] previusModules;
    private int previusY;
    private int previusX;

    private int tryCount;
    private bool needToRegenerate = true;

    void Start()
    {
        GetModulesFromPrefabs();

        while (needToRegenerate)
        {
            map = new Tile[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = new Tile();
                }
            }


            InsertRandom(1, 1);
            needToRegenerate = false;
            Solve();
            if (!needToRegenerate)
            {
                Draw();
            }
        }
    }

    private void GetModulesFromPrefabs()
    {
        foreach (var module in allModules)
        {
            MonoModule monoModule = module.value.GetComponent<MonoModule>();
            module.Sockets = new[]
                { monoModule.socketTop, monoModule.socketRight, monoModule.socketBottom, monoModule.socketLeft };
        }
    }

    private bool CheckIfSolved()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (map[i, j].Modules == null || map[i, j].Modules.Count != 1)
                    return false;
            }
        }

        return true;
    }

    public void Solve()
    {
        while (!CheckIfSolved())
        {
            int minY = -1;
            int minX = -1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (map[i, j].Modules != null && !map[i, j].hasDeveloped)// && map[i, j].Modules.Count > 1)
                    {
                        if (minX > -1 && minY > -1)
                        {
                            if (map[minY, minX].Modules.Count > map[i, j].Modules.Count && map[minY, minX].Modules.Count != map[i, j].Modules.Count)
                            {
                                minY = i;
                                minX = j;
                            }
                        }
                        else
                        {
                            minY = i;
                            minX = j;
                        }
                    }
                }
            }
            if (minX > -1 && minY > -1)
                Collapse(minY, minX);

            if (needToRegenerate)
                break;
        }
    }

    private void Collapse(int y, int x)
    {

        if (map[y, x].Modules.Count != 0)
        {
            map[y, x].hasDeveloped = true;
            System.Random rnd = new System.Random();

            Module module = map[y, x].Modules[rnd.Next(map[y, x].Modules.Count)];
            previusModules = map[y, x].Modules.ToArray();
            previusY = y;
            previusX = x;

            map[y, x].Modules.Clear();
            map[y, x].Modules.Add(module);


            SetNeighborCells(y, x, -1, 0, module);
            SetNeighborCells(y, x, 0, 1, module);
            SetNeighborCells(y, x, 1, 0, module);
            SetNeighborCells(y, x, 0, -1, module);

            FixDiagonals(y, x, -1, 1);
            FixDiagonals(y, x, 1, 1);
            FixDiagonals(y, x, 1, -1);
            FixDiagonals(y, x, -1, -1);
        }
        else
        {
            if (tryCount >= 5)
            {
                needToRegenerate = true;
                return;
            }
            Debug.Log($"Try Count no {tryCount + 1}");
            tryCount++;
            map[previusY, previusX].hasDeveloped = false;
            map[previusY, previusX].Modules.Clear();
            map[previusY, previusX].Modules.AddRange(previusModules);
        }
    }

    private void SetNeighborCells(int y, int x, int yMod, int xMod, Module module)
    {
        if (y + yMod >= 0 && y + yMod < height && x + xMod >= 0 && x + xMod < width)
        {
            int currentSocket = math.abs(yMod) * (1 + yMod) + math.abs(xMod) * (2 - xMod);
            int targetSocket = math.abs(yMod) * (1 - yMod) + math.abs(xMod) * (2 + xMod);
            if (map[y + yMod, x + xMod].Modules is { Count: > 1 })
            {
                List<Module> modulesToDelete = map[y + yMod, x + xMod].Modules
                    .Where(mod => module.Sockets[currentSocket] != mod.Sockets[targetSocket]).ToList();

                foreach (var mod in modulesToDelete)
                {
                    map[y + yMod, x + xMod].Modules.Remove(mod);
                }
            }
            else if (map[y + yMod, x + xMod].Modules == null)
            {
                map[y + yMod, x + xMod].Modules = new List<Module>();

                foreach (var mod in allModules.Where(mod => module.Sockets[currentSocket] == mod.Sockets[targetSocket]))
                {
                    map[y + yMod, x + xMod].Modules.Add(mod);
                }
            }
        }
    }
    private void FixDiagonals(int y, int x, int yMod, int xMod)
    {
        if (y + yMod >= 0 && y + yMod < height && x + xMod >= 0 && x + xMod < width)
        {
            int currentSocketVert = math.abs(yMod) * (1 - yMod);
            int currentSocketHoriz = math.abs(xMod) * (2 + xMod);

            int targetSocketVert = math.abs(yMod) * (1 + yMod);
            int targetSocketHoriz = math.abs(xMod) * (2 - xMod);

            if (map[y + yMod, x + xMod].Modules is { Count: > 1 })
            {

                List<Module> modulesToAdd = new List<Module>();
                foreach (var mod1 in map[y + yMod, x].Modules)
                {
                    foreach (var moduleDiag in map[y + yMod, x + xMod].Modules)
                    {
                        if (moduleDiag.Sockets[currentSocketHoriz] == mod1.Sockets[targetSocketHoriz])
                        {
                            foreach (var mod2 in map[y, x + xMod].Modules)
                            {
                                if (moduleDiag.Sockets[currentSocketVert] == mod2.Sockets[targetSocketVert])
                                {
                                    if (!modulesToAdd.Contains(moduleDiag))
                                    {
                                        modulesToAdd.Add(moduleDiag);
                                    }
                                }
                            }
                        }
                    }
                }

                map[y + yMod, x + xMod].Modules = modulesToAdd;
            }
            else if (map[y + yMod, x + xMod].Modules == null)
            {
                map[y + yMod, x + xMod].Modules = new List<Module>();

                foreach (var allMod in allModules)
                {
                    foreach (var mod1 in map[y + yMod, x].Modules)
                    {
                        if (mod1.Sockets[targetSocketHoriz] == allMod.Sockets[currentSocketHoriz])
                        {
                            foreach (var mod2 in map[y, x + xMod].Modules)
                            {
                                if (mod2.Sockets[targetSocketVert] == allMod.Sockets[currentSocketVert] && !map[y + yMod, x + xMod].Modules.Contains(allMod))
                                {
                                    map[y + yMod, x + xMod].Modules.Add(allMod);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void InsertRandom(int y, int x)
    {
        map[y, x].Modules = new List<Module>();
        map[y, x].Modules.AddRange(allModules);
        Collapse(y, x);
    }

    private void Draw()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                System.Random rnd = new System.Random();
                Instantiate(map[i, j].Modules[0].value, new Vector3(j * 1.1f, -1.1f * i, 0), map[i, j].Modules[0].value.transform.rotation);
            }
        }
    }
}
