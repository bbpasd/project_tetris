using System.Collections.Generic;
using UnityEngine;

/// <summary> 사운드 관리 </summary>
public class SoundManager : MonoBehaviour
{
    public enum BGMEnum
    {
        Tetris,
        AniPang
    }
    
    private AudioSource bgmSource; // BGM을 위한 AudioSource
    private AudioSource sfxSource; // 효과음을 위한 AudioSource

    private AudioClip[] bgmClip;        // 메인 BGM 클립
    private AudioClip[] sfxClips;      // 효과음 클립 배열

    private void Awake() {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true; // BGM은 반복 재생되도록 설정
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmClip = new AudioClip[2];
        bgmClip[0] = Managers.Resource.Load<AudioClip>("Sound/Korobeiniki_Rearranged");
        bgmClip[1] = Managers.Resource.Load<AudioClip>("Sound/Korobeiniki_Rearranged");
    }

    // 현재 미사용
    public void PlaySFX(int clipIndex) {
        if (clipIndex < 0 || clipIndex >= sfxClips.Length) {
            return;
        }

        if (sfxSource != null) {
            sfxSource.PlayOneShot(sfxClips[clipIndex]);
        }
    }

    public void PlayBGM(BGMEnum bgmIdx) {
        if (bgmClip[(int)bgmIdx] != null) {
            bgmSource.clip = bgmClip[(int)bgmIdx];
            bgmSource.Play();
        }
    }
    
    public void StopBGM() {
        if (bgmSource.isPlaying) {
            bgmSource.Stop();
        }
    }
}