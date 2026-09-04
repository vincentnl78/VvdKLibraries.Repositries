namespace Auth.Objects;

public enum ApiAreas
{
    E1,
    E2,
    E3,
    E4,
    E5,
    E6,
    A1,
    A2,
    A3,
    R1,
    R2,
    R3
}

public static class UserConversions
{
    public static string Region(string id)
    {
        return id[^2..].ToUpper();
    }

    public static ApiAreas GetApiAreaFromId(string id)
    {
        if (id.Length < 2)
            return ApiAreas.E1;
        if (Enum.TryParse<ApiAreas>(id[^2..].ToUpper(), out var area)) return area;

        return ApiAreas.E1;
    }

    public static string GenerateId(ApiAreas area)
    {
        return Guid.NewGuid() + "-" + area.ToString().ToLower();
    }
}