using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class Manager : MonoBehaviour {

    [Header("Scene Settings")]

    public List<GameObject> levels = new List<GameObject>();
    public GameObject menuLevel;

    public float timeFromStartToPlay = 1.54f;

    [Header("Pursuer Settings")]

    public string pursuerGameObjName;

    public float pursuitTime;
    [HideInInspector]
    public float pursuitTimeLoc;

    [Header("Skins Settings")]
    public Transform characterSkinsPosition;
    public List<GameObject> characterSkins = new List<GameObject>();

    [Header("Variables")]

    public GameObject GamePanels;
    public GameObject MenuPanels;

    public GameObject GamePanel;
    public GameObject StopPanel;
    public GameObject GameOverPanel;
    public GameObject SettingsPanel;
    public GameObject CharacterSkinsPanel;
    public GameObject SettingsBtn;

    public Text timeScore;
    public Text coinsScore;
    public Text maxTimeScore;
    public Text CoinsTxt;
    public Text GameOverCoinsTxt;

    public Text gameOverScoreTxt;
    public Text congratulationsTxt;

   

    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
   
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public GameObject pursuer;
    [HideInInspector]
    public bool play;

    protected Vector3 cameraPos;
    protected Quaternion cameraRot;

    [HideInInspector]
    public Transform cameraTransform;

    [HideInInspector]
    public Transform pursuerTransform;

    [HideInInspector]
    public bool cameraLerp;

    [HideInInspector]
    public float startPlayerSpeed;


    /// <summary>
	/// Deactivate all active levels on scene
	/// </summary>
     private void Awake()
    {
        Level[] levelsLoc = GameObject.FindObjectsOfType<Level>();

        for (int i = 0; i < levelsLoc.Length; i++)
        {
            levelsLoc[i].transform.parent.gameObject.SetActive(false);
        }

    }

    private void Start ()
    {
       
        cameraTransform = Camera.main.transform;

        cameraRot = cameraTransform.rotation;
        cameraPos = cameraTransform.position;

        

        CreateMenuScene();
        
        GamePanels.SetActive(false);
        MenuPanels.SetActive(true);
        SettingsPanel.SetActive(false);
        CharacterSkinsPanel.SetActive(false);

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");            
        }
        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("SoundVolume");
            soundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }

        //Load Records
        CoinsTxt.text = PlayerPrefs.GetInt("Coins").ToString();
        GameOverCoinsTxt.text = CoinsTxt.text;
        maxTimeScore.text = PlayerPrefs.GetFloat("MaxTimeScore").ToString("F0");
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();

        ClearScene();
        for (int i = 0; i < characterSkins.Count; i++)
        {
            Destroy(characterSkinsPosition.GetChild(i).gameObject);
        }

        Start();
    }

    public void SetAudioVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public void Menu()
    {
        play = false;
        cameraLerp = false;

        player.SaveRecords();
               
        if (StopPanel.activeSelf)
            Stop();

        GamePanels.SetActive(false);
        MenuPanels.SetActive(true);
        SettingsBtn.SetActive(true);

        ClearScene();
        
        CreateMenuScene();

        if(IsInvoking("StopCameraLerp"))
            CancelInvoke("StopCameraLerp");
    }

    public void SetLayer(Transform trans, int layer) 
    {
        trans.gameObject.layer = layer;
        foreach(Transform child in trans)
            SetLayer(child, layer);
    }
 

   

  
    public void Play()
    {
        if (SettingsPanel.activeSelf || CharacterSkinsPanel.activeSelf)
            return;

        pursuitTimeLoc = pursuitTime;
        play = true;
      
        GamePanels.SetActive(true);
        MenuPanels.SetActive(false);

        GamePanel.SetActive(true);
        GameOverPanel.SetActive(false);
        SettingsBtn.SetActive(false);

        Invoke("StopCameraLerp", timeFromStartToPlay);
        cameraLerp = true;
    }

    private void FixedUpdate()
    {
        if(cameraLerp)
        {
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(player.cameraRotation), 0.15f);           
        }
    }

    private void StopCameraLerp()
    {
        cameraLerp = false;
        cameraTransform.rotation = Quaternion.Euler(player.cameraRotation);
    }

    public virtual void Stop()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;

        GamePanel.SetActive(!GamePanel.activeSelf);
        StopPanel.SetActive(!StopPanel.activeSelf);
        SettingsBtn.SetActive(StopPanel.activeSelf);
    }

    public  void ClearScene()
    {
        timeScore.text = "0";
        coinsScore.text = "0";

        Level[] levelsLoc = GameObject.FindObjectsOfType<Level>();

        for (int i = 0; i < levelsLoc.Length; i++)
        {
            Destroy(levelsLoc[i].transform.parent.gameObject);
        }

        Destroy(player.gameObject);
        Destroy(pursuer.gameObject);

    }


    public  void CreateMenuScene()
    {
        GameObject menuLvl = Instantiate(menuLevel);
        menuLvl.SetActive(true);

        player = Instantiate(characterSkins[PlayerPrefs.GetInt("CharacterSkin")], menuLvl.transform.Find("StartPosition").position, Quaternion.identity).GetComponent<Player>();
        startPlayerSpeed = player.speed;
        pursuer = menuLvl.transform.Find(pursuerGameObjName).gameObject;
        pursuer.transform.SetParent(transform.parent);
        pursuerTransform = pursuer.transform;

        cameraTransform.position = cameraPos;
        cameraTransform.rotation = cameraRot;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Demo");
        //if (SettingsPanel.activeSelf)
        //    return;

        //ClearScene();

        //CreateMenuScene();

        //Play();
    }

}
