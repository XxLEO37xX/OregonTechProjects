using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CST238Assign1
{
    public partial class Window : Form
    {
        // current image state
        private Bitmap currentBitmap;
        private string currentFilePath;
        private bool isDirty = false;

        private const int DefaultNewWidth = 800;
        private const int DefaultNewHeight = 600;
        // drawing state
        private enum Tool { None, Pencil, Rectangle, Ellipse, Line }
        private Tool currentTool = Tool.None;
        private Color currentColor = Color.Black;
        private bool fillShapes = false;
        private Point lastPoint; // for pencil drawing
        private bool isDrawing = false;
        private Point startPoint;
        private Point currentPoint;
        private PictureBox canvas => this.canvasPictureBox;
        public Window()
        {
            InitializeComponent();
            // default tool and color
            currentTool = Tool.Rectangle;
            currentColor = Color.Black;
            // reflect defaults in the UI
            try
            {
                rectangleToolStripMenuItem.Checked = true;
                blackToolStripMenuItem.Checked = true;
            }
            catch { }

            // ensure the canvas and backing bitmap exist at startup
            if (canvas != null)
            {
                canvas.Cursor = Cursors.Cross;
                EnsureBitmapSize();
            }
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Rectangle;
            UpdateToolChecks();
            UpdateStatus();
        }

        private void pencilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Pencil;
            UpdateToolChecks();
            UpdateStatus();
        }

        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Ellipse;
            UpdateToolChecks();
            UpdateStatus();
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Line;
            UpdateToolChecks();
            UpdateStatus();
        }

        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle fill mode
            fillShapes = !fillShapes;
            try { fillToolStripMenuItem.Checked = fillShapes; } catch { }
            UpdateStatus();
        }

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.Black;
            UpdateColorChecks();
            UpdateStatus();
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.White;
            UpdateColorChecks();
            UpdateStatus();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.Red;
            UpdateColorChecks();
            UpdateStatus();
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.Blue;
            UpdateColorChecks();
            UpdateStatus();
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.Green;
            UpdateColorChecks();
            UpdateStatus();
        }

        private void UpdateToolChecks()
        {
            try
            {
                pencilToolStripMenuItem.Checked = (currentTool == Tool.Pencil);
                rectangleToolStripMenuItem.Checked = (currentTool == Tool.Rectangle);
                ellipseToolStripMenuItem.Checked = (currentTool == Tool.Ellipse);
                lineToolStripMenuItem.Checked = (currentTool == Tool.Line);
            }
            catch { }
        }

        private void UpdateColorChecks()
        {
            try
            {
                blackToolStripMenuItem.Checked = (currentColor.ToArgb() == Color.Black.ToArgb());
                whiteToolStripMenuItem.Checked = (currentColor.ToArgb() == Color.White.ToArgb());
                redToolStripMenuItem.Checked = (currentColor.ToArgb() == Color.Red.ToArgb());
                blueToolStripMenuItem.Checked = (currentColor.ToArgb() == Color.Blue.ToArgb());
                greenToolStripMenuItem.Checked = (currentColor.ToArgb() == Color.Green.ToArgb());
                // custom if none of the above
                try
                {
                    var isCustom = !(blackToolStripMenuItem.Checked || whiteToolStripMenuItem.Checked || redToolStripMenuItem.Checked || blueToolStripMenuItem.Checked || greenToolStripMenuItem.Checked);
                    customColorToolStripMenuItem.Checked = isCustom;
                }
                catch { }
            }
            catch { }
        }

        private void customColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                dlg.Color = currentColor;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    currentColor = dlg.Color;
                    UpdateColorChecks();
                    UpdateStatus();
                }
            }
        }

        private void UpdateStatus()
        {
            try
            {
                if (toolStatusLabel != null)
                {
                    toolStatusLabel.Text = $"Tool: {currentTool}    Color: {currentColor.Name}    Fill: {(fillShapes?"On":"Off")}";
                }
            }
            catch { }
        }

        private void canvasPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            // ensure we have a bitmap to draw on
            if (currentBitmap == null)
            {
                var bmp = new Bitmap(Math.Max(1, canvas.Width), Math.Max(1, canvas.Height));
                using (var g = Graphics.FromImage(bmp)) g.Clear(Color.White);
                ReplaceCurrentBitmap(bmp, null);
            }

            isDrawing = true;
            startPoint = e.Location;
            currentPoint = e.Location;
            lastPoint = e.Location;
            // For pencil. start with a single dot
            if (currentTool == Tool.Pencil)
            {
                using (var g = Graphics.FromImage(currentBitmap))
                using (var pen = new Pen(currentColor, 2))
                {
                    g.DrawLine(pen, lastPoint, lastPoint);
                }
                canvas.Invalidate();
                isDirty = true;
            }
        }

        private void canvasPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
                return;

            currentPoint = e.Location;
            if (currentTool == Tool.Pencil)
            {
                // draw directly to bitmap for freehand
                try
                {
                    using (var g = Graphics.FromImage(currentBitmap))
                    using (var pen = new Pen(currentColor, 2))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.DrawLine(pen, lastPoint, currentPoint);
                    }
                    lastPoint = currentPoint;
                    canvas.Invalidate();
                    isDirty = true;
                }
                catch { }
                return;
            }

            // request repaint to show preview overlay for other tools
            canvas.Invalidate();
        }

        private void canvasPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
                return;

            isDrawing = false;
            currentPoint = e.Location;

            if (currentTool == Tool.Pencil)
            {
                // already drawn during MouseMove; nothing more to commit
                canvas.Invalidate();
                UpdateTitle();
                return;
            }

            // commit to current bitmap for non-pencil tools
            try
            {
                using (var g = Graphics.FromImage(currentBitmap))
                using (var pen = new Pen(currentColor, 2))
                {
                    DrawShape(g, pen, startPoint, currentPoint);
                }

                // refresh display (Paint will draw overlay over the PictureBox image)
                canvas.Invalidate();

                isDirty = true;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to draw: " + ex.Message, "Draw Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawShape(Graphics g, Pen pen, Point p1, Point p2)
        {
            if (currentTool == Tool.None)
                return;

            var rect = new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p2.X - p1.X), Math.Abs(p2.Y - p1.Y));

            switch (currentTool)
            {
                case Tool.Rectangle:
                    if (fillShapes)
                    {
                        using (var brush = new SolidBrush(currentColor))
                        {
                            g.FillRectangle(brush, rect);
                        }
                        // optional outline
                        g.DrawRectangle(pen, rect);
                    }
                    else
                    {
                        g.DrawRectangle(pen, rect);
                    }
                    break;
                case Tool.Ellipse:
                    if (fillShapes)
                    {
                        using (var brush = new SolidBrush(currentColor))
                        {
                            g.FillEllipse(brush, rect);
                        }
                        g.DrawEllipse(pen, rect);
                    }
                    else
                    {
                        g.DrawEllipse(pen, rect);
                    }
                    break;
                case Tool.Line:
                    g.DrawLine(pen, p1, p2);
                    break;
            }
        }

        private void Window_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptToSave())
                return;

            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*";
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.Multiselect = false;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        // Load into memory to avoid locking the file on disk
                        var bytes = File.ReadAllBytes(dlg.FileName);
                        using (var ms = new MemoryStream(bytes))
                        using (var img = Image.FromStream(ms))
                        {
                            var bmp = new Bitmap(img);
                            ReplaceCurrentBitmap(bmp, dlg.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Failed to open image: " + ex.Message, "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptToSave())
                return;

            // create a blank white bitmap
            var bmp = new Bitmap(DefaultNewWidth, DefaultNewHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
            }
            ReplaceCurrentBitmap(bmp, null);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!PromptToSave())
            {
                e.Cancel = true;
            }
            else
            {
                // dispose bitmap
                if (currentBitmap != null)
                {
                    currentBitmap.Dispose();
                    currentBitmap = null;
                }
            }
        }

        // Helpers
        private void ReplaceCurrentBitmap(Bitmap bmp, string filePath)
        {
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
            currentBitmap = bmp;
            currentFilePath = filePath;
            isDirty = false;
            UpdateTitle();
            // show on canvas
            if (canvas != null)
            {
                canvas.Image = currentBitmap;
            }
        }

        private void canvasPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // PictureBox will draw its Image (currentBitmap). just draw the preview overlay while dragging.
            if (isDrawing && currentBitmap != null && currentTool != Tool.None)
            {
                using (var pen = new Pen(currentColor, 2))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    DrawShape(e.Graphics, pen, startPoint, currentPoint);
                }
            }
        }

        private void canvasPictureBox_Resize(object sender, EventArgs e)
        {
            EnsureBitmapSize();
            canvas.Invalidate();
        }

        private void EnsureBitmapSize()
        {
            if (canvas == null)
                return;
            var w = Math.Max(1, canvas.Width);
            var h = Math.Max(1, canvas.Height);

            if (currentBitmap == null)
            {
                var bmp = new Bitmap(w, h);
                using (var g = Graphics.FromImage(bmp)) g.Clear(Color.White);
                // don't mark dirty when just creating canvas
                currentBitmap = bmp;
                currentFilePath = null;
                if (canvas != null) canvas.Image = currentBitmap;
                return;
            }

            // Instead of shrinking the backing bitmap (which would erase art in the bitmap), grow it when needed
            var newW = Math.Max(currentBitmap.Width, w);
            var newH = Math.Max(currentBitmap.Height, h);

            if (newW == currentBitmap.Width && newH == currentBitmap.Height)
                return; // no change

            var newBmp = new Bitmap(newW, newH);
            using (var g = Graphics.FromImage(newBmp))
            {
                g.Clear(Color.White);
                // copy existing content to top-left
                g.DrawImage(currentBitmap, 0, 0);
            }
            currentBitmap.Dispose();
            currentBitmap = newBmp;
            if (canvas != null) canvas.Image = currentBitmap;
        }

        private void UpdateTitle()
        {
            var name = string.IsNullOrEmpty(currentFilePath) ? "Untitled" : Path.GetFileName(currentFilePath);
            this.Text = $"CST 238 Drawing - {name}" + (isDirty ? " *" : "");
        }

        private bool PromptToSave()
        {
            if (!isDirty)
                return true;

            var displayName = string.IsNullOrEmpty(currentFilePath) ? "Untitled" : Path.GetFileName(currentFilePath);
            var result = MessageBox.Show(this, $"Save changes to {displayName}?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return false;
            if (result == DialogResult.Yes)
            {
                return Save(false);
            }
            return true; // No = continue without saving
        }

        private bool Save(bool saveAs)
        {
            if (currentBitmap == null)
            {
                MessageBox.Show(this, "There is no image to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (string.IsNullOrEmpty(currentFilePath) || saveAs)
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp";
                    dlg.DefaultExt = "png";
                    dlg.AddExtension = true;
                    dlg.OverwritePrompt = true;
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return false;

                    currentFilePath = dlg.FileName;
                }
            }

            try
            {
                var ext = Path.GetExtension(currentFilePath).ToLowerInvariant();
                ImageFormat fmt = ImageFormat.Png;
                if (ext == ".jpg" || ext == ".jpeg") fmt = ImageFormat.Jpeg;
                else if (ext == ".bmp") fmt = ImageFormat.Bmp;

                // Save to a temp file first to avoid partial writes
                var temp = Path.GetTempFileName();
                currentBitmap.Save(temp, fmt);
                File.Copy(temp, currentFilePath, true);
                File.Delete(temp);

                isDirty = false;
                UpdateTitle();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to save image: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void canvasPictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}
