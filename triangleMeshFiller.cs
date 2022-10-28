using System.Windows.Forms;

namespace GK1_PROJ2
{
    public partial class mainWindow : Form
    {
        private List<Vertex> vertices;
        private List<Polygon> polygons;
        private List<Vector> normals;

        public mainWindow()
        {
            InitializeComponent();
            vertices = new List<Vertex>();
            polygons = new List<Polygon>();
            normals = new List<Vector>();
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
                        MessageBox.Show("Loaded succesfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                    v = int.Parse(parts2[0]);
                                    vn = int.Parse(parts2[2]);
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
            catch (Exception ex)
            {
                //MessageBox.Show("Unable to load selected .obj file.","Exeption while loading",MessageBoxButtons.OK,MessageBoxIcon.Error);
                throw ex;
                return false;
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