using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MathLogger;

/// <summary>
/// 问题
/// </summary>
public class Problem
{
    [JsonInclude]
    public required DateOnly CreateDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [JsonInclude]
    public required string TheProblem { get; set; }

    [JsonInclude]
    public required string Description { get; set; }

    [JsonInclude]
    public required string Solution { get; set; }

    [JsonInclude]
    public List<ReviewRecord> ReviewHistory { get; set; } = new();

    /// <summary>
    /// 下次复习日期。实时计算的。null表示还没计算。
    /// </summary>
    [JsonIgnore]
    public DateOnly? NextReviewDate { get; set; } = null;

    public Table GetReviewHistoryTable()
    {
        var table = new Table();

        table.AddColumn("[blue]ReviewDate[/]");
        table.AddColumn("Ease");

        foreach (var item in ReviewHistory)
        {
            string ease = "";
            if (item.Ease == Ease.Again)
            {
                ease = "[red]Again[/]";
            }
            else if (item.Ease == Ease.Hard)
            {
                ease = "[yellow]Again[/]";
            }
            else
            {
                ease = "[lime]Easy[/]";
            }
            table.AddRow(Markup.Escape(item.ReviewDate.ToReadable()), ease);
        }

        return table;
    }
}
