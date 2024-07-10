using Cairo;
using Eto.Forms.Controls.SkiaSharp.Shared;
using SkiaSharp;
using System;

namespace Eto.Forms.Controls.SkiaSharp.GTK
{
    public class SKControlHandler : Eto.GtkSharp.Forms.GtkControl<Gtk.EventBox, SKControl, SKControl.ICallback>, SKControl.ISKControl
    {

        private SKControl_GTK _nativeControl;

        public SKControlHandler()
        {
            _nativeControl = new SKControl_GTK();
            Control = _nativeControl;
        }

        public override Eto.Drawing.Color BackgroundColor { get; set; }

        public Action<SKSurface> PaintSurfaceAction
        {
            get {
                return _nativeControl.PaintSurface;
            }
            set
            {
                _nativeControl.PaintSurface = value;                
            }
        }

    }

    public class SKControl_GTK : Gtk.EventBox
    {

        public Action<SKSurface> PaintSurface;

        public SKControl_GTK()
        {
            AddEvents((int)Gdk.EventMask.PointerMotionMask);
        }

        protected override bool OnDrawn(Context cr)
        {
            var rect = Allocation;
            SKColorType ctype = SKColorType.Bgra8888;

            if (cr == null) { Console.WriteLine("Cairo Context is null"); }
            using (var bitmap = new SKBitmap(rect.Width, rect.Height, ctype, SKAlphaType.Premul))
            {
                if (bitmap == null) { Console.WriteLine("Bitmap is null"); }
                IntPtr len;
                using (var skSurface = SKSurface.Create(bitmap.Info.Width, bitmap.Info.Height, ctype, SKAlphaType.Premul, bitmap.GetPixels(out len), bitmap.Info.RowBytes))
                {
                    if (skSurface == null) { Console.WriteLine("skSurface is null"); }
                    if (PaintSurface != null) PaintSurface.Invoke(skSurface);
                    skSurface.Canvas.Flush();
                    using (Cairo.Surface surface = new Cairo.ImageSurface(bitmap.GetPixels(out len), Cairo.Format.Argb32, bitmap.Width, bitmap.Height, bitmap.Width * 4))
                    {
                        surface.MarkDirty();
                        cr.SetSourceSurface(surface, 0, 0);
                        cr.Paint();
                    }
                }
            }

            return true;
        }

    }
}
