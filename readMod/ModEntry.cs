
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace readMod
{
    /// <summary>模组配置</summary>
    public class ModConfig
    {
        /// <summary>文本文件路径</summary>
        public string TextFilePath { get; set; } = "";
    }

    /// <summary>文本阅读器菜单</summary>
    public class TextReaderMenu : IClickableMenu
    {
        private readonly ModEntry mod;
        private readonly string textContent;
        private readonly string fileName;
        private int scrollPosition;
        private readonly ClickableTextureComponent upButton;
        private readonly ClickableTextureComponent downButton;
        private readonly ClickableTextureComponent closeButton;
        private readonly ClickableTextureComponent reloadButton;

        public TextReaderMenu(ModEntry mod, string content, string filePath)
            : base((Game1.viewport.Width - 800) / 2, (Game1.viewport.Height - 600) / 2, 800, 600, true)
        {
            this.mod = mod;
            this.textContent = content;
            this.fileName = Path.GetFileName(filePath);
            this.scrollPosition = 0;

            // 创建按钮
            this.upButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 40, this.yPositionOnScreen + 20, 32, 32), Game1.mouseCursors, new Rectangle(421, 495, 12, 12), 2f);
            this.downButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 40, this.yPositionOnScreen + this.height - 52, 32, 32), Game1.mouseCursors, new Rectangle(421, 507, 12, 12), 2f);
            this.closeButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 40, this.yPositionOnScreen + 10, 32, 32), Game1.mouseCursors, new Rectangle(322, 342, 16, 16), 2f);
            this.reloadButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 10, this.yPositionOnScreen + 10, 32, 32), Game1.mouseCursors, new Rectangle(128, 495, 12, 12), 2f);
        }

        public override void draw(SpriteBatch b)
        {
            // 绘制背景
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            // 绘制标题
            string title = $"文本阅读器 - {this.fileName}";
            Utility.drawTextWithShadow(b, title, Game1.dialogueFont, new Vector2(this.xPositionOnScreen + 20, this.yPositionOnScreen + 15), Game1.textColor);

            // 绘制文本内容
            string[] lines = this.textContent.Split(new[] { '\n' }, StringSplitOptions.None);
            int maxLines = (this.height - 80) / 20;
            int startLine = Math.Min(this.scrollPosition, lines.Length - 1);
            int endLine = Math.Min(startLine + maxLines, lines.Length);

            for (int i = startLine; i < endLine; i++)
            {
                Utility.drawTextWithShadow(b, lines[i], Game1.smallFont, new Vector2(this.xPositionOnScreen + 20, this.yPositionOnScreen + 50 + (i - startLine) * 20), Game1.textColor);
            }

            // 绘制按钮
            this.upButton.draw(b);
            this.downButton.draw(b);
            this.closeButton.draw(b);
            this.reloadButton.draw(b);

            // 绘制滚动条
            if (lines.Length > maxLines)
            {
                float scrollPercentage = (float)this.scrollPosition / (lines.Length - maxLines);
                int scrollBarHeight = this.height - 100;
                int scrollBarY = this.yPositionOnScreen + 50 + (int)(scrollPercentage * scrollBarHeight);
                b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + this.width - 20, scrollBarY, 10, 20), Color.White);
            }

            base.draw(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (this.upButton.containsPoint(x, y))
            {
                this.scrollPosition = Math.Max(0, this.scrollPosition - 1);
                Game1.playSound("drumkit6");
            }
            else if (this.downButton.containsPoint(x, y))
            {
                string[] lines = this.textContent.Split(new[] { '\n' }, StringSplitOptions.None);
                int maxLines = (this.height - 80) / 20;
                this.scrollPosition = Math.Min(lines.Length - maxLines, this.scrollPosition + 1);
                Game1.playSound("drumkit6");
            }
            else if (this.closeButton.containsPoint(x, y))
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
            else if (this.reloadButton.containsPoint(x, y))
            {
                // 重新加载文件
                this.mod.ReloadTextFile();
                Game1.playSound("coin");
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            this.upButton.tryHover(x, y);
            this.downButton.tryHover(x, y);
            this.closeButton.tryHover(x, y);
            this.reloadButton.tryHover(x, y);
        }
    }

    /// <summary>模组入口点</summary>
    public class ModEntry : Mod
    {
        /*********
         ** 字段
         *********/
        /// <summary>模组配置</summary>
        private ModConfig config = new ModConfig();
        /// <summary>文本内容</summary>
        private string textContent = "";

        /*********
         ** 公共方法
         *********/
        /// <summary>模组的入口点，在首次加载模组后自动调用</summary>
        /// <param name="helper">对象 helper 提供用于编写模组的简化接口</param>
        public override void Entry(IModHelper helper)
        {
            // 加载配置
            this.config = helper.ReadConfig<ModConfig>();

            // 注册事件
            helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.OnButtonPressed);
            helper.Events.GameLoop.GameLaunched += new EventHandler<GameLaunchedEventArgs>(this.OnGameLaunched);
            helper.Events.Display.MenuChanged += new EventHandler<MenuChangedEventArgs>(this.OnMenuChanged);

            this.Monitor.Log("启动了星露谷物语文本阅读器mod");
        }

        /*********
         ** 私有方法
         *********/
        /// <summary>游戏启动时执行</summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // 注册控制台命令
            this.Helper.ConsoleCommands.Add("readtxt", "打开文本阅读器。用法: readtxt [文件路径]", this.OnReadTxtCommand);
        }

        /// <summary>在玩家按下键盘、控制器或鼠标上的按钮后引发</summary>
        /// <param name="sender">对象 sender 表示调用此方法的对象</param>
        /// <param name="e">对象 e 表示事件数据</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // 如果玩家还没有进入存档，则取消执行
            if (!Context.IsWorldReady)
                return;

            // 按下F5键打开文本阅读器
            if (e.Button == SButton.F5)
            {
                if (!string.IsNullOrEmpty(this.config.TextFilePath) && File.Exists(this.config.TextFilePath))
                {
                    this.LoadTextFile();
                    Game1.activeClickableMenu = new TextReaderMenu(this, this.textContent, this.config.TextFilePath);
                }
                else
                {
                    Game1.showRedMessage("请先设置文本文件路径。使用 'readtxt [文件路径]' 命令设置。");
                }
            }

            // 处理鼠标左键点击
            if (e.Button == SButton.MouseLeft)
            {
                // 检查是否在游戏菜单中
                if (Game1.activeClickableMenu is GameMenu gameMenu)
                {
                    // 获取游戏菜单的按钮列表
                    var buttons = gameMenu.GetType().GetField("buttons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gameMenu) as List<ClickableTextureComponent>;
                    if (buttons != null)
                    {
                        // 检查是否点击了文本阅读器按钮
                        var textReaderButton = buttons.FirstOrDefault(button => button.name == "TextReaderButton");
                        if (textReaderButton != null && textReaderButton.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                        {
                            // 打开文本阅读器
                            if (!string.IsNullOrEmpty(this.config.TextFilePath) && File.Exists(this.config.TextFilePath))
                            {
                                this.LoadTextFile();
                                Game1.activeClickableMenu = new TextReaderMenu(this, this.textContent, this.config.TextFilePath);
                            }
                            else
                            {
                                Game1.showRedMessage("请先设置文本文件路径。使用 'readtxt [文件路径]' 命令设置。");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>处理 readtxt 命令</summary>
        /// <param name="command">命令名称</param>
        /// <param name="args">命令参数</param>
        private void OnReadTxtCommand(string command, string[] args)
        {
            if (args.Length > 0)
            {
                string filePath = string.Join(" ", args);
                if (File.Exists(filePath))
                {
                    this.config.TextFilePath = filePath;
                    this.Helper.WriteConfig(this.config);
                    this.LoadTextFile();
                    Game1.activeClickableMenu = new TextReaderMenu(this, this.textContent, filePath);
                    this.Monitor.Log($"已设置并打开文本文件: {filePath}", LogLevel.Info);
                }
                else
                {
                    this.Monitor.Log($"文件不存在: {filePath}", LogLevel.Error);
                    if (Context.IsWorldReady)
                        Game1.showRedMessage($"文件不存在: {filePath}");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.config.TextFilePath) && File.Exists(this.config.TextFilePath))
                {
                    this.LoadTextFile();
                    Game1.activeClickableMenu = new TextReaderMenu(this, this.textContent, this.config.TextFilePath);
                }
                else
                {
                    this.Monitor.Log("请提供文本文件路径。用法: readtxt [文件路径]", LogLevel.Info);
                    if (Context.IsWorldReady)
                        Game1.showRedMessage("请提供文本文件路径。用法: readtxt [文件路径]");
                }
            }
        }

        /// <summary>加载文本文件</summary>
        private void LoadTextFile()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.config.TextFilePath) && File.Exists(this.config.TextFilePath))
                {
                    this.textContent = File.ReadAllText(this.config.TextFilePath);
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"读取文件时出错: {ex.Message}", LogLevel.Error);
                if (Context.IsWorldReady)
                    Game1.showRedMessage("读取文件时出错");
            }
        }

        /// <summary>重新加载文本文件</summary>
        public void ReloadTextFile()
        {
            this.LoadTextFile();
            if (Game1.activeClickableMenu is TextReaderMenu menu)
            {
                Game1.activeClickableMenu = new TextReaderMenu(this, this.textContent, this.config.TextFilePath);
            }
        }

        /// <summary>当游戏菜单改变时执行</summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // 检查是否打开了游戏菜单
            if (e.NewMenu is GameMenu gameMenu)
            {
                // 查找选项列表
                var optionsList = gameMenu.pages.OfType<OptionsPage>().FirstOrDefault();
                if (optionsList != null)
                {
                    // 这里可以添加选项，但更简单的方法是添加到游戏菜单的主页面
                    // 我们将使用更通用的方法，添加一个自定义菜单项
                }

                // 添加到游戏菜单的主页面
                this.AddReaderOptionToGameMenu(gameMenu);
            }
        }

        /// <summary>向游戏菜单添加文本阅读器选项</summary>
        /// <param name="gameMenu">游戏菜单</param>
        private void AddReaderOptionToGameMenu(GameMenu gameMenu)
        {
            // 这里我们使用一个更简单的方法，通过修改游戏菜单的按钮列表
            // 注意：这种方法可能会在游戏更新时失效，但对于当前版本有效

            // 我们将添加一个新的按钮到游戏菜单
            // 首先获取游戏菜单的按钮列表
            var buttons = gameMenu.GetType().GetField("buttons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gameMenu) as List<ClickableTextureComponent>;
            if (buttons != null)
            {
                // 检查是否已经添加了文本阅读器按钮
                bool alreadyAdded = buttons.Any(button => button.name == "TextReaderButton");
                if (!alreadyAdded)
                {
                    // 创建文本阅读器按钮
                    int buttonX = gameMenu.xPositionOnScreen + 20;
                    int buttonY = gameMenu.yPositionOnScreen + gameMenu.height - 60;
                    var button = new ClickableTextureComponent("TextReaderButton", new Rectangle(buttonX, buttonY, 160, 40), null, "文本阅读器", Game1.mouseCursors, new Rectangle(221, 425, 14, 11), 4f);
                    buttons.Add(button);
                }
            }
        }

    }
}