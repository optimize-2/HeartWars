# 制图手册

## 房间结构

一张地图仅由一面组成。

这一面里应包含等候区与战斗区。

### 等候区

应该用墙围起来，需要包含实体 `RedTeamTrigger`, `BlueTeamTrigger`, `GameStartButton` 与 `DefaultSpawnPoint`。

### 战斗区

红方基地里应包含实体 `RedTeamHeart`, `RedTeamSpawnPoint` 与 `GameFeatureTrigger`。

蓝方基地里应包含实体 `BlueTeamHeart`, `BlueTeamSpawnPoint` 与 `GameFeatureTrigger`。

其中 `SpawnPoint` 应该被放置远离基地的地方，配上一个能到基地的机关（如放在被围起来的高塔顶并令玩家出生在一个向下的车，由车载着玩家到基地，用来模拟重生倒计时），同时被放置在 `GameFeatureTrigger` 内（为了保证一出生就激活游戏功能）。

## 实体

### `RedTeamTrigger`

进入即可选择红队。

### `BlueTeamTrigger`

进入即可选择蓝队。

### `GameStartButton`

选择队伍后按下即可开始匹配。

### `DefaultSpawnPoint`

默认重生点。

### `RedTeamHeart`

红队的心，被蓝队玩家吃了后红队成员死亡无法重生。

### `BlueTeamHeart`

蓝队的心，被红队玩家吃了后蓝队成员死亡无法重生。

### `RedTeamSpawnPoint`

红队在心不被吃的情况下的重生点。

### `BlueTeamSpawnPoint`

蓝队在心不被吃的情况下的重生点。

### `GameFeatureTrigger`

用来激活游戏特性（射击，血量显示等）。

### `DashDisableTrigger`

用来禁用冲刺。

### `DashEnableTrigger`

用来启用冲刺。

### `ResourceGenerator`

资源点。

### `Shop`

商店。