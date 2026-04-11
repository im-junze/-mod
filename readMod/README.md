# 星露谷物语Mod开发示例

这是一个简单的星露谷物语Mod开发示例，帮助你了解如何开始开发星露谷物语Mod。

## 项目结构

```
readMod/
├── Class1.cs          # Mod的主入口类
├── manifest.json      # Mod的配置文件
├── readMod.csproj     # 项目配置文件
└── README.md          # 说明文档
```

## 如何使用

1. 确保你已经安装了[SMAPI](https://smapi.io/)（星露谷物语Mod加载器）
2. 构建项目（如果构建失败，可以手动复制文件）
3. 将生成的`readMod.dll`和`manifest.json`文件复制到星露谷物语的Mods文件夹中
4. 启动游戏，SMAPI会自动加载你的Mod

## 功能说明

- 游戏启动时，Mod会在SMAPI日志中显示"星露谷物语Mod已加载！"
- 当你按下F1键时，Mod会在SMAPI日志中显示"你按下了F1键！"

## 开发步骤

1. **创建项目**：使用Visual Studio或其他C# IDE创建一个.NET项目
2. **添加依赖**：添加`Pathoschild.Stardew.ModBuildConfig`包
3. **实现IMod接口**：创建一个类实现IMod接口，包含Entry方法
4. **注册事件**：在Entry方法中注册游戏事件
5. **编写功能**：在事件处理方法中实现你的功能
6. **创建manifest.json**：配置Mod的基本信息
7. **构建和部署**：构建项目并将文件复制到Mods文件夹

## 扩展建议

- 尝试添加更多事件处理，如玩家动作、物品使用等
- 学习使用Helper对象访问游戏API
- 了解如何修改游戏内容，如添加新物品、修改游戏机制等

## 参考资源

- [SMAPI文档](https://smapi.io/docs)
- [星露谷物语Mod开发指南](https://stardewvalleywiki.com/Modding:Index)
- [Pathoschild的Mod模板](https://github.com/Pathoschild/StardewMods)