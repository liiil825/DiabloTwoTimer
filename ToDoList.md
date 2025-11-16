# 现在的问题
- [x] 暂停时时间不对
- [x] 第一次暂停不完成没有添加数据。
- [x] 切换Tab标签，开始的时间和暂停的时间会清零
- [x] 场景的中英文保存问题
ProfileManager 选择时是中文，并且带有ACT前缀
记录保存时，应该去掉ACT前缀并且只保存英文名称
但查询时，应该把对应的名称转化为英文
- [x] 加载未完成的时间显示逻辑不对
- [x] 重构MainForm的UI和逻辑分离
帮我重构下.net代码，将MainForm的UI和逻辑分离，新建一个MainServices类，负责所有的逻辑。并且新提供一个方法SetActiveTabPage，用于切换Tab标签。并新建一个枚举TabPage，用于表示Tab标签的索引。MainServices最好使用单例模式，类似```
 public class TimerService : IDisposable
    {
        #region Singleton Implementation
        private static readonly Lazy<TimerService> _instance =
            new(() => new TimerService());

        public static TimerService Instance => _instance.Value;

        private TimerService()
        {
            _timer = new Timer(100); // 100毫秒间隔
            _timer.Elapsed += OnTimerElapsed;

            // 订阅ProfileService的事件
            var profileService = ProfileService.Instance;
            profileService.ResetTimerRequestedEvent += OnResetTimerRequested;
            profileService.RestoreIncompleteRecordRequestedEvent += OnRestoreIncompleteRecordRequested;
        }
        #endregion
    }```
下面是代码:

- [ ] 修复加载未完成记录时，时间显示问题
1. 加载上次的未完成记录，点击计时Tab正常，但重复切换会更改记录变为完成状态。
2. 点击继续Farm，会将记录直接完成。

- [ ] 删除历史记录
- [ ] 快捷键绑定界面没有做好
- [ ] 切换语言时，场景和难度信息没有更新
- [ ] 保存配置文件时, 场景信息还是保存中文
应该保存英文，根据配置文件中的language字段来显示不同语言

# 给DeepSeek的问题
## WinForm计时器逻辑与UI分离方案
.net WinForm项目中，有个计时器的界面，原来将所有逻辑都放在UI/Timer.cs中
现在我想将逻辑分离出来，放到一个单独的类中，Services/TimerService.cs，原来的UI/Timer.cs中只负责界面的展示，逻辑都放到TimerService.cs中.
问题：1. TimerService.cs中如何与Timer.cs进行通信？
2.TimerService.cs中怎么确保程序在运行时只有一个实例？
3. 在别的UI中如何使用TimerService.cs？
## 计时器
.net WinForm项目中，有个计时器的界面，有开始，暂停等功能，并且会保持记录到yaml文件中。TimerService.cs负责计时器的逻辑，Timer.cs负责界面的展示。MFRecord是记录的数据。问题：
1. 当我从yaml文件中加载未完成的记录时，显示未完成的时间是正确的，但再执行 Timer.cs 122-122 计时器无法正确的按0.1秒时间累计增加。
2. Timer.cs 122-122 执行时，会保存yaml文件，但durationSeconds计算错误.
下面是代码:

## 优化代码
帮我重构下.net代码，现状UI/ProfileManager有个方法SyncTimerControl方法，用于同步更新TimerControl的显示。
我希望

## 需求
### 自动生成房间名称到剪切板
### 增加掉落记录功能


