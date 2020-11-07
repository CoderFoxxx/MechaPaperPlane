/*
 *
 * Bootstrap.cs
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
    class Bootstrap
    {
        private static Sqlite botsDatabase;

        public static void Init()
        {
            if (!Directory.Exists($@"{Directory.GetCurrentDirectory()}\Bots")) Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\Bots");

            //Check, if bot database exists, if it does load it...
            if (File.Exists($@"{Directory.GetCurrentDirectory()}\Bots.db"))
            {
                botsDatabase = new Sqlite(new SQLiteConnection($@"Data Source={Directory.GetCurrentDirectory()}\Bots.db;Version=3;"));
                botsDatabase.GetConnection().Open();
            }
            //...Otherwise, create it and create a table inside the database.
            else
            {
                Logger.Log(new Bootstrap(), "There is no bots database. Creating one...", LogLevel.WARN);
                
                botsDatabase = new Sqlite(new SQLiteConnection($@"Data Source={Directory.GetCurrentDirectory()}\Bots.db;Version=3;"));
                botsDatabase.GetConnection().Open();

                Logger.Log(new Bootstrap(), @$"Successfully created SQLite Bots database in {Directory.GetCurrentDirectory()}\Bots.db", LogLevel.SUCCESS);
                botsDatabase.ConstructCommand("CREATE TABLE Bots (Id INTEGER PRIMARY KEY AUTOINCREMENT, BotName varchar(100), BotMe varchar(255), BotToken varchar(39))").ExecuteNonQuery();
            }

            //Check if any bot exists in the database
            int count = Convert.ToInt32(botsDatabase.ConstructCommand("SELECT count(*) FROM Bots").ExecuteScalar());
            if (count == 0)
            {
                Logger.Log(new Bootstrap(), "Bots database is empty. What name will you give to your first bot? Remember, name must be without any extra symbols and can't be shorter than 4 symbols and not longer than 100 symbols.", LogLevel.REQUEST);

                string botName = "";
                bool nameReadProc = true;
                while (nameReadProc)
                {
                    botName = Program.ReadValue();

                    var reg = new Regex("^[a-zA-Z0-9 ]*$");

                    if ((botName.Length < 4 || botName.Length > 100) || !reg.IsMatch(botName))
                        Logger.Log(new Bootstrap(), "Your bot name is invalid. Name must be w/o any special symbols and can't be shorter than 4 chars and can't be longer than 100 chars", LogLevel.ERROR);
                    else nameReadProc = false;
                }

                Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\Bots\{botName}\");
                Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\Bots\{botName}\Commands");

                Logger.Log(new Bootstrap(), "Alright! Now enter token for your bot. It must be in @BotFather conversation.", LogLevel.REQUEST);

                string token = "";
                bool tokenReadProc = true;

                while (tokenReadProc)
                {
                    token = Program.ReadValue();
                    var tokenReg = new Regex("[0-9]{9}:[a-zA-Z0-9_-]{35}");

                    if (!tokenReg.IsMatch(token))
                        Logger.Log(new Bootstrap(), "Your bot token is invalid.", LogLevel.ERROR);
                    else tokenReadProc = false;
                }

                Logger.Log(new Bootstrap(), "Adding bot to the database...", LogLevel.INFO);
                
                SQLiteCommand add = new SQLiteCommand("INSERT INTO Bots(Id,BotName,BotMe,BotToken) VALUES(1,$name,NULL,$token)", botsDatabase.GetConnection());
                add.Parameters.AddWithValue("$name", botName); add.Parameters.AddWithValue("$token", token);
                add.ExecuteNonQuery();
            }
            else { botsDatabase.GetConnection().Close(); return; }
        }

        public override string ToString() { return "Bootstrap"; }
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
