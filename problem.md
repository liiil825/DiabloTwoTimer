# 现在的问题
- [x] 暂停时时间不对
- [x] 第一次暂停不完成没有添加数据。
- [ ] 切换Tab标签，开始的时间和暂停的时间会清零
- [x] 场景的中英文保存问题
ProfileManager选择时是中文，并且带有ACT前缀
记录保存时，应该去掉ACT前缀并且只保存英文名称
但查询时，应该把对应的名称转化为英文
- [ ] 数据产生重复记录


.net WinForm项目中，有个计时器的界面，原来将所有逻辑都放在UI/Timer.cs中
现在我想将逻辑分离出来，放到一个单独的类中，Services/TimerService.cs，原来的UI/Timer.cs中只负责界面的展示，逻辑都放到TimerService.cs中.
问题：1. TimerService.cs中如何与Timer.cs进行通信？
2.TimerService.cs中怎么确保程序在运行时只有一个实例？
3. 在别的UI中如何使用TimerService.cs？
