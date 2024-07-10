using Eto.Drawing;
using Eto.Forms.Controls.SkiaSharp.Shared;
using Eto.Mac.Forms;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using SkiaSharp;
using System;
using SKMacDrawable = SkiaSharp.Views.Mac.SKDrawable;

namespace Forms.Controls.SkiaSharp.Mac
{
    public class SKControlHandler : MacView<NSView, SKControl, SKControl.ICallback>, SKControl.ISKControl
    {
        private SKControl_Mac _nativeControl;

        public SKControlHandler()
        {
            _nativeControl = new SKControl_Mac();
            Control = _nativeControl;
        }

        public override Color BackgroundColor
        {
            get => Colors.White;
            set { return; }
        }

        public override NSView ContainerControl => Control;

        public override bool Enabled { get; set; }
        public Action<SKSurface> PaintSurfaceAction
        {
            get => _nativeControl.PaintSurface;
            set => _nativeControl.PaintSurface = value;
        }
    }


    public class SKControl_Mac : NSView, IMacControl
    {
        public Action<SKSurface> PaintSurface { get; set; }

        private NSTrackingArea _trackarea;
        
        public float LastTouchX { get; set; }
        public float LastTouchY { get; set; }

        private SKMacDrawable _drawable;

        public SKControl_Mac()
        {
            _drawable = new SKMacDrawable();
            BecomeFirstResponder();
        }

        public override CGRect Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                base.Bounds = value;
                UpdateTrackingAreas();
            }
        }

        public override CGRect Frame
        {
            get => base.Frame;
            set
            {
                base.Frame = value;
                UpdateTrackingAreas();
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public override void UpdateTrackingAreas()
        {
            if (_trackarea != null) 
            {
                RemoveTrackingArea(_trackarea);
            }
            _trackarea = new NSTrackingArea(Frame, NSTrackingAreaOptions.ActiveWhenFirstResponder | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.InVisibleRect, this, null);
            AddTrackingArea(_trackarea);
        }

        public override void DrawRect(CGRect dirtyRect)
        {

            base.DrawRect(dirtyRect);
                     
            var ctx = NSGraphicsContext.CurrentContext.GraphicsPort;

            // create the skia context

            var surface = _drawable.CreateSurface(Bounds, 1.0f, out SKImageInfo info);

            PaintSurface?.Invoke(surface);

            // draw the surface to the context
            _drawable.DrawSurface(ctx, Bounds, info, surface);
        }

        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);
        }

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
        }

        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);
        }
        public WeakReference WeakHandler { get; set; }
    }

}
