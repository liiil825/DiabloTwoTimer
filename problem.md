# 现在的问题
- [x] 暂停时时间不对
- [x] 第一次暂停不完成没有添加数据。
- [x] 切换Tab标签，开始的时间和暂停的时间会清零
- [x] 场景的中英文保存问题
ProfileManager 选择时是中文，并且带有ACT前缀
记录保存时，应该去掉ACT前缀并且只保存英文名称
但查询时，应该把对应的名称转化为英文
- [ ] 加载未完成的时间显示逻辑不对
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

## 需求
### 增加掉落记录功能