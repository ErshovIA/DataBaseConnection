using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using DataBaseConnection.Models;






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

    }
}
