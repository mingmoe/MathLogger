using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MathLogger;
/// <summary>
/// 一条复习记录
/// </summary>
public class ReviewRecord
{
    [JsonInclude]
    public required DateOnly ReviewDate { get; set; }

    [JsonInclude]
    public required Ease Ease { get; set; }
}
