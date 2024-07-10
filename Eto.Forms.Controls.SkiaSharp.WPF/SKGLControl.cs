using Eto.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Windows;
using System.Windows.Media;

namespace Eto.Forms.Controls.SkiaSharp.WinForms
{
    public class SKGLControlHandler : Eto.Wpf.Forms.WpfFrameworkElement<FrameworkElement, Shared.SKGLControl, Shared.SKGLControl.ICallback>, Shared.SKGLControl.ISKGLControl
    {

        private SKGLControl_WPF _nativeControl;

        public SKGLControlHandler()
        {
            _nativeControl = new SKGLControl_WPF();
            _nativeControl.NativeControl = new SKGLControl_Native();
            Control = _nativeControl;
        }
        public Action<SKSurface> PaintSurfaceAction
        {
            get
            {
                return _nativeControl.NativeControl.PaintSurface;
            }
            set
            {
                _nativeControl.NativeControl.PaintSurface = value;
            }
        }

        public override Drawing.Color BackgroundColor
        {
            get => Drawing.Colors.White;
            set { return; }
        }
    }

    public class SKGLControl_WPF : System.Windows.Controls.Grid
    {
        public SKGLControl_Native NativeControl;

        public SKGLControl_WPF()
        {
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();

            NativeControl.WPFHost = true;

            // Assign the winforms control as the host control's child.
            host.Child = NativeControl;

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.Children.Add(host);

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            NativeControl.Invalidate();
        }
    }

    public class SKGLControl_Native : SKGLControl
    {
        public new Action<SKSurface> PaintSurface;

        public bool WPFHost = true;

        public Action<MouseEventArgs> WPFMouseDown;
        public Action<MouseEventArgs> WPFMouseUp;
        public Action<MouseEventArgs> WPFMouseDoubleClick;

        private GRContext grContext;
        private GRBackendRenderTargetDesc renderTarget;

        public SKGLControl_Native()
        {
            ResizeRedraw = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

            //base.OnPaint(e);

            // create the contexts if not done already
            if (grContext == null)
            {
                var glInterface = GRGlInterface.CreateNativeGlInterface();
                grContext = GRContext.Create(GRBackend.OpenGL, glInterface);

                // get initial details
                renderTarget = CreateRenderTarget();
            }

            // update to the latest dimensions
            renderTarget.Width = Width;
            renderTarget.Height = Height;

            // create the surface
            using (var surface = SKSurface.Create(grContext, renderTarget))
            {

                if (PaintSurface != null) PaintSurface.Invoke(surface);

                // start drawing
                OnPaintSurface(new SKPaintGLSurfaceEventArgs(surface, renderTarget));

                surface.Canvas.Flush();
            }

            // update the control
            SwapBuffers();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // clean up
            if (grContext != null)
            {
                grContext.Dispose();
                grContext = null;
            }
        }

        public static GRBackendRenderTargetDesc CreateRenderTarget()
        {
            int framebuffer, stencil, samples;
            Gles.glGetIntegerv(Gles.GL_FRAMEBUFFER_BINDING, out framebuffer);
            Gles.glGetIntegerv(Gles.GL_STENCIL_BITS, out stencil);
            Gles.glGetIntegerv(Gles.GL_SAMPLES, out samples);

            int bufferWidth = 0;
            int bufferHeight = 0;

            return new GRBackendRenderTargetDesc
            {
                Width = bufferWidth,
                Height = bufferHeight,
                Config = GRPixelConfig.Rgba8888,
                Origin = GRSurfaceOrigin.BottomLeft,
                SampleCount = samples,
                StencilBits = stencil,
                RenderTargetHandle = (IntPtr)framebuffer,
            };
        }
    }

}
