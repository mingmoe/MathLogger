using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLogger;

/// <summary>
/// 独家间隔重复算法
/// </summary>
public static class MemoryAlgorithm
{
    public static DateOnly CalculateNextReviewDay(Problem problem)
    {
        // 新卡片
        if (problem.ReviewHistory.Count == 0)
        {
            return problem.CreateDate.AddDays(1);
        }

        // 老卡片
        var history = problem.ReviewHistory;
        var lastReview = history[^1];
        var lastReviewDate = lastReview.ReviewDate;

        if (lastReview.Ease == Ease.Again)
        {
            return lastReviewDate.AddDays(1);
        }

        if (lastReview.Ease == Ease.Hard)
        {
            return lastReviewDate.AddDays(3);
        }

        Debug.Assert(lastReview.Ease == Ease.Easy);
        // 检查历史
        // 连续两次Easy，14天
        if (history.Count >= 2 && history[^2].Ease == Ease.Easy)
        {
            return lastReviewDate.AddDays(14);
        }

        // 连续三次或以上Easy，28天
        if (history.Count >= 3 && history[^2].Ease == Ease.Easy && history[^3].Ease == Ease.Easy)
        {
            return lastReviewDate.AddDays(28);
        }

        // 只有一次Easy，7天
        return lastReviewDate.AddDays(7);
    }

}