using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerPrepareMono : MonoBehaviour
{
    public VideoPlayer m_videoPlayer;


    [ContextMenu("Prepare")]
    public void PrepareVideoPlayer()
    {
        m_videoPlayer.Stop();
        m_videoPlayer.Prepare();
        m_videoPlayer.Pause();

    }

    [ContextMenu("Start")]
    public void PlayPreparedVideo()
    {
        m_videoPlayer.Play();
    }
}
