using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TPMapEditor.Utils
{
    /// <summary>
    /// Provides methods to transform a bitmap into a "starmap" version (filled with a brush, outlined)
    /// </summary>
    public static class BitmapStarmapTransform
    {
        public static BitmapSource GenerateOutline(BitmapSource src, Color outlineColor, double thickness)
        {
            int srcW = src.PixelWidth;
            int srcH = src.PixelHeight;

            int pad = (int)Math.Ceiling(thickness);
            int newW = srcW + 2 * pad;
            int newH = srcH + 2 * pad;

            var srcPixels = new byte[srcW * srcH * 4];
            src.CopyPixels(srcPixels, srcW * 4, 0);

            var outPixels = new byte[newW * newH * 4];

            // Copy original image to center
            for (int y = 0; y < srcH; y++)
            {
                for (int x = 0; x < srcW; x++)
                {
                    int srcIdx = (y * srcW + x) * 4;
                    int dstIdx = ((y + pad) * newW + (x + pad)) * 4;
                    outPixels[dstIdx + 0] = srcPixels[srcIdx + 0];
                    outPixels[dstIdx + 1] = srcPixels[srcIdx + 1];
                    outPixels[dstIdx + 2] = srcPixels[srcIdx + 2];
                    outPixels[dstIdx + 3] = srcPixels[srcIdx + 3];
                }
            }

            // Generate ouline
            for (int y = 0; y < srcH; y++)
            {
                for (int x = 0; x < srcW; x++)
                {
                    int srcIdx = (y * srcW + x) * 4;
                    if (srcPixels[srcIdx + 3] == 0) continue;

                    int centerX = x + pad;
                    int centerY = y + pad;

                    for (int dy = -pad; dy <= pad; dy++)
                    {
                        int ny = centerY + dy;
                        if (ny < 0 || ny >= newH) continue;

                        for (int dx = -pad; dx <= pad; dx++)
                        {
                            int nx = centerX + dx;
                            if (nx < 0 || nx >= newW) continue;
                            if (Math.Sqrt(dx * dx + dy * dy) <= thickness)
                            {
                                int nidx = (ny * newW + nx) * 4;
                                if (outPixels[nidx + 3] == 0)
                                {
                                    outPixels[nidx + 0] = outlineColor.B;
                                    outPixels[nidx + 1] = outlineColor.G;
                                    outPixels[nidx + 2] = outlineColor.R;
                                    outPixels[nidx + 3] = outlineColor.A;
                                }
                            }
                        }
                    }
                }
            }

            return BitmapSource.Create(newW, newH, src.DpiX, src.DpiY, PixelFormats.Pbgra32, null, outPixels, newW * 4);
        }

        public static BitmapSource ApplyGradient(BitmapSource src, Brush brush)
        {
            int w = src.PixelWidth;
            int h = src.PixelHeight;

            var rtb = new RenderTargetBitmap(w, h, src.DpiX, src.DpiY, PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                // Mask from original image
                var maskBrush = new ImageBrush(src);

                // Apply gradient on mask
                dc.PushOpacityMask(maskBrush);
                dc.DrawRectangle(brush, null, new Rect(0, 0, w, h));
                dc.Pop();
            }

            rtb.Render(dv);
            return rtb;
        }
    }
}
