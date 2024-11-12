// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game.cs" company="Virtual Campus Lda">
//   Copyright (c) 2015, Virtual Campus, http://www.virtual-campus.eu
//   Copyright (c) 2020, University of Thessaly 
//    Modifications made by: Olivier Heidmann, Kostantinos Katsimentes
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using MiniJSON;
using Nature.UI;
using Unity.Burst;


[BurstCompile]
public class Game : MonoSingleton<Game>
{
    #region Fields

    public bool showMoney = false;

    [SerializeField]
    private City city;
    
    private CityView cityView = CityView.Surface;

    private OverlayCityLayer currentOverlay;

    private int currentSpeed = 2;

    private int disabledCounter = 0;

    [SerializeField]
    public GameGui gui;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    public LoadingPanel loadingPanel;

    private bool ignoreOverlayChanges = false;

    private bool isInteractionDisabled = false;

    private bool isPlaying = false;

    private int previousSpeed = 2;

    [SerializeField]
    private SelectionMesh selection;

    [SerializeField]
    private GameSetup setup;

    [SerializeField]
    private ThumbnailCamera thumbnailCamera;

    [SerializeField]
    private ToolManager tools;

    [SerializeField]
    private TimeManager timeManager;

    private Dictionary<string, string> unitInfo = new Dictionary<string, string>();

    private SaveDataManagement saveDataManagement;

    private ScenarioManagement scenarioManagement;

   // [SerializeField] private FadeCanvasGroup m_transitionCanvasGroup;

    [SerializeField]
    private EndGameScreen endGameScreen;
    [SerializeField]
    private GameObjectiveManager gameObjectiveManager;

    [SerializeField]
    private GameObject infoButton;

    private bool gameEnded = false;

    #endregion

    #region Public Properties

    public City City
    {
        get
        {
            return this.city;
        }
    }

    public OverlayCityLayer CurrentOverlay
    {
        get
        {
            return this.currentOverlay;
        }
    }

    public GameGui Gui
    {
        get
        {
            return this.gui;
        }
    }

    public bool IsInteractionDisabled
    {
        get
        {
            return this.isInteractionDisabled;
        }

        set
        {
            if (value)
            {
                ++this.disabledCounter;
            }
            else
            {
                --this.disabledCounter;
            }

            if (this.disabledCounter < 0)
            {
                this.disabledCounter = 0;
            }

            this.isInteractionDisabled = this.disabledCounter > 0;
            this.gui.IsInteractionDisabled = this.isInteractionDisabled;
            this.city.GetLayer<TimeLayer>("Time").IsPaused = this.isInteractionDisabled;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return this.isPlaying;
        }
    }

    public bool IsUnderground
    {
        get
        {
            return this.cityView == CityView.Underground;
        }
    }

    public bool GameEnded
    {
        get
        {
            return this.gameEnded;
        }
        set
        {
            this.gameEnded = value;
        }
    }

    public SelectionMesh Selection
    {
        get
        {
            return this.selection;
        }
    }

    public GameSetup Setup
    {
        get
        {
            return this.setup;
        }
    }

    public ToolManager Tools
    {
        get
        {
            return this.tools;
        }
    }

    public CityView View
    {
        get
        {
            return this.cityView;
        }
    }

    #endregion

    #region Public Methods and Operators

    public static void Play(GameSetup setup)
    {
        // ScreenFader.Instance.FadeOut(
        //     () =>
        //         {

        //         });
        GameSetup.Current = setup;
        SceneManager.LoadScene("Game");
        
       
        //Application.LoadLevel("Game");
    }

    public static void PlayDevCheat(GameSetup setup)
    {
        GameSetup.Current = setup;
        SceneManager.LoadScene("Game");
      
        //

    }

    public void ChangeSpeed(int speed)
    {
        this.previousSpeed = this.currentSpeed;
        this.currentSpeed = speed;

        this.gui.ChangeSpeed(this.currentSpeed);
    }

    public void EndGame()
    {
        ScreenFader.Instance.FadeOut(() => SceneManager.LoadScene("Main Menu UI"));
      
        ManagerGlobalVariables.Instance.goingbacktomainmenu = true;
        ManagerGlobalVariables.Instance.currentscreen = 0;
        ManagerGlobalVariables.Instance.currentMap = null;
        EventManager.MainMenuStarted();
        //ScreenFader.Instance.FadeOut(() => Application.LoadLevel("Menu"));
    }

    public string GetInfoForUnit(Unit unit)
    {
        var key = unit.PrefabName;

        if (this.unitInfo.ContainsKey(key))
        {
            return this.unitInfo[key];
        }

        return string.Empty;
    }

    public bool IsPauseScreenVisible() 
    {
        return this.gui.IsPauseScreenVisible();
    }

    public bool IsShowingMenu()
    {
        return this.gui.pauseScreen.IsVisible || this.gui.editorPauseScreen.IsVisible;
    }
    
    


    public void LoadEncyclopedia()
    {

        timeManager.PauseTime();
        StartCoroutine(ExitsceneTransition());      
            
        Debug.Log("loading enc finito");
        
    }
    EventSystem eventSystem1;
    public IEnumerator  ExitsceneTransition()
    {
        loadingPanel.Show();
        Debug.Log("intermediate test");
        yield return new WaitForSeconds(0.7f);
         //this.gameObject.transform.parent.gameObject.SetActive(false);       
        Debug.Log("disabling game camera ");
        mainCamera.GetComponent<AudioListener>().enabled = false;
        mainCamera.enabled = false;        
        gui.gameObject.SetActive(false);
        this.IsInteractionDisabled = true;
        eventSystem1 = FindObjectOfType<EventSystem>();        
        Debug.Log("start loading");
        var  loadingScenee = SceneManager.LoadSceneAsync("Main Menu Encyclopedia", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadingScenee.isDone);
        if (eventSystem1 != null) eventSystem1.enabled = false;
        var root = SceneManager.GetSceneByName("Main Menu Encyclopedia").GetRootGameObjects()[0];
        Debug.Log(root.name);
        root.transform.position = new Vector3(-2000, -1000, -2000);
        Debug.Log("loading ending");                       
        loadingPanel.Hide();      

    }

    
    public void EnableGameCameraAndEvent()
    {
        
        if (eventSystem1 != null) eventSystem1.enabled = true;
        mainCamera.enabled = true;
        mainCamera.GetComponent<AudioListener>().enabled = true;
        TogglePlaying();
        this.IsInteractionDisabled = false;
        this.gui.gameObject.SetActive(true);
        this.gui.UpdateOurDisplay();
        this.isPlaying = true;
    }

    public void MenuExit()
    {
        UnitsLayer unitsLayer = this.city.GetLayer<UnitsLayer>("Units");
        foreach (Unit unit in unitsLayer.units)
        {
            unit.UnSubscribeToTimeEvent();
        }

        TimeLayer timeLayer = this.city.GetLayer<TimeLayer>("Time");
        timeLayer.UnsubscribeFromEvents();

       // loadingPanel.Show();

        ManagerGlobalVariables.Instance.scenario = null;

        ManagerGlobalVariables.Instance.goingbacktomainmenu = true;
        ManagerGlobalVariables.Instance.currentscreen = 0;
        ManagerGlobalVariables.Instance.currentMap = null;

        SceneManager.LoadScene("Main Menu UI");

        EventManager.MainMenuStarted();
       // loadingPanel.Hide();
    }

    public void MenuExport()
    {
        CitySaveHelper.ExportCity(this.city);
    }

    public void MenuLoad()
    {
        /*var json = CitySaveHelper.LoadSaveData();

        if (json != null)
        {
            PlaySave(json);
        }*/
    }

    public void MenuRestart()
    {
        //AlertBox.Instance.ShowConfirm("@warning_restart", this.DoRestart);
    }

    public void MenuResume()
    {
        if (this.Setup.SetupType == GameSetupType.Editor)
        {
            this.gui.HideEditorPauseScreen();
        }
        else
        {
            this.gui.HidePauseScreen();
        }

        this.IsInteractionDisabled = false;
        this.tools.gameObject.SetActive(true);
    }

    public void MenuSave()
    {
        // CitySaveHelper.WriteSaveData(LoadLevel, this.City);
    }

    public void RegenerateTerrain()
    {

    }

    public void SaveAndExitEditor()
    {
        this.InternalSaveEditorLevel(true);
    }

    public void SaveEditorLevel()
    {
        this.InternalSaveEditorLevel(false);
    }

    public void SaveAndExitScenarioInstance()
    {
        this.SaveCurrentGameState(true);
    }

    public void SaveScenarioInstance()
    {
        this.SaveCurrentGameState(false);
    }

    public void SetTimeLimit(float normalizedAmount, int daysLeft)
    {
        this.gui.SetTimeLimit(normalizedAmount);
        /*this.gui.timeWidget.SetTooltip(
            daysLeft == 0 ? string.Empty : string.Format("@days_left", daysLeft));*/
    }

    public void ShowEndMenu()
    {
        // AlertBox.Instance.ShowConfirm(
        //     "@menu_end_of_level",
        //     () => ScreenFader.Instance.FadeOut(() => SceneManager.LoadScene("MainMenu")));
    }

    public void ShowEndMenuBad()
    {
        // AlertBox.Instance.ShowConfirm(
        //     "@menu_end_of_level_bad",
        //     () => ScreenFader.Instance.FadeOut(() => SceneManager.LoadScene("MainMenu")));
    }

    public void ShowMenu()
    {
        if (this.IsInteractionDisabled)
        {
            return;
        }

        // this.menu.Show();
        if (this.Setup.SetupType == GameSetupType.Editor)
        {
            this.gui.ShowEditorPauseScreen();
        }
        else
        {
            this.gui.ShowPauseScreen();
        }

        this.IsInteractionDisabled = true;
        this.tools.gameObject.SetActive(false);
    }

    public void ShowMessage(InformationBoxContent message, Action onEnd = null)
    {
        if (message == null || message.pages.Count == 0 || string.IsNullOrEmpty(message.pages[0].body))
        {
            if (onEnd != null)
            {
                onEnd();
            }

            return;
        }

        this.gui.ShowMessage(message, onEnd);
    }

    public void ShowOverlay(OverlayCityLayer overlay)
    {
        if (this.ignoreOverlayChanges)
        {
            return;
        }

        if (this.CurrentOverlay == overlay)
        {
            //return;
        }

        if (overlay != null)
        {
            this.city.GetLayer<UnitsLayer>("Units").ShowOverlay(true);

            if (overlay.CityView == CityView.Surface)
            {
                Game.Instance.ShowUnderground(false);
            }
            else if (overlay.CityView == CityView.Underground)
            {
                Game.Instance.ShowUnderground(true);
            }
        }
        else
        {
            Game.Instance.ShowUnderground(false);
            this.city.GetLayer<UnitsLayer>("Units").ShowOverlay(false);
        }

        this.currentOverlay = overlay;

        this.ignoreOverlayChanges = true;
        this.gui.ChangeOverlay(overlay);
        this.ignoreOverlayChanges = false;

        var overlayLayer = this.city.GetLayer<OverlayLayer>("Overlay");
        overlayLayer.Show(overlay);
    }

    public void ShowUnderground(bool value)
    {
        var newCityView = value ? CityView.Underground : CityView.Surface;

        if (this.cityView == newCityView)
        {
            return;
        }

        this.cityView = newCityView;
        this.city.ShowUnderground(value);
        this.gui.ChangeUnderground(value);
    }

    [UsedImplicitly]
    public void Start()
    {
        this.saveDataManagement = FindObjectOfType<SaveDataManagement>();

        this.scenarioManagement = FindObjectOfType<ScenarioManagement>();

        this.isPlaying = false;

        if (this.city == null)
        {
            return;
        }

        ManagerEditorVariables.Instance.LoadScenarioEditorValues(this.Setup.ScenarioEditorData);      
        this.city.Initialize(this.Setup);                                                                             
        this.gui.Initialize(this.Setup);    
        this.BuildUnitInfoTable();       
        this.IsInteractionDisabled = true;
        this.gui.SetTitle(ManagerEditorVariables.Instance.scenario_name);      
        gameObjectiveManager.SetObjectiveTargetValues();

       
           
       
        if (ManagerGlobalVariables.Instance.firststart == true)
        { 
            
             
           
        }      

        ScreenFader.Instance.FadeIn();

        if (ManagerGlobalVariables.Instance.currentscreen == 2)
        {
            if (this.City.GetLayer<TimeLayer>("Time").RemainingDays != 0)
            {
                this.StartPlaying();
                
            }
            else
            {
                this.StopPlaying();
              
            }
            //Check if the game has ended
            
        }
        else
        {
            this.StartPlaying();
           
        }
    }

    public void StartPlaying()
    {
       
           
        Debug.Log("Everything Is Ready, Starting Game");
        this.IsInteractionDisabled = false;
        this.gui.gameObject.SetActive(true);
        this.gui.UpdateOurDisplay();
        this.isPlaying = true;



        //New Code!
        EventManager.TickUpdate();
        

    }

    public void StopPlaying()
    {
        this.IsInteractionDisabled = true;
        this.timeManager.PauseTime();
        this.gui.gameObject.SetActive(false);
        this.infoButton.SetActive(false);
        this.isPlaying = false;
        /////  new code for objectives
        if (this.city.TargetAchieved()) { Debug.Log("Target Achieved"); }
        this.endGameScreen.OpenPanel();
        Debug.Log("Game Has Ended!");
    }


    public void TogglePlaying()
    {
        timeManager.NormalTime();

    }

    public void TogglePaused()
    {
        if (this.currentSpeed == 1)
        {
            if (this.previousSpeed == 1)
            {
                this.ChangeSpeed(2);
            }
            else
            {
                this.ChangeSpeed(this.previousSpeed);
            }
        }
        else
        {
            this.ChangeSpeed(1);
        }
    }

    public void SaveCurrentGameState(bool exitAfterwards)
    {
        timeManager.PauseTime();
        loadingPanel.Show();

        Debug.Log("wait panel shown");
        var cityData = this.city.Save();
        var scenarioEditorData = ManagerEditorVariables.Instance.SaveScenarioEditorValues();
        var saveData = SaveData.CreateFromEditor(null, null, null, string.Empty, cityData, scenarioEditorData);
        var saveDataJson = saveData.Serialize();

        string userID = ManagerGlobalVariables.Instance.user.id;
        string scenarioID = ManagerGlobalVariables.Instance.scenario_instance_id;
       
       

        if (exitAfterwards)
        {
            UnitsLayer unitsLayer = this.city.GetLayer<UnitsLayer>("Units");
            foreach (Unit unit in unitsLayer.units)
            {
                unit.UnSubscribeToTimeEvent();
            }

            TimeLayer timeLayer = this.city.GetLayer<TimeLayer>("Time");
            timeLayer.UnsubscribeFromEvents();
           
        }

        scenarioManagement.SaveInstanceData(saveDataJson, userID, scenarioID, exitAfterwards);
      
       
        /*

        WaitPanelScreen.Instance.Hide();
        ScreenFader.Instance.FadeOut(() => SceneManager.LoadScene("Main Menu UI"));
        ManagerGlobalVariables.Instance.currentscreen = 0;
        ManagerGlobalVariables.Instance.scenario = null;
        ManagerGlobalVariables.Instance.currentMap = null;

        EventManager.MainMenuStarted();
        ///  */





    }
     
    #endregion

    #region Methods

    protected override void Awake()
    {
        base.Awake();

        this.setup = GameSetup.Current;

        if (this.setup == null)
        {
            this.setup = GameSetup.ForEditorNew("New City", 128);
        }

        ToolMemento toolMemento = null;

        if (this.Setup.toolMemento != null)
        {
            toolMemento = this.Setup.toolMemento;
        }

        if (toolMemento != null)
        {
            try
            {
                foreach (var toolGroup in this.tools.GetComponentsInChildren<ToolGroup>(true))
                {
                    foreach (var tool in toolGroup.GetComponentsInChildren<Tool>(true))
                    {
                        if (toolMemento.IsDisabled(tool.id))
                        {
                            //tool.isToolEnabled = false;
                        }
                    }
                }
            }
            catch
            {

            }
        }
        // saving the original map data
       

    }

    private void BuildUnitInfoTable()
    {
        foreach (var unitTool in this.GetComponentsInChildren<AddUnitTool>())
        {
            foreach (var unit in unitTool.units)
            {
                this.unitInfo[unit.name] = unitTool.tooltip;
            }
        }
    }

    private void DoRestart()
    {
        ScreenFader.Instance.FadeOut(() => SceneManager.LoadScene("Game"));
      //  SceneManager.LoadScene("Main Menu Encyclopedia", LoadSceneMode.Additive);
    }

    private void InternalSaveEditorLevel(bool exitAfterwards)
    {
      //  loadingPanel.Show();
        this.IsInteractionDisabled = true;

        var cityData = this.city.Save();
        var scenarioEditorData = ManagerEditorVariables.Instance.SaveScenarioEditorValues();
        var saveData = SaveData.CreateFromEditor(null, null, null, string.Empty, cityData, scenarioEditorData);
        var saveDataJson = saveData.Serialize();
        Debug.Log(saveDataJson.Length);
        
        var is_public = ManagerEditorVariables.Instance.is_public;
        ManagerGlobalVariables.Instance.firststart = false;
        this.thumbnailCamera.TakeScreenshot(thumbnail =>
            saveDataManagement.SaveLevel(
                this.setup.CityId,
                this.city.Title,
                thumbnail,
                this.city.Description,
                saveDataJson,
                is_public,
                result =>
                {
                    if (result == "Success")
                    {
                        if (exitAfterwards)
                        {
                            UnitsLayer unitsLayer = this.city.GetLayer<UnitsLayer>("Units");
                            foreach (Unit unit in unitsLayer.units)
                            {
                                unit.UnSubscribeToTimeEvent();
                            }

                            TimeLayer timeLayer = this.city.GetLayer<TimeLayer>("Time");
                            timeLayer.UnsubscribeFromEvents();

                            
                            ScreenFader.Instance.FadeOut(() => SceneManager.LoadScene("Main Menu UI"));
                            ManagerGlobalVariables.Instance.currentscreen = 0;
                            ManagerGlobalVariables.Instance.scenario = null;
                            ManagerGlobalVariables.Instance.currentMap = null;
                            Debug.Log("saved and exiting!!!");
                            EventManager.MainMenuStarted();
                        }
                        else
                        {
                            this.IsInteractionDisabled = false;
                            ManagerGlobalVariables.Instance.scenarioEditing = true;
                            Debug.Log("not exiting!!");

                        }
                    }
                    else
                    {

                        Debug.Log("error in editng!!!");

                        gui.ChangeAlertText("@word_error_occured");

                    }
                   // loadingPanel.Hide();
                }));
    }

    private void SetObjectives(IEnumerable<ScenarioObjective> objectives)
    {
        //this.gui.SetObjectives(objectives);
    }

    #endregion
}