using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System;
using System.Text;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
/*using System.Data;
using Excel;*/



namespace ExcelParser
{
    public class TitleData
    {
        public string name;
        public string type;
        public string des;
    }

    /// <summary>
    /// //第一列为key
    // txt 第一行是字段类型
    //第二行是中文描述
    //第三行是英文字段名称
    /// Excel parser editor. The main class of parse excel to txt,and generate class file
    /// </summary>
    public class CsvParserEditor : Editor
    {
        public static string FileDir_Excel = "./Excel";
        public static string FileDir_DataTxt = "./Data";


        #region  生成对应数据类

        /// <summary>
        /// 将选中的txt 文本生成cs 代码;
        /// </summary>
/*        [MenuItem("Tools/ExcelParser/Add C# Class from SelectTxt")]
        public static void CreateCS_BySelectTxt()
        {
            var objs = Selection.objects;

            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                if (obj is TextAsset)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    GenerateAllClass(path);
                }
            }
        }*/

        /// <summary>
        /// Generate xxxBean.cs and xxxMgr.cs
        /// </summary>
        /// <param name="excelTxtPath">Excel text path.</param>
        static void GenerateAllClass(string excelTxtPath)
        {
            string context = File.ReadAllText(excelTxtPath);


//			string fileName = obj.name.ToString();
            int lastSlashesIndex = excelTxtPath.LastIndexOf('/');
            int lastPointIndex = excelTxtPath.LastIndexOf('.');
            string fileName = excelTxtPath.Substring(lastSlashesIndex + 1, lastPointIndex - lastSlashesIndex - 1);

            //获取各种列的 属性标题信息;
            List<TitleData> titleDataList = GetTitleDataListFromTxt(context);

            GenerateBeanClass(fileName, titleDataList);

            //GenerateMgrClass(fileName);
            //将此数据类 插入管理中心;
            //GenerateDataMgrCs(fileName);
        }

        /// <summary>
        /// Generate xxxBean.cs and xxxMgr.cs
        /// </summary>
        /// <param name="csvPath">Excel text path.</param>
        static void GenerateAllClassByCsv(string csvPath)
        {
            string context = File.ReadAllText(csvPath);


//			string fileName = obj.name.ToString();
            int lastSlashesIndex = csvPath.LastIndexOf('/');
            int lastPointIndex = csvPath.LastIndexOf('.');
            string fileName = csvPath.Substring(lastSlashesIndex + 1, lastPointIndex - lastSlashesIndex - 1);

            //获取各种列的 属性标题信息;
            List<TitleData> titleDataList = GetTitleDataListFromCsv(context);

            GenerateBeanClass(fileName, titleDataList);

            //GenerateMgrClass(fileName);
            //将此数据类 插入管理中心;
            //GenerateDataMgrCs(fileName);
        }


        static List<TitleData> GetTitleDataListFromTxt(string dataTxt)
        {
            List<TitleData> titleDataList = new List<TitleData>();
            //去掉干扰
            dataTxt = dataTxt.Replace("\r", "");
            //获取所有的行数据
            string[] hList = dataTxt.Split('\n');
            //第三行才是 标题
            string title = hList[2];
            string[] titles = title.Split('\t');
            //第一行是属性类型
            string[] types = hList[0].Split('\t');

            //第二行是注释
            string titleDes = hList[1];
            string[] titleDesArr = titleDes.Split('\t');

            for (int i = 0; i < titles.Length; i++)
            {
                TitleData titleData = new TitleData();

                if (string.IsNullOrEmpty(titles[i]))
                {
                    continue;
                }

                titleData.name = titles[i];

                string typeStr = types[i].ToLower();

                if (typeStr == "string" || typeStr == "int" || typeStr == "float" || typeStr == "enum" 
                    || typeStr == "bool"|| typeStr == "long"|| typeStr == "double")
                {
                    titleData.type = typeStr;
                }
                titleData.des = titleDesArr[i];
                titleDataList.Add(titleData);
            }
            return titleDataList;
        }
        static List<TitleData> GetTitleDataListFromCsv(string dataTxt)
        {
            List<TitleData> titleDataList = new List<TitleData>();
            //去掉干扰
            dataTxt = dataTxt.Replace("\r", "");
            //获取所有的行数据
            string[] hList = dataTxt.Split('\n');
            //第三行才是 标题
            string title = hList[2];
            string[] titles = title.Split(',');
            //第一行是属性类型
            string[] types = hList[0].Split(',');

            //第二行是注释
            string titleDes = hList[1];
            string[] titleDesArr = titleDes.Split(',');

            for (int i = 0; i < titles.Length; i++)
            {
                TitleData titleData = new TitleData();

                if (string.IsNullOrEmpty(titles[i]))
                {
                    continue;
                }

                titleData.name = titles[i];

                string typeStr = types[i].ToLower();

                if (typeStr == "string" || typeStr == "int" || typeStr == "float" || typeStr == "enum" 
                    || typeStr == "bool"|| typeStr == "long"|| typeStr == "double")
                {
                    titleData.type = typeStr;
                }
                titleData.des = titleDesArr[i];
                titleDataList.Add(titleData);
            }
            return titleDataList;
        }

        //生成一条属性;
        static string GeneratePropertyBlock(TitleData tileData)
        {
            //用了@忽略转移字符   这是属性的一个模版 
            string propertyBlock = @"
    private {0} {2};
    /// <summary>
    /// {3}
    /// </summary>
    public {0} {1} 
    {
        get 
        {
             return {2};
        }
        set 
        {
             {2} = value;
        }
    }";

            string name = tileData.name.Substring(0, 1).ToLower() + tileData.name.Substring(1);
            string bigName = name.Substring(0, 1).ToUpper() + name.Substring(1);

            if (tileData.type == "enum")
            {
                propertyBlock = propertyBlock.Replace("{0}", bigName);
                propertyBlock = propertyBlock.Replace("{1}", "This"+bigName);
                propertyBlock = propertyBlock.Replace("{2}", name);
                propertyBlock = propertyBlock.Replace("{3}", tileData.des);
            }
            else
            {
                propertyBlock = propertyBlock.Replace("{0}", tileData.type);
                propertyBlock = propertyBlock.Replace("{1}", bigName);
                propertyBlock = propertyBlock.Replace("{2}", name);
                propertyBlock = propertyBlock.Replace("{3}", tileData.des);
            }
            

            return propertyBlock;
        }

        /// <summary>
        /// Generates the bean class.
        /// 生成对象类;
        /// 如果没有文件重新生成。有的话更新字段; 
        /// 这个生成的类因为会被覆盖。所以不要在里面写东西;
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="titles">Titles.</param>
        public static void GenerateBeanClass(string fileName, List<TitleData> titles)
        {
            //string targetPath = Application.dataPath + "/Scripts/CsvModule/Data/DataBeans/";
            string targetPath = @"Hotfix\Module\CSV\Data\DataBeans\";
            string file = targetPath + fileName + "Data.cs";
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            if (File.Exists(file))
            {
//                File.Delete(file);
            }
            using (FileStream fileStream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                StreamWriter outfile = new StreamWriter(fileStream);

//                outfile.WriteLine("using UnityEngine;");
                //outfile.WriteLine("using System.Collections;");
                //outfile.WriteLine("using ExcelParser;");
                outfile.WriteLine("");
                outfile.WriteLine("/// <summary>");
                outfile.WriteLine("/// 自动生成类。不要修改");
                outfile.WriteLine("/// 数据表的第一列为key");
                outfile.WriteLine("/// </summary>");
                outfile.WriteLine("public class " + fileName + "Data {");
//                outfile.WriteLine(" ");

                for (int i = 0; i < titles.Count; i++)
                {
                    TitleData td = titles[i];
                    string block = GeneratePropertyBlock(td);
                    outfile.WriteLine(block);
//                    outfile.WriteLine(" ");
                }

                outfile.WriteLine("}");


                outfile.Close();
                fileStream.Close();
            }


            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Generates the mgr class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public static void GenerateMgrClass(string fileName)
        {
            //string targetPath = Application.dataPath + "../Hotfix/Module/CSV/Data/DataMgr/";
            string targetPath = @"Hotfix\Module\CSV\Data\DataMgr\";
            string file = targetPath + fileName + "Mgr.cs";

            if (!Directory.Exists(targetPath))
            {
                Debug.LogError("no path " + targetPath);
                return;
            }


            FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate);


            string templetePath = Application.dataPath + "/Editor/ExcelCsvParser/Templete/MgrTemplete.txt";

            string classText = File.ReadAllText(templetePath);

            classText = classText.Replace("{0}", fileName);
            classText = classText.Replace("{1}",  fileName+".csv");


            StreamWriter outfile = new StreamWriter(fileStream);

            outfile.Write(classText);
            outfile.Close();
            fileStream.Close();


            AssetDatabase.Refresh();

            Debug.Log("genereate mgr class success!");
        }

        /// <summary>
        /// 清除掉旧的数据集入口;
        /// 只有自动全部打包生成代码的时候才需要用。
        /// </summary>
        static void ClearOldDataMgrInit()
        {
            //string targetPath = Application.dataPath + "../Hotfix/Module/CSV/Data/DataBeans/";
            string targetPath = @"Hotfix\Module\CSV\Data\DataBeans\"; ;
            string file = targetPath + "DataSetMgr.cs";
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            if (File.Exists(file))
            {
                string templetePath = Application.dataPath + "/Editor/ExcelCsvParser/Templete/MgrSetTemplete.txt";
                string classText = File.ReadAllText(templetePath);
                File.WriteAllText(file, classText, Encoding.UTF8);
            }
        }

        static void GenerateDataMgrCs(string fileName)
        {
            //string targetPath = Application.dataPath + "../Hotfix/Module/CSV/Data/";
            string targetPath = @"Hotfix\Module\CSV\Data\"; ;
            string filePath = targetPath + "DataSetMgr.cs";
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            bool isHasFile = File.Exists(filePath);
            string ctStr = "";
            if (!isHasFile)
            {
                string templetePath = Application.dataPath + "/Editor/ExcelCsvParser/Templete/MgrSetTemplete.txt";
                ctStr = File.ReadAllText(templetePath);
            }
            else
            {
                ctStr = File.ReadAllText(filePath);
            }
            int stIndex = ctStr.IndexOf("//Start");
            stIndex += 7;
            if (ctStr.IndexOf(fileName + "Mgr.instance.loadDataFile(prePath);", StringComparison.Ordinal) < 0)
            {
                string insetStr = "\r        " + fileName + "Mgr.instance.loadDataFile(prePath);\r";
                ctStr = ctStr.Insert(stIndex, insetStr);

                File.WriteAllText(filePath, ctStr, Encoding.UTF8);
            }
            else
            {
                Debug.Log("skip create " + fileName + "  mgr");
            }
        }

        #endregion


/*        [MenuItem("Tools/ExcelParser/ExcleToTxtAll_NoCode")]
        public static void ExcelToTxtAll_NoCode()
        {
            ExcelToTxt_All(false);
        }

        [MenuItem("Tools/ExcelParser/ExcelToTxtAll_Code")]
        public static void ExcelToTxtAllWithCode()
        {
            ClearOldDataMgrInit();
            ExcelToTxt_All(true);
        }*/

        static void ExcelToTxt_All(bool isCreateCode)
        {
            if (Directory.Exists(FileDir_Excel)) // i only set directory drop or drap
            {
                string[] filePaths = Directory.GetFiles(FileDir_Excel, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string path = filePaths[i];
                    if (path.Contains(".meta") || path.Contains(".DS_Store"))
                    {
                        continue;
                    }

                    FileInfo fi2 = new FileInfo(path);
                    Debug.Log(fi2.Name);
                    Export_oneExcel_Txt(isCreateCode, path, fi2.Name);
                }
            }
        }

        //====================  尽量不要放excel 在unity 目录下。所以也不需要选择操作了。
        //=====================  有需求可以导出选择的文件，当然要在unity  asset目录里的
/*
        [MenuItem("Tools/ExcelParser/CreateTxtByChoose")]
        public static void ExcelToTxt_ByChoose_NoCode()
        {
            ExcelToTxt(false);
        }
*/

        [MenuItem("Tools/ExcelParser/GenerateClass from Choose")]
        public static void ExcelToTxt_ByChoose_WithCode()
        {
            ExcelToTxt(true);
        }


        /// <summary>
        /// Xlsxs to text.
        /// </summary>
        /// <param name="autoGenerateClass">If set to <c>true</c> auto generate class.</param>
        static void ExcelToTxt(bool autoGenerateClass)
        {
            var objs = Selection.objects;

            for (int i = 0; i < objs.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(objs[i]);
                string fileName = objs[i].name;
                Export_oneExcel_Txt(autoGenerateClass, path, fileName);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 把一个excel文件导出txt
        /// </summary>
        /// <param name="autoGenerateClass"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        static void Export_oneExcel_Txt(bool autoGenerateClass, string path, string fileName)
        {
            if (path.EndsWith(".xlsx"))
            {
                string targetFile = path.Replace(".xlsx", ".txt");

                int lastI = targetFile.LastIndexOf('/');
                targetFile = targetFile.Insert(lastI + 1, "../Data/");

                string direct = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(direct))
                {
                    Directory.CreateDirectory(direct);
                }


                /*FileStream targetFileStream = new FileStream(targetFile, FileMode.OpenOrCreate);


                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                DataSet result = excelReader.AsDataSet();
                int columns = result.Tables[0].Columns.Count;
                int rows = result.Tables[0].Rows.Count;


                StringBuilder txtBuilder = new StringBuilder();

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        txtBuilder.Append(result.Tables[0].Rows[r][c].ToString()).Append("\t");
                    }
                    txtBuilder.Append("\n");
                }


                StreamWriter steamWriter = new StreamWriter(targetFileStream);

                steamWriter.Write(txtBuilder.ToString());


                steamWriter.Close();
                stream.Close();
                targetFileStream.Close();

                if (autoGenerateClass)
                {
                    GenerateAllClass(targetFile);
                }*/
            }
            else if(path.EndsWith(".csv"))
            {
                GenerateAllClassByCsv(path);
            }
        }
    }
}

#endif