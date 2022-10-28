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

        public mainWindow()
        {
            InitializeComponent();
            vertices = new List<Vertex>();
            polygons = new List<Polygon>();
            normals = new List<Vector>();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.HotPink);
            }
        }

        private void clearCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // todo: implement restarting whole program
        }
        private void loadobjFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Wavefront .obj file|*.obj";
                dialog.Title = "Load .obj file";

                if (dialog.ShowDialog() == DialogResult.OK)
                    if (processFile(dialog.FileName))
                    {
                        rescaleVerticies();
                        MessageBox.Show("Succesfully loaded " + polygons.Count.ToString() + " polygons.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //Debug.WriteLine("minX = " + minX.ToString() + ", maxX = " + maxX.ToString() + ".");
                        //Debug.WriteLine("minY = " + minY.ToString() + ", maxY = " + maxY.ToString() + ".");
                        //Debug.WriteLine("minZ = " + minZ.ToString() + ", maxZ = " + maxZ.ToString() + ".");
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
                                    polygons[polygons.Count - 1].vectors.Add(vertices[v]);
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
                MessageBox.Show("Unable to load selected .obj file.","Exeption while loading",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
        }
        private void rescaleVerticies()
        {
            bool Y = canvas.Height <= canvas.Width;
            float k;
            if (Y)
            {
                k = canvas.Height / (maxY - minY);
                foreach (var v in vertices)
                {
                    v.y -= minY;
                    v.y *= k;
                    v.x -= minX;
                    v.x += (canvas.Width - canvas.Height) / 2;
                    v.x *= k;
                    v.z *= k;
                    v.normal.x *= k;
                    v.normal.y *= k;
                    v.normal.z *= k;
                }
            }
            else
            {
                k = canvas.Width / (maxX - minX);
                foreach(var v in vertices)
                {
                    v.x -= minX;
                    v.x *= k;
                    v.y -= minY;
                    v.y += (canvas.Height - canvas.Width) / 2;
                    v.y *= k;
                    v.z *= k;
                    v.normal.x *= k;
                    v.normal.y *= k;
                    v.normal.z *= k;
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
        public List<Vertex> vectors;

        public Polygon()
        {
            this.vectors = new List<Vertex>();
        }
    }
}