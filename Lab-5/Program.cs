using System.Text.RegularExpressions;
using static System.String;


namespace Lab_5
{
    internal static partial class Program
    {
        private const int LowerRandomBound = 10;
        private const int UpperRandomBound = 100;
        private const string TestString = "В лесу родилась елочка! В лесу она росла. " +
                                          "Зимой и летом стройная, зеленая была! В траве сидел кузнечик! " +
                                          "Кузнечик не трогал козявок и дружил с мухом.";

        public static void Main()
        {
            var isRunning = true;
            var option = 0;
            while (isRunning)
            {
                var options = new[]
                {
                    "0. Выход.",
                    "1. Работа с двумерным массивом.",
                    "2. Работа с рваным массивом",
                    "3. Работа со строкой",
                };
                option = Menu(option, options);
                switch (option)
                {
                    case 0:
                        isRunning = false;
                        break;
                    case 1:
                        MatrixRun();
                        break;
                    case 2:
                        JaggedRun();
                        break;
                    case 3:
                        StringRun();
                        break;
                }
            }
            Console.CursorVisible = true;
        }
        private static void MatrixRun()
        {
            var columns = InputLimited("Введите количество столбцов: ", 1, 15);
            var rows = InputLimited("Введите количество строк: ", 1, 15);
            var isRandom = Menu(0, ["0. Ручной ввод.", "1. ДСЧ."]) == 1;
            var matrix = CreateMatrix(rows, columns, isRandom);
            var matrixString = WriteMatrix(matrix);
            var isRunning = true;
            var isPrinted = false;
            var highlightedMessage = "";
            var option = 0;
            while (isRunning)
            {
                var options = new[] { "0. Назад.", "1. Удалить столбец с номером K.", isPrinted ? "2. Скрыть матрицу" : "2. Показать матрицу." };
                option = Menu(option, options, highlightedMessage, isPrinted ? $"\n{matrixString}\n" : "");
                switch (option)
                {
                    case 0:
                        isRunning = false;
                        break;
                    case 1:
                        if (IsEmpty(matrix))
                        {
                            Console.WriteLine("Массив пуст. Нажмите любую кнопку, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }
                        var column = InputLimited($"Какую колонну удаляем?\n{matrixString}", 1, matrix.GetLength(1)) - 1;
                        matrix = DeleteColumn(matrix, column);
                        matrixString = WriteMatrix(matrix);
                        break;
                    case 2:
                        isPrinted = !isPrinted;
                        break;
                }
            }
        }

        private static void JaggedRun()
        {
            var rows = InputLimited("Введите количество строк: ", 1, 15);
            var jagged = CreateJagged(rows);
            var jaggedString = WriteJagged(jagged);
            var isRunning = true;
            var isPrinted = false;
            var highlightedMessage = "";
            var option = 0;
            while (isRunning)
            {
                var options = new[] { "0. Назад.", "1. Добавить K строк в конец.", isPrinted ? "2. Скрыть массив." : "2. Показать массив." };
                option = Menu(option, options, highlightedMessage, isPrinted ? $"\n{jaggedString}" : "");
                switch (option)
                {
                    case 0:
                        isRunning = false;
                        break;
                    case 1:
                        var newRows = InputLimited("Введите количество новых строк: ", 1, 15);
                        var additionalJagged = new int[newRows][];
                        var isRandom = Menu(0, ["0. Ручной ввод.", "1. ДСЧ."]) == 1;
                        for (int i = 0; i < newRows; i++)
                            additionalJagged[i] = FillRow(InputLimited($"Длина {i + jagged.Length + 1}-й строки > ", 1, 15), isRandom);
                        jagged = AddRows(jagged, additionalJagged);
                        jaggedString = WriteJagged(jagged);
                        break;
                    case 2:
                        isPrinted = !isPrinted;
                        break;
                }
            }
        }

        private static void StringRun()
        {
            string? text;
            var options = new[] { "0. Тестовая строка", "1. Ручной ввод" };
            var isManual = Menu(0, options);
            if (isManual == 1)
            {
                Console.WriteLine("Введите текст, состоящий из предложений, оканчивающихся завершающим знаком препинания и соответствующий письменым нормам.");
                Console.CursorVisible = true;
                text = Console.ReadLine();
            }
            else
                text = TestString;
            if (text is null)
            {
                Console.WriteLine("Something went wrong. String is null.");
                Console.ReadKey();
                return;
            }
            Console.Clear();
            var banned = new[] { "??", "!!", "..", ",,", ";;", "::", "(", ")" };
            if (banned.Any(word => text.Contains(word)))
            {
                Console.WriteLine("Недопустима повторяющаяся пунктуация (\"..\", \",,\"\"!!\" и т.п.) и скобки.\nПопробуйте снова...");
                Console.ReadKey();
                return;
            }
            if (!".!?".Contains(text[^1]))
            {
                Console.WriteLine("Строка должна оканчиваться \".?!\".\nПопробуйте снова...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Исходная строка:\n" + text);
            text = RemoveFirstAndLastWords(text);
            text = text == "" ? "Пустая строка." : text;
            Console.WriteLine($"\nРезультат:\n{text}");
 
            Console.WriteLine("\nНажмите любую кнопку, чтобы продолжить...");
            Console.ReadKey();
        }

        private static int Input(string message)
        {
            int number;
            Console.Write(message);
            while (!int.TryParse(Console.ReadLine(), out number))
                Console.Write($"Ошибка! Введите целое число.\n{message}");
            return number;
        }

        private static int InputLimited(string message, int lowerBound = int.MinValue, int upperBound = int.MaxValue)
        {
            int input;
            do
            {
                input = Input(message);
                if (input < lowerBound || input > upperBound)
                    Console.WriteLine($"Ошибка! Число должно быть не меньше {lowerBound} и не больше {upperBound}!");
            } while (input < lowerBound || input > upperBound);
            return input;
        }

        private static void PrintHighlighted(string message)
        {
            var highlight = 0;
            foreach (var sign in message)
            {
                if (sign == '~')
                    highlight = highlight == 1 ? 0 : 1;
                else if (sign == '#')
                    highlight = highlight == 2 ? 0 : 2;
                else if (sign == '^')
                    highlight = highlight == 3 ? 0 : 3;
                else if (highlight == 1)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(sign);
                    Console.ResetColor();
                }
                else if (highlight == 2)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(sign);
                    Console.ResetColor();
                }
                else if (highlight == 3)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(sign);
                    Console.ResetColor();
                }
                else
                    Console.Write(sign);

            }
        }

        private static int Menu(int option, string[] options, string highlightedMessage = "", string array = "")
        {
            Console.CursorVisible = false;
            var isRunning = true;
            var length = options.Length;
            while (isRunning)
            {
                Console.Clear();
                for (var i = 0; i < length; i++)
                {
                    Console.ResetColor();
                    if (i == option)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(options[i]);
                        Console.ResetColor();
                    }
                    else
                        Console.WriteLine(options[i]);
                }

                if (array != "")
                {
                    foreach (var symbol in array)
                    {
                        Console.Write(symbol);
                    }
                }

                if (highlightedMessage != "")
                    PrintHighlighted(highlightedMessage);

                var key = Console.ReadKey();
                ConsoleKey[] keys = [ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9];
                if (key.Key == ConsoleKey.UpArrow)
                    option = (option - 1 + length) % length;
                else if (key.Key == ConsoleKey.DownArrow)
                    option = (option + 1) % length;
                else if (key.Key == ConsoleKey.Enter)
                    isRunning = false;
                else
                {
                    for (var i = 0; i < (length < keys.Length ? length : keys.Length); i++)
                    {
                        if (key.Key == keys[i])
                        {
                            option = i;
                            break;
                        }
                    }
                }
                Console.ResetColor();
                Console.Clear();
            }
            return option;
        }

        private static int[,] CreateMatrix(int rows, int columns, bool isRandom)
        {
            var matrix = new int[rows, columns];
            if (isRandom)
            {
                var random = new Random();
                for (int i = 0; i < columns * rows; i++)
                    matrix[i / columns, i % columns] = random.Next(LowerRandomBound, UpperRandomBound);
            }
            else
                for (int i = 0; i < columns * rows; i++)
                    matrix[i / columns, i % columns] = Input($"[{i / columns + 1}, {i % columns + 1}] ? ");
            return matrix;
        }

        private static int[][] CreateJagged(int rows)
        {
            var jagged = new int[rows][];
            var isRandom = Menu(0, ["0. Ручной ввод.", "1. ДСЧ."]) == 1;
            for (int i = 0; i < rows; i++)
                jagged[i] = FillRow(InputLimited($"Длина {i + 1}-й строки > ", 1, 15), isRandom);
            return jagged;
        }

        private static int[] FillRow(int length, bool isRandom)
        {
            var row = new int[length];
            if (isRandom)
            {
                var random = new Random();
                for (int i = 0; i < length; i++)
                    row[i] = random.Next(LowerRandomBound, UpperRandomBound);
            }
            else
            {
                for (var i = 0; i < length; i++)
                    row[i] = Input($"{i + 1} > ");
            }
            return row;
        }

        private static string WriteMatrix(int[,] matrix)
        {
            if (IsEmpty(matrix))
                return "Тю-тю. Нету массива.";
            var matrixString = "";
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                    matrixString += $"{matrix[i, j]}\t";
                matrixString += '\n';
            }
            return matrixString;
        }

        private static string WriteJagged(int[][] jagged)
        {
            var jaggedString = "";
            var rowsNumber = jagged.Length;
            for (var i = 0; i < rowsNumber; i++)
            {
                for (var j = 0; j < jagged[i].Length; j++)
                    jaggedString += $"{jagged[i][j]}\t";
                jaggedString += '\n';
            }
            return jaggedString;
        }

        private static bool IsEmpty(int[,] matrix)
        {
            return matrix.GetLength(0) == 0 || matrix.GetLength(1) == 0;
        }

        private static int[,] DeleteColumn(int[,] matrix, int column)
        {
            if (IsEmpty(matrix))
                return matrix;
            var temporalMatrix = new int[matrix.GetLength(0), matrix.GetLength(1) - 1];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0, k = 0; j < matrix.GetLength(1); j++)
                {
                    if (j == column)
                        k++;
                    else
                        temporalMatrix[i, j - k] = matrix[i, j];
                }
            }
            return temporalMatrix;
        }

        private static int[][] AddRows(int[][] jagged, int[][] rows)
        {
            var temporalJagged = new int[jagged.Length + rows.Length][];
            for (int i = 0; i < jagged.Length; i++)
                temporalJagged[i] = jagged[i];
            for (int i = jagged.Length, j = 0; i < temporalJagged.Length; i++)
            {
                temporalJagged[i] = rows[j];
                j++;
            }
            return temporalJagged;
        }

        private static string? RemoveFirstAndLastWords(string? text)
        {
            if (text == null)
                return null;
            var sentences = SentencePattern().Split(text);
            for (int i = 0; i < sentences.Length; i++)
            {
                var sentence = sentences[i].Trim();
                var matches = WordAndPunctuationPattern().Matches(sentence);
                var wordsAndPunctuation = matches.Select(m => m.Value).ToList();
                var words = wordsAndPunctuation.Where(w => WordPattern().IsMatch(w)).ToList();

                if (words.Count <= 2)
                {
                    sentences[i] = "";
                    continue;
                }
                
                var firstWordIndex = wordsAndPunctuation.FindIndex(w => WordPattern().IsMatch(w));
                var lastWordIndex = wordsAndPunctuation.FindLastIndex(w => WordPattern().IsMatch(w));
                wordsAndPunctuation.RemoveAt(lastWordIndex);
                wordsAndPunctuation.RemoveAt(firstWordIndex);
                var processedSentence = ReconstructSentence(wordsAndPunctuation);
                processedSentence = HangingPunctuationPattern().Replace(processedSentence, "").Trim();
                processedSentence = char.ToUpper(processedSentence[0]) + processedSentence[1..];

                sentences[i] = processedSentence;
            }
            return Join(" ", sentences).Trim();
        }

        private static string ReconstructSentence(List<string> wordsAndPunctuation)
        {
            var result = "";
            for (int i = 0; i < wordsAndPunctuation.Count; i++)
            {
                var current = wordsAndPunctuation[i];
                if (i > 0 && WordPattern().IsMatch(current))
                    result += " ";
                result += current;
            }
            return result.Trim();
        }

        [GeneratedRegex(@"\b\w+[\w\-']*\b")]
        private static partial Regex WordPattern();

        [GeneratedRegex(@"(?<=[.!?])\s+")]
        private static partial Regex SentencePattern();

        [GeneratedRegex(@"\b\w+[\w\-']*\b|[:;.,!?]")]
        private static partial Regex WordAndPunctuationPattern();

        [GeneratedRegex(@"^[;:,]|[;:,]+(?=[.!?])")]
        private static partial Regex HangingPunctuationPattern();
    }
}