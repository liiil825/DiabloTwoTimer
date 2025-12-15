using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiabloTwoMFTimer.Repositories;

public class KeyMapRepository : IKeyMapRepository
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public KeyMapRepository()
    {
        // 配置 YamlDotNet 使用驼峰命名 (camelCase)，符合 YAML 惯例
        _serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public List<KeyMapNode> LoadKeyMap()
    {
        try
        {
            // 1. 如果文件不存在，创建默认配置
            if (!File.Exists(FolderManager.KeyMapConfigPath))
            {
                var defaults = GenerateDefaultKeyMap();
                SaveKeyMap(defaults);
                return defaults;
            }

            // 2. 读取文件
            using var reader = new StreamReader(FolderManager.KeyMapConfigPath, Encoding.UTF8);
            var nodes = _deserializer.Deserialize<List<KeyMapNode>>(reader);

            return nodes ?? new List<KeyMapNode>();
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("KeyMapRepository", "加载按键映射失败，将使用空配置", ex);
            // 这里为了不让程序崩溃，返回空列表，或者也可以返回默认配置
            return GenerateDefaultKeyMap();
        }
    }

    public void SaveKeyMap(List<KeyMapNode> nodes)
    {
        try
        {
            FolderManager.EnsureDirectoryExists(FolderManager.AppDataPath);
            using var writer = new StreamWriter(FolderManager.KeyMapConfigPath, false, Encoding.UTF8);
            _serializer.Serialize(writer, nodes);
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("KeyMapRepository", "保存按键映射失败", ex);
        }
    }

    /// <summary>
    /// 生成默认的硬核按键映射树
    /// </summary>
    private List<KeyMapNode> GenerateDefaultKeyMap()
    {
        return
        [
            new KeyMapNode
            {
                Key = "q",
                Text = "开始 (Start)",
                Action = "Timer.Next",
            },
            new KeyMapNode
            {
                Key = "w",
                Text = "切换暂停 (Toggle Pause)",
                Action = "Timer.Toggle",
            },
            new KeyMapNode
            {
                Key = "t",
                Text = "计时器 (Timer)",
                Children =
                [
                    new()
                    {
                        Key = "s",
                        Text = "启动 (Start)",
                        Action = "Timer.Start",
                    },
                    new()
                    {
                        Key = "p",
                        Text = "暂停 (Pause)",
                        Action = "Timer.Pause",
                    },
                    new()
                    {
                        Key = "r",
                        Text = "重置 (Reset)",
                        Action = "Timer.Reset",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "e",
                Text = "番茄钟 (Pomodoro)",
                Children =
                [
                    new()
                    {
                        Key = "q",
                        Text = "切换 (Toggle)",
                        Action = "Pomodoro.Toggle",
                    },
                    new()
                    {
                        Key = "p",
                        Text = "暂停 (Pause)",
                        Action = "Pomodoro.Pause",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "休息 (Break)",
                        Action = "Pomodoro.ShowBreakForm",
                    },
                    new()
                    {
                        Key = "e",
                        Text = "增加 (Add)",
                        Action = "Pomodoro.AddMinutes",
                        RequiresInput = true,
                        InputHint = "输入 1 - 59 (分钟)之间的数值",
                    },
                    new()
                    {
                        Key = "t",
                        Text = "设置 (Settings)",
                        Action = "Pomodoro.ShowSettings",
                    },
                    new()
                    {
                        Key = "d",
                        Text = "切换状态 (Switch State)",
                        Action = "Pomodoro.SwitchToNextState",
                    },
                    new()
                    {
                        Key = "r",
                        Text = "重置 (Reset)",
                        Action = "Pomodoro.Reset",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "r",
                Text = "记录 (Record)",
                Children =
                [
                    new()
                    {
                        Key = "q",
                        Text = "添加掉落 (Loot)",
                        Action = "Loot.Add",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "切换显示 (Toggle Loot Visibility)",
                        Action = "Loot.ToggleVisibility",
                    },
                    new()
                    {
                        Key = "d",
                        Text = "删除上一次 (Delete Last)",
                        Action = "Record.DeleteLast",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "g", // Go / Goto
                Text = "导航 (Go)",
                Children =
                [
                    new()
                    {
                        Key = "q",
                        Text = "角色页 (Char)",
                        Action = "Nav.Profile",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "计时页 (Timer)",
                        Action = "Nav.Timer",
                    },
                    new()
                    {
                        Key = "e",
                        Text = "番茄钟 (Pomodoro)",
                        Action = "Nav.Pomodoro",
                    },
                    new()
                    {
                        Key = "r",
                        Text = "设置 (Settings)",
                        Action = "Nav.Settings",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "x",
                Text = "窗口操作 (Window)",
                Children =
                [
                    new KeyMapNode
                    {
                        Key = "q",
                        Text = "最小化到托盘 (Minimize)",
                        Action = "App.Minimize",
                    },
                    // 恢复窗口
                    new KeyMapNode
                    {
                        Key = "w",
                        Text = "恢复窗口 (Restore)",
                        Action = "App.Restore",
                    },
                    new KeyMapNode
                    {
                        Key = "e",
                        Text = "设置透明度 (SetOpacity)",
                        Action = "App.SetOpacity",
                        RequiresInput = true,
                        InputHint = "输入 0.1 - 1.0 之间的数值",
                    },
                    new KeyMapNode
                    {
                        Key = "r",
                        Text = "设置位置 (SetPosition)",
                        Action = "App.SetPosition",
                        RequiresInput = true,
                        InputHint = "0 左上 1 右上 2 左下 3 右下",
                    },
                    new KeyMapNode
                    {
                        Key = "t",
                        Text = "设置大小 (SetSize)",
                        Action = "App.SetSize",
                        RequiresInput = true,
                        InputHint = "输入 1.0 - 2.5 之间的数值,注意修改后需要重新启动程序",
                    },
                    new()
                    {
                        Key = "x",
                        Text = "退出程序 (Quit)",
                        Action = "App.Exit",
                    },
                ],
            },
        ];
    }
}
