# FGUFW.RPG
- 战斗系统相关设定
1. Buff:影响数值
2. Skill:功能
3. Item:道具 容器
4. Controller:控制器 控制技能 由玩家/AI

## Buff
- 属性:
  - Id: 表索引 用于添加和移除
  - Name: 名称
  - Description: 介绍 按理说应该还有icon之类的 不行就改成json
  - Type: buff类型 影响什么字段的数值
  - BaseValue: 数值 float一般够用 不行就object
  - Duration: 持续时间 -1:永久
  - Overlap: 同id处理 [未处理移除逻辑]
    - 大于1:可叠加上限,
    - 1:不变,
    - 0:时间刷新,
    - -1:可重复不叠加,
    - -2:可叠加无上限,
    - -3:时间叠加
  - StackCount:叠加数量
  - Layer: 0:并行,其他同层只能存在一个 LayerWeight>当前层则替换
  - LayerWeight
  - Expandsion: object类型 扩展部分 自己加
  - StartWorldTime: 起始时间 需要时重置

## Skill
- 属性:
  - Id: 表索引 用于添加和移除
  - Name: 名称
  - Description: 介绍 按理说应该还有icon之类的 不行就改成json
  - Usable: 类似解锁 比如道具或武器附带的技能 拥有但无法释放
  - Level: 如果可升级的话
  - Cost: 耗蓝 耗血 cd也算
  - CanCast: 满足代价 自身状态要求等
  - Act: 要干啥
  - 行为id:不同行为需要的字段不同 不可能全都填充到一张表 相同行为在一张表 如果是节点式技能只需id
  - State: 运行状态
    - 待机
    - 前摇
    - 释放
    - 后摇
  - Layer:使用mask 可能覆盖多个层
  - LayerWeight: 当作用层被占用 如果优先级大于当地便可覆盖

- 增删:
  - 被动加buff的技能在 删/升级 的时候需要移除buff

## Controller
- 控制器
- 为兼容AI 用Command传输控制指令

### Command
- Layer: //作用领域
- LayerWeight: //作用优先级