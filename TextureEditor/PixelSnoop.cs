//   Copyright 2019 Kevin Godden
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Wraps a System.Drawing.Bitmap and provides faster
/// GetPixel() and SetPixel() functions for pixel access.
/// 
/// NB While the snoop object is in scope the wrapped
/// bitmap object is locked and cannot be used 
/// as normal.  Once you have finished snooping
/// on a bitmap object, dispose of the snooper to
/// unlock the bitmap and gain normal access to 
/// it again, it is best to employ the 'using' keyword
/// to effectivly manage the snooper's scope as follows:
/// 
/// 
/// using (var snoop = new BmpPixelSnoop(myBitmap))
/// { 
/// 
///     // Snoop away!
///     var pixel = snoop.GetPixel(0, 0);
///     
/// } // Snoop goes out of scope here and bitmap is unlocked
/// 
/// This class is marked as 'unsafe' so to use it in your project
/// you must have the 'Allow unsafe code' setting checked in the
/// project settings.
/// 
/// </summary>
unsafe class BmpPixelSnoop : IDisposable
{
    // A reference to the bitmap to be wrapped
    private readonly Bitmap wrappedBitmap;

    // The bitmap's data (once it has been locked)
    private BitmapData data = null;

    // Pointer to the first pixel
    private readonly byte* scan0;

    // Number of bytes per pixel
    private readonly int depth;

    // Number of bytes in an image row
    private readonly int stride;

    // The bitmap's width
    private readonly int width;

    // The bitmap's height
    private readonly int height;

    /// <summary>
    /// Constructs a BmpPixelSnoop object, the bitmap
    /// object to be wraped is passed as a parameter.
    /// </summary>
    /// <param name="bitmap">The bitmap to snoop</param>
    public BmpPixelSnoop(Bitmap bitmap)
    {
        wrappedBitmap = bitmap ?? throw new ArgumentException("Bitmap parameter cannot be null", "bitmap");

        // Currently works only for: PixelFormat.Format32bppArgb
        if (wrappedBitmap.PixelFormat != PixelFormat.Format32bppArgb)
            throw new System.ArgumentException("Only PixelFormat.Format32bppArgb is supported", "bitmap");

        // Record the width & height
        width = wrappedBitmap.Width;
        height = wrappedBitmap.Height;

        // So now we need to lock the bitmap so that we can gain access
        // to it's raw pixel data.  It will be unlocked when this snoop is 
        // disposed.
        var rect = new Rectangle(0, 0, wrappedBitmap.Width, wrappedBitmap.Height);

        try
        {
            data = wrappedBitmap.LockBits(rect, ImageLockMode.ReadWrite, wrappedBitmap.PixelFormat);
        }
        catch (Exception ex)
        {
            throw new System.InvalidOperationException("Could not lock bitmap, is it already being snooped somewhere else?", ex);
        }

        // Calculate number of bytes per pixel
        depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; // bits per channel

        // Get pointer to first pixel
        scan0 = (byte*)data.Scan0.ToPointer();

        // Get the number of bytes in an image row
        // this will be used when determining a pixel's
        // memory address.
        stride = data.Stride;
    }

    /// <summary>
    /// Disposes BmpPixelSnoop object
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes BmpPixelSnoop object, we unlock
    /// the wrapped bitmap.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (wrappedBitmap != null)
                wrappedBitmap.UnlockBits(data);
        }
        // free native resources if there are any.
    }

    /// <summary>
    /// Calculate the pointer to a pixel at (x, x)
    /// </summary>
    /// <param name="x">The pixel's x coordinate</param>
    /// <param name="y">The pixel's y coordinate</param>
    /// <returns>A byte* pointer to the pixel's data</returns>
    private byte* PixelPointer(int x, int y)
    {
        return scan0 + y * stride + x * depth;
    }

    /// <summary>
    /// Snoop's implemetation of GetPixel() which is similar to
    /// Bitmap's GetPixel() but should be faster.
    /// </summary>
    /// <param name="x">The pixel's x coordinate</param>
    /// <param name="y">The pixel's y coordinate</param>
    /// <returns>The pixel's colour</returns>
    public System.Drawing.Color GetPixel(int x, int y)
    {
        // Better do the 'decent thing' and bounds check x & y
        if (x < 0 || y < 0 || x >= width || y >= height)
            throw new ArgumentException("x or y coordinate is out of range");

        int a, r, g, b;

        // Get a pointer to this pixel
        byte* p = PixelPointer(x, y);

        // Pull out its colour data
        b = *p++;
        g = *p++;
        r = *p++;
        a = *p;

        // And return a color value for it (this is quite slow
        // but allows us to look like Bitmap.GetPixel())
        return System.Drawing.Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    /// Sets the passed colour to the pixel at (x, y)
    /// </summary>
    /// <param name="x">The pixel's x coordinate</param>
    /// <param name="y">The pixel's y coordinate</param>
    /// <param name="col">The value to be assigned to the pixel</param>
    public void SetPixel(int x, int y, System.Drawing.Color col)
    {
        // Better do the 'decent thing' and bounds check x & y
        if (x < 0 || y < 0 || x >= width || y >= height)
            throw new ArgumentException("x or y coordinate is out of range");

        // Get a pointer to this pixel
        byte* p = PixelPointer(x, y);

        // Set the data
        *p++ = col.B;
        *p++ = col.G;
        *p++ = col.R;
        *p = col.A;
    }

    /// <summary>
    /// The bitmap's width
    /// </summary>
    public int Width { get { return width; } }

    // The bitmap's height
    public int Height { get { return height; } }
}
