using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Numerics;
using System.IO;
using System.Runtime.Intrinsics;
using FastBitmapLib;
using System;
using System.Drawing;
using static System.Windows.Forms.AxHost;

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
        private static Brush yellowBrush = Brushes.Yellow;
        private const int pointRadious = 4;
        private const int lightRadious = 10;
        private static Pen edgePen = new Pen(blackBrush, 2);
        private const int padding = 150;
        private const int kMin = 0;
        private const int kMax = 0;
        private const int mMin = 1;
        private const int mMax = 100;
        private string path = string.Empty;
        private (int x, int y) light = (360, 12);
        private int state = 0;
        private const int maxState = 15;
        private bool reverse = false;
        private float[,,] coefs;
        private Vector3 lightColor;
        private static Vector3 vv = new Vector3(0, 0, 1);
        // change this to generalise
        private const int maxVerticies = 3;
        private const int lightStep = 2;
        private float lightSourceZ = 0;
        private float kd = 0;
        private float ks = 0;
        private float m = 0;
        private bool terminate = false;
        private bool active = false;
        private bool colorChangePending = false;
        private bool usingModifiedNormals = false;

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
        // HANDLERS
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
        private void kdTrackBar_ValueChanged(object sender, EventArgs e)
        {
            kd = (float)kdTrackBar.Value / 1000;
            kdTxtBox.Text = kd.ToString("0.000");
            if (lightStopAnimationCbox.Checked)
                repaint();
        }
        private void ksTrackBar_ValueChanged(object sender, EventArgs e)
        {
            ks = (float)ksTrackBar.Value / 1000;
            ksTxtBox.Text = ks.ToString("0.000");
            if (lightStopAnimationCbox.Checked)
                repaint();
        }
        private void mTrackBar_ValueChanged(object sender, EventArgs e)
        {
            m = (float)mTrackBar.Value / 1000;
            mTxtBox.Text = m.ToString("0.000");
            if (lightStopAnimationCbox.Checked)
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
                if (lightStopAnimationCbox.Checked)
                    repaint();
            }
        }
        private void showToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (lightStopAnimationCbox.Checked)
                repaint();
        }
        private void lightSourceAltitudeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            lightSourceZ = (float)lightSourceAltitudeTrackBar.Value / 1000;
            lightSourceAltitudeTxtBox.Text = lightSourceZ.ToString("0.000");
            if (lightStopAnimationCbox.Checked)
                repaint();
        }
        private void objectColorLoadDefaultButton_Click(object sender, EventArgs e)
        {
            objectColorSolidModeRbutton.Checked = false;
            objectColorTextureModeRbutton.Checked = false;
            objectColor = new Bitmap(Image.FromFile(defaultTexture), canvas.Size.Width, canvas.Size.Height);
            if (lightStopAnimationCbox.Checked)
                repaint();
        }
        private void objectColorSolidModeRbutton_CheckedChanged(object sender, EventArgs e)
        {
            var casted = (RadioButton)sender;
            if (casted.Checked)
                objectColorSolidChangeButton.Enabled = true;
            else
                objectColorSolidChangeButton.Enabled = false;
            //colorChange();
            colorChangePending = true;
            if (lightStopAnimationCbox.Checked)
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
                if (lightStopAnimationCbox.Checked)
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
                    colorChangePending = true;
                    //colorChange();
                    if (lightStopAnimationCbox.Checked)
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
                    if (lightStopAnimationCbox.Checked)
                        repaint();
                }
            }
        }
        private void calculatedAtPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculatedAtPointToolStripMenuItem.Checked = true;
            vetrexInterpolationToolStripMenuItem.Checked = false;
            if (lightStopAnimationCbox.Checked)
                repaint();
        }
        private void vetrexInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vetrexInterpolationToolStripMenuItem.Checked = true;
            calculatedAtPointToolStripMenuItem.Checked = false;
            if (lightStopAnimationCbox.Checked)
                repaint();
        }
        private void modifyNormalsCbutton_CheckedChanged(object sender, EventArgs e)
        {
            var casted = (CheckBox)sender;
            if (casted.Checked)
            {
                normalMapLoadButton.Enabled = true;
                usingModifiedNormals = true;
            }
            else
            {
                normalMapLoadButton.Enabled = false;
                usingModifiedNormals = false;
            } 
        }
        private void normalMapLoadButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Load normal map";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        modifyNormals(dialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Unable to load selected nmap file.", "Exeption while loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    normalMapTxtBox.Text = "Loaded";
                    if (lightStopAnimationCbox.Checked)
                        repaint();
                }
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
        private void donutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("ponczek.obj");
        }
        private void buttonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("guzik.obj");
        }
        private void mainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            terminate = true;
        }
        // MY FUNCTIONS
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

            foreach (var v in vertices)
            {
                v.x *= k;
                v.x += center.x;
                v.y *= k;
                v.y += center.y;
                v.z += minZ;
                v.z *= k;
                //v.normal.X *= k;
                //v.normal.Y *= k;
                //v.normal.Z *= k;
            }
        }
        private void repaint()
        {
            using (var fastbitmap = drawArea.FastLock())
                using(var fastbitmap2 = objectColor.FastLock())
                {
                    fastbitmap.Clear(canvasColor);

                foreach (var p in polygons)
                    fillpolygon(p, fastbitmap, fastbitmap2);
                }

            using (Graphics g = Graphics.FromImage(drawArea))
            {
                foreach (var p in polygons)
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
                if (active)
                    paintPoint(g, light.x, light.y, yellowBrush, lightRadious);
            }

            if (colorChangePending)
            {
                using (Graphics g = Graphics.FromImage(objectColor))
                    using (Brush brush = new SolidBrush(objectColorSolidTxtBox.BackColor))
                        g.FillRectangle(brush, 0, 0, objectColor.Width, objectColor.Height);
                colorChangePending = false;
            }

            //sosnowskij bypass
            canvas.Invalidate();
            if (this.IsHandleCreated)
                try
                {
                    canvas.Invoke((MethodInvoker)delegate
                    {
                        if (canvas.IsDisposed) return;
                        canvas.Update();
                    });
                }
                catch { }
            else
                canvas.Update();
        }
        private void paintPoint(Graphics g, float x, float y, Brush color, int rad = pointRadious)
        {
            g.FillEllipse(color, x - rad, y - rad, rad * 2, rad * 2);
        }
        private void clean()
        {
            if (active)
                terminate = true;
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
            vertices = new List<Vertex>();
            polygons = new List<Polygon>();
            normals = new List<Vector3>();
        }
        private void recalcSliders()
        {
            this.lightSourceAltitudeTrackBar.Maximum = this.canvas.Height * 1000;
            this.lightSourceAltitudeTrackBar.Value = (this.lightSourceAltitudeTrackBar.Maximum - this.lightSourceAltitudeTrackBar.Minimum) / 2 + this.lightSourceAltitudeTrackBar.Minimum;
            lightSourceZ = (float)lightSourceAltitudeTrackBar.Value / 1000;
            this.lightSourceAltitudeTxtBox.Text = (((double)(this.lightSourceAltitudeTrackBar.Maximum - this.lightSourceAltitudeTrackBar.Minimum) / 2 + this.lightSourceAltitudeTrackBar.Minimum) / 1000).ToString("0.000");
            this.mTrackBar.Value = (this.mTrackBar.Maximum - this.mTrackBar.Minimum) / 2 + this.mTrackBar.Minimum;
            m = (float)mTrackBar.Value / 1000;
            this.mTxtBox.Text = (((double)(this.mTrackBar.Maximum - this.mTrackBar.Minimum) / 2 + this.mTrackBar.Minimum) / 1000).ToString("0.000");
            this.ksTrackBar.Value = (this.ksTrackBar.Maximum - this.ksTrackBar.Minimum) / 2 + this.ksTrackBar.Minimum;
            ks = (float)ksTrackBar.Value / 1000;
            this.ksTxtBox.Text = (((double)(this.ksTrackBar.Maximum - this.ksTrackBar.Minimum) / 2 + this.ksTrackBar.Minimum) / 1000).ToString("0.000");
            this.kdTrackBar.Value = (this.kdTrackBar.Maximum - this.kdTrackBar.Minimum) / 2 + this.kdTrackBar.Minimum;
            kd = (float)kdTrackBar.Value / 1000;
            this.kdTxtBox.Text = (((double)(this.kdTrackBar.Maximum - this.kdTrackBar.Minimum) / 2 + this.kdTrackBar.Minimum) / 1000).ToString("0.000");

        }
        private void fillpolygon(Polygon p, FastBitmap f, FastBitmap f2)
        {
            SortedDictionary<int, List<Edge>> et = new SortedDictionary<int, List<Edge>>();
            List<Edge> aet = new List<Edge>();
            Vector3[] colors = new Vector3[maxVerticies];

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
                if (vetrexInterpolationToolStripMenuItem.Checked)
                {
                    Vector3 n;
                    if (!usingModifiedNormals)
                        n = normaliseVector(p.verticies[i].normal);
                    else
                        n = normaliseVector(p.verticies[i].mNormal);
                    Vector3 l = normaliseVector(new Vector3(light.x - p.verticies[i].x, light.y - p.verticies[i].y, lightSourceZ - p.verticies[i].z));
                    float cosNL = n.X * l.X + n.Y * l.Y + n.Z * l.Z;
                    if (cosNL < 0)
                        cosNL = 0;
                    var color = f2.GetPixel((int)p.verticies[i].x, (int)p.verticies[i].y);
                    Vector3 colorV = new Vector3((float)color.R / 256, (float)color.G / 256, (float)color.B / 256);
                    Vector3 r = new Vector3(2 * cosNL * n.X - l.X, 2 * cosNL * n.Y - l.Y, 2 * cosNL * n.Z - l.Z);
                    float cosVR = (vv.X * r.X + vv.Y * r.Y + vv.Z * r.Z) / vectorLength(vv) / vectorLength(r);
                    if (cosVR < 0)
                        cosVR = 0;
                    float coe = (float)(kd * cosNL + ks * Math.Pow(cosVR, m));
                    colors[i].X = lightColor.X * colorV.X * coe * 256;
                    colors[i].Y = lightColor.Y * colorV.Y * coe * 256;
                    colors[i].Z = lightColor.Z * colorV.Z * coe * 256;
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
                        if (calculatedAtPointToolStripMenuItem.Checked)
                            calculateAndPaintColor(p, j, curY, f, f2);
                        else
                        {
                            Vector3 finalColor = new Vector3(0, 0, 0);
                            for (int k = 0; k < p.verticies.Count; k++)
                            {
                                finalColor.X += coefs[j, curY, k] * colors[k].X;
                                finalColor.Y += coefs[j, curY, k] * colors[k].Y;
                                finalColor.Z += coefs[j, curY, k] * colors[k].Z;
                            }
                            Color colorF = new Color();
                            colorF = Color.FromArgb((byte)255, (byte)finalColor.X, (byte)finalColor.Y, (byte)finalColor.Z);
                            f.SetPixel(j, curY, colorF);
                        }
                    }
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
                            // actuall baricenter calculations
                            if (maxVerticies != 3)
                                throw new Exception("Generalisation not implemented");

                            float area = 0;

                            for (int k = 0; k < p.verticies.Count; k++)
                            {
                                int kNext = (k + 1 == p.verticies.Count) ? 0 : k + 1;
                                int kkNext = (kNext + 1 == p.verticies.Count) ? 0 : kNext + 1;
                                float a = calcLength(p.verticies[k].x, p.verticies[k].y, p.verticies[kNext].x, p.verticies[kNext].y);
                                float b = calcLength(p.verticies[kNext].x, p.verticies[kNext].y, j, curY);
                                float c = calcLength(j, curY, p.verticies[k].x, p.verticies[k].y);
                                float pe = (a + b + c) / 2;
                                coefs[j, curY, kkNext] = (float)(Math.Sqrt(pe * (pe - a) * (pe - b) * (pe - c)));
                                area += coefs[j, curY, kkNext];
                            }

                            if (area > 0)
                                for (int k = 0; k < maxVerticies; k++)
                                    coefs[j, curY, k] = (float)(coefs[j, curY, k] / area);
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
        private void calculateAndPaintColor(Polygon p, int x, int y, FastBitmap f, FastBitmap f2)
        {
            Vector3 vector = new Vector3();
            float z = 0;

            if (!usingModifiedNormals)
                for (int i = 0; i < p.verticies.Count; i++)
                {
                    vector.X += coefs[x, y, i] * p.verticies[i].normal.X;
                    vector.Y += coefs[x, y, i] * p.verticies[i].normal.Y;
                    vector.Z += coefs[x, y, i] * p.verticies[i].normal.Z;
                    z += coefs[x, y, i] * p.verticies[i].z;
                }
            else
                for (int i = 0; i < p.verticies.Count; i++)
                {
                    vector.X += coefs[x, y, i] * p.verticies[i].mNormal.X;
                    vector.Y += coefs[x, y, i] * p.verticies[i].mNormal.Y;
                    vector.Z += coefs[x, y, i] * p.verticies[i].mNormal.Z;
                    z += coefs[x, y, i] * p.verticies[i].z;
                }

            Vector3 n = normaliseVector(vector);
            Vector3 l = normaliseVector(new Vector3(light.x - x, light.y - y, lightSourceZ - z));
            float cosNL = n.X * l.X + n.Y * l.Y + n.Z * l.Z;
            if (cosNL < 0)
                cosNL = 0;
            var color = f2.GetPixel(x, y);
            Vector3 colorV = new Vector3((float)color.R / 256, (float)color.G / 256, (float)color.B / 256);
            Vector3 r = new Vector3(2 * cosNL * n.X - l.X, 2 * cosNL * n.Y - l.Y, 2 * cosNL * n.Z - l.Z);
            float cosVR = (vv.X * r.X + vv.Y * r.Y + vv.Z * r.Z)/vectorLength(vv)/vectorLength(r);
            if (cosVR < 0)
                cosVR = 0;
            Vector3 finalColor = new Vector3();
            float coe = (float)(kd * cosNL + ks * Math.Pow(cosVR, m));
            finalColor.X = lightColor.X * colorV.X * coe * 256;
            finalColor.Y = lightColor.Y * colorV.Y * coe * 256;
            finalColor.Z = lightColor.Z * colorV.Z * coe * 256;
            Color colorF = new Color();
            colorF = Color.FromArgb((byte)255, (byte)finalColor.X, (byte)finalColor.Y, (byte)finalColor.Z);
            f.SetPixel(x, y, colorF);
        }
        private void loadFile(string path)
        {
            clean();
            if (processFile(path))
            {
                rescaleVerticies();
                normaliseVectors();
                calcCoefficiants();
                repaint();
                MessageBox.Show("Succesfully loaded " + polygons.Count.ToString() + " polygons.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Thread t = new Thread(new ThreadStart(animation));
                t.Start();
            }
        }
        private void loadDefault(string s)
        {
            string s2 = System.IO.Path.GetFullPath(@"..\..\..\") + "sample figures\\" + s;
            loadFile(s2);
        }
        private void normaliseVectors()
        {
            foreach(var p in polygons)
                foreach(var v in p.verticies)
                {
                    float len = (float)Math.Sqrt(v.normal.X * v.normal.X + v.normal.Y * v.normal.Y + v.normal.Z * v.normal.Z);
                    v.normal.X /= len;
                    v.normal.Y /= len;
                    v.normal.Z /= len;
                    v.mNormal = v.normal;
                }
        }
        private Vector3 normaliseVector(Vector3 normal)
        {
            float len = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            normal.X /= len;
            normal.Y /= len;
            normal.Z /= len;
            return normal;
        }
        private float vectorLength(Vector3 vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Z * vector.Z + vector.Z * vector.Z);
        }
        private void moveLightSource()
        {
            (int x, int y) offset = (canvas.Width / 2, canvas.Height / 2);
            (int x, int y) realCoords = (light.x - offset.x, light.y - offset.y);
            int length = (int)Math.Sqrt(realCoords.x * realCoords.x + realCoords.y * realCoords.y);
            int mLength = 0;

            if (!reverse)
                mLength = length - lightStep;
            else
                mLength = length + lightStep;

            if (mLength <= 0)
            {
                mLength = length + 2 * lightStep;
                reverse = true;
            }
            else if (mLength >= offset.y)
            {
                mLength = length - 2 * lightStep;
                reverse = false;
            }

            state = ++state % 15;
            switch (state)
            {
                case 0:
                    {
                        realCoords = (0, -mLength);
                        break;
                    }
                case 1:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (x, -2 * x);
                        break;
                    }
                case 2:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 2);
                        realCoords = (x, -x);
                        break;
                    }
                case 3:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (2 * x, -x);
                        break;
                    }
                case 4:
                    {
                        realCoords = (mLength, 0);
                        break;
                    }
                case 5:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (2 * x, x);
                        break;
                    }
                case 6:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 2);
                        realCoords = (x, x);
                        break;
                    }
                case 7:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (x, 2 * x);
                        break;
                    }
                case 8:
                    {
                        realCoords = (0, mLength);
                        break;
                    }
                case 9:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (-x, 2 * x);
                        break;
                    }
                case 10:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 2);
                        realCoords = (-x, x);
                        break;
                    }
                case 11:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (-2 * x, x);
                        break;
                    }
                case 12:
                    {
                        realCoords = (-mLength, 0);
                        break;
                    }
                case 13:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (-2 * x, -x);
                        break;
                    }
                case 14:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 2);
                        realCoords = (-x, -x);
                        break;
                    }
                case 15:
                    {
                        int x = (int)Math.Sqrt(mLength * mLength / 5);
                        realCoords = (-x, -2 * x);
                        break;
                    }
            }
            light = (realCoords.x + offset.x, realCoords.y + offset.y);
        }
        private void animation()
        {
            active = true;
            while (!terminate) 
            {
                if (!lightStopAnimationCbox.Checked)
                {
                    moveLightSource();
                    repaint();
                }
            }
            terminate = false;
            active = false;
            repaint();
            return;
        }
        private void modifyNormals(string path)
        {
            var nmap = new Bitmap(Image.FromFile(path), canvas.Size.Width, canvas.Size.Height);

            using (var fastbitmap = nmap.FastLock())
                foreach (var p in polygons)
                    foreach(var v in p.verticies)
                    {
                        var color = fastbitmap.GetPixel((int)v.x, (int)v.y);
                        Vector3 nTexture = new Vector3((float)color.R / 128 - 1, (float)color.G / 128 - 1, (float)color.B / 256);
                        Vector3 nSurface = normaliseVector(v.normal);
                        Vector3 b;
                        if (nSurface.X == 0 && nSurface.Y == 0 && nSurface.Z == 1)
                            b = new Vector3(0, 1, 0);
                        else
                            b = new Vector3(nSurface.Y, -nSurface.X, 0);
                        Vector3 t = new Vector3(b.Y * nSurface.Z - b.Z * nSurface.Y, b.Z * nSurface.X - b.X * nSurface.Z, b.X * nSurface.Y - b.Y * nSurface.X);
                        Vector3 res = new Vector3(t.X * nTexture.X + b.X * nTexture.Y + nSurface.X * nTexture.Z, t.Y * nTexture.X + b.Y * nTexture.Y + nSurface.Y * nTexture.Z, t.Z * nTexture.X + b.Z * nTexture.Y + nSurface.Z * nTexture.Z);
                        v.mNormal = normaliseVector(res);
                    }
        }
    }
    public class Vertex
    {
        public float x;
        public float y;
        public float z;
        public Vector3 normal;
        public Vector3 mNormal;

        public Vertex (float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.normal = new Vector3();
            this.mNormal = new Vector3();
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