// --------------------------------------------------------------------------------------------------------------------
// <copyright file="City.cs" company="Virtual Campus Lda">
//   Copyright (c) 2015, Virtual Campus, http://www.virtual-campus.eu
//   Copyright (c) 2020, University of Thessaly 
//    Modifications made by: Olivier Heidmann, Kostantinos Katsimentes, Dimitrios Ziogas
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using MiniJSON;
using UnityEngine;
using TGS;
using Unity.Burst;


[BurstCompile]


public class City : MonoBehaviour
{
    #region Fields

    public bool RunSimulation = true;

    private readonly Dictionary<string, CityLayer> layersDictionary = new Dictionary<string, CityLayer>();

    private readonly List<CityLayer> layersList = new List<CityLayer>();

    private readonly List<OverlayCityLayer> overlays = new List<OverlayCityLayer>();

    private readonly List<MasterOverlayLayer> masterOverlays = new List<MasterOverlayLayer>();

    [SerializeField]
    private string description;

    [SerializeField]
    private List<string> flags = new List<string>();

    [SerializeField]
    private int money;
    [SerializeField]
    private int wood;
    public int ConsumedWood=0;
    public int ProducedWood = 0;
    [SerializeField]
    private int stone;
    public int ConsumedStone = 0;
    public int ProducedStone = 0;
    [SerializeField]
    private int coal;
    public int ConsumedCoal = 0;
    public int ProducedCoal= 0;
    [SerializeField]
    private int ore;
    public int ConsumedOre = 0;
    public int ProducedOre = 0;
    [SerializeField]
    private int rareOre;
    public int ConsumedRareOre = 0;
    public int ProducedRareOre = 0;
    [SerializeField]
    private int oil;
    public int ConsumedOil = 0;
    public int ProducedOil = 0;
    [SerializeField]
    private int gas;
    public int ConsumedGas = 0;
    public int ProducedGas = 0;
    [SerializeField] 
    private int food;
    public int ConsumedFood = 0;
    public int ProducedFood = 0;
    [SerializeField]
    private int garbage;
    public int ConsumedGarbage = 0;
    public int ProducedGarbage = 0;

   
    [SerializeField]
    private int population;
    [SerializeField]
    private int tilesize = 8;

    [SerializeField]
    private int size = 128;

    private int realSize = 1024;

  

    [SerializeField]
    private string title;
    private DataLayer populationLayer;
    private ResourcesManager resourcesManager;
    private UnitsLayer unitsLayer;
    public TerrainGridSystem tgs;

    [SerializeField]
    private TerrainManager terrainManager;

    public ExtractorSimulator extractorSimulator;

    #endregion

    #region Public Events

    public event Action onObjectivesChanged;

    #endregion

    #region Public Properties

    public string Description
    {
        get
        {
            return this.description;
        }

        set
        {
            this.description = value;
        }
    }
    public TerrainGridSystem Tgs
    {
        get
        {
            return this.tgs;
        }
        set
        {
            this.tgs = value;
        }
    }


    
    public int MoneyProduced { get; set; }
    public int MoneySpend { get; set; }
    public int EnergyProduced { get; set; }
    public int EnergyConsumed { get; set; }
    public int AnimalsGenerated { get; set; }
    public int AnimalsDestroyed{ get; set; }
    public int TreesGenerated { get; set; }
    public int TreesDestroyed { get; set; }

    public int Income { get; set; }
    public int Expenses { get; set; }
    public int IndustriesProfit { get; set; }
    public int CommerceProfit { get; set; }

    public int Money
    {
        get
        {
            return this.money;
        }

        set
        {
            this.money = value;
        }
    }

   


    public int Wood
    {
        get
        {
            return this.wood;
        }

        set
        {
            this.wood= value;
        }
    }

    public int Stone
    {
        get
        {
            return this.stone;
        }

        set
        {
            this.stone = value;
        }
    }

    public int Coal
    {
        get
        {
            return this.coal;
        }

        set
        {
            this.coal = value;
        }
    }

    public int Ore
    {
        get
        {
            return this.ore;
        }

        set
        {
            this.ore = value;
        }
    }

    public int RareOre
    {
        get
        {
            return this.rareOre;
        }

        set
        {
            this.rareOre = value;
        }
    }

    public int Oil
    {
        get
        {
            return this.oil;
        }

        set
        {
            this.oil = value;
        }
    }

    public int Gas
    {
        get
        {
            return this.gas;
        }

        set
        {
            this.gas = value;
        }
    }

    public int Food
    {
        get
        {
            return this.food;
        }

        set
        {
            this.food = value;
        }
    }

    public int Garbage
    {
        get
        {
            return this.garbage;
        }
        set
        {
            this.garbage = value;
        }
    }

    public int Biodiversity
    {
        get
        {
            var terrain = this.GetLayer<TerrainRealLayer>("Terrain");
            terrain.CalculateBiodiversityIndex();
            return terrain.biodiversity;
        }
        set
        {
            this.GetLayer<TerrainRealLayer>("Terrain").biodiversity = value;
        }
    }

    public int TreeAmount
    {
        get
        {
            return this.GetLayer<TerrainRealLayer>("Terrain").treeAmount;
        }
        set
        {
           this.GetLayer<TerrainRealLayer>("Terrain").treeAmount = value;
        }
    }

    public int InitialTreeAmount
    {
        get
        {
            return this.GetLayer<TerrainRealLayer>("Terrain").initialTreeAmount;
        }
        set
        {
            this.GetLayer<TerrainRealLayer>("Terrain").initialTreeAmount = value;
        }
    }


    public int AnimalAmount
    {
        get
        {
            return this.GetLayer<TerrainRealLayer>("Terrain").animalAmount;
        }
        set
        {
            this.GetLayer<TerrainRealLayer>("Terrain").animalAmount = value;
        }
    }
    public int Population
    {
        get
        {
            return this.population;
        }

        set
        {
            this.population = value;
        }
    }

    public bool ShouldExecuteGameStartScripts { get; private set; }

    public int Size
    {
        get
        {
            return this.size;
        }
        set
        {
            this.size = value;
        }
    }

    public int RealSize
    {
        get
        {
            return this.realSize;
        }
        set
        {
            this.realSize = value;
        }
    }

    public string Title
    {
        get
        {
            return this.title;
        }

        set
        {
            this.title = value;
        }
    }

    #endregion

    #region Public Methods and Operators

    public void AddFlag(string value)
    {
        if (!this.flags.Contains(value))
        {
            this.flags.Add(value);
        }
    }

    public void AutoBulldoze(IEnumerable<TileIndex> tiles)
    {
        foreach (var cityLayer in this.layersList)
        {
            cityLayer.AutoBulldoze(tiles);
        }
    }

    public void ClearFlags()
    {
        this.flags.Clear();
    }

    public void ClearTiles(IEnumerable<TileIndex> tiles, bool deliberate, bool deep, params string[] ignoreLayers)
    {
        var t = new HashSet<TileIndex>(tiles);

        var deletedTiles = new List<TileIndex>();

        foreach (var cityLayer in this.layersList)
        {
            if (t.Count == 0)
            {
                break;
            }

            if (ignoreLayers != null && ignoreLayers.Contains(cityLayer.name))
            {
                continue;
            }

            if (deep)
            {
                cityLayer.ClearTiles(t, deliberate, null);
            }
            else
            {
                deletedTiles.Clear();

                if (cityLayer.ClearTiles(t, deliberate, deletedTiles))
                {
                    foreach (var deletedTile in deletedTiles)
                    {
                        t.Remove(deletedTile);
                    }
                }
            }
        }
    }

    public void DestroyLayers(IEnumerable<TileIndex> tiles, string[] destroyLayers)
    {
        foreach (var layer in destroyLayers)
        {
            if (this.layersDictionary.ContainsKey(layer))
            {
                this.layersDictionary[layer].ClearTiles(tiles, true, null);
            }
        }
    }

    public IEnumerable<string> GetFlags()
    {
        return this.flags.ToArray();
    }

    public T GetLayer<T>(string layerName) where T : CityLayer
    {
        CityLayer layer;

        if (this.layersDictionary.TryGetValue(layerName, out layer))
        {
            //Debug.Log("Layer " + layerName + " found.");
            return layer as T;
        }

        

        return null;
    }

    public IEnumerable<T> GetLayers<T>() where T : CityLayer
    {
        return this.layersList.Where(layer => layer is T).Cast<T>();
    }

    public IEnumerable<OverlayCityLayer> GetOverlays()
    {
        return this.overlays;
    }

    public IEnumerable<MasterOverlayLayer> GetMasterOverlays()
    {
        return this.masterOverlays;
    }

    public bool HasFlag(string value)
    {
        return this.flags.Contains(value);
    }

    public void Initialize(string data)
    {
        this.Initialize(GameSetup.ForCity(data));
    }

    public void Initialize(GameSetup setup)
    {
        foreach (var layer in this.GetComponentsInChildren<CityLayer>())
        {
            this.layersDictionary[layer.name] = layer;
        }

        foreach (var masterOverlay in this.GetComponentsInChildren<MasterOverlayLayer>())
        {
            this.masterOverlays.Add(masterOverlay);
        }

        this.layersList.AddRange(this.layersDictionary.Values.OrderByDescending(layer => layer.Priority));

        this.populationLayer = GetLayer<DataLayer>("Population");

        this.resourcesManager = FindObjectOfType<ResourcesManager>();

        //this.terrainManager.Initialize();

        //this.size = setup.TerrainSize;
        this.size = (int)(TerrainRealLayer.GetTerrain().terrainData.size.x / tilesize);

        this.realSize = (int)(TerrainRealLayer.GetTerrain().terrainData.size.x);
        

        if (ManagerGlobalVariables.Instance.currentscreen != 2)
        {
           // this.money = setup.Budget;


            if (setup.IsUnlimitedMoney)
            {
                this.Money = 1000000000;
                this.Wood = 1000000000;
                this.Stone = 1000000000;
                this.Coal = 1000000000;
                this.Ore = 1000000000;
                this.RareOre = 1000000000;
                this.Oil = 1000000000;
                this.Gas = 1000000000;
                this.Food = 1000000000;
                this.Garbage = 1000000000;
            }

        }

        this.population = 0;
        this.title = string.IsNullOrEmpty(ManagerEditorVariables.Instance.scenario_name) ? "New City" : ManagerEditorVariables.Instance.scenario_name;
        this.flags.Clear();
        this.ShouldExecuteGameStartScripts = setup.IsGameStart;
       
        
        var isLoading = !string.IsNullOrEmpty(setup.CityData);
        if (isLoading)
        {
            this.Load(setup.CityData);

            this.title = string.IsNullOrEmpty(ManagerEditorVariables.Instance.scenario_name) ? "New City" : ManagerEditorVariables.Instance.scenario_name; ;

            resourcesManager.SetUpToolPlacementRestrictions();  /// 
           
        }
        else
        {
            foreach (var layer in this.layersList)
            {
                layer.InitializeLayer(this, null);
            }

            resourcesManager.SetUpMapResources();

            foreach (var layer in this.layersList)
            {
                layer.PostInitializeLayer();
            }
        }

        foreach (var layer in this.layersList)
        {
            if (layer is OverlayCityLayer)
            {
                this.overlays.Add(layer as OverlayCityLayer);
            }
        }



    }

    public bool IsOccupied(TileIndex tile, string[] ignoreLayers)
    {
        if (this.IsOutsideBounds(new TileIndex(tile.X, tile.Y)))
        {
            return true;
        }

        if (ignoreLayers != null && ignoreLayers.Contains("*"))
        {
            return false;
        }

        foreach (var layer in this.layersList)
        {
            if (ignoreLayers != null && ignoreLayers.Contains(layer.name))
            {
                continue;
            }

            if (layer.IsOccupied(tile))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsOccupiedInGeneral(TileIndex tile, string[] ignoreLayers)
    {
        if (this.IsOutsideBounds(new TileIndex(tile.X, tile.Y)))
        {
            return true;
        }

        if (ignoreLayers != null && ignoreLayers.Contains("*"))
        {
            return false;
        }

        foreach (var layer in this.layersList)
        {
            if (ignoreLayers != null && ignoreLayers.Contains(layer.name))
            {
                continue;
            }

            if (layer.IsOccupiedInGeneral(tile))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsOutsideBounds(TileIndex tile)
    {
        return tile.X < 1 || tile.Y < 1 || tile.X >= this.Size - 1 || tile.Y >= this.Size - 1;
    }

    public bool IsOutsideBoundsFlatten(TileIndex tile)
    {
        return tile.X < 0 || tile.Y < 0 || tile.X >= this.RealSize || tile.Y >= this.RealSize;
    }

    public void RecalculatePopulation()
    {
        this.Population = 0;

        foreach (var residence in this.GetLayer<UnitsLayer>("Units").GetUnitsWithTrait<Residence>())
        {
            this.Population += residence.Population;

            var radious = residence.Unit.SizeX > residence.Unit.SizeY ? residence.Unit.SizeX : residence.Unit.SizeY;
            this.populationLayer.DisplaceValueWithFallout(residence.Unit.CenterTile, radious + 4, residence.Population);
        }

        GameObject.Find("GameGui - Canvas").GetComponent<GameGui>().UpdateOurDisplay();
    }


    public void RemoveFlag(string value)
    {
        if (this.flags.Contains(value))
        {
            this.flags.Remove(value);
        }
    }

    public string Save()
    {
        var data = new Dictionary<string, object>();

        data["Size"] = this.Size;
        if (ManagerGlobalVariables.Instance.currentscreen == 1)
        {
          
            data["Money"] = ManagerEditorVariables.Instance.value_budget;
            data["Wood"] = ManagerEditorVariables.Instance.value_wood;
            data["Stone"] = ManagerEditorVariables.Instance.value_stone;
            data["Coal"] = ManagerEditorVariables.Instance.value_coal;
            data["Ore"] = ManagerEditorVariables.Instance.value_ore;
            data["RareOre"] = ManagerEditorVariables.Instance.value_rare_ore;
            data["Oil"] = ManagerEditorVariables.Instance.value_oil;
            data["Gas"] = ManagerEditorVariables.Instance.value_gas;
            data["Food"] = ManagerEditorVariables.Instance.value_food;
            data["Garbage"] = ManagerEditorVariables.Instance.value_garbage;
            
            Debug.Log("Saving SCENARIO EDITOR: " + data["Money"]);
           
        }
        else
        {
            data["Money"] = this.Money;
            data["Wood"] = this.Wood;
            data["Stone"] = this.Stone;
            data["Coal"] = this.Coal;
            data["Ore"] = this.Ore;
            data["RareOre"] = this.RareOre;
            data["Oil"] = this.Oil;
            data["Gas"] = this.Gas;
            data["Food"] = this.Food;
            data["Garbage"] = this.Garbage;

            // endgame data values 
            data["money_produced"] = MoneyProduced;
            data["money_spend"] = MoneySpend;
            data["energy_produced"] = EnergyProduced;
            data["energy_consumed"] = EnergyConsumed;
            data["trees_generated"] = TreesGenerated;
            data["trees_destroyed"] = TreesDestroyed;
            data["animals_generated"] = AnimalsGenerated;
            data["animals_destroyed"] = AnimalsDestroyed;
            data["initial_trees"] = InitialTreeAmount;
            Debug.Log("Saving GAME MODE: " + data["Money"]);
        }

       

        data["Population"] = this.Population;
        data["Title"] = this.Title;
        data["Description"] = this.Description;
        data["Flags"] = this.SerializeFlags();

        var layerDataList = new List<object>();

        foreach (var layer in this.layersList)
        {
            var layerData = new Dictionary<string, object>();
            layerData["Name"] = layer.name;
            layerData["Data"] = layer.SaveLayer();
            layerDataList.Add(layerData);
        }

        data["Layers"] = layerDataList;
       
        return Json.Serialize(data);
    }

    public void SetFlags(IEnumerable<string> flags)
    {
        this.ClearFlags();
        this.flags.AddRange(flags);
    }

    public void ShowUnderground(bool value)
    {
        foreach (var layer in this.layersList)
        {
            layer.ShowUnderground(value);
        }
    }

    #endregion

    #region Methods

    private void Load(string json)
    {
        var data = Json.Deserialize(json) as Dictionary<string, object>;

        if (data == null)
        {
            Debug.Log("Invalid data: " + json);
            Destroy(Game.Instance.Gui.gameObject);
            Destroy(Game.Instance.gameObject);
            Destroy(this.gameObject);
            SceneManager.LoadScene("MainMenu");
            ManagerGlobalVariables.Instance.goingbacktomainmenu = true;
            ManagerGlobalVariables.Instance.currentscreen = 0;
            EventManager.MainMenuStarted();
            //Application.LoadLevel("Menu");
            return;
        }

        this.size = SerializationHelper.GetInt(data, "Size", 128);
        this.money = SerializationHelper.GetInt(data, "Money");
        ////  NEW SAVED VALUES START
        this.wood = SerializationHelper.GetInt(data, "Wood");
        this.Stone = SerializationHelper.GetInt(data, "Stone");
        this.Coal = SerializationHelper.GetInt(data, "Coal");
        this.Ore = SerializationHelper.GetInt(data, "Ore");
        this.RareOre = SerializationHelper.GetInt(data, "RareOre");
        this.Oil = SerializationHelper.GetInt(data, "Oil");
        this.Gas = SerializationHelper.GetInt(data, "Gas");
        this.Food = SerializationHelper.GetInt(data, "Food");
        this.Garbage = SerializationHelper.GetInt(data, "Garbage");

       // END GAME VALUES 
       if(data.TryGetValue("money_produced",out object outvalue))
            this.MoneyProduced = SerializationHelper.GetInt(data, "money_produced");
       else  
            MoneyProduced = 0;
       if (data.TryGetValue("money_spend", out object outvalue2))
            this.MoneySpend = SerializationHelper.GetInt(data, "money_spend"); 
       else  
            MoneySpend = 0;
       if (data.TryGetValue("energy_produced", out object outvalue3))
           this.EnergyProduced = SerializationHelper.GetInt(data, "energy_produced");
       else
           EnergyProduced = 0;

       if (data.TryGetValue("energy_consumed", out object outvalue4))
           this.EnergyConsumed = SerializationHelper.GetInt(data, "energy_consumed");
       else
           EnergyConsumed = 0;

       if (data.TryGetValue("trees_generated", out object outvalue5))
           this.TreesGenerated = SerializationHelper.GetInt(data, "trees_generated");
       else
           TreesGenerated = 0;
       if (data.TryGetValue("trees_destroyed", out object outvalue6))
           this.TreesDestroyed = SerializationHelper.GetInt(data, "trees_destroyed");
       else
            TreesDestroyed = 0;

        if (data.TryGetValue("animals_generated", out object outvalue7))
            this.AnimalsGenerated = SerializationHelper.GetInt(data, "animals_generated");
        else
            AnimalsGenerated = 0;
        if (data.TryGetValue("trees_destroyed", out object outvalue8))
            this.AnimalsDestroyed = SerializationHelper.GetInt(data, "animals_destroyed");
        else
            AnimalsDestroyed = 0;
        if (data.TryGetValue("initial_trees", out object outvalue9))
            this.InitialTreeAmount = SerializationHelper.GetInt(data, "initial_trees");
        else
            InitialTreeAmount = TreeAmount;

        ///  NEW SAVED VALUES END
        this.population = SerializationHelper.GetInt(data, "Population");
        this.title = SerializationHelper.GetString(data, "Title");
        this.description = SerializationHelper.GetString(data, "Description");
        this.LoadFlags(SerializationHelper.GetString(data, "Flags"));

        var layersDataJson = (List<object>)data["Layers"];
        var layersData = new Dictionary<string, Dictionary<string, object>>();

        // if in editor mode set budget to infinte
        if (ManagerGlobalVariables.Instance.currentscreen == 1)
        {
            this.Money = 1000000000;           
            this.Wood = 1000000000;
            this.Stone = 1000000000;
            this.Coal = 1000000000;
            this.Ore = 1000000000;
            this.RareOre = 1000000000;
            this.Oil = 1000000000;
            this.Gas = 1000000000;
            this.Food = 1000000000;
            this.Garbage =1000000000;
        }
       



        foreach (Dictionary<string, object> layerDataJson in layersDataJson)
        {
            var layerName = SerializationHelper.GetString(layerDataJson, "Name");
            var layerData = SerializationHelper.GetObject(layerDataJson, "Data");
            layersData[layerName] = layerData;
        }

        foreach (var layer in this.layersList)
        {
            if (layersData.ContainsKey(layer.name))
            {
                layer.InitializeLayer(this, layersData[layer.name]);
            }
            else
            {
                layer.InitializeLayer(this, null);
            }
        }

        foreach (var layer in this.layersList)
        {
            if (layersData.ContainsKey(layer.name))
            {
            }
            else
            {
                layer.PostInitializeLayer();
            }
        }

         unitsLayer = this.GetLayer<UnitsLayer>("Units");
       
    }

    private void LoadFlags(string value)
    {
        this.flags.Clear();

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        this.flags.AddRange(value.Split(';'));
    }

    private string SerializeFlags()
    {
        if (this.flags.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(";", this.flags.ToArray());
    }


    //target new code
    public bool TargetAchieved()
    {
        bool goal = true;
        if (this.Wood < ManagerEditorVariables.Instance.target_value_wood) { goal = false; }
        if (this.Stone < ManagerEditorVariables.Instance.target_value_stone) { goal = false; }
        if (this.Coal < ManagerEditorVariables.Instance.target_value_wood) { goal = false; }
        if (this.Ore < ManagerEditorVariables.Instance.target_value_ore) { goal = false; }
        if (this.RareOre < ManagerEditorVariables.Instance.target_value_rare_ore) { goal = false; }
        if (this.Oil < ManagerEditorVariables.Instance.target_value_oil) { goal = false; }
        if (this.Gas < ManagerEditorVariables.Instance.target_value_gas) { goal = false; }
        if (this.Food < ManagerEditorVariables.Instance.target_value_food) { goal = false; }
        if (this.Garbage < ManagerEditorVariables.Instance.target_value_garbage) { goal = false; }
        if (this.Population<ManagerEditorVariables.Instance.target_population) { goal = false; }
        return goal;
    }
    #endregion
}