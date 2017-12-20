using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode2017.Model;

namespace AdventOfCode2017.Generator {
    
    public class SplashScreenGenerator {
        public string Generate(Calendar calendar) {
            string calendarPrinter = CalendarPrinter(calendar);
            return $@"
                |using System;
                |
                |namespace AdventOfCode2017 {{
                |
                |    class SplashScreen {{
                |
                |        public static void Show() {{
                |
                |            var color = Console.ForegroundColor;
                |            {calendarPrinter.Indent(12)}
                |            Console.ForegroundColor = color;
                |            Console.WriteLine();
                |        }}
                |
                |       private static void Write(ConsoleColor color, string text){{
                |           Console.ForegroundColor = color;
                |           Console.Write(text);
                |       }}
                |    }}
                |}}".StripMargin();
        }

        private string CalendarPrinter(Calendar calendar) {

            var theme = new Dictionary<string, string>() {
                ["calendar-edge"] = "ConsoleColor.Gray",
                ["calendar-star"] = "ConsoleColor.Yellow",
                ["calendar-mark-complete"] = "ConsoleColor.Yellow",
                ["calendar-mark-verycomplete"] = "ConsoleColor.Yellow",
                ["calendar-ornament0"] = "ConsoleColor.White",
                ["calendar-ornament1"] = "ConsoleColor.Green",
                ["calendar-ornament2"] = "ConsoleColor.Red",
                ["calendar-ornament3"] = "ConsoleColor.Blue",
                ["calendar-ornament4"] = "ConsoleColor.Magenta",
                ["calendar-ornament5"] = "ConsoleColor.Cyan",
                ["calendar-ornament3"] = "ConsoleColor.DarkCyan",
                ["title"] = "ConsoleColor.Yellow",
            };

            var lines = calendar.Lines.Select(line =>
                new[] { new CalendarToken { Text = "           " } }.Concat(line)).ToList();
            lines.Add(new []{new CalendarToken {
                Style = "title",
                Text = @"
                |   _      _             _          __    ___         _       ___ __  _ ____ 
                |  /_\  __| |_ _____ _ _| |_   ___ / _|  / __|___  __| |___  |_  )  \/ |__  |
                | / _ \/ _` \ V / -_) ' \  _| / _ \  _| | (__/ _ \/ _` / -_)  / / () | | / / 
                |/_/ \_\__,_|\_/\___|_||_\__| \___/_|    \___\___/\__,_\___| /___\__/|_|/_/  "
                .StripMargin()
            }});

            var bw = new BufferWriter();
            foreach (var line in lines) {
                foreach (var token in line) {
                    var consoleColor = token.Style != null && theme.TryGetValue(token.Style, out var themeColor)
                        ? themeColor
                        : "ConsoleColor.DarkGray";

                    bw.Write(consoleColor, token.Text);
                }

                bw.Write(null, "\n");
            }
            return bw.GetContent();
        }

        class BufferWriter {
            StringBuilder sb = new StringBuilder();
            string bufferColor = null;
            string buffer = "";

            public void Write(string color, string text) {
                if (!string.IsNullOrWhiteSpace(text)) {
                    if (color != bufferColor && !string.IsNullOrWhiteSpace(buffer)) {
                        Flush();
                    }
                    bufferColor = color;
                }
                buffer += text.Replace("\\", "\\\\").Replace("\n", "\\n");
            }

            private void Flush() {
                while(buffer.Length > 0){
                    var block = buffer.Substring(0, Math.Min(100, buffer.Length));
                    buffer = buffer.Substring(block.Length);
                    sb.AppendLine($@"Write({bufferColor}, ""{block}"");");
                }
                buffer = "";
            }

            public string GetContent() {
                Flush();
                return sb.ToString();
            }
        }
    }
}