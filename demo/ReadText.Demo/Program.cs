using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace ReadText.Demo
{
	class Program
	{
		public static int Main(string[] args)
		{
		    Func<IOptions, string> reader = opts =>
		        {
		            var fromTop = opts.GetType() == typeof(HeadOptions);
		            return opts.Lines.HasValue
		                ? ReadLines(opts.FileName, fromTop, (int)opts.Lines)
		                : ReadBytes(opts.FileName, fromTop, (int)opts.Bytes);
		        };
		    Func<IOptions, string> header = opts =>
		    {
		        if (opts.Quiet)
		        {
		            return string.Empty;
		        }
                var fromTop = opts.GetType() == typeof(HeadOptions);
		        var builder = new StringBuilder("Reading ");
		        builder = opts.Lines.HasValue
		            ? builder.Append(opts.Lines).Append(" lines")
		            : builder.Append(opts.Bytes).Append(" bytes");
		        builder = fromTop ? builder.Append(" from top") : builder.Append(" from bottom:");
		        return builder.ToString();
		    };

            var result = Parser.Default.ParseArguments<HeadOptions, TailOptions>(args);
            var texts = result
                .Return(
                    (HeadOptions opts) => Tuple.Create(header(opts), reader(opts)),
                    (TailOptions opts) => Tuple.Create(header(opts), reader(opts)),
                    _ => Tuple.Create(string.Empty, string.Empty));

		    if (texts.Item1.Length > 0)
		    {
                Console.WriteLine(texts.Item1);
		    }
            Console.WriteLine(texts.Item2);

		    return 0;
		}

	    private static string ReadLines(string fileName, bool fromTop, int count)
	    {
	        var lines = File.ReadAllLines(fileName);
	        if (fromTop)
	        {
	            return string.Join(Environment.NewLine, lines.Take(count));
	        }
	        return string.Join(Environment.NewLine, lines.Reverse().Take(count));
	    }

        private static string ReadBytes(string fileName, bool fromTop, int count)
	    {
	        var bytes = File.ReadAllBytes(fileName);
	        if (fromTop)
	        {
	            return Encoding.UTF8.GetString(bytes, 0, count);
	        }
            return Encoding.UTF8.GetString(bytes, bytes.Length - count, count);
	    }
	}
}