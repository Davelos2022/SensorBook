
public static class CorrectnessData
{
    private static int _minPage = 2;

    public enum Cheeck
    {
        NotMinPage,
        NullAll,
        NullName,
        NotCover,
        Completed
    }

    public static Cheeck CheckData(int counPage, bool coverExits, string nameBook)
    {
        if (counPage < _minPage)
            return Cheeck.NotMinPage;
        else if (string.IsNullOrEmpty(nameBook) && !coverExits)
            return Cheeck.NullAll;
        else if (string.IsNullOrEmpty(nameBook) && coverExits)
            return Cheeck.NullName;
        else if (!string.IsNullOrEmpty(nameBook) && !coverExits)
            return Cheeck.NotCover;
        else
            return Cheeck.Completed;       
    }

}
