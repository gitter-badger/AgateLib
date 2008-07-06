using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using ERY.AgateLib.Resources;

namespace ResourceEditor
{
    public partial class frmResourceEditor : Form
    {
        AgateResourceManager mResources;

        AgateResourceManager Resources
        {
            get { return mResources; }
            set
            {
                mResources = value;

                stringTableEditor1.ResourceManager = mResources;
            }
        }

        public frmResourceEditor()
        {
            InitializeComponent();

            statusLabel.Text = "";

            UpdateControls();
        }

        public string ShortName
        {
            get { return Path.GetFileName(Resources.Filename); }
        }

        bool updatingControls = false;
        private void UpdateControls()
        {
            if (updatingControls)                return;
            if (IsDisposed)                return;

            updatingControls = true;

            try
            {
                bool save = true;
                bool saveAs = true;
                bool cut = true;
                bool copy = true;
                bool paste = true;
                bool delete = true;
                bool hasDocument = Resources != null;

                if (Resources == null)
                {
                    save = saveAs = cut = copy = paste = delete = false;

                    mainbook.Visible = false;
                    cboLanguages.Items.Clear();

                    btnRemoveLanguage.Enabled = false;
                }
                else
                {
                    cboLanguages.Enabled = true;
                    mainbook.Visible = true;

                    foreach (ResourceGroup lang in Resources.Languages)
                    {
                        if (cboLanguages.Items.Contains(lang.LanguageName))
                            continue;

                        cboLanguages.Items.Add(lang.LanguageName);
                    }
                    for (int i = 0; i < cboLanguages.Items.Count; i++)
                    {
                        if (Resources.Languages.Contains(cboLanguages.Items[i].ToString()) == false)
                        {
                            cboLanguages.Items.RemoveAt(i);
                            i--;
                        }
                    }

                    if (cboLanguages.SelectedIndex == -1)
                        cboLanguages.SelectedIndex = 0;

                    if (cboLanguages.SelectedItem.ToString().Equals(Resources.CurrentLanguage.LanguageName, 
                        StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        cboLanguages.SelectedItem = Resources.CurrentLanguage.LanguageName;
                    }

                    if (cboLanguages.SelectedItem.ToString().ToLower() == "default")
                        btnRemoveLanguage.Enabled = false;
                    else
                        btnRemoveLanguage.Enabled = true;
                }

                saveAsToolStripMenuItem.Enabled = saveAs;
                saveToolStripMenuItem.Enabled = save;
                btnSave.Enabled = save;

                cutToolStripMenuItem.Enabled = cut;
                btnCut.Enabled = cut;

                copyToolStripMenuItem.Enabled = copy;
                btnCopy.Enabled = copy;

                pasteToolStripMenuItem.Enabled = paste;
                btnPaste.Enabled = paste;

                deleteToolStripMenuItem.Enabled = delete;

                cboLanguages.Enabled = hasDocument;
                btnAddLanguage.Enabled = hasDocument;
            }
            finally
            {
                updatingControls = false;
            }
        }

        #region --- File Menu ---

        private void newResourceFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(Resources);
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(Resources);
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            New();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            Open();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save(Resources);
        }


        private void New()
        {
            if (CloseFile() == false)
                return;

            AgateResourceManager newRes = new AgateResourceManager();

            if (SaveAs(newRes) == false)
                return;

            Resources = newRes;

            UpdateControls();
        }
        private void Open()
        {
            if (CloseFile() == false)
                return;

            if (openFileDialog.ShowDialog(this) == DialogResult.Cancel)
                return;

            Resources = new AgateResourceManager();
            Resources.Load(openFileDialog.FileName);

            UpdateControls();
        }
        /// <summary>
        /// Returns true if the file could be closed and an existing operation
        /// should continue.
        /// </summary>
        /// <returns></returns>
        private bool CloseFile()
        {
            if (Resources == null)
                return true;

            DialogResult result = MessageBox.Show(this, 
                "Save changes to " + ShortName + "?",
                "Resource Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, 
                MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                return Save(Resources);
            }

            UpdateControls();
            return true;
        }
        /// <summary>
        /// Returns true if the file was saved and an existing operation should
        /// continue.
        /// </summary>
        /// <returns></returns>
        private bool Save(AgateResourceManager resources)
        {
            try
            {
                resources.Save();

                return true;
            }
            catch (IOException e)
            {
                MessageBox.Show("An error has occurred:" + Environment.NewLine +
                    e.Message);

                return false;
            }
        }
        /// <summary>
        /// Returns true if the file was saved and an existing operation should
        /// continue.
        /// </summary>
        private bool SaveAs(AgateResourceManager resources)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                resources.Filename = saveFileDialog.FileName;

                return Save(resources);
            }
            else
                return false;
        }
        private void Exit()
        {
            if (CloseFile())
            {
                this.Close();
            }

            UpdateControls();
        }

        #endregion
        #region --- Edit Menu ---

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            Cut();
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }
        private void btnPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void Cut()
        {
        }
        private void Copy()
        {
        }
        private void Paste()
        {
        }
        private void Delete()
        {
        }

        #endregion

        private void cboLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Resources.SetCurrentLanguage(cboLanguages.SelectedItem.ToString());

            UpdateControls();
        }

        private void btnAddLanguage_Click(object sender, EventArgs e)
        {
            AddLanguage();
        }

        private void AddLanguage()
        {
            frmNewLanguage frm = new frmNewLanguage(Resources);

            frm.ShowDialog(this);

            UpdateControls();
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmResourceEditor());
        }

        private void stringTableEditor1_StatusText(object sender, StatusTextEventArgs e)
        {
            statusLabel.Text = e.Text;
        }

        private void frmResourceEditor_Load(object sender, EventArgs e)
        {

        }

    }
}