using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridChecker : MonoBehaviour
{

    private List<Enemy> _enemies;
    public GameObject GridCellPrefab;
    private Dictionary<int, SpriteRenderer> _cells;
    private int _gridDimension = 9;

    [HideInInspector]
    public List<List<Enemy>> _enemyCells;
    public List<List<bool>> _enemyChecked;

    public Color[] Colors;
    public Color UnmatchableColor;
    public Color DefaultColor;

    private Spawner _spawner;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _enemyCells = new List<List<Enemy>>();
        _enemyChecked = new List<List<bool>>();
        for (int i = 0; i < _gridDimension; i++)
        {
            var row = new List<Enemy>();
            var row_checked = new List<bool>();
            for (int j = 0; j < _gridDimension; j++)
            {
                row.Add(null);
                row_checked.Add(false);
            }
            _enemyCells.Add(row);
            _enemyChecked.Add(row_checked);
        }
        _enemies = new List<Enemy>();
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        _spawner.RemoveEnemy(enemy);
    }

    public void UpdateEnemyGrid(Enemy enemy, int old_x, int old_y)
    {
        int new_x = Mathf.RoundToInt(enemy.transform.position.x + 4f);
        int new_y = Mathf.RoundToInt(enemy.transform.position.y + 4f);

        if (new_x < 0 || new_x >= _gridDimension || new_y < 0 || new_y >= _gridDimension)
            return;

        if (_enemyCells[new_x][new_y] == null)
        {

            if (old_x >= 0 && old_x < _gridDimension && old_y >= 0 && old_y < _gridDimension)
                _enemyCells[old_x][old_y] = null;
            _enemyCells[new_x][new_y] = enemy;
            enemy.OldX = new_x;
            enemy.OldY = new_y;
        }
    }

    // Use this for initialization
    void Start()
    {
        _spawner = GameObject.FindObjectOfType<Spawner>();
        _cells = new Dictionary<int, SpriteRenderer>();
        for (int i = -_gridDimension / 2; i <= _gridDimension / 2; i++)
        {
            for (int j = -_gridDimension / 2; j <= _gridDimension / 2; j++)
            {
                var c = GameObject.Instantiate(GridCellPrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                _cells.Add(i + j * _gridDimension, c.GetComponent<SpriteRenderer>());
            }
        }
    }

    bool CheckAndAdd(int i, int j, Enemy original, List<Enemy> list)
    {
        if (i >= 0 && i < _gridDimension && j >= 0 && j < _gridDimension)
            if (_enemyChecked[i][j] == false && _enemyCells[i][j] != null && _enemyCells[i][j].AllowedToMatch && _enemyCells[i][j].MatchType == original.MatchType)
            {
                list.Add(_enemyCells[i][j]);
                return true;
            }

        return false;
    }

    List<Enemy> CheckVertical(int i, int j)
    {
        var l = new List<Enemy>();
        l.Add(_enemyCells[i][j]);
        bool check_failed = false;
        int d = 1;
        while (!check_failed && d < _gridDimension)
        {
            check_failed = !CheckAndAdd(i, j + d, _enemyCells[i][j], l);
            d++;
        }
        d = 1;
        check_failed = false;
        while (!check_failed && d < _gridDimension)
        {
            check_failed = !CheckAndAdd(i, j - d, _enemyCells[i][j], l);
            d++;
        }

        return l;
    }

    List<Enemy> CheckHorizontal(int i, int j)
    {
        var l = new List<Enemy>();
        l.Add(_enemyCells[i][j]);
        bool check_failed = false;
        int d = 1;
        while (!check_failed && d < _gridDimension)
        {
            check_failed = !CheckAndAdd(i + d, j, _enemyCells[i][j], l);
            d++;
        }
        d = 1;
        check_failed = false;
        while (!check_failed && d < _gridDimension)
        {
            check_failed = !CheckAndAdd(i - d, j, _enemyCells[i][j], l);
            d++;
        }

        return l;
    }

    public void DestroyGroup(List<Enemy> enemies)
    {
        var position = Vector3.zero;
        foreach (var enemy in enemies)
        {
            position += enemy.transform.position;
            enemy.Die(true, enemies.Count);
        }
        position = position / enemies.Count;
        Effect.SpawnEffect("Explosion_Big", position);

        var r = Random.Range(0, 1f);

        if (r < 0.1f && enemies.Count > 3)
        {
            Bonus.SpawnBonus(BonusType.Mega, position);
        }
        else if (r < 0.25f)
        {
            Bonus.SpawnBonus(BonusType.Triple, position);
        }
        else if (r < 0.40f)
        {
            Bonus.SpawnBonus(BonusType.Double, position);
        }
        else if (r < 0.60f)
        {
            Bonus.SpawnBonus(BonusType.Health, position);
        }

        CameraShake.ShakeCamera(5f);
    }

    public void CheckMatch()
    {
        for (int i = 0; i < _gridDimension; i++)
            for (int j = 0; j < _gridDimension; j++)
                _enemyChecked[i][j] = false;

        for (int i = 0; i < _gridDimension; i++)
            for (int j = 0; j < _gridDimension; j++)
            {
                if (_enemyCells[i][j] != null && _enemyCells[i][j].AllowedToMatch && _enemyChecked[i][j] == false)
                {
                    var vertical = CheckVertical(i, j);
                    var horizontal = CheckHorizontal(i, j);
                    foreach (var enemy in vertical)
                    {
                        _enemyChecked[enemy.OldX][enemy.OldY] = true;
                    }
                    foreach (var enemy in horizontal)
                    {
                        _enemyChecked[enemy.OldX][enemy.OldY] = true;
                    }

                    if (vertical.Count >= 3 || horizontal.Count >= 3)
                    {
                        if (horizontal.Count > vertical.Count)
                        {
                            DestroyGroup(horizontal);
                        }
                        else
                        {
                            DestroyGroup(vertical);
                        }
                    }

                }
            }

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetButton("Match"))
        {
            CheckMatch();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_enemies == null)
            return;

        foreach (var e in _enemies)
        {
            UpdateEnemyGrid(e, e.OldX, e.OldY);
        }

        foreach (var cell in _cells)
        {
            cell.Value.color = DefaultColor;
        }

        for (int i = 0; i < _gridDimension; i++)
        {
            for (int j = 0; j < _gridDimension; j++)
            {
                var enemy = _enemyCells[i][j];
                if (enemy != null)
                {
                    var index = (i - 4) + _gridDimension * (j - 4);

                    if (!_cells.ContainsKey(index))
                        return;
                    var color = UnmatchableColor;
                    if (enemy.AllowedToMatch)
                    {
                        var c = Colors[enemy.MatchType];
                        color = new Color(c.r, 0.3f * c.r, c.b, 1f);
                    }

                    _cells[index].color = color;
                }
            }

        }

        CheckMatch();

    }
}
