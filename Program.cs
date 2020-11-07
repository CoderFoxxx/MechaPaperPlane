/*
 *
 * Program.cs
 * Copyright 2020, CoderFoxxx (https://github.com/CoderFoxxx)
 *
 * This code is licensed under the MIT license.
 * You can find license after the code.
 *
*/

using System;
using System.IO;
using Telegram.Bot;
using System.Data.SQLite;

namespace MechaPaperPlane
{
    class Program
    {
        private static TelegramBotClient api;
        public static Sqlite database;
        public static string BotsDirectory = $@"{Directory.GetCurrentDirectory()}\Bots\";

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("     Telegram Bot Client \"MechaPaperPlane\"");
            Console.WriteLine("                 by CoderFoxxx");
            Console.WriteLine("        ( https://github.com/CoderFoxxx )");
            Console.WriteLine("                     v1.0");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Used Telegram.Bot library: https://github.com/TelegramBots/Telegram.Bot");
            Console.WriteLine("App icon is from Vecteezy: https://www.vecteezy.com/free-vector/paper-airplane");
            Console.WriteLine();
            Console.Title = "Telegram Bot Client \"MechaPaperPlane\" - Initializing...";
            Logger.Log(new Program(), "Initializing bootstrap...", LogLevel.INFO);
            Bootstrap.Init();

            Logger.Log(new Program(), "Initializing bots database...", LogLevel.INFO);
            database = new Sqlite(new SQLiteConnection($@"Data Source={Directory.GetCurrentDirectory()}\Bots.db;Version=3;"));
            database.GetConnection().Open();
            Logger.Log(new Program(), "Checking database...", LogLevel.INFO);
        Actions:
            int count = Convert.ToInt32(database.ConstructCommand("SELECT count(*) FROM Bots").ExecuteScalar());
            if (count != 0)
            {
                Console.Title = "Telegram Bot Client \"MechaPaperPlane\" - Welcome!";
            
                Logger.Log(new Program(), $"There are {count} bot(s) in the database:", LogLevel.INFO);
                Console.WriteLine();

                SQLiteDataReader reader = database.ConstructCommand("SELECT * FROM Bots").ExecuteReader();
                while (reader.Read()) Console.WriteLine($"{reader["Id"]}. {reader["BotName"]}");
                Console.WriteLine();

                Logger.Log(new Program(), "You can:", LogLevel.INFO);
                Logger.Log(new Program(), "1. Enter Bot ID to start;", LogLevel.INFO);
                Logger.Log(new Program(), "2. Add a new bot:", LogLevel.INFO);
                bool actRead = true;
                while(actRead)
                {
                    Logger.Log(new Program(), "Select your action:", LogLevel.REQUEST);
                    string act = ReadValue();
                    switch (act)
                    {
                        case "1":
                            actRead = false;
                            continue;
                        case "2":
                            actRead = false;
                            if (new AddWizard(database).Init() == true) continue;
                            break;
                    }
                }

                bool botIdRead = true;
                int id = 0;
                while (botIdRead)
                {
                    try
                    {
                        Logger.Log(new Program(), "Enter the bot ID here to start: ", LogLevel.REQUEST);
                        id = Convert.ToInt32(ReadValue());

                        bool selBotExist = Convert.ToInt32(database.ConstructCommand("SELECT count(*) FROM Bots WHERE Id=$id", "$id", id).ExecuteScalar()) > 0;
                        if(!selBotExist)
                            Logger.Log(new Program(), $"Bot with ID {id} does not exist.", LogLevel.ERROR);
                        else botIdRead = false;
                    }
                    catch(FormatException)
                    {
                        Logger.Log(new Program(), "Your input is invalid.", LogLevel.ERROR);
                        Console.WriteLine();
                    }
                }

            MainMenu:
                Console.Title = $"Telegram Bot Client \"MechaPaperPlane\" - Selected bot №{id}";
                Logger.Log(new Program(), $"Selected bot with ID {id}. You can: (1) Launch a bot or (2) Edit bot's details (3) Remove bot from database.", LogLevel.INFO);
                Logger.Log(new Program(), "What do you want to do?", LogLevel.REQUEST);

                try
                {
                    int act = Convert.ToInt32(ReadValue());

                    switch (act)
                    {
                        case 1:
                            Console.Title = $"Telegram Bot Client \"MechaPaperPlane\" - Starting bot №{id}...";
                            Bot bot = new Bot(id);
                            if (bot.Start() == StartResult.SUCCESS) Prompt.Init();
                            else goto MainMenu;
                            break;
                        case 2:
                            Console.Title = $"Telegram Bot Client \"MechaPaperPlane\" - Edit wizard (Bot №{id})";
                            if (new EditWizard(database, id).Init() == true) goto MainMenu;
                            break;
                        case 3:
                            SQLiteCommand removeBotCommand = database.ConstructCommand("DELETE FROM Bots WHERE Id=$id", "$id", id);
                            Logger.Log(new Program(), $"Removing bot with ID {id}...", LogLevel.INFO);
                            removeBotCommand.ExecuteNonQuery();
                            Logger.Log(new Program(), $"Bot with ID {id} has been removed from the database.", LogLevel.SUCCESS);
                            goto Actions;
                        default:
                            Logger.Log(new Program(), "Unknown action.", LogLevel.ERROR);
                            goto MainMenu;
                    }
                }
                catch(FormatException)
                {
                    Logger.Log(new Program(), "Your input is invalid.", LogLevel.ERROR);
                    Console.WriteLine();
                    goto MainMenu;
                }
                

            }
            else return;
        }

        public static string ReadValue()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.White;

            return Console.ReadLine();
        }

        public override string ToString() { return "Main"; }
        public static TelegramBotClient GetTelegramBotClient() { return api; }
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
