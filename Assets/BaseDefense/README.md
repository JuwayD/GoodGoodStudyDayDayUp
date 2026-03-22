## BaseDefense 项目概述

本目录包含一个基于 Unity 实现的**基地防御（塔防风格）玩法示例**，同时配合一套用于自动化验证和编辑器辅助的工具脚本。

### 1. 玩法与目标

- **核心玩法**：玩家在网格地图上放置各类建筑（核心、防御塔等），通过合理布局和资源管理，对抗从刷怪点生成并沿导航网格前进的敌人，保护基地核心不被摧毁。
- **胜负条件（示例）**：
  - 胜利：在若干波敌人攻击下生存下来或在限定时间内保护核心建筑。
  - 失败：核心建筑（`BuildingType.Core`）被敌人摧毁。

### 2. 主要游戏系统

- **网格与建筑系统**
  - `GridManager`：负责世界坐标与网格坐标转换，并记录网格占用情况。
  - `BuildingData`（ScriptableObject）：定义建筑类型、预制体、造价、属性等数据。
  - `BuildingEntity`：建筑实体组件，持有 `BuildingData` 并在场景中占用网格。
  - `BuildingGhost`：建造预览“虚影”，根据合法性改变显示状态。
  - `BuildingPlacer`：处理玩家输入、射线检测、网格校验和资源扣除，最终实例化建筑。

- **敌人与战斗系统**
  - `EnemySpawner`：按时间间隔在指定 `SpawnPoint` 生成敌人，并为其寻找目标（通常为核心建筑）。
  - `EnemyEntity`：敌人逻辑（移动、攻击、死亡），基于 `NavMeshAgent` 在导航网格上行走。
  - `TowerEntity`：防御塔逻辑，周期性搜索敌人并发射 `Projectile`。
  - `Projectile`：子弹飞行与命中，调用 `DamageableEntity` / `HealthComponent` 扣除生命值。
  - `HealthComponent` / `DamageableEntity`：统一的生命值与受击接口，供建筑和敌人复用。

- **资源与 UI 系统**
  - `ResourceManager`：管理金币等资源数值，提供检查与扣除接口。
  - `ResourceHUD` / `ResourceBarUI`：在 HUD 上展示当前资源等信息。
  - `UIManager`：确保场景中存在 `Canvas`、`EventSystem` 和 HUD 根节点（如 `InGameHUD_Root`），并提供 `AI_SetupUI` 便于自动搭建 UI 框架。
  - `BuildingPaletteUI`：建筑选择面板，通过调用 `BuildingPlacer.SelectBuilding` 切换当前建造目标。

- **摄像机与输入**
  - `CameraController` / `CameraManager`：处理场景中的摄像机移动、旋转、缩放等行为。
  - 通过鼠标点击与键盘输入驱动建造与视角操作。

### 3. 编辑器与自动化验证工具

本项目强调“可自动验证”的工作流，在 `Scripts/Editor` 中包含多种 Editor 扩展脚本：

- `BDVerificationTool`、`AIVerificationHubEditor`：集中入口，用于一键执行多项检查与自动化操作，例如：
  - 搭建或修复场景中的关键对象（摄像机、HUD、SpawnPoint 等）；
  - 调用 UI / 摄像机 / NavMesh 辅助工具进行配置与验证；
  - 触发敌人生成、建筑放置等流程以便截图验证。
- `CameraVerification`：检查主摄像机是否存在并挂载正确脚本，视角参数是否合规。
- `UIAutomationHelper`：在编辑器中自动创建 `Canvas`、`EventSystem`、HUD 根节点等 UI 基础结构。
- `FontAndNavMeshHelper`：辅助导入字体（如中文字体 `simhei` 及其 SDF）并检查/烘焙 NavMesh。
- `InitialDataGenerator`：一键生成若干默认 `BuildingData` 资产，保证工程克隆后可以直接运行示例关卡。

这些工具通常会配合 `BDLogger` 输出统一格式的日志，并在 `Assets/Screenshots` / `Assets/VerificationReports` 下生成验证截图和报告，方便人工或自动流程审核项目状态。

### 4. 目录结构简述

- `Data/Buildings`：建筑相关的 `ScriptableObject` 数据（如核心、防御塔等）。
- `Prefabs`：建筑、敌人、核心等游戏对象的预制体。
- `Materials` / `Fonts`：材质与字体资源（含中文字体 SDF，用于 UI）。
- `Scenes/MainScene.unity`：主要演示关卡场景。
- `Scripts`：玩法脚本与全局管理器。
- `Scripts/UI`：UI 相关脚本（HUD、资源条、建筑面板等）。
- `Scripts/Editor`：编辑器扩展与自动化验证工具。

### 5. 使用建议

- **快速体验玩法**：
  1. 打开 `Scenes/MainScene.unity`。
  2. 运行游戏，并通过 HUD 中的建筑面板选择建筑，在地面网格上放置核心和防御塔。
  3. 启用敌人自动生成（或通过 `EnemySpawner` 的 Inspector / 上下文菜单手动生成），观察敌人沿 NavMesh 进攻核心的过程。

- **在编辑器中快速修复/搭建环境**：
  - 在 Unity 菜单中打开对应的验证/辅助窗口（如 `BDVerificationTool`），执行：
    - UI 搭建（`AI_SetupUI`）；
    - 摄像机与 NavMesh 检查；
    - 一键生成默认 `BuildingData` 与基础场景元素。

通过上述系统与工具，`BaseDefense` 既可以作为一个完整可玩的塔防示例，也可以作为演示“如何用 Editor 工具和自动化验证来保障关卡与玩法正确性”的参考工程。

