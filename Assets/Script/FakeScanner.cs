using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FakeScanner : MonoBehaviour
{
    [Header("Komponen AR")]
    public ARTrackedImageManager imgManager;
    
    [Header("Model 3D")]
    public GameObject holoPrefab; 
    public GameObject realPrefab; 

    [Header("Komponen UI")]
    public Slider progressBar;
    public GameObject scanText;
    public GameObject enterVRButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scanSfx;
    public AudioClip completeSfx;

    private bool isScanning = false;
    private float scanProgress = 0f;
    private GameObject currentHolo;
    private GameObject currentRealObj;
    private bool hasFinished = false;

    void OnEnable() 
    {
        if (imgManager != null) imgManager.trackablesChanged.AddListener(OnTracked);
    }

    void OnDisable() 
    {
        if (imgManager != null) imgManager.trackablesChanged.RemoveListener(OnTracked);
    }

    void OnTracked(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added)
        {
            if (!hasFinished && holoPrefab != null)
            {
                currentHolo = Instantiate(holoPrefab, img.transform.position, img.transform.rotation);
                StartScan();
            }
        }

        foreach (var img in args.updated)
        {
            if (!hasFinished && currentHolo != null)
            {
                currentHolo.transform.position = img.transform.position;
                currentHolo.transform.rotation = img.transform.rotation;
                
                currentHolo.SetActive(img.trackingState == TrackingState.Tracking);
            }
        }
    }

    void StartScan()
    {
        if (hasFinished) return;

        isScanning = true;
        scanProgress = 0f;

        if (progressBar != null) progressBar.gameObject.SetActive(true);
        if (scanText != null) scanText.SetActive(true);
        if (enterVRButton != null) enterVRButton.SetActive(false);

        if (audioSource != null && scanSfx != null)
        {
            audioSource.clip = scanSfx;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (isScanning && !hasFinished)
        {
            scanProgress += Time.deltaTime * 25f; 
            if (progressBar != null) progressBar.value = scanProgress;

            if (scanProgress >= 100f) FinishScan();
        }
    }

    void FinishScan()
    {
        isScanning = false;
        hasFinished = true;

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (currentHolo != null)
        {
            spawnPos = currentHolo.transform.position;
            spawnRot = Quaternion.Euler(0, currentHolo.transform.rotation.eulerAngles.y, 0);
            
            Destroy(currentHolo);
        }

        if (realPrefab != null)
        {
            currentRealObj = Instantiate(realPrefab, spawnPos, spawnRot);
            currentRealObj.SetActive(true); 
        }

        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (scanText != null) scanText.SetActive(false);
        if (enterVRButton != null) enterVRButton.SetActive(true);

        if (audioSource != null)
        {
            audioSource.Stop();
            if (completeSfx != null) audioSource.PlayOneShot(completeSfx);
        }
    }

    public void GoToVR()
    {
        SceneManager.LoadScene("VR_Scene");
    }
}