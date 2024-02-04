using MySqlConnector;
using System.IO;
using Microsoft.Extensions.Configuration;



namespace DataBaseConnection.Models
{
    public class DataBaseModel
    {
        String connectionString;// = "Server=192.168.0.104;User ID=reader;Password=reader;Database=SENSOR_DB";

        // Перечисления, где хранятся возможные датчики и типы данных
        /*enum Sensors
        {
            BMP180,
			BMP280,
            DHT11,
            MQ-135,

		}*/



        public DataBaseModel()
        {
			//this.connectionString = Configuration["ConnectionStrings:Default"];
			//this.connectionString = configuration.GetConnectionString("DefaultConnection");

			var ConfBuilder = new ConfigurationBuilder();
			ConfBuilder.SetBasePath(Directory.GetCurrentDirectory());
			ConfBuilder.AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
			IConfiguration MyConfiguration = ConfBuilder.Build();
			this.connectionString = MyConfiguration.GetConnectionString("DefaultConnection");
		}

        public List<DataBaseItem> GetItems()
        {
            List<DataBaseItem> resultList = new List<DataBaseItem>();

            using (MySqlConnection connection = new MySqlConnection(this.connectionString))
            {
                connection.Open();

                String selectSqlCmd = "SELECT * FROM " + "SENSOR_DATA_TABLE";
                
                MySqlCommand cmd = new MySqlCommand(selectSqlCmd, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        DataBaseItem item = new DataBaseItem();

                        item.Id = Convert.ToInt32(reader["ID"]);
                        item.SensorName = Convert.ToString(reader["SensorName"]);
                        item.DataType = Convert.ToString(reader["DataType"]);
                        item.Position = Convert.ToString(reader["Position"]);
                        item.Value = Convert.ToString(reader["Value"]);
                        item.Date = Convert.ToString(reader["Date"]);

                        resultList.Add(item);
                    }
                }

            }

            return resultList;
        }


        // 3. Для отображения данных, отсортированных
        public List<DataBaseItem> GetSortedItems(SortingCriterias Cr)
        {
			List<DataBaseItem> resultList = new List<DataBaseItem>();

            List<string> criteria = Cr.ToArray();

			using (MySqlConnection connection = new MySqlConnection(this.connectionString))
			{
				connection.Open();
                String selectSqlCmd = "SELECT * FROM " + "SENSOR_DATA_TABLE";

				if (criteria.Count != 0)
                {
                    if (criteria.Count > 1)
                    {
                        selectSqlCmd = selectSqlCmd + " ORDER BY " + criteria[0];
						for (Int32 i = 1; i < criteria.Count; i++)
						{
							selectSqlCmd = selectSqlCmd + ", " + criteria[i];
						}
					}
                    else
                    {
						selectSqlCmd = selectSqlCmd + " ORDER BY " + criteria[0];
					}
                }

                selectSqlCmd = selectSqlCmd + ";";


				MySqlCommand cmd = new MySqlCommand(selectSqlCmd, connection);

				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						DataBaseItem item = new DataBaseItem();

						item.Id = Convert.ToInt32(reader["ID"]);
						item.SensorName = Convert.ToString(reader["SensorName"]);
						item.DataType = Convert.ToString(reader["DataType"]);
						item.Position = Convert.ToString(reader["Position"]);
						item.Value = Convert.ToString(reader["Value"]);
						item.Date = Convert.ToString(reader["Date"]);

						resultList.Add(item);
					}
				}

			}

			return resultList;
		}


			public Int32 GetNumberOfItems()
        {
            Int32 numOfItems = 0;

            using (MySqlConnection connection = new MySqlConnection(this.connectionString))
            {
                connection.Open();

                String selectSqlCmd = "SELECT COUNT(*) FROM " + "SENSOR_DATA_TABLE";
                
                MySqlCommand cmd = new MySqlCommand( selectSqlCmd, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        numOfItems = Convert.ToInt32(reader[0]);
                    }
                }
            }


            return numOfItems;
        }


        public List<DataBaseItem> GetSensorItems(String SensorName, String DataType, String Position)
        {
            List<DataBaseItem> resultList = new List<DataBaseItem> ();

            using (MySqlConnection connection = new MySqlConnection( this.connectionString)) 
            { 
                connection.Open();
                String selectSqlCommand = "SELECT * FROM " + "SENSOR_DATA_TABLE" +
                                            " WHERE (SensorName = '" + SensorName + "')" + " AND (DataType = '" + DataType + "')" +
                                            " AND (Position = '" + Position + "');";

                MySqlCommand cmd = new MySqlCommand(selectSqlCommand, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        DataBaseItem item = new DataBaseItem();

                        item.Id = Convert.ToInt32(reader["ID"]);
                        item.SensorName = Convert.ToString(reader["SensorName"]);
                        item.DataType = Convert.ToString(reader["DataType"]);
                        item.Position = Convert.ToString(reader["Position"]);
                        item.Value = Convert.ToString(reader["Value"]);
                        item.Date = Convert.ToString(reader["Date"]);

                        resultList.Add(item);
                    }
                }
            }


            return resultList; 
        }









    }
}
