/*
 *
 * Prompt.cs
 * Copyright 2020, CoderFoxxx (https://github.com/CoderFoxxx)
 *
 * This code is licensed under the MIT license.
 * You can find license after the code.
 *
*/

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace MechaPaperPlane
{
    class Prompt
    {
        private static string prompt = "";

        //TODO: Prompt Args 
        //private static string[] promptArgs;

        public static void Init()
        {
            while (true) 
            {
                prompt = Program.ReadValue().ToLower();

                switch(prompt)
                {
                    case "exit":
                    case "quit":
                        Console.Title = "Telegram Bot Client \"MechaPaperPlane\" - Stopping...";
                        Logger.Log(new Program(), $"Stopping {Bot.GetEnabledBots().Count} bot(s)...", LogLevel.INFO);

                        foreach (int id in Bot.GetEnabledBots().ToArray())
                        {
                            Bot.GetBotById(id).Stop();
                            Logger.Log(new Program(), $"Stopped {Bot.GetBotById(id).GetName()} bot.", LogLevel.INFO); 
                        }
                        
                        Logger.Log(new Program(), "Thank you and goodbye!", LogLevel.INFO);
                        Console.Title = "Telegram Bot Client \"MechaPaperPlane\" - by TwinTailedFoxxx (https://github.com/TwinTailedFoxxx/)";
                        Console.WriteLine();
                        Console.Write("Press any key to exit...");
                        Console.ReadKey();
                        Process.GetCurrentProcess().Kill();
                        break;
                    case "start-bot":
                        Logger.Log(new Program(), "List of bots:", LogLevel.INFO);
                        SQLiteDataReader reader = Program.database.ConstructCommand("SELECT * FROM Bots").ExecuteReader();
                        while (reader.Read()) Console.WriteLine($"{reader["Id"]}. {reader["BotName"]}");
                        Console.WriteLine();
                        bool idRead = true;
                        while(idRead)
                        {
                            try
                            {
                                Logger.Log(new Program(), "Enter the ID of the bot you want to start: ", LogLevel.REQUEST);
                                int id = Convert.ToInt32(Program.ReadValue());

                                Bot bot = new Bot(id);
                                if (bot.Start() == StartResult.SUCCESS) Logger.Log(new Program(), "Bot started successfully", LogLevel.SUCCESS);
                                idRead = false;
                                break;
                            }
                            catch(FormatException) { Logger.Log(new Program(), "Your input is invalid", LogLevel.ERROR); }
                        }
                        
                        break;
                    default:
                        if (prompt.Length > 0) Logger.Log(new Program(), "Unknown command.", LogLevel.ERROR);
                        else Console.WriteLine();
                        break;
                }
            }
        }
        public static string GetPrompt() { return prompt; }
    }
}

/*

Copyright © 2020 CoderFoxxx

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the "Software"), to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/
