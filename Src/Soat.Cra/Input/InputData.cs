namespace Soat.Cra.Input
{
    public class InputData
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public InputData(int month, int year)
        {
            Month = month;
            Year = year;
        }
    }
}
