﻿using System;

namespace Eto.Forms.Controls.SkiaSharp.Shared
{

    [Handler(typeof(ISKControl))]
    public class SKControl : Eto.Forms.Control
    {
        new ISKControl Handler { get { return (ISKControl)base.Handler; } }

        public Action<global::SkiaSharp.SKSurface> PaintSurfaceAction
        {
            get
            {
                return Handler.PaintSurfaceAction;
            }
            set {
                Handler.PaintSurfaceAction = value;
            }
        }

        public interface ISKControl : Eto.Forms.Control.IHandler
        {
            Action<global::SkiaSharp.SKSurface> PaintSurfaceAction { get; set; }
        }

    }
}


