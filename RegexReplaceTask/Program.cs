﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using dotnetCampus.Cli;

namespace dotnetCampus.RegexReplaceTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();
            if (string.IsNullOrEmpty(options.FilePath) || !File.Exists(options.FilePath))
            {
                throw new FileNotFoundException($"Can not find {options.FilePath}.FullPath={Path.GetFullPath(options.FilePath)}");
            }

            var text = File.ReadAllText(options.FilePath);
            var matchOutputText = Regex.Match(text, options.ReplaceRegex);

            if (!matchOutputText.Success)
            {
                throw new ArgumentException(
                    $"Can not match regex={options.ReplaceRegex} in OutputFile={options.FilePath} \r\n The file content is\r\n {text}");
            }

            var replaceText = matchOutputText.Groups[0].Value.Replace(matchOutputText.Groups[1].Value, options.Value);
            Console.WriteLine($"Replace {matchOutputText.Groups[0].Value} to {replaceText}");
            text = text.Replace(matchOutputText.Groups[0].Value, replaceText);
            File.WriteAllText(options.FilePath, text);
        }

        class Options
        {
            /// <summary>
            /// 将替换为的内容
            /// </summary>
            [Option('v', "Value")]
            [NotNull]
            public string? Value { set; get; }

            /// <summary>
            /// 用于替换的正则表达式，其中 match.Groups[1].Value 将被替换为 <see cref="Value"/> 的值
            /// </summary>
            [Option('r', "Regex")]
            [NotNull]
            public string? ReplaceRegex { set; get; }

            /// <summary>
            /// 替换内容的文件，要求文件一定存在
            /// </summary>
            [Option('f', "File")]
            [NotNull]
            public string? FilePath { set; get; }
        }
    }
}
