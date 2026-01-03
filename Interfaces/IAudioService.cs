using System;
using System.Collections.Generic;

namespace DiabloTwoMFTimer.Interfaces;

public interface IAudioService : IDisposable
{
    /// <summary>
    /// 初始化音频资源：检查 AppData 目录，若缺失则从 Resources 复制
    /// </summary>
    void Initialize();

    /// <summary>
    /// 播放指定的音频文件 (用于业务逻辑)
    /// </summary>
    /// <param name="fileName">文件名 (相对于 audio 目录)</param>
    void PlaySound(string fileName);

    /// <summary>
    /// 预览/试听音频 (支持播放/暂停切换)
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="onPlaybackStopped">播放结束或停止时的回调</param>
    void PreviewSound(string fileName, Action onPlaybackStopped);

    /// <summary>
    /// 停止当前的试听
    /// </summary>
    void StopPreview();

    /// <summary>
    /// 获取所有可用的音频文件列表
    /// </summary>
    List<string> GetAvailableSounds();

    /// <summary>
    /// 判断当前是否正在试听
    /// </summary>
    bool IsPreviewing { get; }
}