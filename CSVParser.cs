using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class CSVParser {
    private const char Sep = ',';
    private const char Quo = '"';

    private StringReader _reader;

    public CSVParser(string text) {
        _reader = new StringReader(text);
    }

    private int Read() {
        return _reader.Read();
    }

    private int Peek() {
        return _reader.Peek();
    }

    private bool IsEOR(int ch) {
        return ch == 10 || IsEOF(ch);
    }

    private bool IsEOF(int ch) {
        return ch == -1;
    }

    private void Start(List<string> fields) {
        while (true) {
            var ch = Read();
            if (IsEOR(ch)) {
                fields.Add("");
                break;
            }
            else if (ch == Sep) {
                fields.Add("");
            }
            else if (ch == Quo) {
                Quoted(fields);
                break;
            }
            else if (char.IsWhiteSpace((char)ch)) {
            }
            else {
                var sb = new StringBuilder();
                sb.Append((char)ch);
                Unquoted(sb, fields);
                break;
            }
        }
    }

    private void Unquoted(StringBuilder sb, List<string> fields) {
        while (true) {
            var ch = Read();
            if (IsEOR(ch)) {
                fields.Add(sb.ToString());
                break;
            }
            else if (ch == Sep) {
                fields.Add(sb.ToString());
                Start(fields);
                break;
            }
            else if (char.IsWhiteSpace((char)ch)) {
                sb.Append((char)ch);
            }
            else {
                sb.Append((char)ch);
            }
        }
    }

    private void Quoted(List<string> fields) {
        var sb = new StringBuilder();
        while (true) {
            var ch = Read();
            if (IsEOF(ch)) {
                throw new Exception();
            }
            else if (ch == Quo) {
                if (Peek() == Quo) {
                    Read();
                    sb.Append(Quo);
                }
                else {
                    fields.Add(sb.ToString());
                    QuotedTail(fields);
                    break;
                }
            }
            else {
                sb.Append((char)ch);
            }
        }
    }

    private void QuotedTail(List<string> fields) {
        while (true) {
            var ch = Read();
            if (IsEOR(ch)) {
                break;
            }
            else if (ch == Sep) {
                Start(fields);
                break;
            }
        }
    }

    public IEnumerable<string[]> Parse() {
        while (!IsEOF(Peek())) {
            var fields = new List<string>();
            Start(fields);
            yield return fields.Select(e => e.Trim()).ToArray();
        }
    }

    static void Test(string path) {
        string text = "";
        using (var reader = new StreamReader(path, Encoding.UTF8)) {
            text = reader.ReadToEnd();
        }

        var parser = new CSVParser(text);
        int n = 1;
        foreach (var ss in parser.Parse()) {
            Console.WriteLine("\n{0}:", n++);
            foreach (var s in ss) {
                Console.WriteLine("[[{0}]]", s);
            }
        }
    }

    public static void Main() {
        var path = "./test-csv2.csv";
        Test(path);
    }
}
