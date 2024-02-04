namespace DataBaseConnection.Models
{
	public class SortingCriterias
	{
		public string criteria_1 { get; set; }

		public string criteria_2 { get; set; }

		public string criteria_3 { get; set; }

		public string criteria_4 { get; set; }

		public string criteria_5 { get; set; }

		public List<string> ToArray()
		{
			List<string> resultList = new List<string>();

			if (criteria_1 != null) 
				resultList.Add(criteria_1);
			if (criteria_2 != null)
				resultList.Add(criteria_2);
			if (criteria_3 != null) 
				resultList.Add(criteria_3);
			if (criteria_4 != null)
				resultList.Add(criteria_4);
			if (criteria_5 != null)
				resultList.Add(criteria_5);

			if (resultList.Count > 1)
			{
				bool isCrossing = false;
				int last_index = resultList.Count - 1;
				for (int i = 1; i < resultList.Count; i++)
				{
					for (int j = 0; j < i; j++)
					{
						if (resultList[i] == resultList[j])
						{ isCrossing = true; break; }
					}
					if (isCrossing) 
					{ last_index = i - 1; break; }
				}
				resultList = resultList.GetRange(0, last_index + 1);
			}

			return resultList;
		}
	}
}
