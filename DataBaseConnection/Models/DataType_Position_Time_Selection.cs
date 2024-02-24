namespace DataBaseConnection.Models
{
	public class DataType_Position_Time_Selection
	{
		public Int32 Id { get; set; }

		public string SensorName { get; set; }

		public string DataType { get; set; }

		public string Position { get; set; }

		public string Value { get; set; }

		public string Date1 { get; set; }

		public string Date2 { get; set; }

		// Списки, где хранятся возможные типы данных
		private static List<string> available_DataType = new List<string>() { "Temperature", "Humidity", "Pressure", "Dust", "Vibration", "Incline", "Light" };

		public bool DataType_is_available()
		{
            for (int i = 0; i < available_DataType.Count; i++)
            {
                if (DataType == available_DataType[i])
				{  return true; }
            }
			return false;
        }

		public bool Position_is_available()
		{
			int result;
			bool success = int.TryParse(Position, out result);
			return success;
		}

		public bool Date1_is_available()
		{
			DateTime result;
			bool success = DateTime.TryParse(Date1, out result);
			return success;
		}

		public bool Date2_is_available()
		{
			DateTime result;
			bool success = DateTime.TryParse(Date2, out result);
			return success;
		}

		// Если некорректные параметры (sql инъекии)
		public void Check_availability()
		{
            if (!DataType_is_available())
            {
				DataType = available_DataType[0];
			}
            if (!Position_is_available())
            {
				Position = "1";
			}
            if (!Date1_is_available())
            {
				DateTime Date = new DateTime(2000, 4, 1);
				Date1 = Date.ToString();
			}
			if (!Date2_is_available())
			{
				DateTime Date = new DateTime();
				Date = DateTime.Now;
				Date2 = Date.ToString();
			}
        }

	}
}
