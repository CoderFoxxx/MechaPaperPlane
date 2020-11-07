/*
 *
 * TelegramBotCommand.cs
 * Copyright 2020, CoderFoxxx (https://github.com/CoderFoxxx)
 *
 * This code is licensed under the MIT license.
 * You can find license after the code.
 *
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotCommand
{
    public class TelegramBotCommand
    {
        private string name;
        private string description;
        private static string commandPrefix = "/";
        private static List<ITelegramBotCommand> telegramBotCommands = new List<ITelegramBotCommand>();
        public static IReadOnlyList<ITelegramBotCommand> botCommands => telegramBotCommands.AsReadOnly();

        public TelegramBotCommand() { }

        public TelegramBotCommand(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public async void ExecuteCommand(string[] args, Message message, TelegramBotClient botClient, string botName)
        {
            args = message.Text.Split(' ').Skip(1).ToArray();
            foreach(ITelegramBotCommand command in botCommands)
            {
                if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text) return;
                if (message.Text.Contains($"{commandPrefix}{command.Name}")) 
                { 
                    Console.WriteLine($"[{DateTime.Now}] [INFO] [{new TelegramBotCommand().ToString()}] <{botName}> @{message.From.Username} ({message.From.FirstName} {message.From.LastName}) [{message.From.Id}] executed command {commandPrefix}{command.Name}."); 
                    await command.Execute(args, message, botClient); 
                }
                
            }
        }

        public void RegisterCommand(ITelegramBotCommand command) { if(!telegramBotCommands.Contains(command)) telegramBotCommands.Add(command); }

        public string GetName() { return name; }
        public string GetDescription() { return description; }

        public override string ToString(){ return "CommandExec"; }
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

