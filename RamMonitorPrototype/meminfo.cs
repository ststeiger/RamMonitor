
namespace Linux
{


    class ProcFS
    {


        public static System.Data.DataTable GetMemInfo()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("i", typeof(int));
            dt.Columns.Add("Line", typeof(string));
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("Unit", typeof(string));


            System.Data.DataRow dr = null;
            int counter = -1;
            string line;

            string memFile = "/proc/meminfo";

            if (!System.IO.File.Exists(memFile))
                memFile = "meminfo.txt";

            // Read the file and display it line by line.
            using (System.IO.StreamReader file = new System.IO.StreamReader(memFile))
            {

                while ((line = file.ReadLine()) != null)
                {
                    counter++;

                    if (line == null)
                        continue;

                    line = line.Trim();
                    if (line == string.Empty)
                        continue;

                    int iPos = line.IndexOf(':');
                    if (iPos == -1)
                        continue;

                    string strKey = line.Substring(0, iPos);
                    string strValue = line.Substring(iPos + 1);

                    if (strKey != null)
                        strKey = strKey.Trim();

                    if (strValue != null)
                        strValue = strValue.Trim();

                    int iAmount = 0;
                    string strUnit = null;


                    System.Text.RegularExpressions.Match ma =
                        System.Text.RegularExpressions.Regex.Match(strValue, @"(\d+)\s*(.*)\s*");

                    if (ma.Success)
                    {
                        string strValueNumber = ma.Groups[1].Value;
                        int.TryParse(strValueNumber, out iAmount);
                        strUnit = ma.Groups[2].Value;
                    } // End if (ma.Success)

                    dr = dt.NewRow();
                    dr["i"] = counter;
                    dr["Line"] = line;
                    dr["Key"] = strKey;
                    dr["Value"] = iAmount;
                    dr["Unit"] = strUnit;

                    dt.Rows.Add(dr);
                } // Whend 

                file.Close();
            } // End Using System.IO.StreamReader file

            return dt;
        } // End Function GetMemInfo


    }


}
