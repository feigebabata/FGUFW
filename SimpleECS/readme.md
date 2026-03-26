# FGUFW.SimpleECS
- 简化版ECS
- 处理大批量相似Entity Archetype相差大可以考虑开多个World

- 组件:
  - 每个组件类型只有一个缓存区 即便Entity没有这个组件 存储空间会有所浪费
  - 静态元数据缓存 组件类型转int 可能会对存档有影响
  - TransformAccess算是特殊组件 用Archetype最后一位表示
  
- 实体:
  - 每个Entity都有对应的Archetype 用来标记组合模式
  - 用NativeQueue记录增删指令 然后到单独系统统一处理

- 世界:
  - RegisterSystems:登记要用的系统 同时决定执行顺序
  - RegisterComponentBuffers:登记所用到的组件类型