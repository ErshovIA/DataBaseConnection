using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using DataBaseConnection.Models;
using ClosedXML.Excel;
// using AspNetCore;

namespace DataBaseConnection.Controllers
{
    public class DataBaseController : Controller
    {

		public IActionResult Index()
        {
            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseConnection.Models.DataBaseModel)) as DataBaseModel;

			Int32 NOfItems;
            try
            {
                NOfItems = db.GetNumberOfItems();
            }
            catch(MySqlConnector.MySqlException ex)
            {
                var ExType = ex.GetType();
				return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = ex.Message });
            }
            
            //return View(db.GetNumberOfItems());
            return View(NOfItems);
		}

        public IActionResult AboutUs()
        {
            return View();
        }



        public IActionResult ShowDataTable() 
        {
            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseConnection.Models.DataBaseModel)) as DataBaseModel;

            return View(db.GetItems());
        }


        private DataTable ConvertToDataTable(List<DataBaseItem> Lst)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(DataBaseItem));


            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties) 
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType );
            }

            foreach (DataBaseItem item in Lst) 
            { 
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);  
            }

            return table;
        }

        private Byte[] GetExcelFileBinaryContent(DataBaseModel Db)
        {
            List<DataBaseItem> lstItems = Db.GetItems(Db.Criteria);

            DataTable dt = this.ConvertToDataTable(lstItems);

			XLWorkbook wb = new XLWorkbook();

            wb.Worksheets.Add(dt, "Sensors");

            wb.Worksheet(1).Cells(true).Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);
			wb.Worksheet(1).Cells(false).Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);

			wb.Worksheet(1).Cells(true).Style.Fill.BackgroundColor = XLColor.White;
			wb.Worksheet(1).Cells(false).Style.Fill.BackgroundColor = XLColor.White;

			wb.Worksheet(1).Cells(true).Style.Fill.PatternColor = XLColor.Black;
			wb.Worksheet(1).Cells(false).Style.Fill.PatternColor = XLColor.Black;

            wb.Worksheet(1).Cells(true).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
			wb.Worksheet(1).Cells(true).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
			wb.Worksheet(1).Cells(false).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
			wb.Worksheet(1).Cells(false).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            wb.Worksheet(1).Cells(true).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			wb.Worksheet(1).Cells(false).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wb.Worksheet(1).Tables.FirstOrDefault().ShowAutoFilter = false;

            wb.Worksheet(1).Columns().AdjustToContents();

            MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms);

            return ms.ToArray();
		}

        public IActionResult SaveToExcelFile()
        {
            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseModel)) as DataBaseModel;

            db.Criteria = new SortingCriterias();
			Byte[] data = this.GetExcelFileBinaryContent(db);

            return File(data, "application/xlsx", "SensorsData.xlsx");
        }

		// XML:
        public IActionResult SaveToXmlFile()
		{
			DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseModel)) as DataBaseModel;

			db.Criteria = new SortingCriterias();
			List<DataBaseItem> lst = db.GetItems(db.Criteria);

			using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DataBaseItem>));
				xmlSerializer.Serialize(stringWriter, lst);

				Byte[] data = Encoding.Unicode.GetBytes(stringWriter.ToString());

				return File(data, "application/xml", "SensorData.xml");
			}
		}

		///////////////////////////////////////////////////
		// Сохранение отсортированной таблицы в exel, xml
		///////////////////////////////////////////////////
		/// Exel:
		public IActionResult SaveSortedTableToExel()
		{
			// return View("~/Views/DataBase/SimpleErrorMsg.cshtml", "Такого функционала пока нет");
			DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseModel)) as DataBaseModel;

			Byte[] data = this.GetExcelFileBinaryContent(db);

			return File(data, "application/xlsx", "SensorsData.xlsx");
		}

		/// Xml:
		public IActionResult SaveSortedTableToXml()
		{
			DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseModel)) as DataBaseModel;

			List<DataBaseItem> lst = db.GetItems(db.Criteria);

			using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DataBaseItem>));
				xmlSerializer.Serialize(stringWriter, lst);

				Byte[] data = Encoding.Unicode.GetBytes(stringWriter.ToString());

				return File(data, "application/xml", "SensorData.xml");
			}
		}


		// Chart:

		[HttpGet]
        public IActionResult SelectSensor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelectSensor(DataType_Position_Time_Selection Item)
        {
            return RedirectToAction("ShowChart", Item);
        }

        public IActionResult ShowChart(DataType_Position_Time_Selection Item)
        {
            CultureInfo ci = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = ci;
			Thread.CurrentThread.CurrentUICulture = ci;

            DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseModel)) as DataBaseModel;

            List<DataBaseItem> itemsList = db.GetSensorItems(Item.DataType, Item.Position, Item.Date1, Item.Date2);

            if (itemsList.Count != 0)
            {
                return View(db.GetSensorItems(Item.DataType, Item.Position, Item.Date1, Item.Date2));
			}
            else
            {
				return View("~/Views/DataBase/SimpleErrorMsg.cshtml", "Некорректные параметры сортировки");
			}

		}


        // 3. Выбор критериев сортировки и вывод отсоррованной таблицы
		public IActionResult SelectSortingCriteria()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SelectSortingCriteria(SortingCriterias Cr)
		{
			return RedirectToAction("ShowSortedDataTable", Cr);
		}

		public IActionResult ShowSortedDataTable(SortingCriterias Cr)
		{
			DataBaseModel db = HttpContext.RequestServices.GetService(typeof(DataBaseConnection.Models.DataBaseModel)) as DataBaseModel;

			return View(db.GetItems(Cr));
		}


	}
}
