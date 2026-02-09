# FGUFW.mono

unity框架mono分支 回归unity引擎原始的味道  
不把C#脚本和Unity脚本分离  
使用unity提供生命周期接口  
利用editor强大的运行时调试功能

***
![FGUFW.km](./readmeImg~/FGUFW.png)

## 目录
  - [文件路径规划](#文件路径规划)
  - [自制工具](#自制工具)
  - [Gameplay](#gameplay)
  - [UI](#ui)
  - [触发器](#触发器)
  - [战斗系统](#战斗系统)
    - [控制](#控制)
    - [技能](#技能)
    - [游戏对象属性](#游戏对象属性)
  - [日志系统](#日志系统)
  - [资产自检系统](#资产自检系统)
  - [~~网络~~](#网络)
  - [~~热更新~~](#热更新)
  - [对象池](#对象池)
  - [管线](#管线)
  - [JobSystem](#jobsystem)
  - [MonoBehaviour生命周期](#monobehaviour生命周期)
    - [嵌套结构](#嵌套结构)
  - [VisualScripting](#visualscripting)
  - [异步等待](#异步等待)
  - [手柄支持](#手柄支持)
  - [编码风格](#编码风格)
  
***

## 文件路径规划

1. 工程上层路径不要出现非英文数字 工程内可以给美术策划使用的文件夹使用中文 能减少沟通成本
2. 减少嵌套层数 大多数人都很难记住深层嵌套的路径 即使这样便于分割模块 但考虑到其他人学习成本的需要妥协.
3. 一般外部插件都是直接放在Assets目录下 我们开发用的资产最少要包一层

***

## 自制工具

  自制工具的结构 仿Unity插件规范 (当然只是参考)
```
<package-root>
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
  ├── LICENSE.md
  ├── Third Party Notices.md
  ├── Editor
  │   ├── <company-name>.<package-name>.Editor.asmdef
  │   └── EditorExample.cs
  ├── Runtime
  │   ├── <company-name>.<package-name>.asmdef
  │   └── RuntimeExample.cs
  ├── Tests
  │   ├── Editor
  │   │   ├── <company-name>.<package-name>.Editor.Tests.asmdef
  │   │   └── EditorExampleTest.cs
  │   └── Runtime
  │        ├── <company-name>.<package-name>.Tests.asmdef
  │        └── RuntimeExampleTest.cs
  ├── Samples~
  │        ├── SampleFolder1
  │        ├── SampleFolder2
  │        └── ...
  └── Documentation~
       └── <package-name>.md
```

***

## Gameplay

  1. 控制游戏业务逻辑的部分 配合[Visual Scripting][VisualScripting]更加灵活  
  2. Play之间彼此独立 唯有GlobalPlay共存 提供Loading等功能
  3. 自动模式 限制玩家的操作 用于新手引导和过场剧情

  - Play : 负责大的功能模块 例:游戏大厅/战斗场景
  - Part : Play中具体的功能模块 例:设置/排行榜/商店/刷怪/成就 

***

## UI

  - UI : 用户接口 目前泛指图形交互接口
  
  1. 输入和输出拆分出来 ~~虽然会增加工作量~~
  2. UI只是功能模块的接口 需要绑定功能模块的生命周期

***

## 触发器

  - 触发条件
  - 触发消耗
  - 效果

  1. 可以将CD作为消耗之一
  2. 占用层 并行概念 0或不相等为并行 可作为触发条件
  3. 配合[Visual Scripting][VisualScripting]可视化编辑

***

## 战斗系统

  ### 控制
   - 物理法则最优先
   - 默认AI控制
   - 玩家注入控制 类似操控玩偶

  ### 技能
   - 使用触发器逻辑
   - 一段单向链式效果 可终止
   - 可计算时长 必要时加速执行
   - 常规参数
     - 起点
     - 目标点/方向
     - 目标Id
     - 触发范围内目标
     - 生命时间
     - 生命次数
  
  ### 游戏对象属性
   - 生命周期状态
   - 战斗状态
   - 所处空间
   - 阵营
   - Life

***

## 日志系统

  1. 通过不同宏控制日志输出
  2. 保存到本地文件
  3. 输出调用栈
  4. 及时清理日志文件
  5. 方便搜索和过滤的格式
  6. ~~阅读工具~~
  7. 每次运行存一份日志文件 并在结尾加上标识以验证完整度
  8. Assert简化自检代码

***

## 资产自检系统

  - 配置表中的配置大都容易缺失 且只在用的时候发现
  - 使用编辑器工具或者启动时校验资源缺失

***

## ~~网络~~
  无

***

## ~~热更新~~
  无

***

## 对象池

  unity已经有了对象池类 要善加利用

***

## 管线

  为达成目标将整个作业按顺序拆分成不同环节  
  每个环节可替换或插入新环节

***

## JobSystem

  - 善用JobSystem和Native容器 适合在一帧内处理大量对象重复操作
  - 有限制 不能调用引用类型
  - 使用IJobParallelForTransform移动对象时 会影响需要模拟的物理效果
  - Native容器需要主动释放 不然会报错

***

##  MonoBehaviour生命周期
  ### 嵌套结构

  - Awake/Start/Update/FixedUpdate都是无序执行
  - OnDestroy深度优先 最外层无序 子节点从上往下

***

## VisualScripting

  1. ScriptMachine.graph(FlowGraph) 脚本图类型
  2. FlowGraph.units 图中节点
  3. FlowGraph.variables Graph变量
  4. ScriptMachine.enabled 可以控制事件的触发 包括自定义事件
  
  - 没有长节点 长节点功能需要在每帧实现

***

##  异步等待
  - 使用yield return代替await 协程的中断更加方便
  - 将异步操作封装成协程使用 

***

##  手柄支持
  - 手柄玩游戏更加舒服 应该支持手柄操作(RTS等类型不必)
  - 支持手柄的UI都比较简洁 而且原生不支持焦点切换功能

***

## 编码风格

  - 为减少代码的量 方便维护会对常用功能进行封装
  - 各个语言库其实就是官方封装好的功能集

  1. 严谨的API的输入数据都是通过参数传进来的 结果是返回出去的 不在函数中获取和修改域外参数
  2. 函数对输入的参数需要异常检测 担心影响性能可以通过宏控制(可以使用Assert)
  3. 对现有类型封装时使用[Extensions][Extensions]扩展函数 来使你的代码更加优雅
  4. 使用`partial`分割类的不同功能 (状态/动画/编辑器) 避免代码文件行数过多

***

[返回顶部](#fgufwmono)


[Extensions]: https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

[VisualScripting]: https://docs.unity3d.com/Packages/com.unity.visualscripting@1.9/manual/index.html