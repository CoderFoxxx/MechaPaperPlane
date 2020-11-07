/*
 *
 * Logger.cs
 * Copyright 2020, CoderFoxxx (https://github.com/CoderFoxxx)
 *
 * This code is licensed under the MIT license.
 * You can find license after the code.
 *
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace MechaPaperPlane
{
    class Logger
    {
        public static void Log(object clazz, string message, LogLevel logLevel)
        {
            ConsoleColor consoleColor = ConsoleColor.Gray;

            switch(logLevel)
            {
                case LogLevel.ERROR: consoleColor = ConsoleColor.Red; break;
                case LogLevel.INFO: consoleColor = ConsoleColor.Gray; break;
                case LogLevel.SUCCESS: consoleColor = ConsoleColor.Green; break;
                case LogLevel.WARN: consoleColor = ConsoleColor.Yellow; break;
                case LogLevel.REQUEST: consoleColor = ConsoleColor.Magenta; break;
            }

            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"[{DateTime.Now}] [{logLevel}] [{clazz.ToString()}] {message}");
        }
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
