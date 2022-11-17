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
    public SO_postavke postavke;
    public Transform playerTr, enemyTr;
    readonly Vector3 faceIn = new Vector3(90f, 90f, 180f);
    readonly Vector3 faceOut = new Vector3(90f, 180f, 90f);
    [HideInInspector] public PlayerControll playerControll;
    public Color colAlly;
    public Color colEnemy;
    public SplineContainer splineContainer;
    //pickup
    public GameObject fuelParent;
    RectTransform fuelMeter;
    readonly float fuel0pos = -191f;
    public GameObject ammoParent;
    TextMeshProUGUI ammoDisplay;
    [SerializeField] TextMeshProUGUI pointsDisplay;
    //canvas
    [SerializeField] Transform canParent;
    [HideInInspector] public GameObject[] canvases;
    //pools
    [SerializeField] Transform poolBullets;
    Transform[] _bulletsTr;
    Projectile[] _bullets;
    GameObject[] _bulletsGo;
    int _counterBullets;

    int _points;

    public ClosedAreas closedAreas;
    private void Awake()
    {
        gm = this;
        _bulletsTr = HelperScript.GetAllChildernByType<Transform>(poolBullets);
        _bullets = HelperScript.GetAllChildernByType<Projectile>(poolBullets);
        _bulletsGo = new GameObject[_bullets.Length];
        for (int i = 0; i < _bullets.Length; i++)
        {
            _bulletsGo[i] = _bullets[i].gameObject;
        }
        fuelMeter = fuelParent.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>();
        ammoDisplay = ammoParent.GetComponent<TextMeshProUGUI>();
        canvases = new GameObject[canParent.childCount];
        for (int i = 0; i < canParent.childCount; i++)
        {
            canvases[i] = canParent.GetChild(i).gameObject;
        }
        playerControll = playerTr.GetComponent<PlayerControll>();
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
        EventManager.GameReady();

        _points = postavke.score;
        PointsMethod(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
    public void IniConnect(SplineContainer container, bool facingOutward, SplineAnimate pAnim, int fuel, int ammo, SplineAnimate eAnim, ShootingMode[] sm, float[] pocetnePozicije)
    {
        closedAreas = new ClosedAreas(gridControll.tiles);
        splineContainer = container;
        playerTr.parent = pAnim.transform;
        playerTr.localPosition = Vector3.zero;
        playerTr.localEulerAngles = facingOutward ? faceOut : faceIn;
        pAnim.splineContainer = container;
        pAnim.gameObject.SetActive(true);
        StartCoroutine(playerTr.GetComponent<RailCannon>().Inicijalizacija(sm[0], pocetnePozicije[0]));

        if (fuel <= 0)
        {
            playerTr.GetComponent<PlayerControll>().hasFuelFeature = false;
            fuelParent.SetActive(false);
        }
        if (ammo > 0) playerTr.GetComponent<PlayerControll>().Ammo = ammo;
        else
        {
            playerTr.GetComponent<PlayerControll>().hasAmmoFeature = false;
            ammoParent.SetActive(false);
        }
        
        playerTr.gameObject.SetActive(true);

        if (eAnim != null)
        {
            enemyTr.parent = eAnim.transform;
            enemyTr.localPosition = Vector3.zero;
            enemyTr.localEulerAngles = facingOutward ? faceOut : faceIn;
            eAnim.splineContainer = container;
            eAnim.gameObject.SetActive(true);
            StartCoroutine(enemyTr.GetComponent<RailCannon>().Inicijalizacija(sm[1], pocetnePozicije[1]));
            enemyTr.gameObject.SetActive(true);
        }
    }

    #region//UI
    public void PointsMethod(int point)
    {
        _points += point;
        postavke.score = _points;
        pointsDisplay.text = $"Score: {_points}";
    }
    public void FuelDisplay(float fuel)
    {
        fuelMeter.anchoredPosition = new Vector2(0f, fuel0pos - (fuel * fuel0pos));
    }
    public void AmmoDisplay(int ammo)
    {
        ammoDisplay.text = $"Ammuntion: {ammo}";
    }
    #endregion

    #region//EVENTS
    protected override void GameStart()
    {
        base.GameStart();
        canvases[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level: " + postavke.level;
    }
    protected override void EndWin(string message, int level, bool victory)
    {
        base.EndWin(message, level, victory);
        string display = victory ? "You've won!" : "Game Over!";
       // print("Level " + level + " --------- " + display);
        canvases[2].SetActive(true);
        Image img = canvases[2].transform.GetChild(0).GetComponent<Image>();
        img.DOFade(0.8f, 2f)
            .From(0f)
            .SetEase(Ease.InOutQuint);
        canvases[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
    }
    #endregion

    #region//POOLS
    public void ShootBullet(Faction fact, Transform spawnPoint)
    {
        _bulletsTr[_counterBullets].SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        _bullets[_counterBullets].faction = fact;
        _bulletsGo[_counterBullets].SetActive(true);
        _counterBullets = (1 + _counterBullets) % _bullets.Length;
    }
    #endregion

    #region//BUTTONS
    public void Btn_Restart()
    {
        SceneManager.LoadScene(gameObject.scene.buildIndex);
    }
    public void Btn_NextLv()
    {
        postavke.level++;
        if (postavke.level > SceneManager.sceneCountInBuildSettings - 2) postavke.level = 1;
        Btn_Restart();
    }
    #endregion
}

public class ClosedAreas
{
    Tile[,] _allTiles;
    List<Tile> _openTiles = new List<Tile>();
    public class Sector
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
        _allSectors.Add(new Sector());
        _allSectors[0].tiles.Add(_openTiles[0]);

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
                EventManager.LevelDoneWin("Game over! There are enclosed tiles and it is impossible to finish level!", GameManager.gm.postavke.level, false);
                return;
            }
        }

    }

}

