using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideoSyncOnNtpMono : MonoBehaviour
{
    public VideoPlayer m_videoPlayer;
    public long m_tickLenghtOfVideo = 0;
    public float m_frameRate;
    public long m_tickPerFrame;
    public float m_millisecondsPerFrame;
    public ulong m_frameCount;
    public long m_totaleVideoTicks;
    public float m_totaleVideoSeconds;
    public long m_videoPerDay;

    [Range(0f,1f)]
    public float m_percentState;

    private void OnValidate()
    {
        RefreshInfo();
    }

    private void RefreshInfo()
    {
        m_frameRate = m_videoPlayer.frameRate;
        m_frameCount = m_videoPlayer.frameCount;
        m_tickPerFrame = (long)(10000000 / m_frameRate);
        m_millisecondsPerFrame = 1000 / m_frameRate;
        m_tickLenghtOfVideo =(long)( m_videoPlayer.frameCount / m_videoPlayer.frameRate);
        m_totaleVideoTicks = (long) ((long)m_videoPlayer.frameCount * m_tickPerFrame);
        m_totaleVideoSeconds = m_totaleVideoTicks / 10000000f;
        m_videoPerDay = (3600 * 24 * TimeSpan.TicksPerSecond) / m_totaleVideoTicks;
        RefreshTickPerday();
    }

    public long m_tickSinceStartOfDay;
    public long m_videoTickModulo;

    public long m_previousTickPrepare;
    public long m_currentTickPrepare;
    public long m_previousTickPlay;
    public long m_currentTickPlay;

    public void Update()
    {
        m_tickSinceStartOfDay = GetTickSinceMidnight();
        m_videoTickModulo = m_tickSinceStartOfDay % m_totaleVideoTicks;
        float previousPercent = m_percentState;
        m_percentState = (float)m_videoTickModulo / (float)m_totaleVideoTicks;


        m_previousTickPrepare = m_currentTickPrepare;
        m_currentTickPrepare = m_tickSinceStartOfDay;

        m_previousTickPlay = m_currentTickPlay;
        m_currentTickPlay = m_tickSinceStartOfDay- m_prepareToLaunchTicks;


        if (m_tickOfDay.Where(t => t > m_previousTickPrepare && t <= m_currentTickPrepare).Any()) { 
            m_videoPlayer.Prepare();
            m_videoPlayer.frame = m_videoTickModulo;
            m_videoPlayer.Pause();
        }
        if (m_tickOfDay.Where(t => t > m_previousTickPlay && t <= m_currentTickPlay).Any())
        { 
            m_videoPlayer.Play();
        }


       for (int i = 0; i < m_tickOfDay.Count; i++)
        {
            if (i < m_tickOfDay.Count - 2) { 
                if (m_tickOfDay[i] > m_tickSinceStartOfDay)
                {
                    m_nextPrepareTick = m_tickOfDay[i];
                    m_previousPrepareTick = m_tickOfDay[i - 1];

                    break;
                }
            }
        }

        
    }

    public long m_nextPrepareTick;
    public long m_previousPrepareTick;

    public long m_prepareToLaunchTicks=TimeSpan.TicksPerSecond*2;
  

    public List<long> m_tickOfDay;
    public long m_ntpOffset;

    public void SetOffset(long offset)
    {
        m_ntpOffset = offset;
    }

    private void Awake()
    {
        RefreshTickPerday();
    }
    private void RefreshTickPerday()
    {
        m_tickOfDay.Clear();
        DateTime now = GetDateTimeUtcNowNtp();
        DateTime startOfDay = GetDateTimeUtcNowNtpStartOfDay();
        long tickStartOfDay = startOfDay.Ticks;
        for(int i = 0; i < m_videoPerDay; i++)
        {
            m_tickOfDay.Add( i * m_totaleVideoTicks);
        }


    }

    private DateTime GetDateTimeUtcNowNtp()
    {
        DateTime now = DateTime.UtcNow;
        long tick = now.Ticks + m_ntpOffset;
        DateTime dateTime = new DateTime(tick, DateTimeKind.Utc);
        return dateTime;
    }
    private DateTime GetDateTimeUtcNowNtpStartOfDay()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        long tick = start.Ticks + m_ntpOffset;
        DateTime dateTime = new DateTime(tick, DateTimeKind.Utc);
        return dateTime;
    }

    private void RefreshFrame()
    {
        m_videoPlayer.frame = m_videoTickModulo;
    }

    private long GetTickSinceMidnight()
    {
        DateTime now = DateTime.UtcNow;
        DateTime startOfDay = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan timeSinceStartOfDay = now - startOfDay;
        return timeSinceStartOfDay.Ticks;
    }
}
