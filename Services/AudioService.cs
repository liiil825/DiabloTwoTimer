using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Utils;
using NAudio.Wave;

namespace DiabloTwoMFTimer.Services;

public class AudioService : IAudioService
{
    private readonly IAppSettings _appSettings;

    // NAudio 核心对象
    private IWavePlayer? _outputDevice;
    private AudioFileReader? _audioFile;
    private bool _isPreviewing = false;
    private Action? _currentPreviewCallback;

    // 记录当前正在试听的文件名，用于UI状态判断
    private string? _currentPreviewFileName;

    public bool IsPreviewing => _isPreviewing;

    public AudioService(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public void Initialize()
    {
        try
        {
            // 1. 确保音频目录存在 (FolderManager已处理，这里再次确认或作为业务逻辑入口)
            FolderManager.EnsureDirectoryExists(FolderManager.AudioPath);

            // 2. 检查并从资源目录复制默认文件
            if (Directory.Exists(FolderManager.ResourceAudioPath))
            {
                var files = Directory.GetFiles(FolderManager.ResourceAudioPath);
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = FolderManager.GetAudioFilePath(fileName);

                    if (!File.Exists(destFile))
                    {
                        File.Copy(file, destFile);
                        LogManager.WriteDebugLog("AudioService", $"初始化音频文件: {fileName}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("AudioService", "初始化音频资源失败", ex);
        }
    }

    public void PlaySound(string fileName)
    {
        if (!_appSettings.AudioEnabled || string.IsNullOrEmpty(fileName)) return;

        Task.Run(() =>
        {
            try
            {
                string fullPath = FolderManager.GetAudioFilePath(fileName);
                if (!File.Exists(fullPath)) return;

                using var audioFile = new AudioFileReader(fullPath);
                using var outputDevice = new WaveOutEvent();

                audioFile.Volume = _appSettings.AudioVolume / 100f;
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("AudioService", $"播放失败: {fileName}", ex);
            }
        });
    }

    public void PreviewSound(string fileName, Action onPlaybackStopped)
    {
        try
        {
            // 如果点击的是同一个正在播放的文件，则是“暂停/停止”操作
            if (_isPreviewing && _currentPreviewFileName == fileName)
            {
                StopPreview();
                return;
            }

            // 如果正在播放其他文件，先停止
            StopPreview();

            string fullPath = FolderManager.GetAudioFilePath(fileName);

            if (!File.Exists(fullPath))
            {
                string errorMsg = LanguageManager.GetString("Audio.FileNotFound");
                Utils.Toast.Error(errorMsg);
                onPlaybackStopped?.Invoke();
                return;
            }

            _audioFile = new AudioFileReader(fullPath);
            _audioFile.Volume = _appSettings.AudioVolume / 100f;

            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_audioFile);

            _currentPreviewCallback = onPlaybackStopped;
            _currentPreviewFileName = fileName;
            _outputDevice.PlaybackStopped += OnPreviewStopped;

            _outputDevice.Play();
            _isPreviewing = true;
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("AudioService", "试听失败", ex);
            StopPreview();
        }
    }

    public void StopPreview()
    {
        if (_outputDevice != null)
        {
            _outputDevice.Stop();
        }
    }

    private void OnPreviewStopped(object? sender, StoppedEventArgs e)
    {
        _isPreviewing = false;
        _currentPreviewFileName = null;

        _outputDevice?.Dispose();
        _outputDevice = null;
        _audioFile?.Dispose();
        _audioFile = null;

        _currentPreviewCallback?.Invoke();
        _currentPreviewCallback = null;
    }

    public List<string> GetAvailableSounds()
    {
        if (!Directory.Exists(FolderManager.AudioPath)) return new List<string>();

        return Directory.GetFiles(FolderManager.AudioPath)
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrEmpty(x) && (x.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)))
            .Select(x => x!)
            .ToList();
    }

    public void Dispose()
    {
        StopPreview();
    }
}