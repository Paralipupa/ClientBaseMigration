using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientBase;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.IO;
using ClientBaseMigration.Properties;

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
        private readonly List<Table> _tables;
        public List<Table> Tables { get { return _tables; } }

        public List<string> SQLCreate { get; set; }
        public List<string> SQLInsert { get; set; }

        private ExcelData Excel { get; set; }

        public DataBaseNew()
        {
            _tables = new List<Table>();
            SQLCreate = new List<string>();
            SQLInsert= new List<string>();
            Excel = new ExcelData();
        }

        public bool ReadFromExcel(string filename)
        {
            bool bret = Excel.Read(filename);
            return bret;
        }

        public void JSonSerialization()
        {
            FileStream stream = File.Create("DatabaseNew.json");
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(this.GetType());
            //Сериализация
            formatter.WriteObject(stream, this);
            stream.Close();
        }


        public bool GenStruct()
        {
            Sheet sheet = Excel["Данные"];
            bool bret = false;
            if ((sheet != null) && (Tables.Count == 0))
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

        public bool GenMigrate()
        {
            bool bret = false;
            Sheet sheet = Excel["СвязиТаблиц"];

            if (sheet != null)
            {
                for (int i = 0; i < sheet.Cells.Count; i++)
                {
                    if ((sheet.Cells[i][4] == "1") && sheet.Cells[i][5].Contains("Код"))
                    {
                        FormSQLInsert(sheet, i, 4);
                    }
                }
                if (SQLInsert.Count != 0) bret = true;
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
                string type = TypeFromName(ref name);
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

        private string TypeFromName(ref string name)
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

        private void GenCreateTable()
        {
            if (Tables.Count != 0)
            {
                foreach (Table t in Tables)
                {

                    string sql = $"CREATE TABLE `{t.Name}` (";
                    foreach (Field f in t.Fields)
                    {
                        sql += $"`{f.Name}` {f.Type}" + ((f.Name == "Код") ? " AUTO_INCREMENT PRIMARY KEY" : " NULL  DEFAULT NULL") + ",";
                    }
                    sql = sql.Trim(',');
                    sql += ")";
                    SQLCreate.Add(sql);
                }
            }
        }


        private void FormSQLInsert(Sheet sheet, int row, int col)
        {
            List<string> fld = new List<string>();
            List<string> data = new List<string>();
            string nameTable = sheet.Cells[row - 1][col + 1].ToString();

            int k = row;
            int j = col;
            while (!string.IsNullOrWhiteSpace(sheet.Cells[k][j].ToString()))
            {
                string name = sheet.Cells[k][j + 1];
                string cont = null;

                if (name.IndexOf("<") != -1)
                {
                    cont = name.Substring(name.IndexOf("<") + 1);
                    cont = cont.Trim('>');
                    name = name.Substring(0, name.IndexOf("<"));
                }
                fld.Add(name);
                data.Add(cont);
                k++;
            }

            k = row;
            j = 0;
            while (!string.IsNullOrWhiteSpace(sheet.Cells[k][j].ToString()))
            {
                string name = sheet.Cells[k][j + 1];

                int index = fld.IndexOf(name);
                if (index != -1)
                {
                    data[index] = name;
                }
                k++;
            }

            string sql = $"INSERT INTO [{nameTable}] (";
            for(int i = 0; i < fld.Count; i++)
            {
                sql += $"[{fld[i]}],";
            }
            sql = sql.Trim(',')+$") IN '{Settings.Default.PathBaseED2}'";
            sql += " SELECT ";
            for (int i = 0; i < data.Count; i++)
            {
                sql += ((data[i] != null) ? $"[{ data[i]}]" : "null")+",";
            }
            if (string.IsNullOrWhiteSpace(sheet.Cells[row - 1][col + 3]))
            {
                sql = sql.Trim(',') + $" FROM [{sheet.Cells[row - 1][1]}]";
            }
            else
            {
                string name = sheet.Cells[row - 1][col + 3];
                sql = sql.Trim(',') + $" FROM {name}";
            }

            SQLInsert.Add(sql);

        }
    }
}
