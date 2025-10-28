namespace Backend.Services
{
    public class ProcessDateInfo
    {
        public DateTime ConvertFormatToTime(string date)
        {
            string temp = date.Trim();

            if (temp.Contains("/") || temp.Contains("-"))
            {
                // Accepts formats like "DD/MM/YYYY" or "DD-MM-YYYY"
                var parts = temp.Split(new[] { '/', '-' });
                if (parts.Length != 3)
                    throw new FormatException();

                int day = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int year = int.Parse(parts[2]);
                return new DateTime(year, month, day);
            }
            else
            {
                // Accepts format like "DDMMYYYY"
                if (temp.Length != 8)
                    throw new FormatException();

                int day = int.Parse(temp.Substring(0, 2));
                int month = int.Parse(temp.Substring(2, 2));
                int year = int.Parse(temp.Substring(4, 4));
                return new DateTime(year, month, day);
            }
        }
    }
}
