namespace RamMonitorPrototype.Trash
{
    
    
    public enum RenderType
    {
        Pretty,
        Ugly,
        Minified
    }

    

    public class HtmlElement
    {
        protected string m_tagName;
        protected string m_textContent;

        protected System.Collections.Generic.List<HtmlElement> m_childList;
        protected System.Collections.Generic.Dictionary<string, string> m_attributeList;
        
        
        public HtmlElement(string tagName)
        {
            this.m_tagName = tagName;
        }

        public void appendChild(HtmlElement ele)
        {
            if(this.m_childList == null)
                this.m_childList = new System.Collections.Generic.List<HtmlElement>();
            
            this.m_childList.Add(ele);
        }

        public void setAttribute(string attributeName, string attributeValue)
        {
            if (this.m_attributeList == null)
                this.m_attributeList = new System.Collections.Generic.Dictionary<string, string>();
            
            this.m_attributeList[attributeName] = System.Web.HttpUtility.HtmlAttributeEncode(attributeValue);
        }
        
        public void appendChild(string ele)
        {
            this.m_textContent = ele;
        }
        
        
        public string render()
        {
            return render(RenderType.Pretty);
        }
        
        public string render(RenderType prettyPrint)
        {
            return render(prettyPrint, 0);
        }
        

        public string render(RenderType prettyPrint, int indentLevel)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            if ((prettyPrint == RenderType.Pretty)  && indentLevel != 0)
            {
                sb.Append("".PadRight( 3 * indentLevel, ' '));    
            }

            
            
            
            sb.Append("<");
            sb.Append(m_tagName);

            if (this.m_attributeList != null)
            {
                foreach (string attributeName in this.m_attributeList.Keys)
                {
                    sb.Append(" ");
                    sb.Append(attributeName);
                    sb.Append("=\"");
                    sb.Append(this.m_attributeList[attributeName]);
                    sb.Append("\"");
                }
                
            }

            
            sb.Append(">");
            
            if ((prettyPrint != RenderType.Minified) && this.m_textContent == null)
                sb.AppendLine();
                
            
            
            if (this.m_childList != null)
            {
                for (int i = 0; i < this.m_childList.Count; i++)
                {
                    sb.Append( this.m_childList[i].render(prettyPrint, indentLevel+1));
                }
                
            }
            
            if (this.m_textContent != null)
            {
                sb.Append(this.m_textContent);
            }
            else
            {
                if ((prettyPrint == RenderType.Pretty) && indentLevel != 0)
                {
                    sb.Append("".PadRight( 3 * indentLevel, ' '));    
                }    
            }

            
            
            sb.Append("</");
            sb.Append(this.m_tagName);
            if(prettyPrint == RenderType.Minified)
                sb.Append(">");
            else
                sb.AppendLine(">");
            
            string ret = sb.ToString();
            sb.Clear();
            sb = null;
            return ret;
        }

    }


    public class document
    {

        public static HtmlElement createElement(string tagName)
        {
            return new HtmlElement(tagName);
        }
        
        
        public static string createTextNode(string text)
        {
            return System.Web.HttpUtility.HtmlEncode(text);
        }

    }


    public class GeschossRenderer
    {
        public static void Test()
        {
            foobar();
            
            RenderDom("L281 XXX", "1234"
                , new string[]{"Dachgeschoss", "2. Obergeschoss", "1. Obergeschoss", "Erdgeschoss", "1. Untergeschoss", "2. Untergeschoss"}
                ,new string[] { "Quadro", "Rondo", "Longo" }
            );
            
            
            RenderGeschoss("L281 XXX", "1234"
                , new string[]{"Dachgeschoss", "2. Obergeschoss", "1. Obergeschoss", "Erdgeschoss", "1. Untergeschoss", "2. Untergeschoss"}
                ,new string[] { "Quadro", "Rondo", "Longo" }
            );
            
            
            // System.Net.WebUtility.HtmlEncode("foobar");
            // System.Web.HttpUtility.HtmlEncode("foobar");
        }
        
        
        public static void foobar()
        {
            System.Data.Common.DbProviderFactory fac = Microsoft.Data.SqlClient.SqlClientFactory.Instance;
            
            Microsoft.Data.SqlClient.SqlConnectionStringBuilder csb = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder();
            csb.InitialCatalog = "COR_Basic_Sonova";
            csb.DataSource = System.Environment.MachineName + @"\SQLEXPRESS";
            csb.IntegratedSecurity = true;
            csb.Pooling = true;
            csb.PacketSize = 4096;
            
            if(!csb.IntegratedSecurity)
            {
                csb.UserID = "";
                csb.Password = "";
            }
            
            using (System.Data.Common.DbConnection conn = fac.CreateConnection())
            {
                conn.ConnectionString = csb.ConnectionString;
                
                if(conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
                
                using (System.Data.Common.DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT 
     T_AP_Trakt.TK_UID 
    ,T_AP_Trakt.TK_Nr 
    ,T_AP_Trakt.TK_Bezeichnung 
FROM T_AP_Trakt 
WHERE (1=1) 
AND T_AP_Trakt.TK_Status = 1 
AND CURRENT_TIMESTAMP BETWEEN T_AP_Trakt.TK_DatumVon AND T_AP_Trakt.TK_DatumBis 
AND T_AP_Trakt.TK_GB_UID = '5F495EBF-CC51-477E-A03C-919F7DF30C0A' 
";


                    using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; ++i)
                                {
                                    string name = reader.GetName(i);
                                    System.Console.WriteLine("{0}: \t{1}", name, reader.GetValue(i));
                                }
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("No rows found.");
                        }
                    }
                }
                
                if(conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }

            
            
        }

        public static void RenderDom(string header, string svg_uid, string[] geschosse, string[] trakte)
        {
            // <div> <h1 class="floorHeader">L28</h1>
            var div = document.createElement("div");

            // <h1 class="floorHeader">L28</h1>
            var h1 = document.createElement("h1");
            h1.appendChild(document.createTextNode(header));
            h1.setAttribute("class", "floorHeader");
            div.appendChild(h1);


            var tbl = document.createElement("table");
            tbl.setAttribute("class", "floorTable");

            var bdy = document.createElement("tbody");

            for (var i = 0; i < geschosse.Length; ++i)
            {
                var tr = document.createElement("tr");
                if (i == 0)
                    tr.setAttribute("class", "current");
                else
                    tr.setAttribute("class", "gray");



                var td = document.createElement("td");
                td.appendChild(document.createTextNode(geschosse[i]));
                tr.appendChild(td);

                for (var j = 0; j < trakte.Length; ++j)
                {
                    td = document.createElement("td");
                    td.appendChild(document.createTextNode(trakte[j]));
                    tr.appendChild(td);
                }

                bdy.appendChild(tr);
            }
            
            
            tbl.appendChild(bdy);
            div.appendChild(tbl);
            
            string output = div.render(RenderType.Pretty);
            
            System.Console.WriteLine(output);
        }
        
        public static void RenderGeschoss(string header, string svg_uid, string[] geschosse, string[] trakte)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            // <div> <h1 class="floorHeader">L28</h1>
            sb.AppendLine("<div>");
            sb.Append("<h1 class=\"floorHeader\">");
            sb.Append(System.Web.HttpUtility.HtmlEncode( header));
            sb.AppendLine("</h1>");
            
            
            sb.AppendLine("<table class=\"floorTable\">");
            sb.AppendLine("<tbody>");
            
            
            for (int i = 0; i < geschosse.Length; ++i)
            {
                sb.Append("<tr class=\"");
                
                if (i == 0)
                    sb.AppendLine("current");
                else
                    sb.AppendLine("gray");
                
                sb.AppendLine("\">");
                
                sb.Append("<td>");
                sb.Append(System.Web.HttpUtility.HtmlEncode( geschosse[i]));
                sb.AppendLine("</td>");
                
                
                for (var j = 0; j < trakte.Length; ++j)
                {
                    sb.Append("<td>");
                    sb.Append(System.Web.HttpUtility.HtmlEncode( trakte[j]));
                    sb.AppendLine("</td>");
                }
                sb.AppendLine("</tr>");
            }


            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            string str = sb.ToString();
            System.Console.WriteLine(str);
        }
        
        
    }
}