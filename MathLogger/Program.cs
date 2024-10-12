using Spectre.Console;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace MathLogger;

internal class Program
{
    public const string DatabaseFileName = "MathLoggerDatabase.json";

    static void SaveDatabase(Database database)
    {
        var json = Json.ToJson(database);

        File.WriteAllText(DatabaseFileName, json, Encoding.UTF8);
    }

    static Database LoadDatabase()
    {
        if (!File.Exists(DatabaseFileName))
        {
            AnsiConsole.MarkupLine($"[underline green]Not found database({DatabaseFileName}).Create new one.[/]");
            Database database = new();
            SaveDatabase(database);
        }

        var file = File.ReadAllText(DatabaseFileName, Encoding.UTF8);
        return Json.FromJson(file);
    }

    static void CommitToGit()
    {
        Process.Start("git", ["add", DatabaseFileName]).WaitForExit();
        Process.Start("git", ["commit", "-m", "update database"]).WaitForExit();
    }

    static void GitPush()
    {
        Process.Start("git", "push").WaitForExit();
    }

    static string ReadLine(string prompt, string? defaultValue = null)
    {
        AnsiConsole.Markup($"[underline yellow]{prompt} [/]");

        var read = Console.ReadLine();

        while (true)
        {
            if (string.IsNullOrWhiteSpace(read) && defaultValue != null)
            {
                return defaultValue;
            }
            if (!string.IsNullOrEmpty(read))
            {
                break;
            }

            read = Console.ReadLine();
        }
        Console.WriteLine();

        return read.Trim();
    }

    static void Main(string[] args)
    {
        if (args.Length != 0)
        {
            AnsiConsole.MarkupLine($"[underline yellow]Ignore any arguments![/]");
        }

        var database = LoadDatabase();
        AnsiConsole.MarkupLine($"[green]The database was loaded.[/]");

        // 计算间隔时间
        foreach (var unit in database.Units)
        {
            foreach (var problem in unit.ProblemSet)
            {
                problem.NextReviewDate = MemoryAlgorithm.CalculateNextReviewDay(problem);
            }
        }
        AnsiConsole.MarkupLine($"[green]The unique spaced-repetition algorithm was applied.[/]");

        while (true)
        {
            var command = ReadLine(">>").Trim();

            if (string.IsNullOrEmpty(command))
            {
                continue;
            }
            else if (command == "save")
            {
                SaveDatabase(database);
                continue;
            }
            else if (command == "clear")
            {
                Console.Clear();
                continue;
            }
            else if (command == "exit")
            {
                break;
            }
            else if (command == "not-save")
            {
                Environment.Exit(0);
            }

            var commands = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            command = commands[0];

            if (command == "list-unit")
            {
                Table table = new();
                table.AddColumns("Name", "Description");
                foreach (var unit in database.Units)
                {
                    table.AddRow(unit.Name, unit.Description);
                }

                table.Expand();

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
            }
            else if (command == "print-unit")
            {
                var unit = askUnit();

                if (unit == null)
                {
                    continue;
                }

                AnsiConsole.Write(unit.GetUnitTable());
                AnsiConsole.WriteLine();
            }
            else if (command == "new-unit")
            {
                var name = ReadLine("What's the name?");
                var description = ReadLine("What's the description?");

                if (database.Units.Any(u => u.Name == name))
                {
                    AnsiConsole.MarkupLine($"[underline red]The unit name({name}) already exists.[/]");
                    continue;
                }
                var unit = new Unit() { Name = name, Description = description };

                database.Units.Add(unit);
            }
            else if (command == "new-problem")
            {
                var unit = askUnit();

                if (unit == null)
                {
                    continue;
                }

                var theProblem = ReadLine("What's the problem?");
                var description = ReadLine("What's the description?");
                var solution = ReadLine("What's the solution?");
                var createDate = Utilities.ParseHuman(
                    ReadLine("(default today)What's the create date?",
                        DateOnly.FromDateTime(DateTime.Now).ToReadable()));

                if (unit.ProblemSet.Any(p => p.TheProblem == theProblem))
                {
                    AnsiConsole.MarkupLine($"[underline red]The problem({theProblem}) already exists.[/]");
                    continue;
                }

                var problem = new Problem()
                {
                    CreateDate = createDate,
                    TheProblem = theProblem,
                    Description = description,
                    Solution = solution,
                };

                problem.NextReviewDate = MemoryAlgorithm.CalculateNextReviewDay(problem);

                unit.ProblemSet.Add(problem);
            }
            else if (command == "get-todo")
            {
                var dueProblems = getTwoDueProblems();

                // get two problems
                Table table = new();
                table.AddColumns("Unit", "Thr problem", "Description", "Solution");
                foreach (var problem in dueProblems)
                {
                    table.AddRow(problem.Item1.Name,
                        problem.Item2.TheProblem,
                        problem.Item2.Description,
                        problem.Item2.Solution);
                }

                table.Expand();

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
            }
            else if (command == "answer-todo")
            {
                var dueProblems = getTwoDueProblems();

                foreach (var due in dueProblems)
                {
                    Table table = new();
                    table.AddColumns("Unit", "Thr problem", "Description", "Solution");
                    table.AddRow(due.Item1.Name,
                        due.Item2.TheProblem,
                        due.Item2.Description,
                        due.Item2.Solution);
                    table.Expand();
                    AnsiConsole.Write(table);
                    AnsiConsole.WriteLine();

                    var ease = AnsiConsole.Prompt(
                        new TextPrompt<string>("What's the result?")
                        .AddChoices([Ease.Again.ToString(), Ease.Hard.ToString(), Ease.Easy.ToString()]));

                    due.Item2.ReviewHistory.Add(new() { Ease = Enum.Parse<Ease>(ease), ReviewDate = DateOnly.FromDateTime(DateTime.Now) });
                    due.Item2.NextReviewDate = MemoryAlgorithm.CalculateNextReviewDay(due.Item2);
                }

            }
            else if (command == "help")
            {
                AnsiConsole.MarkupLine("help save clear exit not-save list-unit print-unit new-unit new-problem get-todo answer-todo");
            }
            else
            {
                AnsiConsole.MarkupLine($"[underline red]Unknown command:{command}[/]");
            }
        }

        SaveDatabase(database);
        AnsiConsole.MarkupLine($"[green]The database was saved.[/]");
        CommitToGit();
        AnsiConsole.MarkupLine($"[green]The git repository was committed.[/]");
        GitPush();
        AnsiConsole.MarkupLine($"[green]The git repository was pushed to the remote.[/]");

        Unit? askUnit()
        {
            var name = ReadLine("What's the unit name?");

            var unit = database.Units.Find(u => u.Name == name);

            if (unit == null)
            {
                AnsiConsole.MarkupLine($"[underline red]Unknown unit:{name}[/]");
                return null;
            }

            return unit;
        }
        List<(Unit, Problem)> getTwoDueProblems()
        {
            var current = DateOnly.FromDateTime(DateTime.Now);
            List<(Unit, Problem)> dueProblems = [];

            foreach (var unit in database.Units)
            {
                foreach (var problem in unit.ProblemSet)
                {
                    if (problem.NextReviewDate <= current)
                    {
                        dueProblems.Add(new(unit, problem));
                    }
                }
            }
            dueProblems.Sort(ProblemCompare);
            List<(Unit, Problem)> result = new();

            if (dueProblems.Count >= 1)
            {
                result.Add(dueProblems[^1]);
            }
            if (dueProblems.Count >= 2)
            {
                result.Add(dueProblems[^2]);
            }

            return result;
        }
        int ProblemCompare((Unit, Problem) a, (Unit, Problem) b)
        {
            if (a.Item2.NextReviewDate == b.Item2.NextReviewDate)
            {
                // 比较名称，保证我们的值永远没有相等的情况
                // 此举是为了保证排序的稳定性
                return string.Compare(
                    a.Item1.Name + a.Item2.TheProblem,
                    b.Item1.Name + b.Item2.TheProblem);
            }
            if (a.Item2.NextReviewDate > b.Item2.NextReviewDate)
            {
                return -1;
            }
            if (a.Item2.NextReviewDate < b.Item2.NextReviewDate)
            {
                return 1;
            }
            throw new NotImplementedException("Unknown problem compare result.");
        }
    }
}
