﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;


namespace DivaTableManager
{
    public partial class Form1 : Form
    {
        //REGION: Declare some stuff for later use
        OpenFileDialog ofd = new OpenFileDialog();
        public static moduleEntry curModule;

        //END OF REGION: Declare some stuff for later use
        public Form1()
        {
            InitializeComponent();
            if (!Properties.Settings.Default.IsReset) 
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.IsReset = true;
            }
        }

        //REGION: Stuff that needs to be here as it impacts form elements
        //Reused code from ADPEdit, hi bestie

        public void refreshModuleList()
        {
            if (Code.moduleEntries != null) //check if list is null/empty
            {
                var dummy = new List<string>(); // make a new string list
                for (int i = 0; i < Code.moduleEntries.Count; i++) // count all the module entries
                {
                    var x = Code.moduleEntries[i];
                    string finalModuleName = x.name + " " + "(" + x.chara + ")";
                    dummy.Add(finalModuleName);  // list all modules
                }
                listBox1.DataSource = dummy; // listbox data source is now all modules
            }
            else { }
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            try
            {
                ofd.Filter = "Module Table files|*_module_tbl.farc|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (Code.moduleEntries != null)
                    {
                        Code.moduleEntries.Clear(); //clear list of module entries when opening a new file
                    }
                    Code.modulePath = ofd.FileName; //set this on file opening to also allow for saving to use the path
                    Code.readModuleFile(Code.modulePath);
                    populateCharaBox();
                    refreshModuleList();
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.SendToBack();
                }
                else
                {
                }
            }
            catch (Exception) { }
        }

        private void populateCharaBox()
        {
            string[] baseChara = { "MIKU", "RIN", "LEN", "LUKA", "KAITO", "MEIKO" };
            string[] noFChara = { "NERU", "HAKU", "SAKINE" };
            charaComboBox.Items.AddRange(baseChara);
            if (Code.moduleType == "F")
            {
                charaComboBox.Items.Add("EXTRA");
            }
            else
            {
                charaComboBox.Items.AddRange(noFChara);
            }
            if (Code.moduleType == "FT")
            {
                charaComboBox.Items.Add("TETO");
            }
        }

        //END OF REGION: Stuff that needs to be here as it impacts form elements
        //REGION: Detect changes to form information by user and apply to entry in moduleEntries list
        //(Mostly one line voids, unless it impacts the listBox)

        private void attrComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(curModule != null)
            {
                curModule.attr = Code.calcAttr(attrComboBox.Text);
            }
        }

        private void charaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (curModule != null)
            {
                if (charaComboBox.Text != null && listBox1.SelectedItem.ToString() != (curModule.name + " " + "(" + charaComboBox.Text + ")"))
                {
                    curModule.chara = charaComboBox.Text;
                    int indexStore = listBox1.SelectedIndex;
                    refreshModuleList();
                    listBox1.SelectedIndex = indexStore;
                }
            }
        }

        private void cosTextBox_TextChanged(object sender, EventArgs e)
        {
            if (curModule != null && cosTextBox.Text.Contains("COS_"))
            {
                curModule.cos = cosTextBox.Text;
            }
        }

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (Code.moduleEntries != null && e.KeyCode == Keys.Enter)
            {
                if (nameTextBox.Text != null && listBox1.SelectedItem.ToString() != (nameTextBox.Text + " " + "(" + curModule.chara + ")"))
                {
                    int indexStore = listBox1.SelectedIndex;
                    curModule.name = nameTextBox.Text;
                    refreshModuleList();
                    listBox1.SelectedIndex = indexStore;
                }
            }
        }

        private void ngCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (curModule != null)
            {
                if (ngCheck.Checked)
                {
                    curModule.ng = 1;
                }
                else
                {
                    curModule.ng = 0;
                }
            }
        }

        private void priceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (curModule != null && curModule.shop_price != priceTextBox.Text)
            {
                curModule.shop_price = priceTextBox.Text;
            }
        }

        private void indexTextBox_TextChanged(object sender, EventArgs e)
        {
            if (curModule != null)
            {
                curModule.sort_index = indexTextBox.Text;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (curModule != null)
            {
                curModule.shop_st_day = dateTimePicker1.Value.Day.ToString();
                curModule.shop_st_month = dateTimePicker1.Value.Month.ToString();
                curModule.shop_st_year = dateTimePicker1.Value.Year.ToString();
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (curModule != null)
            {
                curModule.shop_ed_day = dateTimePicker2.Value.Day.ToString();
                curModule.shop_ed_month = dateTimePicker2.Value.Month.ToString();
                curModule.shop_ed_year = dateTimePicker2.Value.Year.ToString();
            }
        }

        //END OF REGION: Detect changes and apply to entry
        //REGION: Read entry into form elements

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            curModule = Code.moduleEntries[listBox1.SelectedIndex];
            if (curModule != null && (Code.moduleType == "AC1" || Code.moduleType == "DT" || Code.moduleType == "DT1" || Code.moduleType == "AC" ))
            {
                EditTextBox.Enabled = false;
                attrComboBox.Enabled = false;
                if(Code.moduleType == "AC1" || Code.moduleType == "DT1")
                {
                    indexTextBox.Enabled = false;
                    SleeveTextBox.Enabled = true;
                    SleeveTextBox.Text = curModule.sleeve;
                }
                if (Code.moduleType == "AC" || Code.moduleType == "DT")
                {
                    SleeveTextBox.Enabled = false;
                    indexTextBox.Enabled = true;
                    indexTextBox.Text = curModule.sort_index;
                }
                charaComboBox.Text = curModule.chara;
                cosTextBox.Text = curModule.cos;
                idUpDown.Value = curModule.id;
                nameTextBox.Text = curModule.name;
                ngCheck.Enabled = true;
                priceTextBox.Text = curModule.shop_price;
                ngCheck.Checked = Code.checkNG(curModule.ng);
                var startDate = new DateTime(Int32.Parse(curModule.shop_st_year), Int32.Parse(curModule.shop_st_month), Int32.Parse(curModule.shop_st_day));
                var endDate = new DateTime(Int32.Parse(curModule.shop_ed_year), Int32.Parse(curModule.shop_ed_month), Int32.Parse(curModule.shop_ed_day));
                dateTimePicker1.Value = startDate;
                dateTimePicker2.Value = endDate;
            }
            if (curModule != null && Code.moduleType == "FT")
            {
                EditTextBox.Enabled = false;
                SleeveTextBox.Enabled = false;
                attrComboBox.Enabled = true;
                ngCheck.Enabled = true;
                attrComboBox.Text = Code.attrCalcText(curModule.attr);
                charaComboBox.Text = curModule.chara;
                cosTextBox.Text = curModule.cos;
                idUpDown.Value = curModule.id;
                nameTextBox.Text = curModule.name;
                priceTextBox.Text = curModule.shop_price;
                indexTextBox.Text = curModule.sort_index;
                ngCheck.Checked = Code.checkNG(curModule.ng);
                var startDate = new DateTime(Int32.Parse(curModule.shop_st_year), Int32.Parse(curModule.shop_st_month), Int32.Parse(curModule.shop_st_day));
                var endDate = new DateTime(Int32.Parse(curModule.shop_ed_year), Int32.Parse(curModule.shop_ed_month), Int32.Parse(curModule.shop_ed_day));
                dateTimePicker1.Value = startDate;
                dateTimePicker2.Value = endDate;
            }
            if (curModule != null && Code.moduleType == "F")
            {
                attrComboBox.Enabled = false;
                dateTimePicker1.Enabled = false;
                ngCheck.Enabled = false;
                dateTimePicker2.Enabled = false;
                EditTextBox.Enabled = true;
                SleeveTextBox.Enabled = false;
                indexTextBox.Enabled = true;
                EditTextBox.Text = curModule.edit_size;
                charaComboBox.Text = curModule.chara;
                cosTextBox.Text = curModule.cos;
                idUpDown.Value = curModule.id;
                nameTextBox.Text = curModule.name;
                priceTextBox.Text = curModule.shop_price;
                indexTextBox.Text = curModule.sort_index;
            }
            if (curModule != null)
            {
                Code.setPictureBox(curModule.id);
                pictureBox1.Image = Code.moduleImageBitmap;
            }
            modCount.Text = "Modules: " + Code.moduleEntries.Count;
        }

        private void addModuleEntry_Click(object sender, EventArgs e)
        {
            if (Code.moduleEntries.Count > 0)
            {
                Code.addDummyEntry();
                refreshModuleList();
                listBox1.SelectedIndex = Code.moduleEntries.Count - 1;
            }
            else { MessageBox.Show("Please open a table file before editing the list.", "Error"); }
        }

        private void delModuleEntry_Click(object sender, EventArgs e)
        {
            if (Code.moduleEntries.Count > 0)
            {
                int indexStore = listBox1.SelectedIndex;
                Code.moduleEntries.RemoveAt(listBox1.SelectedIndex);
                refreshModuleList();
                listBox1.SelectedIndex = indexStore - 1;
            }
            else { MessageBox.Show("Please open a table file before editing the list.", "Error"); }
        }

        //WinForms is a bitch sometimes so I need to have these here...
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Code.SaveButton_ClickModule();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormExtras.exitToolStripMenuItem_Click();
        }

        private void moduleTableHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormExtras.moduleTableHelpToolStripMenuItem_Click();
        }

        private void characterItemTableHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormExtras.characterItemTableHelpToolStripMenuItem_Click();
        }

        private void mentalHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormExtras.mentalHelpToolStripMenuItem_Click();
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormExtras.informationToolStripMenuItem_Click();
        }

        private void dDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormExtras.setDirectory2d();
        }

        private void mDATA2DDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormExtras.setDirectory2dMDATA();
        }

        private void idUpDown_ValueChanged(object sender, EventArgs e)
        {
            idCheckLabel.Text = "";
            idCheckLabel.ForeColor = System.Drawing.Color.Transparent;
            if (curModule != null && (int)idUpDown.Value != curModule.id)
            {
                bool checkIDUsage = Code.checkIDuse((int)idUpDown.Value);
                if (checkIDUsage)
                {
                    idCheckLabel.Text = "NG";
                    idCheckLabel.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    curModule.id = (int)idUpDown.Value;
                    idCheckLabel.Text = "OK";
                    idCheckLabel.ForeColor = System.Drawing.Color.Lime;
                }
            }
        }
        //STOP
    }
}