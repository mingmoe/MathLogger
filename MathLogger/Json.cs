using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MathLogger;
public static class Json
{

    public static Database FromJson(string input)
    {
        return JsonSerializer.Deserialize(input, DatabaseContext.Default.Database)!;
    }

    public static string ToJson(Database database)
    {
        return JsonSerializer.Serialize(database, DatabaseContext.Default.Database)!;
    }

}
