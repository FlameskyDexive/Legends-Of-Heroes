using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using System.Threading;
using ETModel;

namespace ExcelParser
{
    /// <summary>
    /// 第一列为key
    /// /数据管理的基类;
    /// 必要功能，读取文件。写入缓存数组;
    /// virtual  获取文件路径  
    /// virtual  获取对象类型
    /// virtual  根据下标获取对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataMgrBase<T> : IDataMgrBase where T : class, new()
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }


        /// <summary>
        /// 把表格序列化成字典，以id为Key，各配表类型为value
        /// </summary>
        public Dictionary<object, IDataBean> idDataDic = new Dictionary<object, IDataBean>();

        /// <summary>
        /// 存放表格所有id列表
        /// </summary>
        public List<int> listIds = new List<int>();

        bool isInit = false;

        /// <summary>
        /// Inits the data.
        /// </summary>
        public void loadDataFile(string prePath )
        {
            if (prePath == null)
            {
                throw new Exception("没有根据平台指定前置 路径 "); 
            }
            if (isInit)
            {
                return;
            }
            //这个路径应当在 是解压之后存放的  Application.persistentDataPath 中  ;
            string filePath = prePath + "/"+GetXlsxPath();
            //Log.Debug($"---csv path---{filePath}");
            Type dataBeanType = GetBeanType();

//            FileInfo info = new FileInfo (filePath);
//            if (info == null)
//            {
//                return;
//            }
//            StreamReader fiSR = info.OpenText ();

            //string dataTxt = File.ReadAllText(filePath);      // old load csv
            string dataTxt = ConfigHelper.GetCsvText(GetXlsxPath());
            //Log.Debug($"----{dataTxt}");
            //第一行是属性类型
            //第二行是注释
            //第三行才是 标题
            dataTxt = dataTxt.Replace("\r", "");
            string[] hList = dataTxt.Split('\n');
            if (hList.Length > 3)
            {
                string[] types = hList[0].Split(',');
                string[] titles = hList[2].Split(',');


                for (int col = 3; col < hList.Length; col++)
                {
                    IDataBean dataBean = null;
                    object key = null;

                    string[] vals = hList[col].Split(',');

                    if (vals.Length != titles.Length)
                    {
                        continue;
                    }


                    dataBean = (IDataBean) Activator.CreateInstance(dataBeanType);
                    for (int row = 0; row < titles.Length; row++)
                    {
                        string titleName = titles[row];

                        if (string.IsNullOrEmpty(titleName))
                        {
                            continue;
                        }

                        string typeStr = types[row];
                        string valStr = vals[row];


                        if (string.IsNullOrEmpty(typeStr))
                        {
                            continue;
                        }

                        string propertyName = titleName.Substring(0, 1).ToUpper() + titleName.Substring(1);
                        PropertyInfo prop;
                        object val;
                        if (typeStr == "enum")
                        {
                            prop = dataBeanType.GetProperty("This" + propertyName, BindingFlags.Public | BindingFlags.Instance);

                            val = ConvertHelper.ChangeType(valStr, prop.PropertyType);
                            //object o = Enum.Parse(typeof(prop))

                        }
                        else
                        {
                            prop = dataBeanType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                            val = Convert.ChangeType(valStr, prop.PropertyType);
                        }
                        


                        prop.SetValue(dataBean, val, null);

                        //set dictionary id
                        if (row == 0)
                        {
                            key = val;
                        }
                    }

                    if (dataBean != null)
                    {
                        if (!this.idDataDic.ContainsKey(key))
                        {
                            idDataDic.Add(key, dataBean);
                            listIds.Add(Convert.ToInt32(key));
                        }
                        else
                        {
                            Log.Error($"---repeat key---{key}");
                        }
                        
                    }
                }
            }

            isInit = true;
        }


        /// <summary>
        /// Gets the xlsx txt path. Need overwrite.
        /// </summary>
        /// <returns>The xlsx path.</returns>
        protected virtual string GetXlsxPath()
        {
            return "";
        }


        /// <summary>
        /// Gets the type of the bean.Need overwrite.
        /// </summary>
        /// <returns>The bean type.</returns>
        protected virtual Type GetBeanType()
        {
            return null;
        }

        public IDataBean _GetDataById(object id)
        {
            if (idDataDic.ContainsKey(id))
            {
                return idDataDic[id];
            }
            else
            {
                return null;
            }
        }

        public int GetDataCount()
        {
            return this.idDataDic.Count;
        }
    }
    
    /// <summary>
    /// 附加转型类       2017.12.08  By Flamesky
    /// </summary>
    public static class ConvertHelper
    {
        #region = ChangeType =
        public static object ChangeType(object obj, Type conversionType)
        {
            return ChangeType(obj, conversionType, Thread.CurrentThread.CurrentCulture);
        }
        public static object ChangeType(object obj, Type conversionType, IFormatProvider provider)
        {
            #region Nullable
            Type nullableType = Nullable.GetUnderlyingType(conversionType);
            if (nullableType != null)
            {
                if (obj == null)
                {
                    return null;
                }
                return Convert.ChangeType(obj, nullableType, provider);
            }
            #endregion
            if (typeof(System.Enum).IsAssignableFrom(conversionType))
            {
                return Enum.Parse(conversionType, obj.ToString());
            }
            return Convert.ChangeType(obj, conversionType, provider);
        }
        #endregion
    }
}