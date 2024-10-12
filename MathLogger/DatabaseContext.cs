using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MathLogger;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Database))]
internal partial class DatabaseContext : JsonSerializerContext
{
}
