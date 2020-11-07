/*
 *
 * Bot.cs
 * Copyright 2020, CoderFoxxx (https://github.com/CoderFoxxx)
 *
 * This code is licensed under the MIT license.
 * You can find license after the code.
 *
*/

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MechaPaperPlane
{
    class Bot
    {
        private static TelegramBotClient client;
        private Sqlite database;
        private static List<int> enabledBots = new List<int>();

        private static string name;
        private int id;

        private Bot() { }
        public Bot(int id) { this.id = id; }

        public StartResult Start()
        {
            if(!IsEnabled())
            {
                Logger.Log(new Bot(), "Grabbing information about the bot from database...", LogLevel.INFO);
                database = new Sqlite(new SQLiteConnection($@"Data Source={Directory.GetCurrentDirectory()}\Bots.db;Version=3;"));
                database.GetConnection().Open();
                SQLiteDataReader botDetails = database.ConstructCommand("SELECT * FROM Bots WHERE Id=$id", "$id", id).ExecuteReader();

                try
                {
                    string token = "";
                    while (botDetails.Read())
                    {
                        name = $"{botDetails["BotName"]}";
                        token = $"{botDetails["BotToken"]}";
                    }

                    Logger.Log(new Bot(), $"Trying to start \"{name}\" bot...", LogLevel.INFO);

                    Logger.Log(new Bot(), $"<{name}> Validating token...", LogLevel.INFO);
                    var tokenReg = new Regex("[0-9]{9}:[a-zA-Z0-9_-]{35}");
                    if (!tokenReg.IsMatch(token))
                    {
                        Logger.Log(new Bot(), $"<{name}> Your bot token is invalid. Please consider editing bot credentials", LogLevel.ERROR);
                        return StartResult.FAIL;
                    }

                    Logger.Log(new Bot(), $"<{name}> Setting bot token...", LogLevel.INFO);
                    client = new TelegramBotClient(token);
                    Logger.Log(new Bot(), $"<{name}> Bot token set successfully.", LogLevel.SUCCESS);

                    Logger.Log(new Bot(), $"<{name}> Registering bot commands...", LogLevel.INFO);
                    if (!Directory.Exists(Program.BotsDirectory + @$"\{name}"))
                    {
                        Logger.Log(new Bot(), $"<{name}> There is no directory for your bot. Creating one...", LogLevel.WARN);
                        Directory.CreateDirectory(Program.BotsDirectory + @$"\{name}");
                    }
                    RegisterCommands(name);

                    var me = client.GetMeAsync().Result;

                    enabledBots.Add(GetId());
                    Logger.Log(new Bot(), $"<{name}> Bot @{me.Username} has been initialized.", LogLevel.INFO);
                    Console.Title = $"Telegram Bot Client \"MechaPaperPlane\" - Running bots: {enabledBots.Count}";

                    client.OnMessage += OnMessage;
                    client.StartReceiving();

                    return StartResult.SUCCESS;
                }
                catch (AggregateException ex)
                {
                    Logger.Log(new Bot(), $"<{name}> There is a problem while trying to launch a bot: [AggregateException: {ex.Message}]", LogLevel.ERROR);
                    database = null;
                    botDetails = null;
                    return StartResult.FAIL;
                }
            }
            else
            {
                Logger.Log(new Bot(), $"This bot is already enabled.", LogLevel.ERROR);
                return StartResult.ALREADY_ENABLED;
            }
        }

        private void RegisterCommands(string currentBot)
        {
            if (!Directory.Exists(Program.BotsDirectory + currentBot + @"\Commands")) { Logger.Log(new Bot(), "There is no \"Commands\" directory. Creating one...", LogLevel.WARN); Directory.CreateDirectory(@$"{Program.BotsDirectory}\{currentBot}\Commands"); }

            if (Directory.GetFiles(Program.BotsDirectory + currentBot + @"\Commands").Length == 0)
            {
                Logger.Log(new Bot(), $"<{name}> There is no commands to register.", LogLevel.WARN);
                return;
            }

            int commandsRegistered = 0;

            dynamic commandDll = Reflections.CreateInstanceOfClass(Program.BotsDirectory + currentBot + @"\Commands\TelegramBotCommand.dll");

            foreach (string dll in Directory.GetFiles(Program.BotsDirectory + currentBot + @"\Commands\", "*.dll").Where(s => !(s.Contains("TelegramBotCommand"))))
            {
                try
                {
                    commandDll.RegisterCommand(Reflections.CreateInstanceOfClass(dll).GetAbstractTelegramBotCommandObject());
                    Logger.Log(new Bot(), $"<{name}> Command /{Reflections.CreateInstanceOfClass(dll).Name} has been registered.", LogLevel.SUCCESS);
                    commandsRegistered++;
                }
                catch (FileLoadException)
                {
                    Logger.Log(new Bot(), $"<{name}> File {dll} is already loaded.", LogLevel.WARN);
                    return;
                }
                catch (BadImageFormatException)
                {
                    Logger.Log(new Bot().ToString(), $"<{name}> File {dll} is not a DLL.", LogLevel.WARN);
                    return;
                }
            }

            Logger.Log(new Bot().ToString(), $"<{name}> {commandsRegistered} command(s) registered.", LogLevel.INFO);
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            string[] args = null;
            Logger.Log(new Bot(), $"<{name}> User @{e.Message.Chat.Username} sent a message: {e.Message.Text}", LogLevel.INFO);

            if (Directory.GetFiles(Program.BotsDirectory + name + @"\Commands").Length == 0) return;
            else Reflections.CreateInstanceOfClass(Program.BotsDirectory + name + @"\Commands\TelegramBotCommand.dll").ExecuteCommand(args, e.Message, client, GetName());
        }

        public void Stop()
        {
            if(IsEnabled())
            {
                Logger.Log(new Bot(), $"<{name}> Stopping receiver...", LogLevel.INFO);
                client.StopReceiving();
                enabledBots.Remove(GetId());
                Logger.Log(new Bot(), $"<{name}> Bot successfully shutted down.", LogLevel.SUCCESS);
            }
            else
            {
                Logger.Log(new Bot(), $"<{name}> Bot is not started.", LogLevel.ERROR);
                return;
            }
        }

        public string GetName() { return name; }
        public int GetId() { return id; }
        public bool IsEnabled() { return enabledBots.Contains(GetId()); }

        public static List<int> GetEnabledBots() { return enabledBots; }

        public static Bot GetBotById(int id) { return new Bot(id); }

        public TelegramBotClient GetTelegramBotClient() { return client; }

        public override string ToString() { return "Bot"; }
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
