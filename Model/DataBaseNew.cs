using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientBase;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.IO;

namespace ClientBaseMigration
{

    public class Field
    {
        public string Name;
        public string Type;
    }

    public class Table
    {
        public string Name;
        public List<Field> Fields;
        public Table(string name, List<Field> fields)
        {
            Name = name;
            Fields = fields;
        }
    }

    [Serializable]
    public class DataBaseNew
    {

        [NonSerialized]
        private List<Table> _tables;
        public List<Table> Tables { get { return _tables; } }

        public List<string> SQLTemplates { get; set; }

        public DataBaseNew()
        {
            _tables = new List<Table>();
            SQLTemplates = new List<string>();
        }

        public bool ReadStruct(string filename)
        {
            bool bret = false;
            ExcelData excel = new ExcelData();
            excel.Read(filename);
            Sheet sheet = excel["Данные"];

            if (sheet != null)
            {
                for (int i = 0; i < sheet.Cells.Count; i++)
                {
                    for (int j = 0; j < sheet.Cells[i].Count; j++)
                    {
                        if ((sheet.Cells[i][j] == "1") && (sheet.Cells[i][j + 1] == "Код"))
                        {
                            AddTable(sheet, i, j);
                        }
                    }
                }
                if (Tables.Count != 0) bret = true;
            }
            return bret;
        }

        private void AddTable(Sheet sheet, int i, int j)
        {
            List<Field> flds = new List<Field>();
            string nameTable = sheet.Cells[i - 1][j + 1].ToString();

            while (!string.IsNullOrWhiteSpace(sheet.Cells[i][j].ToString()))
            {
                string name = sheet.Cells[i][j + 1];
                string type = GenType(ref name);
                Field fld = new Field
                {
                    Name = name,
                    Type = type,
                };
                flds.Add(fld);
                i++;
            }

            Tables.Add(new Table(nameTable, flds));
        }



        private string GenType(ref string name)
        {
            string typename;
            int len = 0;
            if ((name.IndexOf("(") != -1) && (name.IndexOf(")") != -1))
            {
                len = Convert.ToInt32(name.Substring(name.IndexOf("(") + 1, name.IndexOf(")") - name.IndexOf("(") - 1));
                name = name.Substring(0, name.IndexOf("(")).Trim();
            }
            switch (name.ToUpper())
            {
                case "КОД":
                case "КОЛИЧЕСТВО":
                case "ПОРЯДОК":
                case "_ЭД20":
                case "СЧЕТЧИК":
                case "КОПИЯ":
                    typename = "INT";
                    break;
                case "СУММА":
                case "РАЗМЕР":
                case "ЦЕНА":
                case "ИТОГО":
                    typename = "FLOAT";
                    break;
                case "СОДЕРЖАНИЕ":
                    typename = "MEDIUMTEXT";
                    break;
                case "АКТИВНЫЙ":
                case "ОПЛАЧЕНО":
                case "ПРОВЕРЕНО":
                case "ПЕРЕСЧИТАНО":
                case "ШТУЧНЫЙ":
                case "УНИКАЛЬНЫЙ":
                    typename = "BIT";
                    break;
                default:
                    if (name.StartsWith("Дата"))
                    {
                        typename = "DATETIME";
                    }
                    else if (name.StartsWith("Код") || name.StartsWith("_Код"))
                    {
                        typename = "INT";
                    }
                    else
                    {
                        len = (len == 0) ? 150 : len;
                        typename = $"VARCHAR({len})";
                    }
                    break;
            }
            return typename;
        }

        public void JSonSerialization()
        {
            FileStream stream = File.Create("DatabaseNew.json");
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(this.GetType());
            //Сериализация
            formatter.WriteObject(stream, this);
            stream.Close();
        }

        public void GenCreateTable()
        {
            if (Tables.Count != 0)
            {
                string sql = "";
                foreach (Table t in Tables)
                {

                    sql = $"CREATE TABLE `{t.Name}` (";
                    foreach (Field f in t.Fields)
                    {
                        sql += $"`{f.Name}` {f.Type}" + ((f.Name == "Код") ? " AUTO_INCREMENT PRIMARY KEY" : " NULL  DEFAULT NULL") + ",";
                    }
                    sql = sql.Trim(',');
                    sql += ")";
                    SQLTemplates.Add(sql);
                }
            }
        }


    }
}
