using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale.Rendering
{
    internal class Texture
    {
        public uint ID;

        public static Dictionary<int, Texture> Textures = new Dictionary<int, Texture>();

        unsafe public Texture(string filename) {
            ID = Program.gl.GenTexture();
            Program.gl.BindTexture(TextureTarget.Texture2D, ID);


            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(filename), ColorComponents.RedGreenBlueAlpha);

            fixed (byte* ptr = result.Data)
            {
                Program.gl.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    (int)InternalFormat.Rgba,
                    (uint)result.Width,
                    (uint)result.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    ptr
                );
            }

            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);

            Program.gl.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
