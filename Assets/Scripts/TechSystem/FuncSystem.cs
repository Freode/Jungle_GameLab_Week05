public static class FuncSystem
{
    // 숫자 형식 변경
    public static string Format(double number)
    {
        return number switch
        {
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
}
