using System.Diagnostics;
using System.Windows.Forms;

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
        private static Color canvasColor = Color.HotPink;
        private static Brush blackBrush = Brushes.Black;
        private const int pointRadious = 4;
        private static Pen edgePen = new Pen(blackBrush, 2);
        private const int padding = 10;

        public mainWindow()
        {
            InitializeComponent();
            vertices = new List<Vertex>();
            polygons = new List<Polygon>();
            normals = new List<Vector>();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
                g.Clear(canvasColor);
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
                        MessageBox.Show("Succesfully loaded " + polygons.Count.ToString() + " polygons.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //Debug.WriteLine("minX = " + minX.ToString() + ", maxX = " + maxX.ToString() + ".");
                        //Debug.WriteLine("minY = " + minY.ToString() + ", maxY = " + maxY.ToString() + ".");
                        //Debug.WriteLine("minZ = " + minZ.ToString() + ", maxZ = " + maxZ.ToString() + ".");
                        repaint();
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
                g.Clear(canvasColor);

                foreach (var p in polygons)
                    for (int i = 0; i < p.verticies.Count; i++)
                    {
                        if (showVerticiesCbox.Checked)
                            paintPoint(g, p.verticies[i].x, p.verticies[i].y, blackBrush);
                        if (showEdgesCbox.Checked)
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
        private void showCbox_CheckedChanged(object sender, EventArgs e)
        {
            repaint();
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