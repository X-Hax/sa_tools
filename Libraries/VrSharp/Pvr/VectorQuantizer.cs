using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

// Taken from https://github.com/Shenmue-Mods/ShenmueDKSharp/blob/master/Files/Images/_PVRT/VectorQuantizer.cs

namespace VrSharp.Pvr
{
    //TODO: Classes are unsafe and have no complete error handling

    /// <summary>
    /// Vector quantization (k-means) compression for RGBA images similar to the LBG (Linde–Buzo–Gray) algorithm.
    /// Used for compressing an image by an given block size (2x2 for Dreamcast).
    /// Performance: [code block size = 256, block size = 2x2, image size = 256x256] - 1 minute
    /// </summary>
    public class VectorQuantizer
    {
        /// <summary>
        /// Quantizes the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="blockWidth">Width of the block.</param>
        /// <param name="blockHeight">Height of the block.</param>
        /// <param name="codebookSize">Size of the codebook as block count.</param>
        /// <param name="palette">The palette.</param>
        /// <returns></returns>
        public static byte[] QuantizeImage(Bitmap bitmap, int codeBookSize, int blockWidth = 2, int blockHeight = 2)
        {
            VQCodeBook codeBook = CreateCodebook(bitmap, codeBookSize, blockWidth, blockHeight);
            return QuantizeImage(bitmap, codeBook);
        }

        /// <summary>
        /// Quantizes the bitmap with the given code book.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="codeBook">The code book.</param>
        /// <returns></returns>
        public static byte[] QuantizeImage(Bitmap bitmap, VQCodeBook codeBook)
        {
            VQBlock[] blocks = CreateBlocks(bitmap, codeBook.BlockWidth, codeBook.BlockHeight);
            byte[] result = new byte[blocks.Length];
            double dump = 0.0;
            for (int i = 0; i < blocks.Length; i++)
            {
                VQBlock block = blocks[i];
                result[i] = (byte)Nearest(block, codeBook.Entries, codeBook.Entries.Length, ref dump);
            }
            return result;
        }

        private static void ConvertBitmapArgb(ref Bitmap bitmap)
        {
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImage))
                {
                    g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }
                bitmap = newImage;
            }
        }

        /// <summary>
        /// Creates the an block array of the given bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="blockWidth">Width of the blocks.</param>
        /// <param name="blockHeight">Height of the blocks.</param>
        /// <returns></returns>
        public static VQBlock[] CreateBlocks(Bitmap bitmap, int blockWidth = 2, int blockHeight = 2)
        {
            //Read bitmap into buffer
            ConvertBitmapArgb(ref bitmap);
            byte[] imageBuffer = new byte[bitmap.Width * bitmap.Height * 4];
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            Marshal.Copy(bitmapData.Scan0, imageBuffer, 0, imageBuffer.Length);

            //Convert bitmap to blocks
            int width = bitmap.Width / blockWidth;
            int height = bitmap.Height / blockHeight;
            int blockSize = blockWidth * blockHeight * 4;
            int blockCount = width * height;
            VQBlock[] blocks = new VQBlock[blockCount];

            for (int y = 0; y < bitmap.Height - 1; y += blockHeight)
            {
                for (int x = 0; x < bitmap.Width - 1; x += blockWidth)
                {
                    int blockIndex = (y / blockHeight) * width + (x / blockWidth);
                    blocks[blockIndex] = new VQBlock(blockWidth, blockHeight);
                    
                    for (int k = 0; k < blockHeight; k++)
                    {
                        for (int l = 0; l < blockWidth; l++)
                        {
                            int imageIndex = ((y + k) * bitmap.Width + (x + l)) * 4;
                            byte b = imageBuffer[imageIndex];
                            byte g = imageBuffer[imageIndex + 1];
                            byte r = imageBuffer[imageIndex + 2];
                            byte a = imageBuffer[imageIndex + 3];

                            int subVectorIndex = k * blockWidth + l;
                            blocks[blockIndex].Pixels[subVectorIndex].X = b;
                            blocks[blockIndex].Pixels[subVectorIndex].Y = g;
                            blocks[blockIndex].Pixels[subVectorIndex].Z = r;
                            blocks[blockIndex].Pixels[subVectorIndex].W = a;
                        }
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);
            return blocks;
        }

        /// <summary>
        /// Finds the nearest matching block of the given code book cluster
        /// </summary>
        /// <param name="block">The block that will be searched for.</param>
        /// <param name="cluster">The code book cluster where the block will be searched in.</param>
        /// <param name="clusterSize">Size of the code book cluster.</param>
        /// <param name="nearestDistance">The nearest distance that was found.</param>
        /// <returns>Nearest group index</returns>
        private static int Nearest(VQBlock block, VQBlock[] cluster, int clusterSize, ref double nearestDistance)
        {
            int i = 0;
            int nearestIndex = 0;
            double distance = 0;
            double minDistance = 0;
            for (i = 0; i < clusterSize; i++)
            {
                minDistance = double.PositiveInfinity;
                nearestIndex = block.Group;
                for (i = 0; i < clusterSize; i++)
                {
                    if (minDistance > (distance = cluster[i].EuclidianDistance(ref block)))
                    {
                        minDistance = distance;
                        nearestIndex = i;
                    }
                }
            }
            nearestDistance = minDistance;
            return nearestIndex;
        }

        /// <summary>
        /// Initializes the code book.
        /// </summary>
        /// <param name="imageBlocks">The blocks of the image.</param>
        /// <param name="codeBookSize">Size of the code book.</param>
        /// <param name="fastInitialization">True for using simple initialization. False uses improved initialization.</param>
        /// <returns></returns>
        private static VQBlock[] InitCodeBook(ref VQBlock[] imageBlocks, int codeBookSize, bool fastInitialization = false)
        {
            if (fastInitialization)
            {
                int i = 0;
                VQBlock[] codeBook = new VQBlock[codeBookSize];
                for (i = 0; i < codeBookSize; i++)
                {
                    codeBook[i % codeBookSize] = new VQBlock(imageBlocks[i]);
                }
                for (i = 0; i < imageBlocks.Length; i++)
                {
                    imageBlocks[i].Group = i % codeBookSize;
                }
                return codeBook;
            }
            else
            {
                int i = 0;
                int imageLength = imageBlocks.Length;
                double sum = 0.0;
                double[] distances = new double[imageBlocks.Length];
                VQBlock[] codeBook = new VQBlock[codeBookSize];
                Random rand = new Random();

                codeBook[0] = new VQBlock(imageBlocks[rand.Next() % imageBlocks.Length]);
                for (int cluster = 1; cluster < codeBookSize; cluster++)
                {
                    sum = 0;
                    for (i = 0; i < imageLength; i++)
                    {
                        Nearest(imageBlocks[i], codeBook, cluster, ref distances[i]);
                        sum += distances[i];
                    }
                    sum = sum * rand.Next(0x7fff) / (0x7fff - 1.0);
                    for (i = 0; i < imageLength; i++)
                    {
                        if ((sum -= distances[i]) > 0) continue;
                        codeBook[cluster] = new VQBlock(imageBlocks[i]);
                        break;
                    }
                }

                double dump = 0.0;
                for (i = 0; i < imageLength; i++)
                {
                    imageBlocks[i].Group = Nearest(imageBlocks[i], codeBook, codeBookSize, ref dump);
                }
                return codeBook;
            }
        }


        /// <summary>
        /// Generates a code book for the given bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap from which the code book will be generated.</param>
        /// <param name="codeBookSize">Size of the code book.</param>
        /// <param name="blockWidth">Width of the code book blocks.</param>
        /// <param name="blockHeight">Height of the code book blocks.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The image can't be devided by the given block size!</exception>
        public static VQCodeBook CreateCodebook(Bitmap bitmap, int codeBookSize, int blockWidth = 2, int blockHeight = 2)
        {
            if (bitmap.Width % blockWidth != 0 || bitmap.Height % blockHeight != 0)
            {
                throw new ArgumentException("The image can't be devided by the given block size!");
            }

            //Convert image to blocks
            VQBlock[] imageBlocks = CreateBlocks(bitmap, blockWidth, blockHeight);

            //Create and initialize codebook
            VQBlock[] codeBookBlocks = InitCodeBook(ref imageBlocks, codeBookSize);

            //Create codebook (k-means/Lloyd algorithm)
            int i = 0;
            int j = 0;
            int changed = 0;
            int nearestIndex = 0;
            VQBlock imageBlock = null;
            VQBlock codeBookBlock = null;
            double dump = 0.0;
            int runs = 0;

            do
            {
                for (i = 0; i < codeBookSize; i++)
                {
                    codeBookBlock = codeBookBlocks[i];
                    codeBookBlock.Group = 0;
                    codeBookBlock.Zero();
                }
                for (j = 0; j < imageBlocks.Length; j++)
                {
                    imageBlock = imageBlocks[j];
                    codeBookBlock = codeBookBlocks[imageBlock.Group];
                    codeBookBlock.Group += 1;
                    codeBookBlock.Add(imageBlock);
                }
                for (i = 0; i < codeBookSize; i++)
                {
                    codeBookBlock = codeBookBlocks[i];
                    codeBookBlock.Div(codeBookBlock.Group);
                }
                changed = 0;
                for (j = 0; j < imageBlocks.Length; j++)
                {
                    imageBlock = imageBlocks[j];
                    nearestIndex = Nearest(imageBlock, codeBookBlocks, codeBookSize, ref dump);
                    if (nearestIndex != imageBlock.Group)
                    {
                        changed++;
                        imageBlock.Group = nearestIndex;
                    }
                }
                runs++;
            } while (changed > (imageBlocks.Length >> 10));

            for (i = 0; i < codeBookSize; i++)
            {
                codeBookBlock = codeBookBlocks[i];
                codeBookBlock.Group = i;
            }
            return new VQCodeBook(codeBookBlocks);
        }


    }

    /// <summary>
    /// VQ code book
    /// </summary>
    public class VQCodeBook
    {
        public int BlockWidth { get; private set; }
        public int BlockHeight { get; private set; }
        public int BlockSize
        {
            get { return BlockWidth * BlockHeight; }
        }
        public int CodeBookSize { get; private set; }
    
        public VQBlock[] Entries { get; private set; }

        public VQCodeBook(VQBlock[] codeBook)
        {
            if (codeBook == null || codeBook.Length == 0)
            {
                throw new ArgumentException("Given code book was empty");
            }
            CodeBookSize = codeBook.Length;
            BlockWidth = codeBook[0].BlockWidth;
            BlockHeight = codeBook[0].BlockHeight;
            Entries = codeBook;
        }

        public VQCodeBook(int codeBookSize, int blockWidth, int blockHeight)
        {
            Entries = new VQBlock[codeBookSize];
            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i] = new VQBlock(blockWidth, blockHeight);
            }
        }
    }

    /// <summary>
    /// VQ block
    /// </summary>
    public class VQBlock
    {
        private static System.Numerics.Vector4 m_one = System.Numerics.Vector4.One;

        public int BlockWidth;
        public int BlockHeight;
        public int Group;
        public System.Numerics.Vector4[] Pixels;

        public VQBlock(VQBlock block)
        {
            BlockWidth = block.BlockWidth;
            BlockHeight = block.BlockHeight;
            Group = block.Group;
            Pixels = new System.Numerics.Vector4[BlockWidth * BlockHeight];
            for(int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i].X = block.Pixels[i].X;
                Pixels[i].Y = block.Pixels[i].Y;
                Pixels[i].Z = block.Pixels[i].Z;
                Pixels[i].W = block.Pixels[i].W;
            }
        }

        public VQBlock(int blockWidth, int blockHeight)
        {
            BlockWidth = blockWidth;
            BlockHeight = blockHeight;
            Group = 0;
            Pixels = new System.Numerics.Vector4[blockWidth * blockHeight];
        }

        public void Zero()
        {
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] = System.Numerics.Vector4.Zero;
            }
        }

        public void Add(VQBlock block)
        {
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] += block.Pixels[i];
            }
        }

        public void Div(int group)
        {
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] /= group;
            }
        }

        public double EuclidianDistance2x2(ref VQBlock block)
        {
            double total = 0.0;
            total += System.Numerics.Vector4.DistanceSquared(Pixels[0], block.Pixels[0]);
            total += System.Numerics.Vector4.DistanceSquared(Pixels[1], block.Pixels[1]);
            total += System.Numerics.Vector4.DistanceSquared(Pixels[2], block.Pixels[2]);
            total += System.Numerics.Vector4.DistanceSquared(Pixels[3], block.Pixels[3]);
            return total;
        }

        public double EuclidianDistance(ref VQBlock block)
        {
            double total = 0.0f;
            for(int i = 0; i < Pixels.Length; i++)
            {
                total += System.Numerics.Vector4.DistanceSquared(Pixels[i], block.Pixels[i]);
            }
            return total;
        }

        private int[] MakeTwiddleMap(int size)
        {
            int[] twiddleMap = new int[size];
            for (int i = 0; i < size; i++)
            {
                twiddleMap[i] = 0;

                for (int j = 0, k = 1; k <= i; j++, k <<= 1)
                {
                    twiddleMap[i] |= (i & k) << j;
                }
            }
            return twiddleMap;
        }

        public byte[] ToArrayTwiddled()
        {
            byte[] array = new byte[Pixels.Length * 4];
            int destinationIndex = 0;
            int sourceIndex = 0;
            int[] twiddleMap = MakeTwiddleMap(BlockHeight * BlockWidth);
            for (int y = 0; y < BlockHeight; y++)
            {
                for (int x = 0; x < BlockWidth; x++)
                {
                    destinationIndex = (twiddleMap[x] << 1) | twiddleMap[y];
                    array[destinationIndex * 4] = (byte)Pixels[sourceIndex].X;
                    array[destinationIndex * 4 + 1] = (byte)Pixels[sourceIndex].Y;
                    array[destinationIndex * 4 + 2] = (byte)Pixels[sourceIndex].Z;
                    array[destinationIndex * 4 + 3] = (byte)Pixels[sourceIndex].W;
                    sourceIndex++;
                }
            }
            return array;
        }

        public byte[] ToArray()
        {
            byte[] array = new byte[Pixels.Length * 4];
            for (int i = 0; i < Pixels.Length; i++)
            {
                array[i * 4] = (byte)Pixels[i].X;
                array[i * 4 + 1] = (byte)Pixels[i].Y;
                array[i * 4 + 2] = (byte)Pixels[i].Z;
                array[i * 4 + 3] = (byte)Pixels[i].W;
            }
            return array;
        }

        public void FromArray(byte[] array, int sourceIndex = 0)
        {
            if (array.Length != Pixels.Length * 4)
            {
                throw new ArgumentException("Given vector has wrong size!");
            }
            for (int i = 0; i < Pixels.Length; i++)
            {
                int index = sourceIndex + i;
                Pixels[i].X = array[index * 4];
                Pixels[i].Y = array[index * 4 + 1];
                Pixels[i].Z = array[index * 4 + 2];
                Pixels[i].W = array[index * 4 + 3];
            }
        }
    }
}
