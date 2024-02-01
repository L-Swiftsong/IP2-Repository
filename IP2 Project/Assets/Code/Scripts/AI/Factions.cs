[System.Flags]
public enum Factions
{
    Unaligned = 0,
    Player = 1 << 0,
    Yakuza = 1 << 1,
    TestFaction = 1 << 2,
}

public static class FactionsExtensions
{
    public static bool IsAlly(this Factions thisFaction, Factions testFaction)
    {
        // Unaligned Entities are enemies to all.
        if (testFaction == Factions.Unaligned)
            return false;

        // Compare flags.
        return thisFaction.HasFlag(testFaction);
    }
}