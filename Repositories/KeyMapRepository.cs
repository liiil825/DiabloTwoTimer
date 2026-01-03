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
                Text = "开始/下一场",
                Action = "Timer.Next",
            },
            new KeyMapNode
            {
                Key = "w",
                Text = "切换暂停",
                Action = "Timer.Toggle",
            },
            new KeyMapNode
            {
                Key = "e",
                Text = "番茄钟",
                Children =
                [
                    new()
                    {
                        Key = "q",
                        Text = "切换开始/暂停",
                        Action = "Pomodoro.Toggle",
                    },
                    new()
                    {
                        Key = "p",
                        Text = "暂停",
                        Action = "Pomodoro.Pause",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "展示休息页",
                        Action = "Pomodoro.ShowBreakForm",
                    },
                    new()
                    {
                        Key = "s",
                        Text = "设置模式",
                        Children =
                        [
                            new()
                            {
                                Key = "q",
                                Text = "自动",
                                Action = "Pomodoro.SetMode.Automatic",
                            },
                            new()
                            {
                                Key = "w",
                                Text = "半自动",
                                Action = "Pomodoro.SetMode.SemiAuto",
                            },
                            new()
                            {
                                Key = "e",
                                Text = "手动",
                                Action = "Pomodoro.SetMode.Manual",
                            },
                        ],
                    },
                    new()
                    {
                        Key = "a",
                        Text = "增加时间",
                        Action = "Pomodoro.AddMinutes",
                        RequiresInput = true,
                        InputHint = "输入 1 - 59 (分钟)之间的数值",
                    },
                    new()
                    {
                        Key = "t",
                        Text = "设置",
                        Action = "Pomodoro.ShowSettings",
                    },
                    new()
                    {
                        Key = "d",
                        Text = "切换状态",
                        Action = "Pomodoro.SwitchToNextState",
                    },
                    new()
                    {
                        Key = "r",
                        Text = "重置",
                        Action = "Pomodoro.Reset",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "t",
                Text = "计时器",
                Children =
                [
                    new()
                    {
                        Key = "s",
                        Text = "开始/下一场",
                        Action = "Timer.Start",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "切换显示掉落",
                        Action = "Loot.ToggleVisibility",
                    },
                    new()
                    {
                        Key = "p",
                        Text = "切换暂停",
                        Action = "Timer.Pause",
                    },
                    new()
                    {
                        Key = "r",
                        Text = "重置",
                        Action = "Timer.Reset",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "r",
                Text = "记录",
                Children =
                [
                    new()
                    {
                        Key = "a",
                        Text = "添加掉落",
                        Action = "Loot.Add",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "切换显示掉落",
                        Action = "Loot.ToggleVisibility",
                    },
                    new()
                    {
                        Key = "f",
                        Text = "删除最后时间记录",
                        Action = "Record.DeleteLastHistory",
                    },
                    new()
                    {
                        Key = "g",
                        Text = "删除最后掉落",
                        Action = "Record.DeleteLastLoot",
                    },
                    new()
                    {
                        Key = "d",
                        Text = "删除选中记录",
                        Action = "Record.DeleteSelected",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "s",
                Text = "展示",
                Children =
                [
                    new()
                    {
                        Key = "q",
                        Text = "掉落",
                        Action = "Loot.ShowHistory",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "统计",
                        Action = "Pomodoro.ShowBreakForm",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "c",
                Text = "角色",
                Children =
                [
                    new()
                    {
                        Key = "q",
                        Text = "创建",
                        Action = "Character.Create",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "切换",
                        Action = "Character.Switch",
                    },
                    new()
                    {
                        Key = "e",
                        Text = "导出",
                        Action = "Character.Export",
                    },
                    new()
                    {
                        Key = "s",
                        Text = "设置场景",
                        Action = "Scene.Switch",
                        RequiresInput = true,
                        InputHint = "输入场景的英文短名称 (如: Coun, Pit, Gamble)",
                    },
                    new()
                    {
                        Key = "d",
                        Text = "删除",
                        Action = "Character.Delete",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "g", // Go / Goto
                Text = "导航",
                Children =
                [
                    new()
                    {
                        Key = "q",
                        Text = "角色页",
                        Action = "Nav.Profile",
                    },
                    new()
                    {
                        Key = "w",
                        Text = "计时页",
                        Action = "Nav.Timer",
                    },
                    new()
                    {
                        Key = "e",
                        Text = "番茄钟",
                        Action = "Nav.Pomodoro",
                    },
                    new()
                    {
                        Key = "r",
                        Text = "设置",
                        Action = "App.ShowSettings",
                    },
                ],
            },
            new KeyMapNode
            {
                Key = "x",
                Text = "窗口操作",
                Children =
                [
                    new KeyMapNode
                    {
                        Key = "q",
                        Text = "最小化到托盘",
                        Action = "App.Minimize",
                    },
                    // 恢复窗口
                    new KeyMapNode
                    {
                        Key = "w",
                        Text = "恢复窗口",
                        Action = "App.Restore",
                    },
                    new KeyMapNode
                    {
                        Key = "e",
                        Text = "设置透明度",
                        Action = "App.SetOpacity",
                        RequiresInput = true,
                        InputHint = "输入 0.1 - 1.0 之间的数值",
                    },
                    new KeyMapNode
                    {
                        Key = "r",
                        Text = "设置位置",
                        Children =
                        [
                            new()
                            {
                                Key = "q",
                                Text = "左上",
                                Action = "App.SetPosition.TopLeft",
                                RequiresInput = false,
                            },
                            new()
                            {
                                Key = "w",
                                Text = "右上",
                                Action = "App.SetPosition.TopRight",
                                RequiresInput = false,
                            },
                            new()
                            {
                                Key = "e",
                                Text = "左下",
                                Action = "App.SetPosition.BottomLeft",
                                RequiresInput = false,
                            },
                            new()
                            {
                                Key = "r",
                                Text = "右下",
                                Action = "App.SetPosition.BottomRight",
                                RequiresInput = false,
                            },
                        ],
                    },
                    new KeyMapNode
                    {
                        Key = "t",
                        Text = "设置大小",
                        Action = "App.SetSize",
                        RequiresInput = true,
                        InputHint = "输入 1.0 - 2.5 之间的数值,注意修改后需要重新启动程序",
                    },
                    new KeyMapNode
                    {
                        Key = "s",
                        Text = "切换显示导航",
                        Action = "App.ToggleNavigation",
                    },
                    new()
                    {
                        Key = "x",
                        Text = "退出程序",
                        Action = "App.Exit",
                    },
                ],
            },
        ];
    }
}
