# UFlowChart

## Overview

**UFlowChart** is a programmable flowchart editor designed for Unity Editor. It allows developers to visually build and control structured logic such as behavior trees, story flows, and task logic by writing C# scripts to define custom nodes.

> Basic knowledge of C# scripting is required to use this editor, as all node logic is implemented through code.

Key Features:

1. **C#-based customizable node system**: Developers can implement and extend node behavior by inheriting interfaces or base classes.
2. **Simple and modular script architecture**: New functional nodes can be added without modifying the core editor.
3. **Subgraph support**: A group of nodes can be encapsulated into a reusable module.
4. **Node overloading**: Nodes can dynamically change behavior based on connected parameter types.
5. **Integrated visual UI**: Built on [ZXMLui](https://github.com/ZSaltedFish/ZXMLui), providing a clean and flexible XML-driven UI editing experience.

## Installation

> ⚠️ ZXMLui is a required dependency. Please install it before installing UFlowChart.

Install both packages via Unity Package Manager:

- ZXMLui (dependency)  
  Git URL:  
  `https://github.com/ZSaltedFish/ZXMLui.git?path=Packages/ZXMLui`

- UFlowChart (main)  
  Git URL:  
  `https://github.com/ZSaltedFish/UFlowChart.git?path=Packages/FlowChart`

Steps:

1. Open Unity Editor
2. Navigate to `Window > Package Manager`
3. Click the `+` button in the top-left corner and choose `Add package from git URL...`
4. Paste the URLs above to install ZXMLui first, then UFlowChart

## Documentation

Refer to the detailed user guide, examples, and API reference in your preferred language:

- [English](Readme_EN.pdf)
- [简体中文](Readme_CN.pdf)
- [日本語](Readme_JP.pdf)
