using Colorful;
using Soat.Cra.Interfaces;
using System;
using System.Drawing;
using Console = Colorful.Console;

namespace Soat.Cra.Input
{
    public class InputReader : IInputReader
    {
        public InputData Read()
        {
            var defaultInput = new InputData(DateTime.Today.Month, DateTime.Today.Year);
            var input = new InputData(DateTime.Today.Month, DateTime.Today.Year);

            do
            {
                Console.WriteFormatted("Month to be processed (1-12) [{0}] ", Color.White, new Formatter(defaultInput.Month, Color.Yellow));

                try
                {
                    input.Month = int.Parse(Console.ReadLine());
                }
                catch { }

            } while (input.Month <= 0 || input.Month >= 13);

            do
            {
                Console.WriteFormatted("Year to be processed [{0}] ", Color.White, new Formatter(defaultInput.Year, Color.Yellow));

                try
                {
                    input.Year = int.Parse(Console.ReadLine());
                }
                catch { }

            } while (input.Year <= 2010 || input.Year > defaultInput.Year);

            return input;
        }
    }
}
