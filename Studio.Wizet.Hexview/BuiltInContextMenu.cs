using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Studio.Wizet.Hexview
{
    /// <summary>
    /// Defines a build-in ContextMenuStrip manager for ByteEditor control to show Copy, Cut, Paste menu in contextmenu of the control.
    /// </summary>
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public sealed class BuiltInContextMenu : Component
    {
        /// <summary>
        /// Contains the ByteEditor control.
        /// </summary>
        ByteEditor _ByteEditor;
        /// <summary>
        /// Contains the ContextMenuStrip control.
        /// </summary>
        ContextMenuStrip _contextMenuStrip;
        /// <summary>
        /// Contains the "Cut"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _cutToolStripMenuItem;
        /// <summary>
        /// Contains the "Copy"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _copyToolStripMenuItem;
        /// <summary>
        /// Contains the "Paste"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _pasteToolStripMenuItem;
        ToolStripMenuItem _copyBytesToolStripMenuItem;
        /// <summary>
        /// Contains the "Paste"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _pasteBytesToolStripMenuItem;
        /// <summary>
        /// Contains the "Select All"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _selectAllToolStripMenuItem;
        /// <summary>
        /// Initializes a new instance of BuildInContextMenu class.
        /// </summary>
        /// <param name="ByteEditor">the ByteEditor control</param>
        internal BuiltInContextMenu(ByteEditor ByteEditor)
        {
            _ByteEditor = ByteEditor;
            _ByteEditor.ByteProviderChanged += new EventHandler(ByteEditor_ByteProviderChanged);
        }
        /// <summary>
        /// If ByteProvider
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void ByteEditor_ByteProviderChanged(object sender, EventArgs e)
        {
            CheckBuiltInContextMenu();
        }
        /// <summary>
        /// Assigns the ContextMenuStrip control to the ByteEditor control.
        /// </summary>
        void CheckBuiltInContextMenu()
        {
            if (Util.DesignMode)
                return;

            if (this._contextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip();
                
                _cutToolStripMenuItem = new ToolStripMenuItem(CutMenuItemTextInternal, CutMenuItemImage, new EventHandler(CutMenuItem_Click));
                _cutToolStripMenuItem.BackColor = Color.Black;
                _cutToolStripMenuItem.ForeColor = Color.White;
                _cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
                cms.Items.Add(_cutToolStripMenuItem);

                _copyToolStripMenuItem = new ToolStripMenuItem(CopyMenuItemTextInternal, CopyMenuItemImage, new EventHandler(CopyMenuItem_Click));
                _copyToolStripMenuItem.BackColor = Color.Black;
                _copyToolStripMenuItem.ForeColor = Color.White;
                _copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
                cms.Items.Add(_copyToolStripMenuItem);

                _pasteToolStripMenuItem = new ToolStripMenuItem(PasteMenuItemTextInternal, PasteMenuItemImage, new EventHandler(PasteMenuItem_Click));
                _pasteToolStripMenuItem.BackColor = Color.Black;
                _pasteToolStripMenuItem.ForeColor = Color.White;
                _pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
                cms.Items.Add(_pasteToolStripMenuItem);

                _copyBytesToolStripMenuItem = new ToolStripMenuItem("Copy String", CopyMenuItemImage, new EventHandler(CopyStringMenuItem_Click));
                _copyBytesToolStripMenuItem.BackColor = Color.Black;
                _copyBytesToolStripMenuItem.ForeColor = Color.White;
                _copyBytesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.C;
                cms.Items.Add(_copyBytesToolStripMenuItem);

                _pasteBytesToolStripMenuItem = new ToolStripMenuItem("Paste String", PasteMenuItemImage, new EventHandler(PasteStringMenuItem_Click));
                _pasteBytesToolStripMenuItem.BackColor = Color.Black;
                _pasteBytesToolStripMenuItem.ForeColor = Color.White;
                _pasteBytesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.P;
                cms.Items.Add(_pasteBytesToolStripMenuItem);

                //cms.Items.Add(new ToolStripSeparator());

                _selectAllToolStripMenuItem = new ToolStripMenuItem(SelectAllMenuItemTextInternal, SelectAllMenuItemImage, new EventHandler(SelectAllMenuItem_Click));
                _selectAllToolStripMenuItem.BackColor = Color.Black;
                _selectAllToolStripMenuItem.ForeColor = Color.White;
                cms.Items.Add(_selectAllToolStripMenuItem);

                cms.Opening += new CancelEventHandler(BuildInContextMenuStrip_Opening);

                _contextMenuStrip = cms;
            }

            if (this._ByteEditor.ByteProvider == null && this._ByteEditor.ContextMenuStrip == this._contextMenuStrip)
                this._ByteEditor.ContextMenuStrip = null;
            else if (this._ByteEditor.ByteProvider != null && this._ByteEditor.ContextMenuStrip == null)
                this._ByteEditor.ContextMenuStrip = _contextMenuStrip;
        }
        /// <summary>
        /// Before opening the ContextMenuStrip, we manage the availability of the items.
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void BuildInContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            _cutToolStripMenuItem.Enabled = this._ByteEditor.CanCut();
            _copyToolStripMenuItem.Enabled = this._ByteEditor.CanCopy();
            _pasteToolStripMenuItem.Enabled = this._ByteEditor.CanPaste();
            _selectAllToolStripMenuItem.Enabled = this._ByteEditor.CanSelectAll();
        }
        /// <summary>
        /// The handler for the "Cut"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void CutMenuItem_Click(object sender, EventArgs e) { this._ByteEditor.Cut(); }
        /// <summary>
        /// The handler for the "Copy"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void CopyMenuItem_Click(object sender, EventArgs e) { this._ByteEditor.CopyHex(); }
        /// <summary>
        /// The handler for the "Paste"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void PasteMenuItem_Click(object sender, EventArgs e) { this._ByteEditor.PasteHex(); }


        void CopyStringMenuItem_Click(object sender, EventArgs e) { this._ByteEditor.Copy(); }
        /// <summary>
        /// The handler for the "Paste"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void PasteStringMenuItem_Click(object sender, EventArgs e) { this._ByteEditor.Paste(); }

        /// <summary>
        /// The handler for the "Select All"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void SelectAllMenuItem_Click(object sender, EventArgs e) { this._ByteEditor.SelectAll(); }
        /// <summary>
        /// Gets or sets the custom text of the "Copy" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string CopyMenuItemText
        {
            get { return _copyMenuItemText; }
            set { _copyMenuItemText = value; }
        } string _copyMenuItemText;

        /// <summary>
        /// Gets or sets the custom text of the "Cut" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string CutMenuItemText
        {
            get { return _cutMenuItemText; }
            set { _cutMenuItemText = value; }
        } string _cutMenuItemText;

        /// <summary>
        /// Gets or sets the custom text of the "Paste" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string PasteMenuItemText
        {
            get { return _pasteMenuItemText; }
            set { _pasteMenuItemText = value; }
        } string _pasteMenuItemText;

        /// <summary>
        /// Gets or sets the custom text of the "Select All" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string SelectAllMenuItemText
        {
            get { return _selectAllMenuItemText; }
            set { _selectAllMenuItemText = value; }
        } string _selectAllMenuItemText = null;

        /// <summary>
        /// Gets the text of the "Cut" ContextMenuStrip item.
        /// </summary>
        internal string CutMenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "Cut"; } }
        /// <summary>
        /// Gets the text of the "Copy" ContextMenuStrip item.
        /// </summary>
        internal string CopyMenuItemTextInternal { get { return !string.IsNullOrEmpty(CopyMenuItemText) ? CopyMenuItemText : "Copy"; } }
        /// <summary>
        /// Gets the text of the "Paste" ContextMenuStrip item.
        /// </summary>
        internal string PasteMenuItemTextInternal { get { return !string.IsNullOrEmpty(PasteMenuItemText) ? PasteMenuItemText : "Paste"; } }
        /// <summary>
        /// Gets the text of the "Select All" ContextMenuStrip item.
        /// </summary>
        internal string SelectAllMenuItemTextInternal { get { return !string.IsNullOrEmpty(SelectAllMenuItemText) ? SelectAllMenuItemText : "Select All"; } }

        /// <summary>
        /// Gets or sets the image of the "Cut" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image CutMenuItemImage
        {
            get { return _cutMenuItemImage; }
            set { _cutMenuItemImage = value; }
        } Image _cutMenuItemImage = null;
        /// <summary>
        /// Gets or sets the image of the "Copy" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image CopyMenuItemImage
        {
            get { return _copyMenuItemImage; }
            set { _copyMenuItemImage = value; }
        } Image _copyMenuItemImage = null;
        /// <summary>
        /// Gets or sets the image of the "Paste" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image PasteMenuItemImage
        {
            get { return _pasteMenuItemImage; }
            set { _pasteMenuItemImage = value; }
        } Image _pasteMenuItemImage = null;
        /// <summary>
        /// Gets or sets the image of the "Select All" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image SelectAllMenuItemImage
        {
            get { return _selectAllMenuItemImage; }
            set { _selectAllMenuItemImage = value; }
        } Image _selectAllMenuItemImage = null;
    }
}
