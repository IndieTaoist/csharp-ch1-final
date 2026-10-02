using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_IntroLevel_GameProgram
{
    internal class Program
    {
        /// <summary>
        /// Enum: Scene name
        /// </summary>
        private enum SceneTag
        {
            /// <summary>
            /// Scene: Main menu
            /// </summary>
            Menu, 

            /// <summary>
            /// Scene: Instruction
            /// </summary>
            Instruction,

            /// <summary>
            /// Scene: Game
            /// </summary>
            Game,

            /// <summary>
            /// Scene: Result
            /// </summary>
            Result,
        }

        /// <summary>
        /// Enum: Game info
        /// </summary>
        private enum InfoType
        {
            /// <summary>
            /// Distance between boss & player
            /// </summary>
            Dist,

            /// <summary>
            /// Player & Boss ATK
            /// </summary>
            ATK,

            /// <summary>
            /// Battle state
            /// </summary>
            Battle,

            /// <summary>
            /// Final result
            /// </summary>
            Result,
        }

        #region Part 1. const vars
        // const: Window's dimension
        private const int Width = 40, Height = 30;
        // const: Player's initial position
        private const int InitXpos = 2, InitYPos = 1;
        // const: Player's moving steps
        private const int StepX = 2, StepY = 1;
        // const: Buttons' cursor positions
        private const int GeneralButtonCol = 16, ButtonMinRow = 10, MenuButtonMaxRow = 14, FinalButtonMaxRow = 12;

        // const: The initial blood — monster & player
        private const int InitMonsterBlood = 30, InitPlayerBlood = 25;
        // const: Random ATK range — monster & player
        private const int PlayerMinATK = 2, PlayerMaxATK = 12, MonsterMinATK = 1, MonsterMaxATK = 10;
        #endregion

        #region Part 2. readonly vars
        // readonly: All main-menu-buttons' names
        private static readonly string[] mainMenuButtons = {"开始游戏", "操作说明", "退出程序"};
        // readonly: All final-menu-buttons' names
        private static readonly string[] finalMenuButtons = { "再玩一次", "返回菜单" };
        // readonly: All games' icons
        private static readonly string iconWall = "■", iconPlayer = "●", iconMonster = "▲", iconPrincess = "♥";
        // readonly: All game info's head-row
        private static readonly int hInfoDist = Height - 6, hInfoATK = Height - 5, hInfoBattle = Height - 4, hInfoResult = Height - 3;
        #endregion

        #region Part 3. Static vars
        // Current scene's name
        private static SceneTag sceneTag = SceneTag.Menu;
        // Tag: Update the target scene layout
        private static bool isInMenuScene = false, isInstrScene = false;
        private static bool isInGameScene = false, isInFinalScene = false;
        private static bool isGameOver = true, isExitApp = false;
        // Button index: Main menu & final menu
        private static int buttonIndex = 0, buttonRowIndex = ButtonMinRow;

        // Cursor position: Monster、Princess、and Player
        private static int bossPosX, bossPosY;
        private static int princessX, princessY;
        private static int xPos, yPos, oldXPos, oldYPos;
        // Distance: From player to monster
        private static int distance;
        // ATK & blood —— Player & monster
        private static int playerATK, playerBlood, monsterATK, monsterBlood;
        // Random generator
        private static Random random = new Random();
        #endregion

        static void Main(string[] args)
        {
            // Step 0. Set up game window
            SetupGameWindow(Width, Height);

            try
            {
                // Player's input
                ConsoleKey inputKey;

                do
                {
                    // Execute scene logic
                    switch (sceneTag)
                    {
                        #region Scene Logic 1: Main menu
                        case SceneTag.Menu:
                            if (!isInMenuScene)
                            {
                                SetupMenuScene();
                            }
                            else
                            {
                                inputKey = Console.ReadKey(true).Key;
                                InvokeSceneLogicOfMainMenu(inputKey);
                            }
                            break;
                        #endregion

                        #region Scene Logic 2: Instruction
                        case SceneTag.Instruction:
                            if (!isInstrScene)
                            {
                                SetupInstrScene();
                            }
                            else
                            {
                                inputKey = Console.ReadKey(true).Key;
                                InvokeSceneLogicOfInstruction(inputKey);
                            }
                            break;
                        #endregion

                        #region Scene Logic 3: Game
                        case SceneTag.Game:
                            if (!isInGameScene)
                            {
                                SetupGameScene();
                            }
                            else
                            {
                                inputKey = Console.ReadKey(true).Key;
                                InvokeSceneLogicOfGame(inputKey);
                            }
                            break;
                        #endregion

                        #region Scene Logic 4: Final result
                        case SceneTag.Result:
                            if (!isInFinalScene)
                            {
                                SetupFinalScene();
                            }
                            else
                            {
                                inputKey = Console.ReadKey(true).Key;
                                InvokeSceneLogicOfFinalScene(inputKey);
                            }
                            break;
                        #endregion
                    }

                    // Break game loop if user choose to exit
                    if (isExitApp) { break; }

                } while (true);
            }
            finally
            {
                SetupExitScene();
            }
        }

        #region Part 1. All general helper funcs
        /// <summary>
        /// Setup game window
        /// </summary>
        /// <param name="width"> window's width </param>
        /// <param name="height"> window's height </param>
        static void SetupGameWindow(int width, int height)
        {
            // Step 1. Setup window & buffer size
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height); 
            // Step 2. Hide cursor
            Console.CursorVisible = false;
        }

        /// <summary>
        /// Set up menu buttons: Main menu & final menu
        /// </summary>
        static void SetupButtonList(string[] targetButtons)
        {
            for (int i = 10, j = 0; i < 16; i += 2, j++)
            {
                if (j < targetButtons.Length)
                    DrawIconOrText(GeneralButtonCol, i, (i == 10 ? ConsoleColor.Green : ConsoleColor.White), targetButtons[j]);
            }
        }

        /// <summary>
        /// Draw icon or text
        /// </summary>
        /// <param name="cursorX">cursor pos: playerX</param>
        /// <param name="cursorY">cursor pos: playerY</param>
        /// <param name="strColor">text or icon color</param>
        /// <param name="str">text or icon</param>
        static void DrawIconOrText(int cursorX, int cursorY, ConsoleColor strColor, string str)
        {
            Console.ForegroundColor = strColor;
            Console.SetCursorPosition(cursorX, cursorY);
            Console.Write(str);
        }
        #endregion

        #region Part 2. All helper funcs: Main menu
        /// <summary>
        /// Set up main menu scene
        /// </summary>
        static void SetupMenuScene()
        {
            // 1. Setup scene's title
            SetupMenuSceneTitle();
            // 2. Setup button list
            SetupButtonList(mainMenuButtons);
            // 3. The current scene is main menu
            isInMenuScene = true; 
        }

        /// <summary>
        /// Set up menu scene title
        /// </summary>
        static void SetupMenuSceneTitle()
        {
            Console.SetCursorPosition(14, 5);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("勇者");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" 救 ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("公主");

            DrawIconOrText(11, 5, ConsoleColor.Blue, iconPlayer);
            DrawIconOrText(13, 4, ConsoleColor.Cyan, "■ ■ ■");
            DrawIconOrText(13, 6, ConsoleColor.Cyan, "■ ■ ■");

            DrawIconOrText(19, 4, ConsoleColor.Yellow, "※");
            DrawIconOrText(19, 6, ConsoleColor.Yellow, "※");

            DrawIconOrText(22, 4, ConsoleColor.Red, "■ ■ ■");
            DrawIconOrText(22, 6, ConsoleColor.Red, "■ ■ ■");
            DrawIconOrText(28, 5, ConsoleColor.Magenta, iconPrincess);
        }

        /// <summary>
        /// Execute main menu's logic
        /// </summary>
        /// <param name="inputKey">The user's input</param>
        static void InvokeSceneLogicOfMainMenu(ConsoleKey inputKey)
        {
            switch (inputKey)
            {
                // Button 1: Move up
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (buttonRowIndex > ButtonMinRow)
                    {
                        DrawIconOrText(GeneralButtonCol, buttonRowIndex, 
                            ConsoleColor.White, mainMenuButtons[buttonIndex]);

                        buttonRowIndex -= 2;
                        buttonIndex--;

                        DrawIconOrText(GeneralButtonCol, buttonRowIndex, 
                            ConsoleColor.Green, mainMenuButtons[buttonIndex]);
                    }
                    else
                    {
                        buttonRowIndex = ButtonMinRow;
                        buttonIndex = 0;
                    }
                    break;

                // Button 2: Move down
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (buttonRowIndex < MenuButtonMaxRow)
                    {
                        DrawIconOrText(GeneralButtonCol, buttonRowIndex,
                            ConsoleColor.White, mainMenuButtons[buttonIndex]);

                        buttonRowIndex += 2;
                        buttonIndex++;

                        DrawIconOrText(GeneralButtonCol, buttonRowIndex,
                            ConsoleColor.Green, mainMenuButtons[buttonIndex]);
                    }
                    else
                    {
                        buttonRowIndex = MenuButtonMaxRow;
                        buttonIndex = 2;
                    }
                    break;

                // Button 3: Enter key
                case ConsoleKey.Enter:
                    Console.Clear();
                    isInMenuScene = false;

                    switch (buttonIndex)
                    {
                        // Go to game scene
                        case 0:
                            sceneTag = SceneTag.Game;
                            break;
                        
                        // Go to instruction scene
                        case 1:
                            sceneTag = SceneTag.Instruction;
                            break;

                        // Result the game
                        case 2:
                            isExitApp = true;
                            break;
                    }

                    // Reset the state of main menu's button before leaving
                    buttonIndex = 0;
                    buttonRowIndex = ButtonMinRow;
                    break;
            }
        }
        #endregion

        #region Part 3. All helper funcs: Instruction scene
        /// <summary>
        /// Set up instruction scene
        /// </summary>
        static void SetupInstrScene()
        {
            // Instr 1. Main menu
            SetupInstrOfMainMenu();
            // Instr 2. Game scene
            SetupInstrOfGameScene();
            // Instr 3. Backspace
            SetupInstrOfBackToMainMenu();
            // The current scene is instruction
            isInstrScene = true;
        }

        /// <summary>
        /// Set up info: Main & Final menu
        /// </summary>
        static void SetupInstrOfMainMenu()
        {
            DrawIconOrText(1, 1, ConsoleColor.Green, "♦ 主菜单 & 结束菜单: ");
            DrawIconOrText(3, 3, ConsoleColor.Cyan, "W / ↑: 上移");
            DrawIconOrText(23, 3, ConsoleColor.Cyan, "S / ↓: 下移");
            DrawIconOrText(3, 5, ConsoleColor.Cyan, "Enter: 确认");

            DrawIconOrText(1, 7, ConsoleColor.White, "======================================");
        }

        /// <summary>
        /// Set up info: Game scene
        /// </summary>
        static void SetupInstrOfGameScene()
        {
            // Part 1. Basic operation
            DrawIconOrText(1, 9, ConsoleColor.DarkYellow, "♦ 游戏界面: 操作篇");
            DrawIconOrText(3, 11, ConsoleColor.Yellow, "W / ↑: 上移");
            DrawIconOrText(23, 11, ConsoleColor.Yellow, "S / ↓: 下移");
            DrawIconOrText(3, 13, ConsoleColor.Yellow, "A / ←: 左移");
            DrawIconOrText(23, 13, ConsoleColor.Yellow, "D / →: 右移");
            DrawIconOrText(3, 15, ConsoleColor.Yellow, "Space: 攻击");

            DrawIconOrText(1, 17, ConsoleColor.White, "--------------------------------------");

            // Part 2. Game icon
            DrawIconOrText(1, 19, ConsoleColor.DarkYellow, "♦ 游戏界面: 图标篇");
            DrawIconOrText(3, 21, ConsoleColor.White, iconWall);
            DrawIconOrText(4, 21, ConsoleColor.Yellow, " : 边界墙");
            DrawIconOrText(23, 21, ConsoleColor.Blue, iconPlayer);
            DrawIconOrText(24, 21, ConsoleColor.Yellow, " : 勇者");
            DrawIconOrText(3, 23, ConsoleColor.Red, iconMonster);
            DrawIconOrText(4, 23, ConsoleColor.Yellow, " : BOSS");
            DrawIconOrText(23, 23, ConsoleColor.Magenta, iconPrincess);
            DrawIconOrText(24, 23, ConsoleColor.Yellow, " : 公主");

            DrawIconOrText(1, 25, ConsoleColor.White, "======================================");

        }

        /// <summary>
        /// Set up info: Back to main menu
        /// </summary>
        static void SetupInstrOfBackToMainMenu()
        {
            DrawIconOrText(4, 28, ConsoleColor.White, "请按下 BACKSPACE 键返回到主菜单!");
        }

        /// <summary>
        /// Execute instruction scene
        /// </summary>
        /// <param name="inputKey">The user's input key</param>
        static void InvokeSceneLogicOfInstruction(ConsoleKey inputKey)
        {
            switch (inputKey)
            {
                // Go back to main menu
                case ConsoleKey.Backspace:
                    Console.Clear();
                    sceneTag = SceneTag.Menu;
                    isInstrScene = false;
                    break;
            }
        }
        #endregion

        #region Part 4. All helper funcs: Game Scene
        /// <summary>
        /// Set up game scene
        /// </summary>
        static void SetupGameScene()
        {
            // Setting 1: Game's boundary
            SetUpGameBoundaries();
            // Setting 2: Initial position of player and boss
            SetupInitPosOfPlayerAndBoss();
            // Setting 3: Game's state
            SetupInitGameState();
        }

        /// <summary>
        /// Set up game boundaries
        /// </summary>
        static void SetUpGameBoundaries()
        {
            for (int i = 0; i < Width; i+= StepX)
            {
                DrawIconOrText(i, 0, ConsoleColor.White, iconWall);
                DrawIconOrText(i, Height-8, ConsoleColor.White, iconWall);
                DrawIconOrText(i, Height-1, ConsoleColor.White, iconWall);
            }

            for(int i = 0; i < Height - 1; i += StepY)
            {
                DrawIconOrText(0, i, ConsoleColor.White, iconWall);
                DrawIconOrText(Width-2, i, ConsoleColor.White, iconWall);
            }
        }

        /// <summary>
        /// Set up initial position: Player & Boss
        /// </summary>
        static void SetupInitPosOfPlayerAndBoss()
        {
            // Player's initial position: (2, 1)
            xPos = InitXpos;
            yPos = InitYPos;
            oldXPos = xPos;
            oldYPos = yPos;

            // Draw player's icon
            DrawIconOrText(xPos, yPos, ConsoleColor.Blue, iconPlayer);

            // Get random pos of boss
            GetRandomPos(ref bossPosX, ref bossPosY, xPos, yPos);
            // Draw monster's icon
            DrawIconOrText(bossPosX, bossPosY, ConsoleColor.Red, iconMonster);

            // Display the initial game info
            UpdateTwoPointsDist();
        }

        /// <summary>
        /// Set up initial game state
        /// </summary>
        static void SetupInitGameState()
        {
            // Initial blood of both player and boss
            playerBlood = InitPlayerBlood;
            monsterBlood = InitMonsterBlood;
            // Initial game state
            isInGameScene = true;
            // Start playing game
            isGameOver = false;
        }

        /// <summary>
        /// Get random cursor position
        /// </summary>
        /// <param name="xPos">random X</param>
        /// <param name="yPos">random Y</param>
        /// <param name="playerX">Cursor X: Player</param>
        /// <param name="playerY">Cursor Y: Player</param>
        static void GetRandomPos(ref int xPos, ref int yPos, int playerX, int playerY)
        {
            do
            {
                // xPos is always even number
                xPos = (random.Next(1, (Width / 2) - 1) * 2);
                yPos = random.Next(1, Height - 8);

            } while (xPos == playerX && yPos == playerY); // Make sure it does not overlap with player's pos
        }

        /// <summary>
        /// Update the distance between player's pos and monster's pos
        /// </summary>
        static void UpdateTwoPointsDist()
        {
            int dx = xPos - bossPosX;
            int dy = yPos - bossPosY;

            distance = Convert.ToInt32(Math.Sqrt(dx * dx + dy * dy));
            UpdateTargetGameInfo(InfoType.Dist, ConsoleColor.Gray, "玩家 和 怪物 之间相差 " + distance + " 米.");
        }

        /// <summary>
        /// Update battle state of each turn
        /// </summary>
        static void UpdateBattleState()
        {
            // Get random ATK
            playerATK = random.Next(PlayerMinATK, PlayerMaxATK + 1);
            monsterATK = random.Next(MonsterMinATK, MonsterMaxATK + 1);

            // Update blood val
            playerBlood -= monsterATK;
            monsterBlood -= playerATK;

            // Case 1: Both are alive
            if (playerBlood > 0 && monsterBlood > 0)
            {
                ConsoleColor targetTextColor;

                // Same ATK: Yellow
                if (playerATK == monsterATK)
                {
                    targetTextColor = ConsoleColor.Yellow;
                }

                // Player's ATK is greater: Blue
                else if (playerATK > monsterATK)
                {
                    targetTextColor = ConsoleColor.Blue;
                }

                else // Monster's ATK is greater: Red
                {
                    targetTextColor = ConsoleColor.Red;
                }

                // Update ATK & Battle info
                UpdateTargetGameInfo(InfoType.ATK, targetTextColor, "玩家攻击: " + playerATK + ", 怪物攻击: " + monsterATK);
                UpdateTargetGameInfo(InfoType.Battle, targetTextColor, "玩家血量: " + playerBlood + ", 怪物血量: " + monsterBlood);
            }

            else
            {
                isGameOver = true;
                ClearTargetGameInfo(hInfoDist);
                ClearTargetGameInfo(hInfoATK);

                // Both die
                if (playerBlood <= 0 && monsterBlood <= 0)
                {
                    EraseIcon(xPos, yPos);
                    EraseIcon(bossPosX, bossPosY);

                    UpdateTargetGameInfo(InfoType.Battle, ConsoleColor.DarkMagenta, "玩家血量: " + 0 + ", 怪物血量: " + 0);
                    UpdateTargetGameInfo(InfoType.Result, ConsoleColor.DarkMagenta, "两败俱伤, 请按下 Enter 键退出!");

                    // Release boss's pos
                    bossPosX = 0;
                    bossPosY = 0;
                }

                // Player lost
                else if (playerBlood <= 0)
                {
                    EraseIcon(xPos, yPos);
                    UpdateTargetGameInfo(InfoType.Battle, ConsoleColor.Red, "玩家血量: " + 0 + ", 怪物血量: " + monsterBlood);
                    UpdateTargetGameInfo(InfoType.Result, ConsoleColor.DarkRed, "勇者已死, 请按下 Enter 键退出!");
                }
                else
                {
                    EraseIcon(bossPosX, bossPosY);
                    UpdateTargetGameInfo(InfoType.Battle, ConsoleColor.Blue, "玩家血量: " + playerBlood + ", 怪物血量: " + 0);
                    UpdateTargetGameInfo(InfoType.Result, ConsoleColor.Green, "击杀怪物成功, 请去营救公主!");

                    // Release boss's pos
                    bossPosX = 0;
                    bossPosY = 0;

                    // Release the princess
                    GetRandomPos(ref princessX, ref princessY, xPos, yPos);
                    DrawIconOrText(princessX, princessY, ConsoleColor.Magenta, iconPrincess);
                }
            }
        }

        /// <summary>
        /// Update target game info
        /// </summary>
        /// <param name="info"> Type of info </param>
        /// <param name="infoColor"> Color of info </param>
        /// <param name="infoStr"> Specific info </param>
        static void UpdateTargetGameInfo(InfoType info, ConsoleColor infoColor, string infoStr)
        {
            switch (info)
            {
                // Distance btween player & boss
                case InfoType.Dist:
                    ClearTargetGameInfo(hInfoDist);
                    DrawIconOrText(2, hInfoDist, infoColor, infoStr);
                    break;

                // Player & boss' ATK
                case InfoType.ATK:
                    ClearTargetGameInfo(hInfoATK);
                    DrawIconOrText(2, hInfoATK, infoColor, infoStr);
                    break;

                // Game battle state
                case InfoType.Battle:
                    ClearTargetGameInfo(hInfoBattle);
                    DrawIconOrText(2, hInfoBattle, infoColor, infoStr);
                    break;

                // Game result
                case InfoType.Result:
                    ClearTargetGameInfo(hInfoResult);
                    DrawIconOrText(2, hInfoResult, infoColor, infoStr);
                    break;
            }
        }

        /// <summary>
        /// Clear target game info
        /// </summary>
        /// <param name="targetRow"> Row: Info's pos </param>
        static void ClearTargetGameInfo(int targetRow)
        {
            for(int i = 2; i < Width - 2; i++)
            {
                Console.SetCursorPosition(i, targetRow);
                Console.Write(" ");
            }
        }

        /// <summary>
        /// Execute scene logic of game
        /// </summary>
        /// <param name="inputKey">The user's input</param>
        static void InvokeSceneLogicOfGame(ConsoleKey inputKey)
        {
            // Always record Player's pre-pos
            oldXPos = xPos;
            oldYPos = yPos;

            switch (inputKey)
            {
                // Move left
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    xPos = Math.Max(2, xPos - StepX);
                    break;
                
                // Move right
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    xPos = Math.Min(Width - 4, xPos + StepX);
                    break;

                // Move up
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    yPos = Math.Max(1, yPos - StepY);
                    break;

                // Move down
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    yPos = Math.Min(Height - 9, yPos + StepY);
                    break;

                // Attack
                case ConsoleKey.Spacebar:
                    // Pre-condition: isGameOver = false
                    if (!isGameOver)
                    {
                        // ClearTargetGameInfo(hInfoBattle);

                        if (IsAllowToATK(distance))
                        {
                            UpdateBattleState();
                        }
                        else
                        {
                            UpdateTargetGameInfo(InfoType.ATK, ConsoleColor.Black, string.Empty);
                            UpdateTargetGameInfo(InfoType.Battle, ConsoleColor.DarkYellow, "玩家离怪物太远了!");
                        }
                    }
                    break;

                case ConsoleKey.Enter:
                    // Pre-condition: Game over & player lost
                    if (isGameOver && playerBlood <= 0)
                    {
                        JumpToFinalScene();
                    }
                    break;
            }

            // As long as player does not die
            if (playerBlood > 0 && (oldXPos != xPos || oldYPos != yPos))
            {
                // Case 1: Game is running
                if (!isGameOver)
                {
                    if (xPos != bossPosX || yPos != bossPosY)
                    {
                        DrawIconOrText(oldXPos, oldYPos, ConsoleColor.Black, " ");
                    }

                    // Special case: Overlap with Boss's pos
                    else if (xPos == bossPosX && yPos == bossPosY)
                    {
                        int tempX = oldXPos;
                        int tempY = oldYPos;

                        xPos = tempX;
                        yPos = tempY;
                    }

                    UpdateTwoPointsDist();
                    DrawIconOrText(xPos, yPos, ConsoleColor.Blue, iconPlayer);
                } 
                else
                {
                    DrawIconOrText(oldXPos, oldYPos, ConsoleColor.Black, " ");

                    // Save princess: Move to Princess's pos
                    if (xPos == princessX && yPos == princessY)
                    {
                        JumpToFinalScene();
                    }
                    else
                    {
                        DrawIconOrText(xPos, yPos, ConsoleColor.Blue, iconPlayer);
                    }
                }
            }
        }

        /// <summary>
        /// Check availability of attacking
        /// </summary>
        /// <param name="currDist">current distance between player and boss</param>
        /// <returns>true if distance is less or equal than 3 </returns>
        static bool IsAllowToATK(int currDist)
        {
            return currDist <= 3;
        }

        /// <summary>
        /// Erase icon
        /// </summary>
        /// <param name="cursorX">icon's X pos</param>
        /// <param name="cursorY">icon's Y </param>
        static void EraseIcon(int cursorX, int cursorY)
        {
            DrawIconOrText(cursorX, cursorY, ConsoleColor.Black, " ");
        }

        /// <summary>
        /// Jump to final scene
        /// </summary>
        static void JumpToFinalScene()
        {
            // Clear the current layout
            Console.Clear();
            // Update the current scene's name
            sceneTag = SceneTag.Result;
            isInGameScene = false;
        }
        #endregion

        #region Part 5.All helper funcs: Final Scene
        /// <summary>
        /// Set up final scene according to the game result
        /// </summary>
        static void SetupFinalScene()
        {
            // Case 1: Die
            if (playerBlood <= 0 && monsterBlood <= 0)
            {
                SetupTitleForDie();
            }

            // Case 2: Player lost
            else if (playerBlood <= 0)
            {
                SetupTitleForLost();
            }
            else
            {
                SetupTitleForWin();
            }

            // Setup button list
            SetupButtonList(finalMenuButtons);
            isInFinalScene = true;
        }

        /// <summary>
        /// Result title: Die
        /// </summary>
        static void SetupTitleForDie()
        {
            DrawIconOrText(12, 4, ConsoleColor.DarkYellow, "※※※※※※※※");
            DrawIconOrText(12, 5, ConsoleColor.DarkYellow, "※");
            DrawIconOrText(16, 5, ConsoleColor.DarkMagenta, "两败俱伤");
            DrawIconOrText(26, 5, ConsoleColor.DarkYellow, "※");
            DrawIconOrText(12, 6, ConsoleColor.DarkYellow, "※※※※※※※※");
        }

        /// <summary>
        /// Result title: Player Lost
        /// </summary>
        static void SetupTitleForLost()
        {
            DrawIconOrText(11, 4, ConsoleColor.DarkRed, "☠️☠️☠️☠️☠️☠️☠️☠️☠️");
            DrawIconOrText(11, 5, ConsoleColor.DarkRed, "☠️");
            DrawIconOrText(14, 5, ConsoleColor.Red, "勇者战死沙场!");
            DrawIconOrText(27, 5, ConsoleColor.DarkRed, "☠️");
            DrawIconOrText(11, 6, ConsoleColor.DarkRed, "☠️☠️☠️☠️☠️☠️☠️☠️☠️");
        }

        /// <summary>
        /// Result title: Player win
        /// </summary>
        static void SetupTitleForWin()
        {
            DrawIconOrText(11, 4, ConsoleColor.Magenta, "❤️❤️❤️❤️❤️❤️❤️❤️❤️");
            DrawIconOrText(11, 5, ConsoleColor.Magenta, "❤️");
            DrawIconOrText(14, 5, ConsoleColor.Blue, "营救公主成功!");
            DrawIconOrText(27, 5, ConsoleColor.Magenta, "❤️");
            DrawIconOrText(11, 6, ConsoleColor.Magenta, "❤️❤️❤️❤️❤️❤️❤️❤️❤️");
        }
        
        /// <summary>
        /// Execute final scene
        /// </summary>
        /// <param name="inputKey">The user's input</param>
        static void InvokeSceneLogicOfFinalScene(ConsoleKey inputKey)
        {
            switch (inputKey)
            {
                // Button 1: Move up
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (buttonRowIndex > ButtonMinRow)
                    {
                        DrawIconOrText(GeneralButtonCol, buttonRowIndex,
                            ConsoleColor.White, finalMenuButtons[buttonIndex]);

                        buttonRowIndex -= 2;
                        buttonIndex--;

                        DrawIconOrText(GeneralButtonCol, buttonRowIndex,
                            ConsoleColor.Green, finalMenuButtons[buttonIndex]);
                    }
                    else
                    {
                        buttonRowIndex = ButtonMinRow;
                        buttonIndex = 0;
                    }
                    break;

                // Button 2: Move down
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (buttonRowIndex < FinalButtonMaxRow)
                    {
                        DrawIconOrText(GeneralButtonCol, buttonRowIndex,
                            ConsoleColor.White, finalMenuButtons[buttonIndex]);

                        buttonRowIndex += 2;
                        buttonIndex++;

                        DrawIconOrText(GeneralButtonCol, buttonRowIndex,
                            ConsoleColor.Green, finalMenuButtons[buttonIndex]);
                    }
                    else
                    {
                        buttonRowIndex = FinalButtonMaxRow;
                        buttonIndex = 2;
                    }
                    break;

                // Button 3: Enter key
                case ConsoleKey.Enter:
                    isInFinalScene = false;
                    Console.Clear();

                    switch (buttonIndex)
                    {
                        // Go to game scene
                        case 0:
                            sceneTag = SceneTag.Game;
                            break;

                        // Go to instruction scene
                        case 1:
                            sceneTag = SceneTag.Menu;
                            break;
                    }

                    // Reset the state of main menu's button before leaving
                    buttonIndex = 0;
                    buttonRowIndex = ButtonMinRow;
                    break;
            }
        }
        #endregion

        #region Part 6. All helper funcs: Exit scene
        /// <summary>
        /// Set up exit scene
        /// </summary>
        static void SetupExitScene()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Black;

            DrawIconOrText(8, 6, ConsoleColor.White, "*************************");
            DrawIconOrText(8, 7, ConsoleColor.White, "*");
            DrawIconOrText(11, 7, ConsoleColor.White, "HAVE A GREAT DAY 😀");
            DrawIconOrText(32, 7, ConsoleColor.White, "*");
            DrawIconOrText(8, 8, ConsoleColor.White, "*************************");

            DrawIconOrText(9, 10, ConsoleColor.White, "<(￣︶￣)↗[Thank you!]");
            DrawIconOrText(10, 13, ConsoleColor.Green, "👍");
            DrawIconOrText(8, 15, ConsoleColor.Yellow, "[点赞]");
            DrawIconOrText(20, 13, ConsoleColor.Green, "🪙");
            DrawIconOrText(18, 15, ConsoleColor.Yellow, "[投币]");
            DrawIconOrText(29, 13, ConsoleColor.Green, "🌟");
            DrawIconOrText(27, 15, ConsoleColor.Yellow, "[收藏]");
            DrawIconOrText(8, 17, ConsoleColor.White, "*************************");
            DrawIconOrText(8, 18, ConsoleColor.White, "*");
            DrawIconOrText(10, 18, ConsoleColor.White, "Made by @indietaoist");
            DrawIconOrText(32, 18, ConsoleColor.White, "*");
            DrawIconOrText(8, 19, ConsoleColor.White, "*************************");

            Console.ForegroundColor = ConsoleColor.Black;
            Environment.Exit(0);
        }
        #endregion
    }
}
