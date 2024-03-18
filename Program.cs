using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

class Program
{
    static string[] palabrasReservadas = {
        "False", "class", "from", "or",
        "None", "continue", "global", "pass",
        "True", "def", "if", "raise",
        "and", "del", "import", "return",
        "as", "elif", "in", "try",
        "assert", "else", "is", "while",
        "async", "except", "lambda", "with",
        "await", "finally", "nonlocal", "yield",
        "break", "for", "not","print"
    };

    static string[] operadores = {
        "+", "-", "*", "/", "%", "**", "//",
        ">", "<", "==", ">=", "<=", "!=",
        "&", "|", "^", "~", ">>", "<<",
        "=", "+=", "-=", "*=", "/=", "%=", "**=", "//=", "&=", "|=", "^=", ">>=", "<<=",
    };

    static string[] simbolos = {
        "(", ")", "{", "}", "[", "]", ":", "$", "!", ".", ",", "\t"
    };

    static List<string> arrRev;

    static void Main(string[] args)
    {
        arrRev = operadores.Concat(simbolos).ToList();
        arrRev.Sort((x, y) => y.Length.CompareTo(x.Length));

        string fileName = @"H:\Escritorio\Proyectos\compiladores\ejem.txt"; // Ruta del archivo
        string content = ReadFile(fileName);

        if (content != null)
        {
            string jsonString = AnalyzeLex(content);
            Console.WriteLine(jsonString);
        }
    }

    static string ReadFile(string fileName)
    {
        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File '{fileName}' not found.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return null;
        }
    }

    static string AnalyzeLex(string text)
    {
        List<Token> categorization = new List<Token>();

        foreach (string token in TokenizationRec(text))
        {
            categorization.Add(CategorizacionToken(token.Trim()));
        }

        return JsonConvert.SerializeObject(categorization, Formatting.Indented);
    }

    static IEnumerable<string> TokenizationRec(string text)
    {
        string pattern = @"\b\w+\b|[\+\-\*\/\%\>\<\&\|\^\(\)\{\}\[\]\:\$\!\.\,\t]";
        return Regex.Matches(text, pattern).Cast<Match>().Select(m => m.Value);
    }

    static Token CategorizacionToken(string token)
    {
        if (palabrasReservadas.Contains(token))
        {
            return new Token { Text = token, Category = "Palabra Reservada" };
        }
        else if (operadores.Contains(token))
        {
            return new Token { Text = token, Category = "Operador" };
        }
        else if (simbolos.Contains(token))
        {
            return new Token { Text = token, Category = "Simbolo" };
        }
        else if (IsNumberConstant(token))
        {
            return new Token { Text = token, Category = "Numero" };
        }
        else if (IsStringConstant(token))
        {
            return new Token { Text = token, Category = "String" };
        }
        else if (IsValidIdentifier(token))
        {
            return new Token { Text = token, Category = "Identificador" };
        }
        else
        {
            return new Token { Text = token, Category = "Error" };
        }
    }

    static bool IsNumberConstant(string token)
    {
        double result;
        return double.TryParse(token, out result);
    }

    static bool IsStringConstant(string token)
    {
        return token.StartsWith("\"") && token.EndsWith("\"") || token.StartsWith("'") && token.EndsWith("'");
    }

    static bool IsValidIdentifier(string token)
    {
        Regex regex = new Regex(@"^([a-zA-Z_]|_[a-zA-Z])[a-zA-Z0-9_]*$");
        return regex.IsMatch(token);
    }
}

class Token
{
    public string Text { get; set; }
    public string Category { get; set; }
}