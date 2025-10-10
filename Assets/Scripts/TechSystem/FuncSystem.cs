public static class FuncSystem
{
    // 숫자 형식 변경
    public static string Format(decimal number)
    {
        return number switch
        {
            // 100경 (Quintillion) 이상
            >= 1_000_000_000_000_000_000 => (number / 1_000_000_000_000_000_000).ToString("F2") + "Qi",
            // 1000조 (Quadrillion) 이상
            >= 1_000_000_000_000_000 => (number / 1_000_000_000_000_000).ToString("F2") + "Qa",
            // 1조 (Trillion) 이상
            >= 1_000_000_000_000 => (number / 1_000_000_000_000).ToString("F2") + "T",
            // 10억 (Billion) 이상
            >= 1_000_000_000 => (number / 1_000_000_000).ToString("F2") + "B",
            // 100만 (Million) 이상
            >= 1_000_000 => (number / 1_000_000).ToString("F2") + "M",
            // 1천 (Kilo) 이상
            >= 1_000 => (number / 1_000).ToString("F2") + "K",
            // 1천 미만
            _ => ((long)number).ToString()
        };
    }

    // 랜덤으로 숫자 반환
    public static long RandomLongRange(long min, long max)
    {
        decimal dt = (decimal)UnityEngine.Random.value;
        return (long)(min + (max - min) * dt);
    }
}
