using UnityEngine;
using System.Collections;

public class HornSound : MonoBehaviour
{

	[SerializeField]
	AudioClip hornSound = null;
	AudioSource hornSource = null;

	[SerializeField]
	[Range(0f, 1f)]
	float playProbability = 1f;

	[SerializeField]
	[Range(0f, 200f)]
	float playDistance = 200f;

	bool played;

	// Use this for initialization
	void Start()
	{
		hornSource = AudioHelper.CreateAudioSource(gameObject, hornSound);
		played = false;
	}

    public void playSound()
    {
        hornSource.Play();
    }
	
}
