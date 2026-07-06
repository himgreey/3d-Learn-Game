# 3D Learn Game

一款基于 **Unity 2022** 的第三人称波次生存射击游戏。玩家选择角色进入战场，抵御逐波增强的怪物进攻，通过击杀与建造炮塔积累金币，并可在局内暂停、主动结算或死亡后查看战绩。

仓库地址：[github.com/himgreey/3d-Learn-Game](https://github.com/himgreey/3d-Learn-Game)

---

## 功能特性

- **角色系统**：4 名可选角色，不同生命 / 攻击属性，部分角色需金币解锁
- **波次刷怪**：准备阶段 → 多波怪物进攻 → 波次间隔倒计时，难度随波次提升
- **战斗操作**：WASD 移动、鼠标视角、近战 / 远程攻击、蹲下与翻滚
- **射击辅助**：准星射线优先；未命中时在角色正面扇形范围内锁定最近敌人
- **炮塔建造**：靠近 `FireDome` 建造点，按 `E` 打开菜单，用 `1` / `2` / `3` 建造或升级三级炮塔
- **经济系统**：击杀怪物获得金币，用于解锁角色与建造炮塔，局末自动存档
- **UI 与交互**：血条、波次、金币 HUD；提示面板、ESC 暂停、死亡 / 主动结算界面
- **数据驱动**：角色、怪物、炮塔配置通过 `StreamingAssets` 下 JSON 文件加载

---

## 技术栈

| 项目 | 说明 |
|------|------|
| 引擎 | Unity **2022.3.62f3c1** |
| 语言 | C# |
| UI | uGUI + TextMeshPro |
| 导航 | AI Navigation（NavMesh） |
| 数据 | LitJson + `StreamingAssets/*.json` |
| 版本管理 | Git + **Git LFS**（模型、贴图、音频等大文件） |

---

## 环境要求

- [Unity Hub](https://unity.com/download) + **Unity 2022.3.62f3c1**（或同系列 2022.3 LTS）
- [Git](https://git-scm.com/)
- [Git LFS](https://git-lfs.com/)（克隆前安装并初始化）

```bash
git lfs install
```

---

## 快速开始

### 1. 克隆仓库

```bash
git clone git@github.com:himgreey/3d-Learn-Game.git
cd 3d-Learn-Game
git lfs pull
```

HTTPS 克隆：

```bash
git clone https://github.com/himgreey/3d-Learn-Game.git
cd 3d-Learn-Game
git lfs pull
```

> 若未安装 Git LFS，部分 `.fbx`、`.psd`、`.wav` 等资源可能无法正常下载。

### 2. 用 Unity 打开项目

1. 打开 Unity Hub → **Add** → 选择项目根目录
2. 确认编辑器版本为 **2022.3.62f3c1**
3. 等待首次导入与编译完成

### 3. 运行游戏

1. 打开场景 `Assets/Scenes/BeginScene.unity`
2. 点击 **Play**
3. 主菜单 → 选角 → 进入 `GameScene` 开始战斗

---

## 操作说明

### 主菜单 / 选角

| 操作 | 功能 |
|------|------|
| 开始游戏 | 进入选角界面 |
| 设置 | 调节背景音乐与音效 |
| 左 / 右切换 | 预览角色 |
| 购买 | 花费金币解锁角色 |
| 开始 | 进入战斗场景 |

### 战斗中

| 按键 | 功能 |
|------|------|
| `W` `A` `S` `D` | 移动 |
| 鼠标 | 转动视角 |
| 鼠标左键 | 攻击 |
| 左 `Ctrl` | 蹲下 |
| `空格` | 翻滚 |
| `E` | 在 FireDome 附近打开 / 关闭炮塔建造菜单 |
| `1` `2` `3` | 建造 / 升级对应等级炮塔 |
| `ESC` | 关闭提示弹窗；无弹窗时打开 / 关闭暂停菜单 |

### 炮塔费用（可在配置中修改）

| 等级 | 费用 | 攻击 | 射程 | 攻击间隔 |
|------|------|------|------|----------|
| 1 级 | $30 | 5 | 8m | 4s |
| 2 级 | $60 | 10 | 10m | 3s |
| 3 级 | $100 | 20 | 12m | 2s |

击杀每只怪物奖励 **$20**。

---

## 项目结构

```
Assets/
├── Scenes/
│   ├── BeginScene.unity      # 主菜单与选角
│   └── GameScene.unity       # 战斗场景
├── Scripts/
│   ├── BeginScene/           # 主菜单 UI、镜头动画
│   ├── GameScene/            # 玩家、怪物、炮塔、波次、暂停与结算
│   ├── Data/                 # 数据管理与配置类型
│   ├── UI/                   # UIManager、BasePanel、光标控制
│   └── Json/                 # LitJson 解析
├── Resources/                # 运行时加载的预制体（角色、怪物、炮塔、UI）
├── StreamingAssets/          # JSON 配置（角色 / 怪物 / 炮塔）
└── ArtRes/                   # 美术与第三方资源
```

### 主要脚本

| 脚本 | 职责 |
|------|------|
| `PlayerObject` | 玩家移动、攻击、受伤、金币与死亡结算 |
| `MonsterObject` / `MonsterPoint` | 怪物 AI 与波次刷怪 |
| `TowerBuildManager` / `BuiltTower` | 炮塔建造与自动攻击 |
| `GamePanel` | 战斗 HUD（血条、波次、金币、建造菜单） |
| `GamePauseManager` | ESC 暂停与弹窗优先级处理 |
| `GameDataManager` | 存档、音效、配置读取 |
| `UIManager` / `GameCursor` | 面板管理与鼠标锁定 / 解锁 |

### 配置文件

| 文件 | 内容 |
|------|------|
| `StreamingAssets/RoleInfo.json` | 角色属性与解锁价格 |
| `StreamingAssets/MonsterInfo.json` | 怪物属性 |
| `StreamingAssets/TowerInfo.json` | 炮塔等级、费用与战斗参数 |

修改 JSON 后重新运行即可生效，无需改代码。

---

## 场景流程

```
BeginScene（主菜单）
    ↓ 选角并开始
GameScene（战斗）
    ↓ 死亡或主动结算
SettlementPanel（战绩统计）
    ↓ 确认
BeginScene（金币已存档）
```

---

## 开发说明

### 不建议提交的内容

项目 `.gitignore` 已排除：

- `Library/`、`Temp/`、`obj/`、`Build/`、`UserSettings/`
- `*.csproj`、`*.sln`、`.vs/`

### Git LFS 跟踪类型

`.gitattributes` 中对以下扩展名启用了 LFS：

`*.fbx` `*.psd` `*.tif` `*.wav` `*.mp4` `*.asset` 等

### 第三方资源

`Assets/ArtRes/` 中包含外部美术与特效资源（如 PureNature、GabrielAguiarProductions 等），版权归原作者所有，请遵守各资源包许可协议，勿用于未经授权的商业用途。

---

## 常见问题

**Q: 克隆后模型 / 音频缺失？**  
A: 确认已执行 `git lfs install` 与 `git lfs pull`。

**Q: 打开项目版本不匹配？**  
A: 请安装 Unity **2022.3.62f3c1**，或在 Hub 中切换到 2022.3 LTS 同系列版本。

**Q: 弹出面板后无法点击按钮？**  
A: 项目已在 `UIManager` 中统一处理鼠标解锁；若仍异常，检查是否有自定义脚本再次锁定了光标。

**Q: 波次一直显示「准备」？**  
A: 确认 `GameScene` 中存在 `PlayerBorn`、`MonsterPoint`，且 `GamePanel` 已正确挂载到场景 UI。

---

## 许可证

本项目代码与仓库结构可自由学习与参考。  
`Assets/ArtRes/` 内第三方素材遵循其原始授权，不在本仓库统一开源许可范围内。

---

## 作者

[himgreey](https://github.com/himgreey)

如有问题或建议，欢迎通过 [Issues](https://github.com/himgreey/3d-Learn-Game/issues) 反馈。
