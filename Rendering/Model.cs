using Silk.NET.OpenGL;

namespace Tamale.Rendering
{
    internal class Model
    {
        public uint VAO { get; private set; }
        public uint VBO { get; private set; }

        public uint vertexCount { get; private set; }

        unsafe public Model(float[] vertices) {
            VAO = Program.gl.GenVertexArray();

            Program.gl.BindVertexArray(VAO);

            VBO = Program.gl.GenBuffer();
            Program.gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);

            vertexCount = (uint)(vertices.Length / 5);

            fixed (float* v = vertices)
            {
                Program.gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            }

            const uint positionLoc = 0;
            Program.gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
            Program.gl.EnableVertexAttribArray(positionLoc);

            const uint texCoordLoc = 1;
            Program.gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            Program.gl.EnableVertexAttribArray(texCoordLoc);

            Program.gl.BindVertexArray(0);
            Program.gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        }

        public static float[] LoadCOF(string FileName)
        {
            string[] lines = File.ReadAllLines(FileName);
            List<float> vertices = new List<float>();

            foreach (string line in lines)
            {
                string[] parts = line.Trim().Split(',');

                if (parts.Length != 5)
                    continue;

                // I am evil and will use a 1-based index for this
                int i = 1;
                foreach (string part in parts)
                {
                    if (float.TryParse(part, out float value))
                    {
                        if (i == 5) value = 1 - value; // Invert the u and v coordinate
                        vertices.Add(value);
                        i++;
                    }
                    else
                    {
                        throw new FormatException($"Invalid float value in COF model: {line}");
                    }
                }
            }
            Console.WriteLine($"Loaded {vertices.Count / 5} vertices from COF model.");
            return vertices.ToArray();
        }
    }
}
