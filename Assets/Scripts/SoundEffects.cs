using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource src;
    public AudioClip sfx1;

    public void Button1()
    {
        src.clip = sfx1;
        src.Play();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
