namespace Soat.Cra.Interfaces
{
    public interface IPdfWatermarker
    {
        void AddWatermark(string input, string destination, string watermark);
    }
}