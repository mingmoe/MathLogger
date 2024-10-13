using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MathLogger;

public class Unit
{
    [JsonInclude]
    public required string Name { get; set; }

    [JsonInclude]
    public required string Description { get; set; }

    [JsonInclude]
    public List<Problem> ProblemSet { get; set; } = new();

    public Table GetUnitTable()
    {
        var table = new Table();

        table.AddColumn("[aqua]CreateDate[/]");
        table.AddColumn("TheProblem");
        table.AddColumn("Description");
        table.AddColumn("Solution");
        table.AddColumn("[teal]NextReviewDate[/]");
        table.AddColumn("ReviewHistory");

        var current = DateOnly.FromDateTime(DateTime.Now);
        foreach (var problem in ProblemSet)
        {
            var dateStyle = "teal";
            if (problem.NextReviewDate == current)
            {
                dateStyle = "yellow";
            }
            else if (problem.NextReviewDate < current)
            {
                dateStyle = "red";
            }

            table.AddRow(
                new Markup($"[aqua]{Markup.Escape(problem.CreateDate.ToReadable())}[/]"),
                new Markup(Markup.Escape(problem.TheProblem)),
                new Markup(Markup.Escape(problem.Description)),
                new Markup(Markup.Escape(problem.Solution)),
                new Markup($"[{dateStyle}]{Markup.Escape(problem.NextReviewDate!.Value.ToReadable())}[/]"),
                problem.GetReviewHistoryTable());
        }

        table.Expand();

        return table;
    }
}
