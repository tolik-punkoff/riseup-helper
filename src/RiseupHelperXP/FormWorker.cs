using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Data;

namespace RiseupHelperXP
{
    public class FormWorker
    {
        private object DataObject = null;
        private object DataForm = null;
        private Type ObjectType = null;

        private DataSet dsDataSet = null;
        private string TableName = string.Empty;

        private List<RadioButton> RadioButtons = null;        

        public string ErrorMessage { get; private set; }

        public FormWorker(object dataobject, object dataform)
        {
            DataObject = dataobject;
            DataForm = dataform;

            ObjectType = dataobject.GetType();            

            Form form = (Form)DataForm;
            RadioButtons = FindAllRadiobuttons(form.Controls);                        
        }

        public FormWorker(DataSet dataset, string tablename, object dataform)
        {
            dsDataSet = dataset;
            DataForm = dataform;
            TableName = tablename;            

            Form form = (Form)DataForm;
            RadioButtons = FindAllRadiobuttons(form.Controls);
        }

        private List<RadioButton> FindAllRadiobuttons(Control.ControlCollection collection)
        {
            List <RadioButton> result = new List<RadioButton>();
            foreach (Control ctrl in collection)
            {
                if (ctrl.HasChildren)
                {
                    result.AddRange(this.FindAllRadiobuttons(ctrl.Controls));
                }
                if (ctrl is RadioButton)
                {
                    result.Add((RadioButton)ctrl);
                }
            }

            return result;
        }

        private Control FindControl(string ControlName, Form form)
        {
            Control ctrl = null;

            Control[] buf = form.Controls.Find(ControlName, true);
            if (buf.Length == 0) return null;
            if (buf.Length > 1) return null;

            ctrl = buf[0];

            return ctrl;
        }

        private string GetEnumValFromRb(string PropName)
        {
            string EnumVal = string.Empty;
            string rbName = "rb"+PropName;            
            
            foreach (RadioButton rb in RadioButtons)
            {                
                if (rb.Checked)
                {
                    if (rb.Name.StartsWith(rbName))
                    {
                        EnumVal = rb.Name.Substring(rbName.Length);
                    }
                }
            }

            return EnumVal;
        }

        private bool IsEnum(object TestObject)
        {
            try
            {
                Enum en = (Enum)TestObject;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void FillForm()
        {
            //получаем поля объекта
            PropertyInfo [] properties = ObjectType.GetProperties();
            Form form = (Form)DataForm;

            foreach (PropertyInfo pr in properties)
            {
                Type propType = pr.PropertyType; //тип свойства - от него и будем танцевать
                object propValue = pr.GetValue(DataObject, null);                

                if (propType == typeof(bool))
                {
                    CheckBox check = (CheckBox)FindControl("chk" + pr.Name, form);

                    if (check != null)
                    {
                        check.Checked = (bool)propValue;
                    }
                }

                if ((propType == typeof(int)) || (propType == typeof(string)))
                {
                    TextBox text = (TextBox)FindControl("txt" + pr.Name, form);

                    if (text != null)
                    {
                        if (propValue != null)
                        {
                            text.Text = propValue.ToString();
                        }
                        else
                        {
                            text.Text = string.Empty;
                        }
                    }
                }

                if (propType.IsEnum)
                {
                    string rbname = "rb" + pr.Name + 
                        Enum.GetName(propValue.GetType(),propValue);
                    RadioButton rb = (RadioButton)FindControl(rbname,form);

                    if (rb != null)
                    {
                        rb.Checked = true;
                    }
                }
            }
        }

        public bool GetData()
        {
            PropertyInfo [] properties = ObjectType.GetProperties();
            Form form = (Form)DataForm;

            foreach (PropertyInfo pr in properties)
            {
                Type propType = pr.PropertyType;
                object propValue = pr.GetValue(DataObject, null);

                if (propType == typeof(bool))
                {
                    CheckBox check = (CheckBox)FindControl("chk" + pr.Name,form);

                    if (check != null)
                    {
                        pr.SetValue(DataObject, check.Checked, null);
                    }
                }

                if ((propType == typeof(string)) || (propType == typeof(int)))
                {
                    TextBox text = (TextBox)FindControl("txt" + pr.Name,form);

                    if (text != null)
                    {                        
                        if (propType == typeof(string))
                        {
                            pr.SetValue(DataObject, text.Text, null);
                        }
                        if (propType == typeof(int))
                        {
                            int val = 0;
                            try
                            {
                                val = Convert.ToInt32(text.Text);
                                pr.SetValue(DataObject, val, null);
                            }
                            catch (Exception ex)
                            {
                                ErrorMessage = ex.Message;
                                return false;
                            }
                        }
                    }
                }

                if (propType.IsEnum)
                {
                    string val = GetEnumValFromRb(pr.Name);
                    if (!string.IsNullOrEmpty(val))
                    {
                        object enumObj = Enum.Parse(propValue.GetType(), val);
                        pr.SetValue(DataObject, enumObj, null);
                    }
                }
            }

            return true;
        }

        public bool AddOrEditRow(object idValue)
        {
            Form form = (Form)DataForm;
            DataRow row = null;
            if (idValue == null) //добавляем
            {
                row = dsDataSet.Tables[TableName].NewRow();
            }
            else //редактируем
            {
                row = dsDataSet.Tables[TableName].Rows.Find(idValue);
                if (row == null)
                {
                    ErrorMessage = "Not found " + idValue.ToString();
                    return false;
                }
            }

            foreach (DataColumn col in dsDataSet.Tables[TableName].Columns)
            {
                Type colType = col.DataType;
                string colName = col.ColumnName;

                if (colType == typeof(bool))
                {
                    CheckBox check = (CheckBox)FindControl("chk" + colName, form);

                    if (check != null)
                    {
                        row[colName]=check.Checked;
                    }
                }

                if ((colType == typeof(string)) || (colType == typeof(int)))
                {
                    TextBox text = (TextBox)FindControl("txt" + colName, form);

                    if (text != null)
                    {
                        if (colType == typeof(string))
                        {
                            row[colName] = text.Text;
                        }
                        if (colType == typeof(int))
                        {
                            int val = 0;
                            try
                            {
                                val = Convert.ToInt32(text.Text);
                                row[colName]=val;
                            }
                            catch (Exception ex)
                            {
                                ErrorMessage = ex.Message;
                                return false;
                            }
                        }
                    }                    

                }                
                
                if (colType.IsEnum)
                {
                    string val = GetEnumValFromRb(colName);
                    if (!string.IsNullOrEmpty(val))
                    {
                        object enumObj = Enum.Parse(colType, val);
                        row[colName] = enumObj;
                    }
                }
            }

            if (idValue == null) //добавляем новую запись
            {
                dsDataSet.Tables[TableName].Rows.Add(row);
            }

            return true;
        }

        public bool FillFormFromDataSet(object idValue)
        {
            Form form = (Form)DataForm;
            DataRow row = dsDataSet.Tables[TableName].Rows.Find(idValue);
            if (row == null)
            {
                ErrorMessage = "Not found " + idValue.ToString();
                return false;
            }

            foreach (DataColumn col in dsDataSet.Tables[TableName].Columns)
            {
                Type colType = col.DataType;
                string colName = col.ColumnName;
                object colValue = row[colName];

                if (colType == typeof(bool))
                {
                    CheckBox check = (CheckBox)FindControl("chk" + colName, form);

                    if (check != null)
                    {
                        check.Checked = (bool)colValue;
                    }
                }

                if ((colType == typeof(int)) || (colType == typeof(string)))
                {
                    TextBox text = (TextBox)FindControl("txt" + colName, form);

                    if (text != null)
                    {
                        if (colValue != null)
                        {
                            text.Text = colValue.ToString();
                        }
                        else
                        {
                            text.Text = string.Empty;
                        }
                    }
                }

                if (colType.IsEnum)
                {
                    RadioButton rb = null;
                    if (colValue != DBNull.Value)
                    {
                        string rbname = "rb" + colName +
                            Enum.GetName(colType, colValue);
                        rb = (RadioButton)FindControl(rbname, form);
                    }

                    if (rb != null)
                    {
                        rb.Checked = true;
                    }
                }
            }
            return true;
        }
    }
}
