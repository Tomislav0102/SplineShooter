using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Splines;

public class GameManager : EventManager
{
    public static GameManager gm;
    [HideInInspector] public GridControll gridControll;
    [HideInInspector] public LevelManager levelManager;
    public Camera mainCam;
    public SO_postavke postavke;
    [HideInInspector] public PlayerControll playerControll;
    public Color colAlly;
    public Color colEnemy;
    public SplineContainer splineContainer;

    [Header("--CLASSES")]
    public PoolManager poolManager;
    public UImanager uimanager;
    public ClosedAreas closedAreas;


    #region//INITIALIZATION
    private void Awake()
    {
        gm = this;
    }

    IEnumerator Start()
    {
        for (int i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (SceneManager.GetSceneByBuildIndex(i).isLoaded) SceneManager.UnloadSceneAsync(i);
        }
        SceneManager.LoadScene(postavke.level + 1, LoadSceneMode.Additive);
        while(!SceneManager.GetSceneByBuildIndex(postavke.level + 1).isLoaded)
        {
            yield return null;
        }

        GameReady?.Invoke();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        uimanager.btnNext.onClick.AddListener(Btn_NextLv);
        uimanager.btnRestart.onClick.AddListener(Btn_Restart);
        uimanager.btnQuit.onClick.AddListener(Btn_QuitToMain);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        uimanager.btnNext.onClick.RemoveListener(Btn_NextLv);
        uimanager.btnRestart.onClick.RemoveListener(Btn_Restart);
        uimanager.btnQuit.onClick.RemoveListener(Btn_QuitToMain);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        //if (Input.GetKeyDown(KeyCode.Alpha1)) poolManager.DeployShield(playerControll);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) poolManager.DeployShield(enemyTr.GetComponent<RailCannon>());
    }
    #endregion

    #region//EVENTS
    protected override void GameStart()
    {
        base.GameStart();
        closedAreas = new ClosedAreas(gridControll.tiles);
        poolManager.Ini();
        uimanager.Ini();
    }
    protected override void EndWin(string message, int level, bool victory)
    {
        base.EndWin(message, level, victory);
        uimanager.EndLevel(message);
    }
    #endregion

    #region//BUTTONS
    public void Btn_NextLv()
    {
        postavke.level++;
        if (postavke.level > SceneManager.sceneCountInBuildSettings - 2) postavke.level = 1;
        Btn_Restart();
    }
    public void Btn_Restart()
    {
        SceneManager.LoadScene(gameObject.scene.buildIndex);
    }
    public void Btn_QuitToMain()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}

public class ClosedAreas
{
    Tile[,] _allTiles;
    List<Tile> _openTiles = new List<Tile>();
    class Sector
    {
        public HashSet<Tile> tiles = new HashSet<Tile>();
        bool _hasExit;
        public bool HasExit
        {
            get => _hasExit;
            set
            {
                _hasExit = value;
                foreach (Tile item in tiles)
                {
                    item.HasExit = _hasExit;
                }
            }
        }
    }
    List<Sector> _allSectors = new List<Sector>();

    public ClosedAreas(Tile[,] tiles)
    {
        _allTiles = tiles;
    }

    bool ValidNeighbour(Tile neigbourTile, int x, int y)
    {
        if (Mathf.Abs(x) != Mathf.Abs(y) && neigbourTile.CurrentState == TileState.Open) return true;
        return false;
    }
    bool ExitNeighbour(Tile neigbourTile, int x, int y)
    {
        if (Mathf.Abs(x) != Mathf.Abs(y) && neigbourTile.CurrentState == TileState.Danger) return true;
        return false;
    }
    public void EnclosureCheck()
    {
        _openTiles.Clear();
        _allSectors.Clear();
        for (int i = 0; i < _allTiles.GetLength(0); i++)
        {
            for (int j = 0; j < _allTiles.GetLength(1); j++)
            {
                if (_allTiles[i, j].CurrentState == TileState.Open)
                {
                    _openTiles.Add(_allTiles[i, j]);
                    _allTiles[i, j].SectorOrdinal = -1;
                }
            }
        }
        if (_openTiles.Count <= 0) return;

        _allSectors.Add(new Sector());
        _allSectors[0].tiles.Add(_openTiles[0]);
        RecursionMethod(_openTiles[0]);

        for (int k = 0; k < _openTiles.Count; k++)
        {
            Tile tl = _openTiles[k];
            if (tl.SectorOrdinal == -1)
            {
                _allSectors.Add(new Sector());
                _allSectors[_allSectors.Count - 1].tiles.Add(tl);
                UpdateGroup();
            }
            RecursionMethod(tl);
        }

        CheckForExits();
    }
    void RecursionMethod(Tile tl)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Tile neighbour = _allTiles[tl.pozicija.x + i, tl.pozicija.y + j];
                if (!ValidNeighbour(neighbour, i, j)) continue;
                if (neighbour.SectorOrdinal == -1)
                {
                    _allSectors[_allSectors.Count - 1].tiles.Add(neighbour);
                    UpdateGroup();
                    RecursionMethod(neighbour);
                }
                else if (neighbour.SectorOrdinal != tl.SectorOrdinal)
                {
                    foreach (Tile item in _allSectors[neighbour.SectorOrdinal].tiles)
                    {
                        _allSectors[tl.SectorOrdinal].tiles.Add(item);
                    }
                    _allSectors.RemoveAt(neighbour.SectorOrdinal);
                    UpdateGroup();
                    RecursionMethod(neighbour);
                }


            }
        }

    }
    void UpdateGroup()
    {
        for (int i = 0; i < _allSectors.Count; i++)
        {
            foreach (Tile item in _allSectors[i].tiles)
            {
                item.SectorOrdinal = i;
            }
        }
    }
    void CheckForExits()
    {
        for (int k = 0; k < _allSectors.Count; k++)
        {
            foreach (Tile item in _allSectors[k].tiles)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        Tile neighbour = _allTiles[item.pozicija.x + i, item.pozicija.y + j];
                        if (ExitNeighbour(neighbour, i, j))
                        {
                            //  print($"Sve grupe rb je {k}, a nighbour je {neighbour.pozicija}");
                            _allSectors[k].HasExit = true;
                        }
                    }
                }
            }
        }
        for (int k = 0; k < _allSectors.Count; k++)
        {
            if (!_allSectors[k].HasExit)
            {
                EventManager.LevelDoneWin?.Invoke("Game over! There are enclosed tiles and it is impossible to finish level!", GameManager.gm.postavke.level, false);
                return;
            }
        }

    }

}

[System.Serializable]
public class PoolManager
{
    [SerializeField] Transform poolBullets, poolShields;

    Transform[] _bulletsTr;
    Projectile[] _bullets;
    GameObject[] _bulletsGo;
    int _counterBullets;

    Shield[] _shields;
    GameObject[] _shieldsGo;
    int _counterShields;

    public void Ini()
    {
        _bulletsTr = HelperScript.GetAllChildernByType<Transform>(poolBullets);
        _bullets = HelperScript.GetAllChildernByType<Projectile>(poolBullets);
        _bulletsGo = new GameObject[_bullets.Length];
        for (int i = 0; i < _bullets.Length; i++)
        {
            _bulletsGo[i] = _bullets[i].gameObject;
        }

        _shields = HelperScript.GetAllChildernByType<Shield>(poolShields);
        _shieldsGo = new GameObject[_shields.Length];
        for (int i = 0; i < _shields.Length; i++)
        {
            _shieldsGo[i] = _shields[i].gameObject;
        }

    }
    public void ShootBullet(Faction fact, Transform spawnPoint)
    {
        _bulletsTr[_counterBullets].SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        _bullets[_counterBullets].faction = fact;
        _bulletsGo[_counterBullets].SetActive(true);
        _counterBullets = (1 + _counterBullets) % _bullets.Length;
    }
    public void DeployShield(RailCannon railCannonTraget)
    {
        if (railCannonTraget.activeShield != null) return;
        _shields[_counterShields].DeployMe(railCannonTraget);
        _shieldsGo[_counterShields].SetActive(true);
        _counterShields = (1 + _counterShields) % _shields.Length;
    }

}

[System.Serializable]
public class UImanager
{
    GameManager gm;

    public GameObject fuelParent;
    RectTransform _fuelMeter;
    const float CONST_FUELSTARTPOS = -191f;
    public GameObject ammoParent;
    TextMeshProUGUI _ammoDisplay;
    [SerializeField] TextMeshProUGUI pointsDisplay;
    const int CONST_SHIELDAWARD = 1000;

    [SerializeField] Transform canParent;
    GameObject[] canvases;
    public Button btnNext, btnRestart, btnQuit;

    public void Ini()
    {
        gm = GameManager.gm;
        _fuelMeter = fuelParent.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>();
        _ammoDisplay = ammoParent.GetComponent<TextMeshProUGUI>();
        canvases = new GameObject[canParent.childCount];
        for (int i = 0; i < canParent.childCount; i++)
        {
            canvases[i] = canParent.GetChild(i).gameObject;
        }
        PointsMethod(0);
        canvases[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level: " + gm.postavke.level;

    }

    public void PointsMethod(int point)
    {
        gm.postavke.score += point;
        pointsDisplay.text = $"Score: {gm.postavke.score}";

        //  int shieldAward = gm.postavke.score / CONST_SHIELDAWARD;
        ////  Debug.Log(shieldAward);
        //  if (shieldAward > gm.postavke.shieldsByScore)
        //  {
        //      gm.postavke.shieldsByScore++;
        //      gm.poolManager.DeployShield(gm.playerControll);
        //  }
        if (gm.postavke.score >= CONST_SHIELDAWARD)
        {
            gm.postavke.score -= CONST_SHIELDAWARD;
            gm.poolManager.DeployShield(gm.playerControll);
        }
    }
    public void FuelDisplay(float fuel)
    {
        _fuelMeter.anchoredPosition = new Vector2(0f, CONST_FUELSTARTPOS - (fuel * CONST_FUELSTARTPOS));
    }
    public void AmmoDisplay(int ammo)
    {
        _ammoDisplay.text = $"Ammuntion: {ammo}";
    }

    public void EndLevel(string message)
    {
        canvases[2].SetActive(true);
        Image img = canvases[2].transform.GetChild(0).GetComponent<Image>();
        img.DOFade(0.8f, 2f)
            .From(0f)
            .SetEase(Ease.InOutQuint);
        canvases[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
    }
}



