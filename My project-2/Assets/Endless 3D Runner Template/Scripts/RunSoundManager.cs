using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSoundManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource audioSource;
    public AudioClip jumpSideClip;
    public AudioClip jumpClip;
    public AudioClip gameOverClip;
    public AudioClip crouchClip;
    public AudioClip bonusClip;
    
    void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume");
           // musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");            
        }
        if (musicAudioSource != null)
        {
            musicAudioSource.ignoreListenerVolume = true;
            musicAudioSource.Play();
        }

        RunEventServices.GameMechanicAction.StartGame += StartGame;
        RunEventServices.GameMechanicAction.LeftMove += LeftMove;
        RunEventServices.GameMechanicAction.RightMove += RightMove;
        RunEventServices.GameMechanicAction.Jump += Jump;
        RunEventServices.GameMechanicAction.Crouch += Crouch;
        RunEventServices.GameMechanicAction.GameOver += GameOver;
        RunEventServices.GameMechanicAction.BonusSound += BonusSound;
    }

    private void OnDestroy()
    {
        RunEventServices.GameMechanicAction.StartGame -= StartGame;
        RunEventServices.GameMechanicAction.LeftMove -= LeftMove;
        RunEventServices.GameMechanicAction.RightMove -= RightMove;
        RunEventServices.GameMechanicAction.Jump -= Jump;
        RunEventServices.GameMechanicAction.Crouch -= Crouch;
        RunEventServices.GameMechanicAction.GameOver -= GameOver;
        RunEventServices.GameMechanicAction.BonusSound -= BonusSound;
    }

    public void StartGame()
    {
        
    }
    public void LeftMove()
    {
        audioSource.PlayOneShot(jumpSideClip);
    }
    public void RightMove()
    {
        audioSource.PlayOneShot(jumpSideClip);
    }
    public void Jump()
    {
        audioSource.PlayOneShot(jumpClip);
    }
    public void Crouch()
    {
        audioSource.PlayOneShot(crouchClip);
    }

    public void GameOver()
    {
        audioSource.PlayOneShot(gameOverClip);
        
    }
    public void BonusSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        
    }
}
