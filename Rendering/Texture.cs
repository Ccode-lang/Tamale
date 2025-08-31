using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale.Rendering
{
    // Represents a texture loaded from an image file
    public class Texture
    {
        // OpenGL texture ID
        public uint ID;

        unsafe public Texture(string filename) {
            // Generate and bind texture
            ID = Program.gl.GenTexture();
            Program.gl.BindTexture(TextureTarget.Texture2D, ID);

            // Load image using StbImageSharp
            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(filename), ColorComponents.RedGreenBlueAlpha);


            // Upload image data to OpenGL
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

            // Set texture parameters
            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Program.gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Unbind texture
            Program.gl.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
