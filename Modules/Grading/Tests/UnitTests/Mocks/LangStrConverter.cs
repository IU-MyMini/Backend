using System.Text.Json;

using BuildingBlocks.Domain;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GradingModule.Tests.UnitTests.Mocks;

public class LangStrConverter() : ValueConverter<LangStr, string>(
    v => JsonSerializer.Serialize(v, null as JsonSerializerOptions),
    v => JsonSerializer.Deserialize<LangStr>(v, null as JsonSerializerOptions)!
);