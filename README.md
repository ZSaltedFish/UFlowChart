# UFlowChart

## 简介

**UFlowChart** 是一款支持 C# 编程扩展的流程图编辑器，适用于 Unity 编辑器环境。该工具允许开发者以高度可定制的方式，通过脚本实现逻辑节点的可视化编辑和流程控制。适合用于行为树、任务系统、AI 流程、剧情控制等结构化逻辑的可视化构建。

主要特性：

1. **基于 C# 脚本的可编程节点系统**：开发者可通过继承接口或基类来自定义节点逻辑与表现形式。
2. **易于扩展的脚本体系**：添加新功能节点无需修改编辑器核心，只需创建新的脚本类。
3. **支持子图结构（Subgraph）**：可将一组节点封装成子图模块，实现功能复用与逻辑模块化。
4. **节点多态与重载**：同一个节点类型可根据连接的参数类型切换具体行为，支持更灵活的数据驱动设计。
5. **高度集成的可视化界面**：基于 [ZXMLui](https://github.com/ZSaltedFish/ZXMLui) 实现的 XML 配置式 UI 系统，提供清晰直观的编辑体验。

## 安装方式

> ⚠️ 使用 UFlowChart 前，请确保已安装依赖库 [ZXMLui](https://github.com/ZSaltedFish/ZXMLui.git?path=Packages/ZXMLui)

通过 Unity Package Manager 安装：

- ZXMLui（依赖库）  
  Git 安装路径：  
  `https://github.com/ZSaltedFish/ZXMLui.git?path=Packages/ZXMLui`

- UFlowChart 本体  
  Git 安装路径：  
  `https://github.com/ZSaltedFish/UFlowChart.git?path=Packages/FlowChart`

安装步骤：

1. 打开 Unity 编辑器
2. 进入 `Window > Package Manager`
3. 点击左上角 `+` 按钮，选择 `Add package from git URL...`
4. 分别粘贴上面的两个地址，先安装 ZXMLui，再安装 UFlowChart

## 文档与教程

详细使用说明、示例教程及 API 文档请参考以下语言版本的文档文件：

- [English](Readme_EN.pdf)
- [简体中文](Readme_CN.pdf)
- [日本語](Readme_JP.pdf)