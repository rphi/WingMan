using System;
using System.Windows.Forms;
using WingMan.Osc;

namespace WingMan.Sources
{
    public partial class ArduinoConfigForm : Form
    {
        public ArduinoConfigObject ArduinoConfig;

        public ArduinoConfigForm(ArduinoConfigObject config)
        {
            InitializeComponent();
            ArduinoConfig = config;
            Shown += WhenShown;
            BuildConfigIfNew();
            BuildFaderDataGrid();
            BuildButtonDataGrid();
        }

        private void WhenShown(object o, EventArgs e)
        {
            
        }

        private void BuildButtonDataGrid()
        {
            buttonMapDataGridTypeColumn.DataSource = Enum.GetValues(typeof (OscButtonType));
            buttonMapDataGridTypeColumn.ValueType = typeof (OscButtonType);
            buttonMapDataGridTargetIdColumn.ValueType = typeof(int);
            var i = 1;
            foreach (var map in ArduinoConfig.ButtonMap)
            {
                if (map == null)
                {
                    buttonMapDataGrid.Rows.Add(i, OscButtonType.FireOnly, "", null);
                }
                else
                {
                    buttonMapDataGrid.Rows.Add(i, map.Type, map.Address, map.Id);
                }
                i++;
            }
        }

        private void BuildFaderDataGrid()
        {
            var i = 1;
            foreach (var map in ArduinoConfig.FaderMap)
            {
                faderMapDataGrid.Rows.Add(i, map);
                i++;
            }
        }

        private void BuildConfigIfNew()
        {
            if (ArduinoConfig.FaderMap == null)
            {
                // new fadermap
                ArduinoConfig.FaderMap = new string[ArduinoConfig.Faders];
            }
            if (ArduinoConfig.ButtonMap == null)
            {
                // new buttonmap
                ArduinoConfig.ButtonMap = new OscButtonCommandMap[ArduinoConfig.Buttons];
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (ReadButtonDataGrid() && ReadFaderDataGrid())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ReadFaderDataGrid()
        {
            var rows = faderMapDataGrid.Rows;
            var newFmapArray = new string[ArduinoConfig.Faders];
            foreach (DataGridViewRow row in rows)
            {
                newFmapArray[(int) row.Cells[0].Value - 1] = (string) row.Cells[1].Value;
            }
            ArduinoConfig.FaderMap = newFmapArray;
            return true;
        }

        private bool ReadButtonDataGrid()
        {
            var rows = buttonMapDataGrid.Rows;
            var newBmapArray = new OscButtonCommandMap[ArduinoConfig.Buttons];
            foreach (DataGridViewRow row in rows)
            {
                switch ((OscButtonType) row.Cells[1].Value)
                {
                    case OscButtonType.FireOnly:
                        newBmapArray[(int)row.Cells[0].Value - 1] = new OscButtonCommandMap((string) row.Cells[2].Value);
                        break;
                    case OscButtonType.SendId:
                        if (row.Cells[3].Value == null)
                        {
                            MessageBox.Show(
                                "Unable to save row " + row.Cells[0].Value +
                                " in button map as it is set to the type SendId and no Target ID has been provided. Please correct this before proceeding.",
                                "Error saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        newBmapArray[(int)row.Cells[0].Value - 1] = new OscButtonCommandMap((string)row.Cells[2].Value, (int) row.Cells[3].Value);
                        break;
                }
            }
            ArduinoConfig.ButtonMap = newBmapArray;
            return true;
        }
    }
}
