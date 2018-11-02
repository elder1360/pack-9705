using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ChooseSeats
{
    public partial class dataget : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var name = Request.QueryString["name"];
            var family = Request.QueryString["family"];
            var seat = Request.QueryString["seat"];

            using (StreamWriter sw = new StreamWriter(Server.MapPath("~/dataget/Users.txt"), true))
            {
                sw.WriteLine($"{name},{family},{seat}");
            }

            Response.Write("ok");
        }
    }
}