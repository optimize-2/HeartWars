# HeartWars

## Network:

已完成：

 - DataMissileShot / 子弹发射
 - DataPlayerDie / 玩家死亡
 - DataStartRequest / 开始游戏申请
 - DataStartResponse / 开始游戏回应
 - DataGameStart / 开始游戏
 - DataHeartBreak / 吃心

写完但废弃：
 - DataGameEnd / 游戏结束
 - DataGameStatusQuery / 游戏状态查询
 - DataGameStateResponse / 游戏状态回应

未完成：

 - DataBuyXXX / 购买 XXX （团队升级，陷阱等）

## Entity:

已完成：

 - Missile / 子弹
 - RedTeamTrigger / 红队选择
 - BlueTeamTrigger / 蓝队选择
 - GameStartButton / 游戏开始按钮
 - GameFeatureTrigger / 游戏开始触发器
 - RedTeamHeart / 红心
 - BlueTeamHeart / 蓝心
 - RedTeamSpawnPoint / 红队出生点
 - BlueTeamSpawnPoint / 蓝队出生点
 - ResourceGeneratorI / 3s 1xp
 - ResourceGeneratorII / 30s 200xp
 - ResourceGeneratorIII / 60s 800xp
 - DashEnableTrigger / 启用冲刺，吃心用
 - DashDisableTrigger / 禁用冲刺，不禁用子弹追不上玛德琳

未完成：

 - RedTeamTrapTrigger / 红队陷阱触发器
 - BlueTeamTrapTrigger / 蓝队陷阱触发器
 - ShopItemXXX / 售卖 XXX 的商店（跳跃提升，敌人提示，武器升级）

## Inventory:

已完成：
 - 物品栏渲染及武器选择
 - Gun / 枪
 - GoldenBerry / 金草莓
 - FireBall / 笨弹（独步：期待一个在群服里打炮）（伤害 + 击退，可以拿来起飞）
 - BadPearl / badeline 珍珠

未完成
 - RayGun / 激光枪
 - DamageEnhance / 伤害加成（1.0 -> 1.2 -> 1.4 -> 1.5）
 - HealthEnhance / 生命加成（100 -> 120 -> 140 -> 150）
 - WindEnhance / 风加成（敌人吃心的时候有逆风）
 - BlindTrap / 失明陷阱
 - BroadcastTrap / 广播陷阱

不知道要不要做：
 - Blade / 剑

## Shop

写完了，哈哈！

## Effects

写完了，哈哈！

## Feedback

### enhancements:

 - 单手操作不便

    方案：二段跳冲刺、禁用抓、增加平射按键
 - 反馈不够

    受伤：并做好受伤反馈（如改变头发颜色和增加打击感）

    死亡：直接放个烟花就行了

### bugs:

 - 有的时候打不中

    由于群服延迟不能完全避免，考虑增加子弹速度和碰撞箱等
 - 后来进来的人可以发起匹配，导致数据紊乱

    解决不来，希望以后玩这张图的人素质高一点吧，目前只能做到检测到数据紊乱就停止游戏
 - 复活没有回到出生点

    到时候尝试复现一下