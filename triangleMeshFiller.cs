using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GK1_PROJ2
{
    public partial class mainWindow : Form
    {
        private List<Vertex> vertices;
        private List<Polygon> polygons;
        private List<Vector> normals;
        private float minX;
        private float minY;
        private float minZ;
        private float maxX;
        private float maxY;
        private float maxZ;
        private Bitmap drawArea;
        private Color canvasColor;
        private static Brush blackBrush = Brushes.Black;
        private const int pointRadious = 4;
        private static Pen edgePen = new Pen(blackBrush, 2);
        private const int padding = 10;
        private const int kMin = 0;
        private const int kMax = 0;
        private const int mMin = 1;
        private const int mMax = 100;
        //Image image;

        public mainWindow()
        {
            InitializeComponent();
            vertices = new List<Vertex>();
            polygons = new List<Polygon>();
            normals = new List<Vector>();
            canvasColor = Color.HotPink;
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            //image = Image.FromFile(System.IO.Path.GetFullPath(@"..\..\..\") + @"\defaultObjectColor.jpg");
            //drawArea = new Bitmap(image, new Size(canvas.Width, canvas.Height));
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
                g.Clear(canvasColor);
            recalcSliders();
        }
        private void clearCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clean();
        }
        private void loadobjFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Wavefront .obj file|*.obj";
                dialog.Title = "Load .obj file";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    clean();
                    if (processFile(dialog.FileName))
                    {
                        rescaleVerticies();
                        repaint();
                        MessageBox.Show("Succesfully loaded " + polygons.Count.ToString() + " polygons.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //Debug.WriteLine("minX = " + minX.ToString() + ", maxX = " + maxX.ToString() + ".");
                        //Debug.WriteLine("minY = " + minY.ToString() + ", maxY = " + maxY.ToString() + ".");
                        //Debug.WriteLine("minZ = " + minZ.ToString() + ", maxZ = " + maxZ.ToString() + ".");
                    }
                }
            }
        }
        private bool processFile(string path)
        {
            string[] parts;
            string[] parts2;
            float x = 0;
            float y = 0;
            float z = 0;
            int v = 0;
            int vn = 0;

            try
            {
                foreach (string line in File.ReadLines(path))
                {
                    parts = line.Split(' ');

                    switch (parts[0])
                    {
                        case "v":
                            {
                                x = float.Parse(parts[1]);
                                y = float.Parse(parts[2]);
                                z = float.Parse(parts[3]);
                                vertices.Add(new Vertex(x, y, z));
                                if (x < minX)
                                    minX = x;
                                if (x > maxX)
                                    maxX = x;
                                if (y < minY)
                                    minY = y;
                                if (y > maxY)
                                    maxY = y;
                                if (z < minZ)
                                    minZ = z;
                                if (z > maxZ)
                                    maxZ = z;
                                break;
                            }
                        case "vn":
                            {
                                x = float.Parse(parts[1]);
                                y = float.Parse(parts[2]);
                                z = float.Parse(parts[3]);
                                normals.Add(new Vector(x, y, z));
                                break;
                            }
                        case "f":
                            {

                                polygons.Add(new Polygon());
                                for (int i = 1; i < parts.Length; i++)
                                {
                                    parts2 = parts[i].Split('/');
                                    v = int.Parse(parts2[0]) - 1;
                                    vn = int.Parse(parts2[2]) - 1;
                                    vertices[v].normal = normals[vn];
                                    polygons[polygons.Count - 1].verticies.Add(vertices[v]);
                                }
                                break;
                            }
                        default:
                            {
                                // exeption maybe?
                                break;
                            }
                    }
                }

                if (vertices.Count > 0 && normals.Count > 0 && polygons.Count > 0)
                    return true;
                else
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Unable to load selected .obj file.", "Exeption while loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void rescaleVerticies()
        {
            var height = canvas.Height - padding;
            var width = canvas.Width - padding;

            bool Y = ((height / (maxY - minY)) < (width / (maxX - minX)));
            float k;
            if (Y)
            {
                k = height / (maxY - minY);
                foreach (var v in vertices)
                {
                    v.y -= minY;
                    v.y *= k;
                    v.y += (padding / 2);
                    v.x -= minX;
                    v.x += (width - height) / 2;
                    v.x *= k;
                    v.x += (padding / 2);
                    v.z -= minZ;
                    v.z *= k;
                    v.normal.x *= k;
                    v.normal.y *= k;
                    v.normal.z *= k;
                }
            }
            else
            {
                k = width / (maxX - minX);
                foreach (var v in vertices)
                {
                    v.x -= minX;
                    v.x *= k;
                    v.x += (padding / 2);
                    v.y -= minY;
                    v.y += (height - width) / 2;
                    v.y *= k;
                    v.y += (padding / 2);
                    v.z -= minZ;
                    v.z *= k;
                    v.normal.x *= k;
                    v.normal.y *= k;
                    v.normal.z *= k;
                }
            }
        }
        private void repaint()
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                if (objectColorSolidModeRbutton.Checked)
                {
                    canvasColor = objectColorSolidTxtBox.BackColor;
                }
                g.Clear(canvasColor);

                foreach (var p in polygons)
                    for (int i = 0; i < p.verticies.Count; i++)
                    {
                        if (showVerticiesToolStripMenuItem.Checked)
                            paintPoint(g, p.verticies[i].x, p.verticies[i].y, blackBrush);
                        if (showEdgesToolStripMenuItem.Checked)
                        {
                            int inext = (i + 1 == p.verticies.Count) ? 0 : i + 1;
                            PointF start = new PointF(p.verticies[i].x, p.verticies[i].y);
                            PointF end = new PointF(p.verticies[inext].x, p.verticies[inext].y);
                            g.DrawLine(edgePen, start, end);
                        }
                    }
            }
            canvas.Refresh();
        }
        private void paintPoint(Graphics g, float x, float y, Brush color, int rad = pointRadious)
        {
            g.FillEllipse(color, x - rad, y - rad, rad * 2, rad * 2);
        }
        private void clean()
        {
            vertices = new List<Vertex>();
            polygons = new List<Polygon>();
            normals = new List<Vector>();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
                g.Clear(canvasColor);
        }
        private void kdTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double val = kdTrackBar.Value;
            val /= 1000;
            kdTxtBox.Text = val.ToString("0.000");
        }
        private void ksTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double val = ksTrackBar.Value;
            val /= 1000;
            ksTxtBox.Text = val.ToString("0.000");
        }
        private void mTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double val = mTrackBar.Value;
            val /= 1000;
            mTxtBox.Text = val.ToString("0.000");
        }
        private void changeColorButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.Color = lightSourceColorPreview.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    lightSourceColorPreview.BackColor = colorDialog.Color;
            }
        }
        private void showToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            repaint();
        }
        private void recalcSliders()
        {
            this.lightSourceAltitudeTrackBar.Maximum = this.canvas.Height * 1000;
            this.lightSourceAltitudeTrackBar.Value = (this.lightSourceAltitudeTrackBar.Maximum - this.lightSourceAltitudeTrackBar.Minimum) / 2 + this.lightSourceAltitudeTrackBar.Minimum;
            this.lightSourceAltitudeTxtBox.Text = (((double)(this.lightSourceAltitudeTrackBar.Maximum - this.lightSourceAltitudeTrackBar.Minimum) / 2 + this.lightSourceAltitudeTrackBar.Minimum) / 1000).ToString("0.000");
            this.mTrackBar.Value = (this.mTrackBar.Maximum - this.mTrackBar.Minimum) / 2 + this.mTrackBar.Minimum;
            this.mTxtBox.Text = (((double)(this.mTrackBar.Maximum - this.mTrackBar.Minimum) / 2 + this.mTrackBar.Minimum) / 1000).ToString("0.000");
            this.ksTrackBar.Value = (this.ksTrackBar.Maximum - this.ksTrackBar.Minimum) / 2 + this.ksTrackBar.Minimum;
            this.ksTxtBox.Text = (((double)(this.ksTrackBar.Maximum - this.ksTrackBar.Minimum) / 2 + this.ksTrackBar.Minimum) / 1000).ToString("0.000");
            this.kdTrackBar.Value = (this.kdTrackBar.Maximum - this.kdTrackBar.Minimum) / 2 + this.kdTrackBar.Minimum;
            this.kdTxtBox.Text = (((double)(this.kdTrackBar.Maximum - this.kdTrackBar.Minimum) / 2 + this.kdTrackBar.Minimum) / 1000).ToString("0.000");

        }
        private void lightSourceAltitudeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double val = lightSourceAltitudeTrackBar.Value;
            val /= 1000;
            lightSourceAltitudeTxtBox.Text = val.ToString("0.000");
        }
        private void objectColorLoadDefaultButton_Click(object sender, EventArgs e)
        {
            objectColorSolidModeRbutton.Checked = false;
            objectColorTextureModeRbutton.Checked = false;
            repaint();
        }
        private void objectColorSolidModeRbutton_CheckedChanged(object sender, EventArgs e)
        {
            var casted = (RadioButton)sender;
            if (casted.Checked)
                objectColorSolidChangeButton.Enabled = true;
            else
                objectColorSolidChangeButton.Enabled = false;
            repaint();
        }
        private void objectColorTextureModeRbutton_CheckedChanged(object sender, EventArgs e)
        {
            var casted = (RadioButton)sender;
            if (casted.Checked)
                objectColorTextureLoadButton.Enabled = true;
            else
                objectColorTextureLoadButton.Enabled = false;
            repaint();
        }
        private void objectColorSolidChangeButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.Color = lightSourceColorPreview.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    objectColorSolidTxtBox.BackColor = colorDialog.Color;
                    repaint();
                }  
            }
        }
        private void objectColorTextureLoadButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Load texture";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var separated = dialog.FileName.Split("\\");
                    var last = separated[separated.Length - 1];
                    objectColorTextureTxtBox.Text = last;
                }
            }
        }
        private void calculatedAtPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vetrexInterpolationToolStripMenuItem.Checked = false;
        }
        private void vetrexInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculatedAtPointToolStripMenuItem.Checked = false;
        }
        private void modifyNormalsCbutton_CheckedChanged(object sender, EventArgs e)
        {
            var casted = (CheckBox)sender;
            if (casted.Checked)
                normalMapLoadButton.Enabled = true;
            else
                normalMapLoadButton.Enabled = false;
        }
        private void normalMapLoadButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Load normal map";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var separated = dialog.FileName.Split("\\");
                    var last = separated[separated.Length - 1];
                    normalMapTxtBox.Text = last;
                }
            }
        }
    }
    public class Vertex
    {
        public float x;
        public float y;
        public float z;
        public Vector normal;

        public Vertex (float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.normal = new Vector();
        }
    }
    public class Vector
    {
        public float x;
        public float y;
        public float z;
        
        public Vector(float x = 0, float y = 0, float z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public class Polygon
    {
        public List<Vertex> verticies;

        public Polygon()
        {
            this.verticies = new List<Vertex>();
        }
    }
}