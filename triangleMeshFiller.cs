using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Numerics;
using System.IO;

namespace GK1_PROJ2
{
    public partial class mainWindow : Form
    {
        private List<Vertex> vertices;
        private List<Polygon> polygons;
        private List<Vector3> normals;
        private float minX;
        private float minY;
        private float minZ;
        private float maxX;
        private float maxY;
        private float maxZ;
        private Bitmap drawArea;
        private Bitmap objectColor;
        private string defaultTexture;
        private static Color canvasColor = Color.HotPink;
        private static Brush blackBrush = Brushes.Black;
        private const int pointRadious = 4;
        private static Pen edgePen = new Pen(blackBrush, 2);
        private const int padding = 60;
        private const int kMin = 0;
        private const int kMax = 0;
        private const int mMin = 1;
        private const int mMax = 100;
        private string path = string.Empty;
        private static (float, float) light = (20, 20);
        private float[,,] coefs;
        private Vector3 lightColor;
        // change this to generalise
        private const int maxVerticies = 3;

        public mainWindow()
        {
            InitializeComponent();
            defaultTexture = System.IO.Path.GetFullPath(@"..\..\..\") + "\\default_object_color.jpg";
            vertices = new List<Vertex>();
            polygons = new List<Polygon>();
            normals = new List<Vector3>();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            objectColor = new Bitmap(Image.FromFile(defaultTexture), canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
                g.Clear(canvasColor);
            recalcSliders();
            minX = float.MaxValue;
            maxX = float.MinValue;
            minY = float.MaxValue;
            maxY = float.MinValue;
            minZ = float.MaxValue;
            maxZ = float.MinValue;
            coefs = new float[canvas.Size.Width, canvas.Size.Height, maxVerticies];
            lightColor = new Vector3(1, 1, 1);
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
                    loadFile(dialog.FileName);
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
                                normals.Add(new Vector3(x, y, z));
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
            float height = canvas.Height - padding;
            float width = canvas.Width - padding;

            (int x, int y) center = ((canvas.Width / 2), (canvas.Height / 2));

            float k;

            if ((width / height) > ((maxX - minX) / (maxY - minY)))
                k = height / (maxY - minY);
            else
                k = width / (maxX - minX);

            foreach(var v in vertices)
            {
                v.x *= k;
                v.x += center.x;
                v.y *= k;
                v.y += center.y;
                v.z += minZ;
                v.z *= k;
                v.normal.X *= k;
                v.normal.Y *= k;
                v.normal.Z *= k;
            }
        }
        private void repaint()
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(canvasColor);

                foreach (var p in polygons)
                    fillpolygon(g,p);

                foreach(var p in polygons)
                {
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
            normals = new List<Vector3>();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
                g.Clear(canvasColor);
            minX = float.MaxValue;
            maxX = float.MinValue;
            minY = float.MaxValue;
            maxY = float.MinValue;
            minZ = float.MaxValue;
            maxZ = float.MinValue;
        }
        private void kdTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double val = kdTrackBar.Value;
            val /= 1000;
            kdTxtBox.Text = val.ToString("0.000");
            repaint();
        }
        private void ksTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double val = ksTrackBar.Value;
            val /= 1000;
            ksTxtBox.Text = val.ToString("0.000");
            repaint();
        }
        private void mTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double val = mTrackBar.Value;
            val /= 1000;
            mTxtBox.Text = val.ToString("0.000");
            repaint();
        }
        private void changeColorButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.Color = lightSourceColorPreview.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    lightSourceColorPreview.BackColor = colorDialog.Color;
                    lightColor.X = ((float)lightSourceColorPreview.BackColor.R) / 256;
                    lightColor.Y = ((float)lightSourceColorPreview.BackColor.G) / 256;
                    lightColor.Z = ((float)lightSourceColorPreview.BackColor.B) / 256;
                }
                repaint();
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
            repaint();
        }
        private void objectColorLoadDefaultButton_Click(object sender, EventArgs e)
        {
            objectColorSolidModeRbutton.Checked = false;
            objectColorTextureModeRbutton.Checked = false;
            objectColor = new Bitmap(Image.FromFile(defaultTexture), canvas.Size.Width, canvas.Size.Height);
            repaint();
        }
        private void objectColorSolidModeRbutton_CheckedChanged(object sender, EventArgs e)
        {
            var casted = (RadioButton)sender;
            if (casted.Checked)
                objectColorSolidChangeButton.Enabled = true;
            else
                objectColorSolidChangeButton.Enabled = false;
            colorChange();
            repaint();
        }
        private void objectColorTextureModeRbutton_CheckedChanged(object sender, EventArgs e)
        {
            var casted = (RadioButton)sender;
            if (casted.Checked)
                objectColorTextureLoadButton.Enabled = true;
            else
                objectColorTextureLoadButton.Enabled = false;
            if (path != string.Empty)
            {
                textureChange();
                repaint();
            }
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
                    colorChange();
                    repaint();
                }  
            }
        }
        private void objectColorTextureLoadButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Load texture";
                dialog.Filter = "All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var exPath = path;
                    path = dialog.FileName;
                    try
                    {
                        textureChange();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to load selected image file.", "Exeption while loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        path = exPath;
                        return;
                    }
                    objectColorTextureTxtBox.Text = "Loaded";
                    repaint();
                }
            }
        }
        private void calculatedAtPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculatedAtPointToolStripMenuItem.Checked = true;
            vetrexInterpolationToolStripMenuItem.Checked = false;
            repaint();
        }
        private void vetrexInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vetrexInterpolationToolStripMenuItem.Checked = true;
            calculatedAtPointToolStripMenuItem.Checked = false;
            repaint();
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
                    normalMapTxtBox.Text = "Loaded";
            }
        }
        private void fillpolygon(Graphics g, Polygon p)
        {
            SortedDictionary<int,List<Edge>> et = new SortedDictionary<int,List<Edge>>();
            List<Edge> aet = new List<Edge>();

            for (int i = 0; i < p.verticies.Count; i++)
            {
                int inext = (i + 1 == p.verticies.Count) ? 0 : i + 1;
                if (Math.Abs(p.verticies[inext].y - p.verticies[i].y) != 0)
                {
                    var ed = new Edge(p.verticies[i], p.verticies[inext]);
                    if (!et.ContainsKey(ed.ymin))
                        et.Add(ed.ymin, new List<Edge>());
                    et[ed.ymin].Add(ed);
                }
            }

            var curY = et.First().Key;
            while (et.Count > 0 || aet.Count > 0)
            {
                if (et.Count > 0)
                    if (curY == et.First().Key)
                    {
                        foreach (var e in et.First().Value)
                            aet.Add(e);
                        et.Remove(curY);
                        //MAYBE SORT HERE
                        aet.Sort((p, q) => xComparator(p, q));
                    }
                for (int i = 0; i < aet.Count;)
                {
                    if (aet[i].ymax == curY)
                        aet.RemoveAt(i);
                    else
                        i++;
                }
                //aet.Sort((p, q) => xComparator(p, q));

                for (int i = 0; i + 1 < aet.Count;)
                {
                    int x1 = (int)aet[i].x;
                    int x2 = (int)aet[i + 1].x;
                    int xMax = Math.Max(x1, x2);
                    int xMin = Math.Min(x1, x2);

                    for (int j = xMin; j <= xMax; j++)
                        paintPixel(p, j, curY);
                    aet[i].x += aet[i].d;
                    aet[i + 1].x += aet[i + 1].d;
                    i += 2;
                }
                curY++;
            }
        }
        private static int xComparator(Edge e1, Edge e2)
        {
            if (e1.x < e2.x) return -1;
            else if (e1.x > e2.x) return 1;
            else return 0;
        }
        private void colorChange()
        {
            using (Graphics g = Graphics.FromImage(objectColor))
                using (Brush brush = new SolidBrush(objectColorSolidTxtBox.BackColor))
                    g.FillRectangle(brush, 0, 0, objectColor.Width, objectColor.Height);
        }
        private void textureChange()
        {
            objectColor = new Bitmap(Image.FromFile(path), canvas.Size.Width, canvas.Size.Height);
        }
        private void calcCoefficiants()
        {
            coefs = new float[canvas.Size.Width, canvas.Size.Height,maxVerticies];

            foreach(var p in polygons)
            {
                SortedDictionary<int, List<Edge>> et = new SortedDictionary<int, List<Edge>>();
                List<Edge> aet = new List<Edge>();

                for (int i = 0; i < p.verticies.Count; i++)
                {
                    int inext = (i + 1 == p.verticies.Count) ? 0 : i + 1;
                    if (Math.Abs(p.verticies[inext].y - p.verticies[i].y) != 0)
                    {
                        var ed = new Edge(p.verticies[i], p.verticies[inext]);
                        if (!et.ContainsKey(ed.ymin))
                            et.Add(ed.ymin, new List<Edge>());
                        et[ed.ymin].Add(ed);
                    }
                }

                var curY = et.First().Key;
                while (et.Count > 0 || aet.Count > 0)
                {
                    if (et.Count > 0)
                        if (curY == et.First().Key)
                        {
                            foreach (var e in et.First().Value)
                                aet.Add(e);
                            et.Remove(curY);
                            //MAYBE SORT HERE
                            aet.Sort((p, q) => xComparator(p, q));
                        }
                    for (int i = 0; i < aet.Count;)
                    {
                        if (aet[i].ymax == curY)
                            aet.RemoveAt(i);
                        else
                            i++;
                    }
                    //aet.Sort((p, q) => xComparator(p, q));

                    for (int i = 0; i + 1 < aet.Count;)
                    {
                        int x1 = (int)aet[i].x;
                        int x2 = (int)aet[i + 1].x;
                        int xMax = Math.Max(x1, x2);
                        int xMin = Math.Min(x1, x2);

                        for (int j = xMin; j <= xMax; j++)
                        {
                            double area = 0;

                            for (int k = 0; k < p.verticies.Count; k++)
                            {
                                int kNext = (k + 1 == p.verticies.Count) ? 0 : k + 1;
                                float a = calcLength(p.verticies[k].x, p.verticies[k].y, p.verticies[kNext].x, p.verticies[kNext].y);
                                float b = calcLength(p.verticies[kNext].x, p.verticies[kNext].y, j, curY);
                                float c = calcLength(j, curY, p.verticies[k].x, p.verticies[k].y);
                                float pe = (a + b + c) / 3;
                                double aaaaaa = (Math.Sqrt(pe * (pe - a) * (pe - b) * (pe - c)));
                                if (Double.IsNaN(aaaaaa))
                                    aaaaaa = 0;
                                coefs[j, curY, k] = (float)aaaaaa;
                                // about to cause problems later hehe
                                area += coefs[j, curY, k];
                            }

                            if (area > 0)
                                for (int k = 0; k < maxVerticies; k++)
                                {
                                    coefs[j, curY, k] = (float)(coefs[j, curY, k] / area);
                                    //Debug.Write(coefs[j, curY, k]);
                                    //Debug.Write(' ');
                                }
                            //Debug.WriteLine(' ');
                        }
                        aet[i].x += aet[i].d;
                        aet[i + 1].x += aet[i + 1].d;
                        i += 2;
                    }
                    curY++;
                }
            }
        }
        private float calcLength(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
        private void paintPixel(Polygon p, int x, int y)
        {
            if (calculatedAtPointToolStripMenuItem.Checked)
            {
                Vector3 vector = new Vector3();
                foreach(var v in p.verticies)
                    for(int i = 0; i < maxVerticies; i++)
                    {
                        vector.X += coefs[x, y, i] * v.normal.X;
                        vector.Y += coefs[x, y, i] * v.normal.Y;
                        vector.Z += coefs[x, y, i] * v.normal.Z;
                    }

                var color = objectColor.GetPixel(x, y);

                drawArea.SetPixel(x, y, objectColor.GetPixel(x, y));
            }
            else
            {
                //
            }
            
        }
        private void loadFile(string path)
        {
            clean();
            if (processFile(path))
            {
                rescaleVerticies();
                calcCoefficiants();
                repaint();
                MessageBox.Show("Succesfully loaded " + polygons.Count.ToString() + " polygons.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void hemisphereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("hemisphereAVG.obj");
        }
        private void coneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("coneAVG.obj");
        }
        private void loadDefault(string s)
        {
            string s2 = System.IO.Path.GetFullPath(@"..\..\..\") + "sample figures\\" + s;
            loadFile(s2);
        }
        private void cylinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("cylinderAVG.obj");
        }
        private void asymmetricCylinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("2OYcylinderAVG.obj");
        }
        private void pyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("pyramidAVG.obj");
        }
    }
    public class Vertex
    {
        public float x;
        public float y;
        public float z;
        public Vector3 normal;

        public Vertex (float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.normal = new Vector3();
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
    public class Edge
    {
        public float x;
        public int ymax;
        public float d;
        public int ymin;

        public Edge(Vertex v1, Vertex v2)
        {
            if (v1.y < v2.y)
            {
                ymin = (int)v1.y;
                ymax = (int)v2.y;
                x = (int)v1.x;
            }
            else
            {
                ymin = (int)v2.y;
                ymax = (int)v1.y;
                x = (int)v2.x;
            }

            d = (v2.x - v1.x) / (v2.y - v1.y);
        }
    }
}