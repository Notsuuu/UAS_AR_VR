using UnityEngine;
using UnityEngine.SceneManagement;

public class VRManager : MonoBehaviour
{
    public void ReturnToAR()
    {
        SceneManager.LoadScene("AR_Scene"); 
    }
}