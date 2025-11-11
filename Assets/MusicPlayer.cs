using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{

    public static MusicPlayer instance;

    public AudioClip introField;
    public AudioClip musicField;

    public AudioClip introBoss;
    public AudioClip musicBoss;

    public AudioClip GunSound;

    public AudioClip EndMusic;

    public AudioClip Pressedbutton;

    private AudioSource audiosource;

    public List<AudioClip> DeathCrys;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        PlayMapMusic();
    }

    public void PlayBossMusic()
    {
        StopAllCoroutines();
        StartCoroutine(PlayMusic(introBoss, musicBoss, 9f));
    }

    public void PlayEndMusic()
    {
        audiosource.clip = EndMusic;
        audiosource.Play();
    }

    public void PlayMapMusic()
    {
        StopAllCoroutines();
        StartCoroutine(PlayMusic(introField, musicField, 13.785f));
    }
    public IEnumerator PlayMusic(AudioClip intro, AudioClip loop, float waitTime)
    {
        Debug.Log("playing intro");
        audiosource.clip = intro;
        audiosource.Play();
        yield return new WaitForSeconds(waitTime);
        Debug.Log("playing music");
        audiosource.clip = loop;
        audiosource.Play();
    }

    public void PlayGunSound()
    {

        StartCoroutine(PlayGun());
    }
    public IEnumerator PlayGun()
    {
        GameObject newobject = new GameObject("GunSound");
        AudioSource newsource = newobject.AddComponent<AudioSource>();

        newsource.clip = GunSound;
        newsource.volume = 0.025f;
        newsource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        newsource.loop = false;
        newsource.spatialBlend = 0;

        newsource.Play();

        yield return new WaitForSeconds(GunSound.length);
        Destroy(newobject);
    }

    public void PlayDeathSound()
    {

        int ID = UnityEngine.Random.Range(0,DeathCrys.Count);
        while (ID >= DeathCrys.Count)
        {
            ID = UnityEngine.Random.Range(0, DeathCrys.Count);
        }
        StartCoroutine(PlayDeath(DeathCrys[ID]));
    }
    public IEnumerator PlayDeath(AudioClip deathclip)
    {
        GameObject newobject = new GameObject("DeathSound");
        AudioSource newsource = newobject.AddComponent<AudioSource>();

        newsource.clip = deathclip;
        newsource.volume = 0.5f;
        newsource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        newsource.loop = false;
        newsource.spatialBlend = 0;

        newsource.Play();

        yield return new WaitForSeconds(deathclip.length);
        Destroy(newobject);
    }

    public void PlayButtonSound()
    {

        StartCoroutine(PlayButton());
    }
    public IEnumerator PlayButton()
    {
        GameObject newobject = new GameObject("ButtonSound");
        AudioSource newsource = newobject.AddComponent<AudioSource>();

        newsource.clip = Pressedbutton;
        newsource.volume = 0.1f;
        newsource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        newsource.loop = false;
        newsource.spatialBlend = 0;

        newsource.Play();

        yield return new WaitForSeconds(Pressedbutton.length);
        Destroy(newobject);
    }
}
