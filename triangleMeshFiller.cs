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
using System.Media;
using NAudio.Wave;
using NAudio.Gui;
using System.Xml.Serialization;

namespace GK1_PROJ2
{
    public partial class mainWindow : Form
    {
        private List<Figure> figures;
        private Polygon shade;
        private Polygon cloud;
        private Bitmap drawArea;
        private Bitmap objectColor;
        private Bitmap cloudTexture;
        private string defaultTexture;
        private static Color canvasColor = Color.HotPink;
        private static Brush blackBrush = Brushes.Black;
        private static Brush yellowBrush = Brushes.Yellow;
        private static Color cloudColor = Color.Blue;
        private static Color shadeColor = Color.Black;
        private const int pointRadious = 4;
        private const int lightRadious = 10;
        private static Pen edgePen = new Pen(blackBrush, 2);
        private const int padding = 150;
        private string texturePath = string.Empty;
        private (int x, int y) light = (360, 12);
        private int state = 0;
        private bool reverse = false;
        private Vector3 lightColor;
        private static Vector3 vv = new Vector3(0, 0, 1);
        private const int maxVerticies = 3;
        private const int lightStep = 2;
        private const float cloudStep = 10;

        private int loadedFigures = 0;
        private float lightSourceZ = 0;
        private float kd = 0;
        private float ks = 0;
        private float m = 0;
        private bool terminate = false;
        private bool active = false;
        private bool colorChangePending = false;
        private bool usingModifiedNormals = false;
        private bool musicPlaying = false;
        private bool cloudReverse = false;
        private bool cloudEnabled = false;
        private bool clear = false;
        public mainWindow()
        {
            InitializeComponent();
            defaultTexture = System.IO.Path.GetFullPath(@"..\..\..\") + "\\default_object_color.jpg";
            string cloudString = System.IO.Path.GetFullPath(@"..\..\..\") + "\\cloud.jpg";
            figures = new List<Figure>();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            objectColor = new Bitmap(Image.FromFile(defaultTexture), canvas.Size.Width, canvas.Size.Height);
            cloudTexture = new Bitmap(Image.FromFile(cloudString), canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
                g.Clear(canvasColor);
            lightColor = new Vector3(1, 1, 1);
            cloud = new Polygon();
            cloud.verticies.Add(new Vertex(100, 200, 150));
            cloud.verticies.Add(new Vertex(150, 250, 150));
            cloud.verticies.Add(new Vertex(250, 250, 150));
            cloud.verticies.Add(new Vertex(300, 200, 150));
            cloud.verticies.Add(new Vertex(250, 150, 150));
            cloud.verticies.Add(new Vertex(150, 150, 150));
            shade = new Polygon();
            recalcSliders();
        }
        // HANDLERS
        private void clearCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!terminate)
            {
                clear = true;
                terminate = true;
            }
            else
            {
                figures = new List<Figure>();
                loadedFigures = 0;
                repaint();
            }
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
            foreach (var v in cloud.verticies)
                v.z = lightSourceZ - 200;
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
            if (texturePath != string.Empty)
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
                    var exPath = texturePath;
                    texturePath = dialog.FileName;
                    try
                    {
                        textureChange();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to load selected image file.", "Exeption while loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        texturePath = exPath;
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
            noneToolStripMenuItem.Checked = false;
            if (lightStopAnimationCbox.Checked)
                repaint();
            if (!active && loadedFigures != 0)
                launchKernel();
        }
        private void vetrexInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vetrexInterpolationToolStripMenuItem.Checked = true;
            calculatedAtPointToolStripMenuItem.Checked = false;
            noneToolStripMenuItem.Checked = false;
            if (lightStopAnimationCbox.Checked)
                repaint();
            if (!active && loadedFigures != 0)
                launchKernel();
        }
        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noneToolStripMenuItem.Checked = true;
            vetrexInterpolationToolStripMenuItem.Checked = false;
            calculatedAtPointToolStripMenuItem.Checked = false;
            showEdgesToolStripMenuItem.Checked = true;
            if (lightStopAnimationCbox.Checked)
                repaint();
            terminate = true;
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
        private void fullTorrusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDefault("fullTorrus.obj");
        }
        private void mainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            terminate = true;
        }
        private void enableCloudsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cloudEnabled = !cloudEnabled;

            if (cloudEnabled)
                enableCloudsToolStripMenuItem.Text = "Disable clouds";
            else
                enableCloudsToolStripMenuItem.Text = "Enable clouds";
        }
        private void showVerticiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneToolStripMenuItem.Checked)
                repaint();
        }
        private void showEdgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneToolStripMenuItem.Checked)
                repaint();
        }
        // MY FUNCTIONS
        private void loadFile(string path)
        {
            if (processFile(path))
            {
                (int, int) center = (180 + (loadedFigures % 3) * 180, 90 + ((loadedFigures/3) % 3) * 180);
                if (loadedFigures >= 9)
                {
                    figures.RemoveAt(figures.Count - 1);
                    MessageBox.Show("No more space for polygons", " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                rescaleVerticies(center, 170);
                normaliseVectors();
                calcCoefficiants();
                loadedFigures++;
                if (!active)
                {
                    if (noneToolStripMenuItem.Checked)
                        repaint();
                    else
                        launchKernel();
                }
                MessageBox.Show("Succesfully loaded " + figures[figures.Count - 1].polygons.Count.ToString() + " polygons.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            figures.Add(new Figure());
            int indx = figures.Count - 1;

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
                                figures[indx].vertices.Add(new Vertex(x, y, z));
                                if (x < figures[indx].minX)
                                    figures[indx].minX = x;
                                if (x > figures[indx].maxX)
                                    figures[indx].maxX = x;
                                if (y < figures[indx].minY)
                                    figures[indx].minY = y;
                                if (y > figures[indx].maxY)
                                    figures[indx].maxY = y;
                                if (z < figures[indx].minZ)
                                    figures[indx].minZ = z;
                                if (z > figures[indx].maxZ)
                                    figures[indx].maxZ = z;
                                break;
                            }
                        case "vn":
                            {
                                x = float.Parse(parts[1]);
                                y = float.Parse(parts[2]);
                                z = float.Parse(parts[3]);
                                figures[indx].normals.Add(new Vector3(x, y, z));
                                break;
                            }
                        case "f":
                            {
                                figures[indx].polygons.Add(new Polygon());
                                for (int i = 1; i < parts.Length; i++)
                                {
                                    parts2 = parts[i].Split('/');
                                    v = int.Parse(parts2[0]) - 1;
                                    vn = int.Parse(parts2[2]) - 1;
                                    figures[indx].vertices[v].normal = figures[indx].normals[vn];
                                    figures[indx].polygons[figures[indx].polygons.Count - 1].verticies.Add(figures[indx].vertices[v]);
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

                if (figures[indx].vertices.Count > 0 && figures[indx].normals.Count > 0 && figures[indx].polygons.Count > 0)
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
        private void rescaleVerticies((int x, int y) center, int diameter)
        {
            int indx = figures.Count - 1;

            //float height = canvas.Height - padding;
            //float width = canvas.Width - padding;
            //(int x, int y) center = ((canvas.Width / 2), (canvas.Height / 2));

            float k;

            //if ((width / height) > ((figures[indx].maxX - figures[indx].minX) / (figures[indx].maxY - figures[indx].minY)))
            //    k = height / (figures[indx].maxY - figures[indx].minY);
            //else
            //    k = width / (figures[indx].maxX - figures[indx].minX);

            if (1 > ((figures[indx].maxX - figures[indx].minX) / (figures[indx].maxY - figures[indx].minY)))
                k = diameter / (figures[indx].maxY - figures[indx].minY);
            else
                k = diameter / (figures[indx].maxX - figures[indx].minX);

            foreach (var v in figures[indx].vertices)
            {
                v.x *= k;
                v.x += center.x;
                v.y *= k;
                v.y += center.y;
                v.z += figures[indx].minZ;
                v.z *= k;
                //v.normal.X *= k;
                //v.normal.Y *= k;
                //v.normal.Z *= k;
            }
        }
        private void normaliseVectors()
        {
            foreach (var p in figures[figures.Count - 1].polygons)
                foreach (var v in p.verticies)
                {
                    float len = (float)Math.Sqrt(v.normal.X * v.normal.X + v.normal.Y * v.normal.Y + v.normal.Z * v.normal.Z);
                    v.normal.X /= len;
                    v.normal.Y /= len;
                    v.normal.Z /= len;
                    v.mNormal = v.normal;
                }
        }
        private void launchKernel()
        {
            Thread t = new Thread(new ThreadStart(animation));
            t.Start();
        }
        private void animation()
        {
            bool playing = false;
            string music = System.IO.Path.GetFullPath(@"..\..\..\") + "\\animation_music.mp3";
            IWavePlayer waveOutDevice = new WaveOut();
            AudioFileReader audioFileReader = new AudioFileReader(music);
            waveOutDevice.Init(audioFileReader);
            //waveOutDevice.PlaybackStopped += endOfMusicEventHandler();

            active = true;
            while (!terminate)
            {
                if (!lightStopAnimationCbox.Checked)
                {
                    moveLightSource();
                    repaint();
                    if (!playing && musicPlaying)
                    {
                        waveOutDevice.Play();
                        playing = true;
                    }
                }
                else
                {
                    if (playing)
                    {
                        waveOutDevice.Stop();
                        playing = false;
                    }
                }
                if (playing && !musicPlaying)
                {
                    waveOutDevice.Stop();
                    playing = false;
                }
            }
            waveOutDevice.Stop();
            audioFileReader.Dispose();
            waveOutDevice.Dispose();
            active = false;
            if (clear)
            {
                figures = new List<Figure>();
                loadedFigures = 0;
            }
            clear = false;
            repaint();
            terminate = false;
            return;
        }
        private void repaint()
        {
            using (var fastbitmap = drawArea.FastLock())
                using(var fastbitmap2 = objectColor.FastLock())
                {
                    fastbitmap.Clear(canvasColor);

                    if (!noneToolStripMenuItem.Checked)
                        for(int i = 0; i < loadedFigures; i++)
                            foreach (var p in figures[i].polygons)
                                fillpolygon(p, fastbitmap, fastbitmap2, figures[i].coefs);
                }

            using (Graphics g = Graphics.FromImage(drawArea))
            {
                for (int j = 0; j < loadedFigures; j++)
                    foreach (var p in figures[j].polygons)
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

            if (cloudEnabled && !noneToolStripMenuItem.Checked)
                using (var fastbitmap = drawArea.FastLock())
                    using (var fastbitmap2 = cloudTexture.FastLock())
                    {
                        recalcShade();
                        paintCloud(shade, fastbitmap, shadeColor, null);
                        paintCloud(cloud, fastbitmap, cloudColor, fastbitmap2);
                    }   

            if (active)
                using (Graphics g = Graphics.FromImage(drawArea))
                    paintPoint(g, light.x, light.y, yellowBrush, lightRadious);

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
        private void fillpolygon(Polygon p, FastBitmap f, FastBitmap f2, float[,,] coefs)
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
                            calculateAndPaintColor(p, j, curY, f, f2, coefs);
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
            objectColor = new Bitmap(Image.FromFile(texturePath), canvas.Size.Width, canvas.Size.Height);
        }
        private void calcCoefficiants()
        {
            int indx = figures.Count - 1;

            figures[indx].coefs = new float[canvas.Size.Width, canvas.Size.Height, maxVerticies];

            foreach (var p in figures[indx].polygons)
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
                                figures[indx].coefs[j, curY, kkNext] = (float)(Math.Sqrt(pe * (pe - a) * (pe - b) * (pe - c)));
                                area += figures[indx].coefs[j, curY, kkNext];
                            }

                            if (area > 0)
                                for (int k = 0; k < maxVerticies; k++)
                                    figures[indx].coefs[j, curY, k] = (float)(figures[indx].coefs[j, curY, k] / area);
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
        private void calculateAndPaintColor(Polygon p, int x, int y, FastBitmap f, FastBitmap f2, float[,,] coefs)
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
        private void loadDefault(string s)
        {
            string s2 = System.IO.Path.GetFullPath(@"..\..\..\") + "sample figures\\" + s;
            loadFile(s2);
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
            float maxX = float.MinValue;
            float minX = float.MaxValue;

            foreach(var v in cloud.verticies)
            {
                if (v.x > maxX)
                    maxX = v.x;
                if (v.x < minX) 
                    minX = v.x;
            }

            if (!cloudReverse)
            {
                if (maxX + cloudStep >= canvas.Width)
                    cloudReverse = true;
            }
            else
            {
                if (minX - cloudStep <= 0)
                    cloudReverse = false;
            }

            foreach (var v in cloud.verticies)
            {
                if (cloudReverse)
                    v.x -= cloudStep;
                else
                    v.x += cloudStep;
            }

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
        private void modifyNormals(string pa)
        {
            var nmap = new Bitmap(Image.FromFile(pa), canvas.Size.Width, canvas.Size.Height);

            using (var fastbitmap = nmap.FastLock())
                foreach (var p in figures[figures.Count - 1].polygons)
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
        private void enableAnimationMusicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicPlaying = !musicPlaying;

            if (musicPlaying)
                enableAnimationMusicToolStripMenuItem.Text = "Disable music";
            else
                enableAnimationMusicToolStripMenuItem.Text = "Enable music";
        }
        private void paintCloud(Polygon p, FastBitmap f, Color c, FastBitmap? f2)
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
            }

            if (et.Count > 0)
            {
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
                            if (f2 != null)
                            {
                                var color = f2.GetPixel(j, curY);
                                f.SetPixel(j, curY, color);
                            }
                            else
                            {
                                f.SetPixel(j, curY, c);
                            }
                        }
                        aet[i].x += aet[i].d;
                        aet[i + 1].x += aet[i + 1].d;
                        i += 2;
                    }
                    curY++;
                }
            }
        }
        private void recalcShade()
        {
            shade.verticies.Clear();

            foreach (var v in cloud.verticies)
            {
                //float x = lightSourceZ * (v.x - light.x) / (v.z - lightSourceZ) + light.x;
                float x = (light.x * v.z - lightSourceZ * v.x) / (v.z - lightSourceZ) + light.x;
                if (x <= 0)
                    x = 1;
                if (x >= drawArea.Width)
                    x = drawArea.Width - 1;
                //float y = lightSourceZ * (v.y - light.y) / (v.z - lightSourceZ) + light.y;
                float y = (light.y * v.z - lightSourceZ * v.y) / (v.z - lightSourceZ) + light.y;
                if (y <= 0)
                    y = 1;
                if (y >= drawArea.Height)
                    y = drawArea.Height - 1;
                shade.verticies.Add(new Vertex(x, y, 0));
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
    public class Figure
    {
        public List<Polygon> polygons;
        public List<Vertex> vertices;
        public List<Vector3> normals;
        public float[,,] coefs;
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float minZ;
        public float maxZ;

        public Figure()
        {
            this.polygons = new List<Polygon>();
            this.vertices = new List<Vertex>();
            this.normals = new List<Vector3>();
            this.coefs = new float[0, 0, 0];
            minX = float.MaxValue;
            maxX = float.MinValue;
            minY = float.MaxValue;
            maxY = float.MinValue;
            minZ = float.MaxValue;
            maxZ = float.MinValue;
        }
    }
}