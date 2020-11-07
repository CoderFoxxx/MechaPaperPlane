/*
 *
 * EditWizard.cs
 * Copyright 2020, CoderFoxxx (https://github.com/CoderFoxxx)
 *
 * This code is licensed under the MIT license.
 * You can find license after the code.
 *
*/

using System;
using System.IO;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace MechaPaperPlane
{
    class EditWizard
    {
        private Sqlite sqlite;
        private int id;
        private EditWizard() { }
        public EditWizard(Sqlite sqlite, int id) { this.sqlite = sqlite; this.id = id; }

        public bool Init()
        {
            SQLiteDataReader reader = sqlite.ConstructCommand("SELECT * FROM Bots WHERE Id=$id", "$id", id).ExecuteReader();
            Console.WriteLine("Bot details:");
            Console.WriteLine("ID           Name            BotToken");
            while (reader.Read()) Console.WriteLine($"{reader["Id"]}           {reader["BotName"]}            It's secret ;)");

            Logger.Log(new EditWizard(), "Tip: To exit type 'exit'.", LogLevel.INFO);
            Logger.Log(new EditWizard(), "Tip: You can only change bot's name and token.", LogLevel.INFO);
            Logger.Log(new EditWizard(), "Type name of the parameter you want to change:", LogLevel.REQUEST);

            bool readParam = true;
            while(readParam)
            {
                string parameter = Program.ReadValue().ToLower();
                switch (parameter)
                {
                    case "name":
                        string oldName = "";
                        string newName = "";
                        bool newNameRead = true;

                        while(newNameRead)
                        {
                            Logger.Log(new EditWizard(), "Enter a new name for your bot: ",  LogLevel.REQUEST);
                            newName = Program.ReadValue();

                            var reg = new Regex("^[a-zA-Z0-9 ]*$");

                            if ((newName.Length < 4 || newName.Length > 100) || !reg.IsMatch(newName))
                                Logger.Log(new EditWizard(), "Your bot name is invalid. Name must be w/o any special symbols and can't be shorter than 4 chars and can't be longer than 100 chars", LogLevel.ERROR);
                            else newNameRead = false;
                        }

                        Logger.Log(new EditWizard(), "Step 1: Renaming bot directory...", LogLevel.INFO);
                        reader = sqlite.ConstructCommand("SELECT * FROM Bots WHERE Id=$id", "$id", id).ExecuteReader();
                        while (reader.Read()) oldName = $"{reader["BotName"]}";
                        Directory.CreateDirectory($"{Program.BotsDirectory}{newName}");
                        Directory.Move(@$"{Program.BotsDirectory}{$"{oldName}"}\Commands", $@"{Program.BotsDirectory}{newName}\Commands");
                        Directory.Delete($"{Program.BotsDirectory}{$"{oldName}"}");
                        Logger.Log(new EditWizard(), "Bot directory renamed successfully.", LogLevel.SUCCESS);
                        
                        Logger.Log(new EditWizard(), "Step 2: Writing changes into database...", LogLevel.INFO);
                        SQLiteCommand newNameCommand = sqlite.ConstructCommand("UPDATE Bots SET BotName=$name WHERE Id=$id");
                        newNameCommand.Parameters.AddWithValue("$name", newName);
                        newNameCommand.Parameters.AddWithValue("$id", id);
                        newNameCommand.ExecuteNonQuery();
                        Logger.Log(new EditWizard(), "Successfully set your new name for your bot! Exiting...", LogLevel.SUCCESS);

                        return true;
                    case "token":
                        string newToken = "";
                        bool newTokenRead = true;

                        while (newTokenRead)
                        {
                            newToken = Program.ReadValue();

                            var tokenReg = new Regex("[0-9]{9}:[a-zA-Z0-9_-]{35}");

                            if (!tokenReg.IsMatch(newToken))
                                Logger.Log(new Bootstrap(), "Your bot token is invalid.", LogLevel.ERROR);
                            else newTokenRead = false;
                        }

                        SQLiteCommand newTokenCommand = sqlite.ConstructCommand("UPDATE Bots SET BotToken=$token WHERE Id=$id");
                        newTokenCommand.Parameters.AddWithValue("$token", newToken);
                        newTokenCommand.Parameters.AddWithValue("$id", id);
                        newTokenCommand.ExecuteNonQuery();
                        Logger.Log(new EditWizard(), "Successfully set your new token for your bot! Exiting...", LogLevel.SUCCESS);
                        newTokenRead = false;

                        return true;
                    case "exit":
                        sqlite = null;
                        id = 0;
                        oldName = null;
                        newName = null;
                        newToken = null;

                        Logger.Log(new EditWizard(), "Exiting...", LogLevel.INFO);
                        return true;
                    default:
                        Logger.Log(new EditWizard(), "Invalid parameter.", LogLevel.ERROR);
                        break;
                }
            }
            return true;
        }

        public override string ToString() { return "EditWizard"; }
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
