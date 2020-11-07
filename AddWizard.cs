/*
 *
 * AddWizard.cs
 * Copyright 2020, CoderFoxxx (https://github.com/CoderFoxxx)
 *
 * This code is licensed under the MIT license.
 * You can find license after the code.
 *
*/

using System;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

namespace MechaPaperPlane
{
    class AddWizard
    {
        private Sqlite sqlite;

        private AddWizard() { }
        public AddWizard(Sqlite sqlite) { this.sqlite = sqlite; }

        public bool Init()
        {
        Begin:
            Logger.Log(new AddWizard(), "A new bot, huh? Let's add 'em", LogLevel.INFO);

            string name = "";
            bool nameRead = true;
            while(nameRead)
            {
                Logger.Log(new AddWizard(), "Type a name for your new bot. You can exit wizard by typing \"/exit\"", LogLevel.REQUEST);
                name = Program.ReadValue();

                var reg = new Regex("^[a-zA-Z0-9 ]*$");

                if ((name.Length < 4 || name.Length > 100) || !reg.IsMatch(name))
                    Logger.Log(new AddWizard(), "Your bot name is invalid. Name must be w/o any special symbols and can't be shorter than 4 chars and can't be longer than 100 chars", LogLevel.ERROR);
                else
                {
                    SQLiteDataReader reader = sqlite.ConstructCommand("SELECT * FROM Bots WHERE BotName=$name", "$name", name).ExecuteReader();
                    if (!reader.HasRows) nameRead = false;
                    else Logger.Log(new AddWizard(), $"Bot with name {name} already exists.", LogLevel.ERROR);
                }
            }

            string token = "";
            bool tokenRead = true;
            while (tokenRead)
            {
                Logger.Log(new AddWizard(), "Alright, now we need a bot token by @BotFather. Enter it here:", LogLevel.REQUEST);
                token = Program.ReadValue();
                var tokenReg = new Regex("[0-9]{9}:[a-zA-Z0-9_-]{35}");
                if(tokenReg.IsMatch(token))
                {
                    SQLiteDataReader reader = sqlite.ConstructCommand("SELECT * FROM Bots WHERE BotToken=$token", "$token", token).ExecuteReader();
                    if (!reader.HasRows) tokenRead = false;
                    else Logger.Log(new AddWizard(), "Seems like some bot is already using this token. Please, try another token.", LogLevel.ERROR);
                }
                else Logger.Log(new AddWizard(), $"Your token is invalid. Token regex: {tokenReg.ToString()}", LogLevel.ERROR);
            }

            Logger.Log(new AddWizard(), "Inserting bot into the database...", LogLevel.INFO);
            SQLiteCommand write = sqlite.ConstructCommand("INSERT INTO Bots(BotName,BotMe,BotToken) VALUES($name,NULL,$token)");
            write.Parameters.AddWithValue("$name", name);
            write.Parameters.AddWithValue("$token", token);
            write.ExecuteNonQuery();
            Logger.Log(new AddWizard(), "Creating bot directory...", LogLevel.INFO);
            Directory.CreateDirectory($@"{Program.BotsDirectory}\{name}");
            Logger.Log(new AddWizard(), $"Added {name} bot successfully.", LogLevel.SUCCESS);

            Logger.Log(new AddWizard(), "Do you want to add another bot? (y/n)", LogLevel.REQUEST);
            ConsoleKeyInfo yorn = Console.ReadKey();
            if (yorn.Key == ConsoleKey.Y) goto Begin;
            else return true;
        }

        public override string ToString() { return "AddWizard"; }
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
