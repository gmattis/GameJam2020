public class Statistics
{
    public int Damages { get; set; } = 0;
    public float FireRate { get; set; } = 0;
    public int Life { get; set; } = 0;
    public float Speed { get; set; } = 0;

    public Statistics() { }

    public static Statistics operator +(Statistics stats1, Statistics stats2)
        => new Statistics
        {
            Damages = stats1.Damages + stats2.Damages,
            FireRate = stats1.FireRate + stats2.FireRate,
            Life = stats1.Life + stats2.Life,
            Speed = stats1.Speed + stats2.Speed
        };
}
